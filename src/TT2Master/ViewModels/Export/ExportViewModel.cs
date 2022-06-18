using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Resources;
using Xamarin.Forms;
using Plugin.Clipboard;
using System.Web;
using SkiaSharp;
using TT2Master.Model.Drawing;
using TT2Master.Helpers;
using TT2Master.Model.Clan;
using Xamarin.Essentials;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using System.Linq;

namespace TT2Master
{
    public class ExportViewModel : ViewModelBase
    {
        #region Properties
        public ICommand ExportCommand { get; private set; }
        public ICommand ExportSmallCommand { get; private set; }
        public ICommand ExportProfileCommand { get; private set; }
        public ICommand ExportRaidCommand { get; private set; }
        public ICommand ExportRaidJsonCommand { get; private set; }
        public ICommand ExportSnapshotDiffCommand { get; private set; }

        private readonly IPageDialogService _dialogService;

        private List<ExportArtifact> _arts = new List<ExportArtifact>();
        private List<ExportSkill> _skills = new List<ExportSkill>();

        private List<ExportArtifactLvl> _artLvls = new List<ExportArtifactLvl>();
        private List<ExportSkillLvl> _skillLvls = new List<ExportSkillLvl>();

        private int _snapshotFromId;
        public int SnapshotFromId { get => _snapshotFromId; set => SetProperty(ref _snapshotFromId, value); }

        private int _snapshotToId;
        public int SnapshotToId { get => _snapshotToId; set => SetProperty(ref _snapshotToId, value); }

        #endregion

        #region Ctor
        public ExportViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.ExportTitle;
            SnapshotFromId = LocalSettingsORM.LastSnapshotExportFromId;
            SnapshotToId = LocalSettingsORM.LastSnapshotExportToId;

            _dialogService = dialogService;

            ExportCommand = new DelegateCommand(async () => await ExportExecute());
            ExportSmallCommand = new DelegateCommand(async () => await ExportSmallExecute());
            ExportProfileCommand = new DelegateCommand(async () => await ExportProfileExecute());
            ExportRaidCommand = new DelegateCommand(async () => await ExportRaidExecute());
            ExportRaidJsonCommand = new DelegateCommand(async () => await ExportRaidJsonExecute());
            ExportSnapshotDiffCommand = new DelegateCommand(async () => await ExportSnapshotDiffExecute());
        }
        #endregion

        #region Command Methods

        private async Task<bool> ExportSnapshotDiffExecute()
        {
            #region prep
            if (!LocalSettingsORM.IsReadingDataFromSavefile)
            {
                await _dialogService.DisplayAlertAsync("Not possible", "Sorry, this can only be done with savefile as data source :(", AppResources.OKText);
                return true;
            }

            var options = new string[]
            {
                AppResources.ShareFile,
                AppResources.CopyContentToClipboard
            };

            // promt user
            string response = await _dialogService.DisplayActionSheetAsync(AppResources.ExportText
            , AppResources.CancelText
            , AppResources.DestroyText
            , options);

            //handle user cancel
            if (response == AppResources.CancelText || response == AppResources.DestroyText)
            {
                return false;
            }
            #endregion

            #region build csv string
            var snapshotFrom = await App.DBRepo.GetSnapshotByID(SnapshotFromId);
            var snapshotTo = await App.DBRepo.GetSnapshotByID(SnapshotToId);

            if (snapshotFrom == null || snapshotTo == null)
            {
                Logger.WriteToLogFile($"snapshotFrom {SnapshotFromId} or snapshotTo {SnapshotToId} is null");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }

            snapshotFrom.MemberSnapshotItems = await App.DBRepo.GetAllMemberSnapshotItemAsync(snapshotFrom.ID);
            snapshotFrom.MemberSnapshotItems = snapshotFrom.MemberSnapshotItems.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId).ToList();

            snapshotTo.MemberSnapshotItems = await App.DBRepo.GetAllMemberSnapshotItemAsync(snapshotTo.ID);
            snapshotTo.MemberSnapshotItems = snapshotTo.MemberSnapshotItems.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId).ToList();

            var diffCsvString = "Id,Name,Morale From,Morale To,Morale Diff,MS From,MS To,MS Diff,Status";

            foreach (var item in snapshotTo.MemberSnapshotItems)
            {
                var compareItem = snapshotFrom.MemberSnapshotItems.Where(x => x.PlayerId == item.PlayerId).FirstOrDefault();
                if (compareItem == null)
                {
                    Logger.WriteToLogFile($"Could not find compare item for {item.PlayerId}");
                    diffCsvString += $"\n{item.PlayerId}" + // ID
                        $",{item.Name}" +                   // Name
                        $",0" +                             // Morale From
                        $",{item.RaidTicketsCollected}" +   // Morale To
                        $",0" +                             // Morale Diff
                        $",0" +                             // MS From
                        $",{item.StageMax}" +               // MS To
                        $",0" +                             // MS Diff
                        $",New Player";                     // Status
                    continue;
                }

                diffCsvString += $"\n{item.PlayerId}" +                                 // ID
                    $",{item.Name}" +                                                   // Name
                    $",{compareItem.RaidTicketsCollected}" +                            // Morale From
                    $",{item.RaidTicketsCollected}" +                                   // Morale To
                    $",{item.RaidTicketsCollected - compareItem.RaidTicketsCollected}" +// Morale Diff
                    $",{compareItem.StageMax}" +                                        // MS From
                    $",{item.StageMax}" +                                               // MS To
                    $",{item.StageMax - compareItem.StageMax}" +                        // MS Diff
                    $",OK";                                                             // Status
            }

            foreach (var item in snapshotFrom.MemberSnapshotItems)
            {
                if (snapshotTo.MemberSnapshotItems.Any(x => x.PlayerId == item.PlayerId))
                    continue;

                diffCsvString += $"\n{item.PlayerId}" + // ID
                    $",{item.Name}" +                   // Name
                    $",{item.RaidTicketsCollected}" +   // Morale From
                    $",0" +                             // Morale To
                    $",0" +                             // Morale Diff
                    $",{item.StageMax}" +               // MS From
                    $",0" +                             // MS To
                    $",0" +                             // MS Diff
                    $",Player left";                    // Status
            }
            #endregion

            //reaction
            if (response == AppResources.ShareFile)
            {
                await FileHelper.WriteAndShareFileAsync(diffCsvString, $"snapshotDiff{DateTime.Now:yyyy.MM.dd hh:mm:ss}.csv");
            }
            else if (response == AppResources.CopyContentToClipboard)
            {
                CrossClipboard.Current.SetText(diffCsvString.ToString());
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
            }

            return true;
        }

        private async Task<bool> ExportExecute()
        {
            try
            {
                #region prompt
                var options = new string[]
                        {
                    AppResources.ShareFile,
                    AppResources.CopyContentToClipboard,
                        };

                if (Device.RuntimePlatform != Device.iOS)
                {
                    options = new string[]
                    {
                        AppResources.SaveToDownloads,
                        AppResources.ShareFile,
                        AppResources.CopyContentToClipboard
                    };
                }

                // promt user
                string response = await _dialogService.DisplayActionSheetAsync(AppResources.ExportText
                , AppResources.CancelText
                , AppResources.DestroyText
                , options); 
                #endregion

                //handle user cancel
                if (response == AppResources.CancelText || response == AppResources.DestroyText)
                {
                    return false;
                }

                //Load Artifacts
                if (!await ReloadArtifactsAsync(false))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }

                //Load SP
                if(!await LoadSP(false))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }

                //Export
                string artsp = JsonConvert.SerializeObject(new { _arts, _skills, App.Save.CurrentRelics });

                // reaction
                if (response == AppResources.SaveToDownloads)
                {
                    ExportWriter.DeleteFile(false);
                    string pathWrittenTo = ExportWriter.WriteFile(artsp, false);

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + $" {pathWrittenTo}", AppResources.OKText);
                }
                else if (response == AppResources.ShareFile)
                {
                    await FileHelper.WriteAndShareFileAsync(artsp, "artspLvl.json");
                }
                else if (response == AppResources.CopyContentToClipboard)
                {
                    CrossClipboard.Current.SetText(artsp.ToString());
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
                }

                return true;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);

                return false;
            }
        }

        private async Task<bool> ExportSmallExecute()
        {
            try
            {
                var options = new string[]
                {
                    AppResources.ShareFile,
                    AppResources.CopyContentToClipboard,
                };

                if (Device.RuntimePlatform != Device.iOS)
                {
                    options = new string[]
                    {
                        AppResources.SaveToDownloads,
                        AppResources.ShareFile,
                        AppResources.CopyContentToClipboard,
                    };
                }

                //promt user
                string response = await _dialogService.DisplayActionSheetAsync(AppResources.ExportText
                , AppResources.CancelText
                , AppResources.DestroyText
                , options);


                //handle user cancel
                if(response == AppResources.CancelText || response == AppResources.DestroyText)
                {
                    return false;
                }

                //Load Artifacts
                if (!await ReloadArtifactsAsync(true))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }

                //Load SP
                if (!await LoadSP(true))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }

                //Export
                string artsp = JsonConvert.SerializeObject(new { _artLvls, _skillLvls, App.Save.CurrentRelics });

                //reaction
                if (response == AppResources.SaveToDownloads)
                {
                    ExportWriter.DeleteFile(true);
                    string pathWrittenTo = ExportWriter.WriteFile(artsp, true);

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + $" {pathWrittenTo}", AppResources.OKText);
                }
                else if (response == AppResources.ShareFile)
                {
                    await FileHelper.WriteAndShareFileAsync(artsp, "artsp.json");
                }
                else if (response == AppResources.CopyContentToClipboard)
                {
                    CrossClipboard.Current.SetText(artsp.ToString());
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
                }

                return true;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);

                return false;
            }
        }

        /// <summary>
        /// Exports profile picture
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ExportProfileExecute()
        {
            try
            {
                // promt user
                string response = await _dialogService.DisplayActionSheetAsync(AppResources.ExportText
                    , AppResources.CancelText
                    , AppResources.DestroyText
                    , new string[] { AppResources.SaveText }
                );


                //handle user cancel
                if (response == AppResources.CancelText || response == AppResources.DestroyText)
                {
                    return false;
                }

                //Export
                var profileDrawer = new ProfileDrawer();
                profileDrawer.Draw();
                var result = await profileDrawer.SaveImage();

                if (!result.Item1)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                    return false;
                }

                if(response == AppResources.ShareFile)
                {
                    await FileHelper.ShareFileAsync(AppResources.Profile, result.Item2);
                }
                else
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + $" Gallery", AppResources.OKText);
                }

                return result.Item1;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);

                return false;
            }
        }

        /// <summary>
        /// Exports raid picture
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ExportRaidExecute()
        {
            try
            {
                // promt user
                string response = await _dialogService.DisplayActionSheetAsync(AppResources.ExportText
                    , AppResources.CancelText
                    , AppResources.DestroyText
                    , new string[] { AppResources.SaveText }
                );


                //handle user cancel
                if (response == AppResources.CancelText || response == AppResources.DestroyText)
                {
                    return false;
                }

                //Export
                var profileDrawer = new RaidDrawer();
                profileDrawer.Draw();
                var result = await profileDrawer.SaveImage();

                if (!result.Item1)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                    return false;
                }

                if (response == AppResources.ShareFile)
                {
                    await FileHelper.ShareFileAsync(AppResources.Raid, result.Item2);
                }
                else
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + $" Gallery", AppResources.OKText);
                }

                return result.Item1;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);

                return false;
            }
        }

        private async Task<bool> ExportRaidJsonExecute()
        {
            try
            {
                var options = new string[]
                {
                    AppResources.ShareFile,
                    AppResources.CopyContentToClipboard,
                };

                if (Device.RuntimePlatform != Device.iOS)
                {
                    options = new string[]
                    {
                        AppResources.SaveToDownloads,
                        AppResources.ShareFile,
                        AppResources.CopyContentToClipboard,
                    };
                }

                //promt user
                string response = await _dialogService.DisplayActionSheetAsync(AppResources.ExportRaidAsJson
                , AppResources.CancelText
                , AppResources.DestroyText
                , options);


                //handle user cancel
                if (response == AppResources.CancelText || response == AppResources.DestroyText)
                {
                    return false;
                }

                //Load Raid Card Handler
                if (!RaidCardHandler.LoadItemsFromInfofile())
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }

                //Load Player info for raid cards
                if (!RaidCardHandler.FillItems())
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }

                //Export
                string exportStr = JsonConvert.SerializeObject(new { RaidCardHandler.RaidCards, RaidCardHandler.RaidCardSets });

                //reaction
                if (response == AppResources.SaveToDownloads)
                {
                    ExportWriter.DeleteFile(true);
                    string pathWrittenTo = ExportWriter.WriteFile(exportStr, true);

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + $" {pathWrittenTo}", AppResources.OKText);
                }
                else if (response == AppResources.ShareFile)
                {
                    await FileHelper.WriteAndShareFileAsync(exportStr, "raidinfo.json");
                }
                else if (response == AppResources.CopyContentToClipboard)
                {
                    CrossClipboard.Current.SetText(exportStr.ToString());
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
                }

                return true;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);

                return false;
            }
        }
        #endregion

        #region Helper

        #region Artifact Load
        /// <summary>
        /// Reloads Artifacts Async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadArtifactsAsync(bool small)
        {
            try
            {
                //Load Save file -> Only Arts
                if (!await App.Save.Initialize(loadPlayer: false, loadAccountModel: false, loadSkills: false))
                {
                    OptimizeLogger.WriteToLogFile($"ExportVM.ReloadArtifactsAsync: Could not load file");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotLoadFileText, AppResources.OKText);
                    return false;
                }

                //Load Artifacts
                ArtifactHandler.OnProblemHaving += ArtifactConstants_OnProblemHaving;

                if (!ArtifactHandler.LoadArtifacts())
                {
                    OptimizeLogger.WriteToLogFile($"ExportVM.ReloadArtifactsAsync: Could not load arts");

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotFillArtifactsText, AppResources.OKText);
                    ArtifactHandler.OnProblemHaving -= ArtifactConstants_OnProblemHaving;
                    return false;
                }

                OptimizeLogger.WriteToLogFile($"ExportVM.ReloadArtifactsAsync: filling arts");
                ArtifactHandler.FillArtifacts(App.Save);

                ArtifactHandler.OnProblemHaving -= ArtifactConstants_OnProblemHaving;

                //Repopulate
                OptimizeLogger.WriteToLogFile($"ExportVM.ReloadArtifactsAsync: repopulating");

                if (small)
                {
                    _artLvls = new List<ExportArtifactLvl>();
                    foreach (var item in ArtifactHandler.Artifacts)
                    {
                        _artLvls.Add(new ExportArtifactLvl(item));
                    }
                }
                else
                {
                    _arts = new List<ExportArtifact>();
                    foreach (var item in ArtifactHandler.Artifacts)
                    {
                        _arts.Add(new ExportArtifact(item));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                OptimizeLogger.WriteToLogFile($"ExportVM.ReloadArtifactsAsync: Error {ex.Message}");

                return false;
            }
        }

        /// <summary>
        /// When ArtifactConstants crashes
        /// </summary>
        /// <param name="e"></param>
        private async void ArtifactConstants_OnProblemHaving(object sender, CustErrorEventArgs e)
        {
            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, $"{e.MyException.Message}", AppResources.OKText);
            Logger.WriteToLogFile($"ARTIFACT ERROR {sender.ToString()}-> {e.MyException.Message} \n{e.MyException.Data}");
        }
        #endregion

        #region SP Load
        private async Task<bool> LoadSP(bool small)
        {
            try
            {
                //Load Skills
                SkillInfoHandler.OnProblemHaving += Child_OnProblemHaving;

                if (!SkillInfoHandler.LoadSkills())
                {
                    OptimizeLogger.WriteToLogFile($"ExportVM.LoadSP: Could not load Skils");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotLoadSkillsText, AppResources.OKText);
                    SkillInfoHandler.OnProblemHaving -= Child_OnProblemHaving;
                    return false;
                }

                OptimizeLogger.WriteToLogFile($"ExportVM.LoadSP: filling skills");
                SkillInfoHandler.FillSkills(App.Save);

                SkillInfoHandler.OnProblemHaving -= Child_OnProblemHaving;

                //Repopulate
                OptimizeLogger.WriteToLogFile($"ExportVM.LoadSP: repopulating");

                if (small)
                {
                    _skillLvls = new List<ExportSkillLvl>();
                    foreach (var item in SkillInfoHandler.Skills)
                    {
                        _skillLvls.Add(new ExportSkillLvl(item));
                    }
                }
                else
                {
                    _skills = new List<ExportSkill>();
                    foreach (var item in SkillInfoHandler.Skills)
                    {
                        _skills.Add(new ExportSkill(item));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                OptimizeLogger.WriteToLogFile($"ExportVM.LoadSP: Exception {ex.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);

                return false;
            }

        }

        /// <summary>
        /// When a child has problems
        /// </summary>
        /// <param name="e"></param>
        private async void Child_OnProblemHaving(object sender, CustErrorEventArgs e)
        {
            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, $"{e.MyException.Message}", AppResources.OKText);
            Logger.WriteToLogFile($"SP Load {sender.ToString()} ERROR -> {e.MyException.Message} \n{e.MyException.Data}");
        }
        #endregion
        #endregion

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            LocalSettingsORM.LastSnapshotExportFromId = SnapshotFromId;
            LocalSettingsORM.LastSnapshotExportToId = SnapshotToId;
            
            base.OnNavigatedFrom(parameters);
        }
    }
}