using Microsoft.AppCenter.Analytics;
using Plugin.Clipboard;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Helper;
using Xamarin.Forms;

namespace TT2Master
{
    public class ClanExportViewModel : ViewModelBase
    {
        #region Properties
        private List<CsvExportProperty> _exportProperties = new List<CsvExportProperty>();
        public List<CsvExportProperty> ExportProperties { get => _exportProperties; set => SetProperty(ref _exportProperties, value); }

        private Snapshot _currentSnapshot = new Snapshot();
        public Snapshot CurrentSnapshot { get => _currentSnapshot; set => SetProperty(ref _currentSnapshot, value); }

        private List<MemberSnapshotItem> _snapshots = new List<MemberSnapshotItem>();
        /// <summary>
        /// List of stored snapshots
        /// </summary>
        public List<MemberSnapshotItem> Snapshots { get => _snapshots; set => SetProperty(ref _snapshots, value); }

        public ICommand ExportCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ICommand SortUpCommand { get; private set; }
        public ICommand SortDownCommand { get; private set; }
        public ICommand EnableDisableCommand { get; private set; }

        private readonly IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Bool for <see cref="EnableDisableCommand"/>. If true, all items get enabled else disabled
        /// </summary>
        private bool _isEnable = false;

        private int _idToExport = 0;
        private readonly string _exportReference = "ClanMember";
        #endregion

        #region Ctor
        public ClanExportViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.ClanExportTitle;

            _dialogService = dialogService;
            _navigationService = navigationService;

            CancelCommand = new DelegateCommand(async () => 
            {
                var result = await _navigationService.GoBackAsync();
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            } );
            ExportCommand = new DelegateCommand(async () => await ExportExecute());
            SortUpCommand = new DelegateCommand<object>(SortUp);
            SortDownCommand = new DelegateCommand<object>(SortDown);
            EnableDisableCommand = new DelegateCommand(EnableDisable);
        }
        #endregion

        #region Command methods
        private void EnableDisable()
        {
            for (int i = 0; i < ExportProperties.Count; i++)
            {
                ExportProperties[i].IsExportWished = _isEnable;
            }

            _isEnable = _isEnable ? false : true;
        }

        /// <summary>
        /// Decreases the sort-id
        /// </summary>
        /// <param name="obj"></param>
        private void SortUp(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var item = obj as CsvExportProperty;

                // find item in collection
                int currentSortId = 0;
                int posInList = 0;

                for (int i = 0; i < ExportProperties.Count; i++)
                {
                    if(ExportProperties[i].ID == item.ID)
                    {
                        posInList = i;
                        currentSortId = ExportProperties[i].SortId;
                        break;
                    }
                }

                // return if it cannot decrease
                if(currentSortId <= 0)
                {
                    return;
                }

                ExportProperties[posInList].SortId--;
                ExportProperties[posInList - 1].SortId++;

                ExportProperties = ExportProperties.OrderBy(x => x.SortId).ToList();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error in ClanExport sorting up: {e.Message}");
            }
        }

        /// <summary>
        /// Increases the sort-id
        /// </summary>
        /// <param name="obj"></param>
        private void SortDown(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var item = obj as CsvExportProperty;

                // find item in collection
                int currentSortId = 0;
                int posInList = 0;

                for (int i = 0; i < ExportProperties.Count; i++)
                {
                    if (ExportProperties[i].ID == item.ID)
                    {
                        posInList = i;
                        currentSortId = ExportProperties[i].SortId;
                        break;
                    }
                }

                // return if it cannot increase
                if (currentSortId >= ExportProperties.Count)
                {
                    return;
                }

                ExportProperties[posInList].SortId++;
                ExportProperties[posInList + 1].SortId--;

                ExportProperties = ExportProperties.OrderBy(x => x.SortId).ToList();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error in ClanExport sorting down: {e.Message}");
            }
        }

        private async Task<bool> ExportExecute()
        {

            // ask user what he wants
            string response = await _dialogService.DisplayActionSheetAsync(AppResources.ExportQuestionText
                , AppResources.CancelText
                , AppResources.DestroyText
                , new string[] { "CSV", AppResources.Clipboard, AppResources.ShareFile });

            bool toClipboard = false;
            bool toShare = false;

            if (response == AppResources.CancelText || response == AppResources.DestroyText)
            {
                return true;
            }
            else if (response == AppResources.Clipboard)
            {
                toClipboard = true;
            }
            else if (response == AppResources.ShareFile)
            {
                toShare = true;
            }

            // save current settings
            await App.DBRepo.DeleteCsvExportPropertiesByBuild(_exportReference);
            foreach (var item in ExportProperties)
            {
                await App.DBRepo.AddNewCsvExportPropertyAsync(item);
            }

            // prepare export string
            string strToExport = GetClanCSVString();

            // Do what user asked
            if (toClipboard)
            {
                CrossClipboard.Current.SetText(strToExport);
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
                return true;
            }

            //filename
            string fileDate = _idToExport == 0 ? DateTime.Now.ToString("yyyyMMdd") : CurrentSnapshot.Timestamp.ToString("yyyyMMdd");
            string name = $"{fileDate}_clanmember.csv";


            if (!toShare)
            {
                //get path and dir
                string path = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPathName(name);
                string dir = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPath();

                FileHelper.DeleteFile(dir, path);
                string savedTo = FileHelper.WriteFileToDownloads(strToExport, name);

                await ToastSender.SendToastAsync(AppResources.FileSavedToText + $" {savedTo}", _dialogService);
            }
            else
            {
                await FileHelper.WriteAndShareFileAsync(strToExport, name);
            }

            return true;
        }
        #endregion

        #region Helper

        private List<ExportClanMember> GetExportList()
        {
            var exp = new List<ExportClanMember>();

            if(_idToExport == 0)
            {
                //populate from clan member list
                foreach (var item in App.Save.ThisClan.ClanMember)
                {
                    var member = new ExportClanMember()
                    {
                        ID = item.PlayerId,
                        Name = item.PlayerName,
                        StageMax = item.StageMax,
                        WeeklyTicketCount = item.WeeklyTicketCount,
                        RaidTicketsCollected = item.RaidTicketsCollected,
                        RaidAttackCount = item.RaidAttackCount,
                        RaidTotalXP = item.RaidTotalXP,
                        CraftingShardsSpent = item.CraftingShardsSpent,
                        EquipmentSetCount = item.EquipmentSetCount,
                        RaidBaseDamage = item.RaidBaseDamage,
                        RaidPlayerLevel = item.RaidPlayerLevel,
                        RaidTotalCardLevel = item.RaidTotalCardLevel,
                        RaidUniqueSkillCount = item.RaidUniqueSkillCount,
                        TotalHelperScrolls = item.TotalHelperScrolls,
                        TotalHelperWeapons = item.TotalHelperWeapons,
                        TotalPetLevels = item.TotalPetLevels,
                        TotalSkillPoints = item.TotalSkillPoints,
                        ArtifactCount = item.ArtifactCount,
                        TournamentCount = item.TournamentCount,
                        TitanPoints = item.TitanPoints,
                        ClanRole = item.ClanRole,
                        LastTimestamp = item.LastTimestamp,
                        SaveDate = $"{ DateTime.Now.ToString("yyyy.MM.dd") }",
                        UndisputedWins = item.UndisputedWins,
                    };

                    exp.Add(member);
                }
            }
            else
            {
                // populate from snapshot
                foreach (var item in Snapshots)
                {
                    exp.Add(new ExportClanMember()
                    {
                        ID = item.PlayerId,
                        Name = item.Name,
                        StageMax = item.StageMax,
                        WeeklyTicketCount = item.WeeklyTicketCount,
                        RaidAttackCount = item.RaidAttackCount,
                        ArtifactCount = item.ArtifactCount,
                        RaidTicketsCollected = item.RaidTicketsCollected,
                        RaidTotalXP = item.RaidTotalXP,
                        TournamentCount = item.TournamentCount,
                        CraftingShardsSpent = item.CraftingShardsSpent,
                        TotalSkillPoints = item.TotalSkillPoints,
                        TotalPetLevels = item.TotalPetLevels,
                        TotalHelperWeapons = item.TotalHelperWeapons,
                        TotalHelperScrolls = item.TotalHelperScrolls,
                        EquipmentSetCount = item.EquipmentSetCount,
                        RaidUniqueSkillCount = item.RaidUniqueSkillCount,
                        RaidBaseDamage = item.RaidBaseDamage,
                        RaidPlayerLevel = item.RaidPlayerLevel,
                        RaidTotalCardLevel = item.RaidTotalCardLevel,
                        TitanPoints = item.TitanPoints,
                        ClanRole = item.ClanRole,
                        LastTimestamp = item.LastTimestamp,
                        SaveDate = $"{ CurrentSnapshot.Timestamp.ToString("yyyy.MM.dd") }",
                        UndisputedWins = item.UndisputedWins,
                    });
                }

            }
                return exp;
        }

        private string GetHeaderLine()
        {
            string line = "";
            string del = LocalSettingsORM.CsvDelimiter;

            var expList = ExportProperties.Where(x => x.IsExportWished).ToList();
            for (int i = 0; i < expList.Count; i++)
            {
                if(expList[i].IsExportWished)
                {
                    line += expList[i].DisplayName + (i + 1 == expList.Count ? "" : del);
                }
            }

            line += "\n";

            return line;
        }

        /// <summary>
        /// Little export of clan member information
        /// </summary>
        private string GetClanCSVString()
        {
            try
            {
                var exp = GetExportList();

                var mitVals = new List<List<CsvExportProperty>>();
                var props = typeof(ExportClanMember).GetProperties();
                string del = LocalSettingsORM.CsvDelimiter;
                var expList = ExportProperties.Where(x => x.IsExportWished).ToList();


                foreach (var item in exp)
                {
                    var tmp = new List<CsvExportProperty>();

                    foreach (var expProp in expList)
                    {
                        tmp.Add(
                            new CsvExportProperty()
                            {
                                DisplayName = expProp.DisplayName,
                                IsExportWished = expProp.IsExportWished,
                                ExportReference = expProp.ExportReference,
                                Identifier = expProp.Identifier,
                                SortId = expProp.SortId,
                                PrintValue = props.Where(x => x.Name == expProp.ID).FirstOrDefault().GetValue(item) ?? "",
                        });
                    }

                    mitVals.Add(tmp);
                }

                // build export string
                string expStr = GetHeaderLine();

                foreach (var item in mitVals)
                {
                    string line = "";

                    for (int i = 0; i < item.Count; i++)
                    {
                        line += item[i].PrintValue + (i + 1 == item.Count ? "" : del);
                    }
                    expStr += line + "\n";
                }

                return expStr;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanExport error: {e.Message}");
                return "????";
            }
        }

        private async Task<List<CsvExportProperty>> GetExportProps()
        {
            var allProps = ExportProperyLists.GetClanMemberProperties();

            var savedProps = await App.DBRepo.GetAllCsvExportPropertiesAsync(_exportReference);

            if(savedProps == null)
            {
                return allProps;
            }
            if(savedProps.Count == 0)
            {
                return allProps;
            }

            // set stored values
            foreach (var item in allProps)
            {
                item.ExportReference = _exportReference;
                // get saved props setting
                var savedItem = savedProps.Where(x => x.ID == item.ID).FirstOrDefault();

                if(savedItem == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(savedItem.Identifier))
                {
                    continue;
                }

                item.IsExportWished = savedItem.IsExportWished;
                item.SortId = savedItem.SortId;
            }

            return allProps.OrderBy(x => x.SortId).ToList();
        }

        /// <summary>
        /// Reloads <see cref="Snapshots"/> from DB and sorts the list
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadSnapshotsAsync()
        {
            Snapshots = await App.DBRepo.GetAllMemberSnapshotItemAsync(CurrentSnapshot.ID);
            Snapshots.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId);

            return true;
        }
        #endregion

        #region Overrides
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Analytics.TrackEvent("Module used", new Dictionary<string, string> { { "Name", "Clan export" } });

                if (parameters != null)
                {
                    // has a snapshot id been passed? Then load it
                    if (parameters.ContainsKey("id"))
                    {
                        _idToExport = JfTypeConverter.ForceInt(parameters["id"].ToString());

                        if (_idToExport >= 0)
                        {
                            //Load from DB
                            CurrentSnapshot = await App.DBRepo.GetSnapshotByID(_idToExport);

                            if (CurrentSnapshot == null)
                            {
                                CurrentSnapshot = new Snapshot();
                                Snapshots = new List<MemberSnapshotItem>();
                            }

                            bool snLoaded = await ReloadSnapshotsAsync();
                        }
                        else
                        {
                            await _dialogService.DisplayAlertAsync(AppResources.ErrorOccuredText, "could not parse id", AppResources.OKText);
                            var result = await _navigationService.GoBackAsync();
                            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                        }
                    }
                }


                //load export settings here
                //ExportProperties = GetMockExportProps();
                ExportProperties = await GetExportProps();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanExport error: {e.Message}");
            }
            
            base.OnNavigatedTo(parameters);
        } 
        #endregion
    }
}