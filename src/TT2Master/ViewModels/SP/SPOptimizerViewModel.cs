using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Follow SP Builds
    /// </summary>
    public class SPOptimizerViewModel : ViewModelBase
    {
        #region Properties
        private readonly string _defaultFallbackBuild = "_test";

        private readonly INavigationService _navigationService;

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private readonly bool _allFuncsAccess = PurchaseableItems.GetAllFuncsAccess();

        private bool _isBuildPassed = false;

        private bool _isRefreshing = false;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        private SPBuild _currentBuild;
        /// <summary>
        /// The current build
        /// </summary>
        public SPBuild CurrentBuild
        {
            get => _currentBuild;
            set
            {
                if(value != _currentBuild)
                {
                    SetProperty(ref _currentBuild, value);
                }
            }
        }

        private string _selectedBuild;
        /// <summary>
        /// Currently selected Build
        /// </summary>
        public string SelectedBuild
        {
            get => _selectedBuild;
            set
            {
                if (value == null)
                {
                    return;
                }

                if (_selectedBuild == null)
                {
                    SetProperty(ref _selectedBuild, value);
                }
                else if (_selectedBuild != value && value != CurrentBuild.ID)
                {
                    SetProperty(ref _selectedBuild, value, () => { OnBuildChanged(value); });
                }
            }
        }

        private string[] _availableBuilds = { };
        /// <summary>
        /// Array of Builds available for this player
        /// </summary>
        public string[] AvailableBuilds { get => _availableBuilds; set => SetProperty(ref _availableBuilds, value); }

        private bool _onlyRelevant;
        /// <summary>
        /// Show only relevant skills
        /// </summary>
        public bool OnlyRelevant
        {
            get => _onlyRelevant;
            set
            {
                if(value != _onlyRelevant && string.IsNullOrWhiteSpace(SelectedBuild))
                {
                    SetProperty(ref _onlyRelevant, value);
                }
                else if(value != _onlyRelevant && !string.IsNullOrWhiteSpace(SelectedBuild))
                {
                    SetProperty(ref _onlyRelevant, value, () => { OnNeedRefresh(); });

                }
            }
        }

        private string _milestoneProgressText;
        /// <summary>
        /// text that indicates where you stand in milestones
        /// </summary>
        public string MilestoneProgressText
        {
            get => _milestoneProgressText;
            set
            {
                if(value != _milestoneProgressText)
                {
                    SetProperty(ref _milestoneProgressText, value);
                }
            }
        }

        private string _currentSP;
        /// <summary>
        /// Current SP you have
        /// </summary>
        public string CurrentSP
        {
            get => _currentSP;
            set
            {
                if(value != _currentSP)
                {
                    SetProperty(ref _currentSP, value);
                }
            }
        }

        private string _availableSP;
        /// <summary>
        /// Available SP you can spend
        /// </summary>
        public string AvailableSP
        {
            get => _availableSP;
            set
            {
                if(value != _availableSP)
                {
                    SetProperty(ref _availableSP, value);
                }
            }
        }

        private SPBuildMilestone _currentMilestone;
        public SPBuildMilestone CurrentMilestone
        {
            get => _currentMilestone;
            set
            {
                if(value != _currentMilestone)
                {
                    SetProperty(ref _currentMilestone, value);
                }
            }
        }

        private ObservableCollection<SPPatch> _optimizeList;
        /// <summary>
        /// List of <see cref="SPPatch"/> with level up info
        /// </summary>
        public ObservableCollection<SPPatch> OptimizeList
        {
            get => _optimizeList;
            set
            {
                if (value != _optimizeList)
                {
                    SetProperty(ref _optimizeList, value);
                }
            }
        }

        /// <summary>
        /// Reloads Levels
        /// </summary>
        public ICommand ReloadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand BuildInfoCommand { get; private set; }
        public ICommand VersionInfoCommand { get; private set; }
        public ICommand OnlyRelevantInfoCommand { get; private set; }
        public ICommand MilestoneInfoCommand { get; private set; }
        public ICommand SPReqInfoCommand { get; private set; }
        public ICommand CurrSPInfoCommand { get; private set; }
        public ICommand AvailableSPInfoCommand { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public SPOptimizerViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = "SP Follow";

            CurrentBuild = new SPBuild();
            CurrentMilestone = new SPBuildMilestone();
            OptimizeList = new ObservableCollection<SPPatch>();

            ReloadCommand = new DelegateCommand(async () => await ReloadAsync());
            SaveCommand = new DelegateCommand(async () => await SaveAsync());
            DeleteCommand = new DelegateCommand(async () => await DeleteAsync());
            BuildInfoCommand = new DelegateCommand(async () => await BuildInfoExecute());
            VersionInfoCommand = new DelegateCommand(async () => await VersionInfoExecute());
            OnlyRelevantInfoCommand = new DelegateCommand(async () => await OnlyRelevantInfoExecute());
            MilestoneInfoCommand = new DelegateCommand(async () => await MilestoneInfoExecute());
            SPReqInfoCommand = new DelegateCommand(async () => await SPReqInfoExecute());
            CurrSPInfoCommand = new DelegateCommand(async () => await CurrSPInfoExecute());
            AvailableSPInfoCommand = new DelegateCommand(async () => await AvailableSPInfoExecute());
        }
        #endregion

        #region Command Methods
        private async Task<bool> DeleteAsync()
        {
            if (!CurrentBuild.Editable)
            {
                return false;
            }

            //is there something to delete?
            var storedBuild = await App.DBRepo.GetSPBuildByName(CurrentBuild.ID);

            if(storedBuild == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(storedBuild.ID))
            {
                return false;
            }

            #region delete
            //Get milestoneitems and delete them
            await App.DBRepo.DeleteSPBuildMilestoneItemByBuild(CurrentBuild.ID);
            //get milestones and delete them
            await App.DBRepo.DeleteSPBuildMilestoneByBuild(CurrentBuild.ID);

            //Delete Build
            await App.DBRepo.DeleteSPBuildByName(CurrentBuild.ID);
            #endregion

            //Alert User
            await ToastSender.SendToastAsync(AppResources.DeletedText, _dialogService);

            return true;
        }

        private async Task<bool> SaveAsync()
        {
            try
            {
                bool settingsSaved = SaveSettings();

                if (!CurrentBuild.Editable)
                {
                    return false;
                }
                if (!_allFuncsAccess)
                {
                    await ToastSender.SendToastAsync(AppResources.OnlyForSupporterInfoText, _dialogService);
                    return false;
                }

                #region delete
                //Get milestoneitems and delete them
                await App.DBRepo.DeleteSPBuildMilestoneItemByBuild(CurrentBuild.ID);
                //get milestones and delete them
                await App.DBRepo.DeleteSPBuildMilestoneByBuild(CurrentBuild.ID);

                //Delete Build
                await App.DBRepo.DeleteSPBuildByName(CurrentBuild.ID);
                #endregion

                #region insert
                foreach (var ms in CurrentBuild.Milestones)
                {
                    int mssaved = await App.DBRepo.UpdateSPBuildMilestoneAsync(ms);
                    Logger.WriteToLogFile($"{CurrentBuild.ID} milestone saved? {mssaved}");

                    int msisaved = 0;
                    //save milestoneitems
                    foreach (var item in ms.MilestoneItems)
                    {
                        msisaved += await App.DBRepo.UpdateSPBuildMilestoneItemAsync(item);
                    }

                    Logger.WriteToLogFile($"{CurrentBuild.ID} milestoneitems saved? {mssaved}");
                }

                //save build
                await App.DBRepo.UpdateSPBuildAsync(CurrentBuild);
                #endregion

                //Alert User
                await ToastSender.SendToastAsync(AppResources.SettingsSavedText, _dialogService);

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"SPOptimizerViewModel Error: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reloads Async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadAsync()
        {
            IsRefreshing = true;

            if(CurrentBuild.ID != SelectedBuild)
            {
                CurrentBuild = await App.DBRepo.GetSPBuildByName(SelectedBuild);
            }

            OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: checking selectedBuild");

            //if there is no build selected -> return
            if (string.IsNullOrWhiteSpace(SelectedBuild))
            {
                OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: selectedBuild is empty");
                IsRefreshing = false;
                return false;
            }

            OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: got selected build {SelectedBuild}. Saving settings now");

            //Save settings
            SaveSettings();

            //Load Save file -> Only SP builds
            OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: loading Save info");

            if (!await App.Save.Initialize(loadPlayer: false, loadAccountModel: false, loadArtifacts: false, loadClan: false, loadTournament: false, loadEquipment: false))
            {
                IsRefreshing = false;
                await _dialogService.DisplayAlertAsync("Info", "Could not load from save file", "ok");
                return false;
            }

            //Set current SP and available SP from Save file
            CurrentSP = SaveFile.SPReceived.ToString();
            AvailableSP = (SaveFile.SPReceived - SaveFile.SPSpent).ToString();

            OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: currentsp: {CurrentSP}, availablesp {AvailableSP}");

            //Load Skills
            SkillInfoHandler.OnProblemHaving += Child_OnProblemHaving;

            if (!SkillInfoHandler.LoadSkills())
            {
                IsRefreshing = false;
                OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: Could not load Skils");
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotLoadSkillsText, AppResources.OKText);
                SkillInfoHandler.OnProblemHaving -= Child_OnProblemHaving;
                return false;
            }

            OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: filling skills");
            SkillInfoHandler.FillSkills(App.Save);

            SkillInfoHandler.OnProblemHaving -= Child_OnProblemHaving;

            await Calculate();

            IsRefreshing = false;
            return true;
        }

        #region Info
        private async Task<bool> BuildInfoExecute() => await OpenPopupInfoAsync("Build", AppResources.BuildInfoText);

        private async Task<bool> VersionInfoExecute() => await OpenPopupInfoAsync(AppResources.VersionHeader, AppResources.VersionInfoText);

        private async Task<bool> OnlyRelevantInfoExecute() => await OpenPopupInfoAsync(AppResources.ShortHeader, AppResources.ShortInfoText);

        private async Task<bool> MilestoneInfoExecute() => await OpenPopupInfoAsync(AppResources.MilestoneHeader, AppResources.MilestoneInfoText);

        private async Task<bool> SPReqInfoExecute() => await OpenPopupInfoAsync("SPReq", string.Format(AppResources.SPReqInfoText, CurrentMilestone.SPReq));

        private async Task<bool> CurrSPInfoExecute() => await OpenPopupInfoAsync("CurrSP", AppResources.CurrSPInfoText);

        private async Task<bool> AvailableSPInfoExecute() => await OpenPopupInfoAsync(" AvailableSP", AppResources.AvailableSPInfoText);
        #endregion
        #endregion

        #region Helper
        /// <summary>
        /// Initializes List
        /// </summary>
        private void InitList()
        {
            OptimizeLogger.WriteToLogFile($"SPOptimizer.InitList: entry");
            OptimizeList = new ObservableCollection<SPPatch>();

            for (int i = 0; i < SkillInfoHandler.Skills.Count; i++)
            {
                OptimizeLogger.WriteToLogFile($"SPOptimizer.InitList: processing {SkillInfoHandler.Skills[i].TalentID}");
                OptimizeList.Add(new SPPatch(SkillInfoHandler.Skills[i]));
            }
        }

        /// <summary>
        /// Action for <see cref="CalculateCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Calculate()
        {
            //Reload Skills
            InitList();

            OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Initialized List");

            #region Getting values for build
            //fill milestones if not done
            if (CurrentBuild.Milestones.Count == 0)
            {
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: No Milestones loaded for {CurrentBuild.ID}. Doing it now");
                CurrentBuild.Milestones = await App.DBRepo.GetAllSPBuildMilestoneAsync(CurrentBuild.ID);
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: loaded {CurrentBuild.Milestones.Count} milestones");
            }

            OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Checking if Milestone-Items are loaded");
            //fill milestoneItems if not done
            foreach (var ms in CurrentBuild.Milestones)
            {
                if (ms.MilestoneItems.Count == 0)
                {
                    OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: no Milestone-Items for Milestone {ms.Milestone}. Loading...");

                    ms.MilestoneItems = await App.DBRepo.GetAllSPBuildMilestoneItemAsync(CurrentBuild.ID, ms.Milestone);

                    OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: got {ms.MilestoneItems.Count} Milestone-Items.");
                }
            } 
            #endregion

            int sprec = SaveFile.SPReceived;

            OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: setting received SP to {sprec}. Using this value to find fitting milestone");
            
            #region Getting current milestone
            CurrentMilestone = new SPBuildMilestone();

            try
            {
                CurrentMilestone = CurrentBuild.Milestones.Where(x => x.SPReq > sprec).FirstOrDefault();

                if (CurrentMilestone == null)
                {
                    OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Could not find Milestone that has more than {sprec} Spell Points.");
                }
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Could not find Milestone that has more than {sprec} Spell Points. ({ex.Message})");
            }

            //If not found, maybe the milestone has been passed?
            if (CurrentMilestone == null)
            {
                try
                {
                    CurrentMilestone = CurrentBuild.Milestones.Where(x => x.SPReq <= sprec).LastOrDefault();

                    if (CurrentMilestone == null)
                    {
                        OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Could not find Milestone that has less or equal than {sprec} Spell Points.");
                    }
                }
                catch (Exception ex)
                {
                    IsRefreshing = false;
                    OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Could not find Milestone that has less or equal than {sprec} Spell Points. ({ex.Message})");
                }
            }

            if (CurrentMilestone == null)
            {
                IsRefreshing = false;
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Could not find any Milestone. returning");
                return false; //should not pass this
            }

            OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Found Milestone {CurrentMilestone.Milestone}");

            //set MilestoneProgressText
            MilestoneProgressText = $"{CurrentMilestone.Milestone}/{CurrentBuild.Milestones.LastOrDefault().Milestone}";
            #endregion

            #region Creating List
            OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Setting SPPatch-Objects");
            for (int i = 0; i < OptimizeList.Count; i++)
            {
                var tmp = OptimizeList[i];
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Processing {tmp.TalentID}");

                tmp.CurrentMilestone = CurrentMilestone.Milestone;
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Setting {CurrentMilestone.Milestone} for {tmp.TalentID}");

                //try to find the skill in milestone
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Trying to find the skill in Milestone");
                try
                {
                    tmp.LevelToAmount = CurrentMilestone.MilestoneItems.Where(x => x.SkillID == tmp.TalentID).FirstOrDefault().Amount;
                }
                catch (Exception)
                {
                    IsRefreshing = false;
                    OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Could not find it. Setting it to -1");
                    tmp.CurrentMilestone = -1;
                }
            }

            OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Checking OnlyRelevant-Setting: {OnlyRelevant.ToString()}");
            if (OnlyRelevant)
            {
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: OnlyRelevant-Option turned on. Cleaning list");
                var cleanedList = OptimizeList.Where(x => x.CurrentMilestone >= 0 && x.LevelUpAmount != 0).ToList();
                OptimizeList = new ObservableCollection<SPPatch>(cleanedList);
            }
            else
            {
                OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: OnlyRelevant-Option turned off. not cleaning list");
            }

            #endregion

            OptimizeLogger.WriteToLogFile($"SPOptimizer.Calculate: Calculation finished.");
            return true;
        }

        /// <summary>
        /// Load Settings async
        /// </summary>
        private async Task<bool> LoadSettingsAsync(bool loadBuildDirectly = true)
        {
            OptimizeLogger.WriteToLogFile($"SPOptimizer.LoadSettings");

            string currBuildName;

            if (!loadBuildDirectly)
            {
                currBuildName = SelectedBuild;
            }
            else
            {
                //Default SP Build
                currBuildName = LocalSettingsORM.GetCurrSPBuildName() == "" ? _defaultFallbackBuild : LocalSettingsORM.GetCurrSPBuildName();
            }

            //check if stored value is valid
            if (string.IsNullOrWhiteSpace(currBuildName))
            {
                currBuildName = _defaultFallbackBuild;
            }

            //Only Relevant
            OnlyRelevant = LocalSettingsORM.GetOnlyRelevant();

            OptimizeLogger.WriteToLogFile($"SPOptimizer.LoadSettings: selected Build -> {currBuildName}");
            OptimizeLogger.WriteToLogFile($"SPOptimizer.LoadSettings: getting Build from DB");

            if (!AvailableBuilds.Contains(currBuildName))
            {
                OptimizeLogger.WriteToLogFile($"SPOptimizer.LoadSettings: selected Build does not exist. Setting it to \"\"");

                currBuildName = "";
            }

            if (string.IsNullOrWhiteSpace(currBuildName))
            {
                CurrentBuild = new SPBuild();
                SelectedBuild = CurrentBuild.ID;
            }

            else if (loadBuildDirectly)
            {
                CurrentBuild = await App.DBRepo.GetSPBuildByName(currBuildName);
                SelectedBuild = CurrentBuild.ID;
            }

            return true;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        private bool SaveSettings()
        {
            //only save current build id if it is valid
            //string[] savedBuilds = await App.DBRepo.GetSPBuildNamesArrayAsync();

            LocalSettingsORM.SetCurrSPBuildName(SelectedBuild);
            LocalSettingsORM.SetOnlyRelevant(OnlyRelevant);

            return true;
        }

        /// <summary>
        /// When a child has problems
        /// </summary>
        /// <param name="e"></param>
        private async void Child_OnProblemHaving(object sender, CustErrorEventArgs e)
        {
            await _dialogService.DisplayAlertAsync($"{sender.ToString()} Error", $"{e.MyException.Message}", "ok");
            Logger.WriteToLogFile($"{sender.ToString()} ERROR -> {e.MyException.Message} \n{e.MyException.Data}");
        }

        /// <summary>
        /// When Selected Build has changed - Reload CurrentBuild
        /// </summary>
        private async void OnBuildChanged(object obj)
        {
            if(obj == null)
            {
                return;
            }

            try
            {
                CurrentBuild = await App.DBRepo.GetSPBuildByName(obj.ToString());
                await ReloadAsync();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"SPOptimizerViewModel Error: {e.Message}");
            }
        }

        private async void OnNeedRefresh() => await ReloadAsync();

        /// <summary>
        /// Displays a Popup Message to the user
        /// </summary>
        /// <param name="header"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private async Task<bool> OpenPopupInfoAsync(string header, string info)
        {
            await _dialogService.DisplayAlertAsync(header, info, AppResources.OKText);

            return true;
        } 
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            OptimizeLogger.DeleteLogFile();
            OptimizeLogger.WriteToLogFile("Navigating to SP Optimizer: getting available Builds");
            //Load available Builds
            AvailableBuilds = await App.DBRepo.GetSPBuildNamesArrayAsync();

            OptimizeLogger.WriteToLogFile($"Navigating to SP Optimizer: got {AvailableBuilds.Length} items");

            if (parameters.ContainsKey("build"))
            {
                _isBuildPassed = true;
                CurrentBuild = parameters["build"] as SPBuild;

                var newBuilds = AvailableBuilds.ToList();

                if (!newBuilds.Contains(CurrentBuild.ID))
                {
                    newBuilds.Add(CurrentBuild.ID);
                }

                AvailableBuilds = newBuilds.ToArray();
                SelectedBuild = CurrentBuild.ID;
            }

            await LoadSettingsAsync(!_isBuildPassed);

            await ReloadAsync();

            base.OnNavigatedTo(parameters);
        }

        /// <summary>
        /// When navigating away
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            SaveSettings();

            base.OnNavigatedFrom(parameters);
        } 
        #endregion
    }
}