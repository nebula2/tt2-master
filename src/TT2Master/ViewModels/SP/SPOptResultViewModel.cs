using Microsoft.AppCenter.Analytics;
using Plugin.Clipboard;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Drawing;
using TT2Master.Model.Navigation;
using TT2Master.Model.SP;
using TT2Master.Resources;

namespace TT2Master
{
    /// <summary>
    /// Optimize SP
    /// </summary>
    public class SPOptResultViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private readonly INavigationService _navigationService;

        private string _attentionText = "";
        public string AttentionText
        {
            get => _attentionText;
            set
            {
                SetProperty(ref _attentionText, value);
                IsAttentionTextVisible = !string.IsNullOrWhiteSpace(value);
            }
        }

        private bool _isAttentionTextVisible = false;
        public bool IsAttentionTextVisible { get => _isAttentionTextVisible; set => SetProperty(ref _isAttentionTextVisible, value); }

        public ICommand PaintCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand FollowCommand { get; private set; }

        private SKColor _treePseudoColor;
        public SKColor TreePseudoColor { get => _treePseudoColor; set => SetProperty(ref _treePseudoColor, value); }

        private SPOptimizer _optimizer;
        public SPOptimizer Optimizer { get => _optimizer; set => SetProperty(ref _optimizer, value); }

        private ObservableCollection<SPOptSkill> _upgradeCollection = new ObservableCollection<SPOptSkill>();
        /// <summary>
        /// Holds the collection of upgrades in the correct order in which they have to be made (the single steps)
        /// </summary>
        public ObservableCollection<SPOptSkill> UpgradeCollection { get => _upgradeCollection; set => SetProperty(ref _upgradeCollection, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public SPOptResultViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method to initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            PaintCommand = new DelegateCommand<object>(BuildTree);
            ExportCommand = new DelegateCommand(async () => await ExportExecute());
            FollowCommand = new DelegateCommand(async () => await FollowExecute());
        }

        /// <summary>
        /// Action for <see cref="FollowCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> FollowExecute()
        {
            try
            {
                // Build properties for SP follower
                var build = new SPBuild(Optimizer.UpgradeSimulation, LocalSettingsORM.CurrentTTVersion);

                var result = await _navigationService.NavigateAsync(NavigationConstants.DefaultPath + "SPOptPage/SPOptResultPage/SPOptimizerPage", new NavigationParameters() { { "build", build } });
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Could not navigate to build from optimizer Error. {e.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }
        }

        /// <summary>
        /// Action for <see cref="ExportCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ExportExecute()
        {
            // Ask user what he wants
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

            // prepare export string
            string strToExport = GetSimulationCSVString();

            // Do what user asked
            if (toClipboard)
            {
                CrossClipboard.Current.SetText(strToExport);
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
                return true;
            }

            //filename
            string name = $"{DateTime.Now.ToString("yyyyMMdd")}_spBuild.csv";

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

        #region Stuff
        private string GetSimulationCSVString()
        {
            try
            {
                string del = LocalSettingsORM.CsvDelimiter;
                // convert simulation to SPBuild

                var build = new SPBuild(Optimizer.UpgradeSimulation, LocalSettingsORM.GetCurrentTTMasterVersion());

                #region Basics
                string buildStr = build.ID;
                buildStr += $"\n{build.OwnerName}";
                buildStr += $"\n{build.Name}";
                buildStr += $"\n{build.Editable.ToString().ToUpper()}";
                buildStr += $"\n{build.Description}";
                buildStr += $"\n{build.Version}";
                #endregion

                #region SkillTree
                //Build first line. (LEFT TO RIGHT)
                buildStr += $"\nSkill{del}";
                foreach (var ms in build.Milestones)
                {
                    buildStr += $"{ms.SPReq}{del}";
                }

                //Build skill lines

                //get list of Skills (TOP TO BOT)
                var skills = new List<string>();

                foreach (var msi in build.Milestones[0].MilestoneItems)
                {
                    skills.Add(msi.SkillID);
                }

                //Write a line for each skill
                for (int row = 0; row < skills.Count; row++)
                {
                    string rowString = $"\n{SkillInfoHandler.Skills.Where(x => x.TalentID == skills[row]).FirstOrDefault().Name}{del}";

                    for (int column = 0; column < build.Milestones.Count; column++)
                    {
                        rowString += $"{build.Milestones[column].MilestoneItems.Where(x => x.SkillID == skills[row]).FirstOrDefault().Amount}{del}";
                    }

                    buildStr += rowString;
                }

                #endregion

                return buildStr;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanExport error: {e.Message}");
                return "????";
            }
        }

        /// <summary>
        /// Puts the skill collections back together to a single upgraded list and returns it
        /// </summary>
        /// <returns><see cref="List{T}<see cref="SPOptSkill"/>"/> with result for every skill</returns>
        private List<SPOptSkill> CreateUpgradedSkillList()
        {
            var lst = new List<SPOptSkill>();
            foreach (var item in Optimizer.SelectedConfig.SkillSettings)
            {
                // get skill from currentState (now upgraded collection)
                var upgradedSkill = SPOptimizer.UnignoredTree.Where(x => x.TalentID == item.SkillId).FirstOrDefault();

                if (upgradedSkill == null)
                {
                    // skill is not in currentState tree. add item
                    lst.Add(item.MySPOptSkill.Clone());
                }
                else
                {
                    // take upgraded skill
                    lst.Add(upgradedSkill.Clone());
                }
            }

            return lst;
        }

        /// <summary>
        /// This method builds the SkillTree for visualization
        /// </summary>
        private void BuildTree(object o)
        {
            if (o == null)
            {
                return;
            }

            if (!(o is SKPaintSurfaceEventArgs args))
            {
                return;
            }

            // return if collection is empty
            if (UpgradeCollection.Count == 0)
            {
                return;
            }

            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            // divide the size by 4 to get the room a single tree can have
            int treeWidth = info.Width / 2;
            int treeHeight = info.Height / 2;

            // Create drawing info
            var drawingInfo = new BranchDrawingInfo(treeWidth, treeHeight, canvas);

            // Create tree
            var finalTree = CreateUpgradedSkillList();

            // draw skills
            drawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchRed").ToList());

            drawingInfo.SetCoordinates(treeWidth, 0f);
            drawingInfo.SetSkillColorFromBranchName("BranchYellow");
            drawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchYellow").ToList());

            drawingInfo.SetCoordinates(0f, treeHeight);
            drawingInfo.SetSkillColorFromBranchName("BranchBlue");
            drawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchBlue").ToList());

            drawingInfo.SetCoordinates(treeWidth, treeHeight);
            drawingInfo.SetSkillColorFromBranchName("BranchGreen");
            drawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchGreen").ToList());
        }
        #endregion
        
        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Analytics.TrackEvent("Module used", new Dictionary<string, string> { { "Name", "SP Optimizer" } });

                // Load configuration if passed in parameters. if not check for default one
                var config = await SPOptConfigFactory.LoadConfigAsync(App.DBRepo, App.Save, parameters.ContainsKey("id") ? parameters["id"].ToString() : LocalSettingsORM.DefaultSPConfiguration ?? "", true);


                if(config == null)
                {
                    Logger.WriteToLogFile("could not get config");
                }

                Logger.WriteToLogFile($"got {config.Name}, with  damage source {config.DamageSourceString}, gold source {config.GoldSourceString}, mode {config.ModeString} as config");

                Optimizer = new SPOptimizer(config, App.Save);
                Optimizer.OnLogMePlease += Optimizer_OnLogMePlease;
                if (OptimizeLogger.WriteLog)
                {
                    Optimizer.OnOptiLogMePlease += Optimizer_OnOptiLogMePlease;
                }

                Title = Optimizer.SelectedConfig.Name;

                bool initOkay = Optimizer.Initialize();

                if (initOkay)
                {
                    Logger.WriteToLogFile($"initialization completed successfully");

                    Optimizer.DoRun();

                    Logger.WriteToLogFile($"run completed");


                    AttentionText = Optimizer.AttentionText;
                    UpgradeCollection = new ObservableCollection<SPOptSkill>(Optimizer.UpgradeCollection);

                    // Change Color to reload canvas -.-
                    TreePseudoColor = SKColors.OrangeRed;
                }
                else
                {
                    Logger.WriteToLogFile($"initialization could not be completed successfully");

                    await _dialogService.DisplayAlertAsync(AppResources.AlertHeader, AppResources.EverySkillIgnoredAlert, AppResources.OKText);
                }
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"{e.Message} - {e.Data} - {(e.InnerException?.Message ?? "no inner ex")}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, e.Message, AppResources.OKText);
            }

            base.OnNavigatedTo(parameters);
        }

        private void Optimizer_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"{sender?.ToString()} -  {e.Information}");
        private void Optimizer_OnOptiLogMePlease(object sender, InformationEventArgs e) => OptimizeLogger.WriteToLogFile($"{sender?.ToString()} -  {e.Information}");
        #endregion
    }
}