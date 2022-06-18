using System.IO;
using Plugin.Connectivity;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Mailing;
using TT2Master.Model.Assets;
using TT2Master.Resources;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Model.Arti;
using TT2Master.Model.Dashboard;
using Xamarin.Essentials;
using TT2Master.Model.Information;
using TT2Master.Model;
using TT2Master.Interfaces;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// VM for Splash Screen
    /// </summary>
    public class SplashViewModel : ViewModelBase
    {
        #region private member
        /// <summary>
        /// <see cref="ProgressText"/>
        /// </summary>
        private string _progressText;

        /// <summary>
        /// <see cref="InitializationCompleted"/>
        /// </summary>
        private bool _initializationCompleted = false;

        private string _logoImage = "logo_clean";
        public string LogoImage { get => _logoImage; set => SetProperty(ref _logoImage, value); }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        /// <summary>
        /// True, if <see cref="App.CurrentAppVersion"/> == <see cref="App.StoredAppVersion"/>
        /// If false, I need to update a few thingies
        /// </summary>
        private bool _appValuesNotUpToDate = false;
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public SplashViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Indicates if initialization is complete
        /// </summary>
        public bool InitializationCompleted
        {
            get => _initializationCompleted;
            private set
            {
                if (value != _initializationCompleted)
                {
                    SetProperty(ref _initializationCompleted, value);
                    ProgressText = AppResources.StartMainAppText;
                    Logger.WriteToLogFile("SplashVM: starting Main App");
                    
                    SaveFile.OnProgressMade -= OnProgressMade;
                    var result = NavigationService.NavigateAsync(new Uri($"app:///" + NavigationConstants.NavigationPath<DashboardPage>(), UriKind.Absolute)
                        , new NavigationParameters() { { "loadAds", true } }).Result;

                    if (!result.Success)
                    {
                        Logger.WriteToLogFile($"Navigation to app failed - i will reset dashboard config: \n{result.Exception}");
                        DashboardShortcutHandler.RestoreDefault();
                        OnProblemHaving?.Invoke(result.Exception);
                    }
                }
            }
        }

        /// <summary>
        /// Progresstext for display-purpose
        /// </summary>
        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }
        #endregion

        #region Private methods

        #region Helper
        /// <summary>
        /// Check for Permission
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="ask"></param>
        /// <param name="warn"></param>
        /// <returns></returns>
        private async Task<bool> Check<T>(bool ask = true, bool warn = true) where T : Permissions.BasePermission, new()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<T>();

                if (status != PermissionStatus.Granted && ask)
                {
                    status = await Permissions.RequestAsync<T>();
                }

                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                await ProcessCriticalProblemsAsync(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Processes a critical Problem in the initialization.
        /// App is closing after this is done
        /// </summary>
        /// <param name="s">string for Exception - should be passed to view</param>
        /// <param name="p">string for progresstext</param>
        private async Task<bool> ProcessCriticalProblemsAsync(string s, string p = "")
        {
            //Set p to s if p is not set
            if (string.IsNullOrWhiteSpace(p))
            {
                p = s;
            }

            ProgressText = p;
            OnProblemHaving?.Invoke(new Exception(s));

            Logger.WriteToLogFile("SplashVM (ProcessCriticalProblemsAsync): " + s + " " + p);

            await Task.Yield();
            bool problem = await _dialogService.DisplayAlertAsync(AppResources.AlertHeader, s + AppResources.KeepUpText, AppResources.SendToDevText, AppResources.DontCareText);

            if (problem)
            {
                try
                {
                    var sender = new EmailSender();
                    bool sent = await sender.SendErrorEmail();
                }
                catch (Exception e) 
                {
                    Logger.WriteToLogFile($"ProcessCriticalProblemsAsync Error: {e.Message}\n{e.Data}");
                }
            }

            Environment.Exit(0);

            //pseudo
            return false;
        }

        /// <summary>
        /// Checks if default builds are stored in db. If not - it creates them
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckDefaultBuilds()
        {
            ProgressText = AppResources.CheckingDefaultBuildsText;
            Logger.WriteToLogFile("SplashVM: Checking default artifact builds. Abo on DefaultBuildFactory.OnProblemHaving");

            DefaultBuildFactory.OnProblemHaving += DefaultBuildFactory_OnProblemHaving;
            DefaultBuildFactory.OnProgressMade += OnProgressMade;

            if (AssetManager.IsDefaultBuildRebuildRequired)
            {
                Logger.WriteToLogFile("SplashVM: because the app version is not up to date i will recreate default builds");
                bool success = await DefaultBuildFactory.RecreateAllDefaultBuildsAsync();

                if (!success)
                {
                    await ProcessCriticalProblemsAsync(AppResources.CouldNotRecreateDefaultBuildsText);
                    DefaultBuildFactory.OnProblemHaving -= DefaultBuildFactory_OnProblemHaving;
                    DefaultBuildFactory.OnProgressMade -= OnProgressMade;
                    return false;
                }
            }

            Logger.WriteToLogFile("SplashVM: deabo on DefaultBuildFactory.OnProblemHaving");
            DefaultBuildFactory.OnProblemHaving -= DefaultBuildFactory_OnProblemHaving;
            DefaultBuildFactory.OnProgressMade -= OnProgressMade;
            return true;
        }

        /// <summary>
        /// Checks if there are any banned players and creates a notification of so
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckBannedPlayersAsync()
        {
            try
            {
                string[] banned = await App.DBRepo.GetBannedPlayersArrayAsync();

                if(banned == null)
                {
                    return true;
                }

                if(banned.Length == 0)
                {
                    return true;
                }

                foreach (var item in App.Save?.ThisClan?.ClanMember)
                {
                    item.IsBanned = banned.Contains(item.PlayerId);
                }

                //Notify banned
                if (App.Save?.ThisClan?.ClanMember?.Count > 0)
                {
                    string[] bannedPlayers = App.Save.ThisClan.ClanMember.Where(x => x.IsBanned).Select(n => n.PlayerName).ToArray();

                    if (bannedPlayers != null && bannedPlayers.Length > 0)
                    {
                        string notification = bannedPlayers[0];
                        for (int i = 1; i < bannedPlayers.Length; i++)
                        {
                            notification += ", " + bannedPlayers[i];
                        }
                        Xamarin.Forms.DependencyService.Get<ISendNotification>().ShowNotification(AppResources.AlertHeader, $"{AppResources.BannedPlayersInClan}\n{notification}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"CheckBannedPlayersAsync() ERROR: {ex.Message}");
                return false;
            }
        }

        private async Task SetForcedDataSourceSetting()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                Logger.WriteToLogFile("I am a iOS device. Can not load data from savefile");
                LocalSettingsORM.IsReadingDataFromSavefile = false;
            }

            else if(Device.RuntimePlatform == Device.Android)
            {
                int v = Xamarin.Forms.DependencyService.Get<IGetClientOSVersion>().GetClientOSVersion();
                Logger.WriteToLogFile($"Android version is: {v}");
                if (v >= 30)
                {
                    if (!LocalSettingsORM.IsDataFromSavefileDescisionMade)
                    {
                        Logger.WriteToLogFile($"asking user for data source");

                        var result = await _dialogService.DisplayActionSheetAsync("Data source setting"
                            , "Do you wish to load data from savefile or clipboard?"
                            , AppResources.Yes
                            , AppResources.No);

                        // promt user
                        string response = await _dialogService.DisplayActionSheetAsync("Data source setting"
                        , AppResources.CancelText
                        , AppResources.DestroyText
                        , new string[] {
                            "Savefile"
                            , AppResources.Clipboard
                        });

                        // default in clipboard loading. only take savefile as data source on Android R and above if explicitly chosen
                        LocalSettingsORM.IsReadingDataFromSavefile = response == "Savefile";
                        LocalSettingsORM.IsDataFromSavefileDescisionMade = true;
                    }
                }
            }
        }

        private async Task LoadSavefileAsync()
        {
            await SetForcedDataSourceSetting();
            
            var sfHandler = new SavefileHandler(_dialogService);
            sfHandler.OnLogMePlease += Save_OnLogMePlease;
            sfHandler.OnProgressMade += OnProgressMade;
            sfHandler.OnProblemHaving += ProblemHandler;

            sfHandler.InitPaths();
            _ = await sfHandler.LoadSavefileAsync();

            sfHandler.OnLogMePlease -= Save_OnLogMePlease;
            sfHandler.OnProgressMade -= OnProgressMade;
            sfHandler.OnProblemHaving -= ProblemHandler;
        }

        private async Task SecureAssets()
        {
            #region Check Network
            ProgressText = "Checking Network accessibility";
            //Check for Network
            App.IsUserOnline = CrossConnectivity.Current.IsConnected;
            #endregion

            #region Info file checks
            ProgressText = "Checking Assets";
            AssetManager.OnLogMePlease += Save_OnLogMePlease;
            AssetManager.OnError += ProblemHandler;

            _ = await AssetManager.CheckAndDownloadAssetsAsync();

            // do we have all needed files? if not we have a serious issue here
            if (!await AssetManager.IsAssetStateSecure())
            {
                await ProcessCriticalProblemsAsync(string.Format(AppResources.UnstableAssetState
                    , Enum.GetName(typeof(AssetDownloadResult), AssetDownloadResult.TotalFuckUp).TranslatedString())
                    , "AssetManager state is insecure.");
            }

            AssetManager.OnLogMePlease -= Save_OnLogMePlease;
            AssetManager.OnError -= ProblemHandler;

            AssetReader.OnProblemHaving += ProblemHandler;
            AssetReader.InitializeAssetNameHeaderMappings();
            AssetReader.OnProblemHaving -= ProblemHandler;
            #endregion
        }
        #endregion

        private async Task<bool> EnsureCorrectArtStepAmountAsync()
        {
            //Load Settings
            var settings = await App.DBRepo.GetArtOptSettingsByID("1");

            if (settings == null)
            {
                return true;
            }

            if (settings.StepAmountId <= ArtStepAmounts.StepAmounts.Count)
            {
                return true;
            }

            try
            {
                settings.StepAmountId = 0;
                App.DBRepo.StatusMessage = "";
                int res = await App.DBRepo.UpdateArtOptSettingsAsync(settings);
                Logger.WriteToLogFile($"EnsureCorrectArtStepAmountAsync: Fixed artifact optimizer settings. Last DB Status {App.DBRepo.StatusMessage}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"EnsureCorrectArtStepAmountAsync ERROR: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Initializes async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InitializeAsync()
        {
            try
            {
                #region Checking Permissions
                ProgressText = AppResources.CheckingPermissionsText;
                //Check Permissions
                if (!await Check<Permissions.StorageRead>())
                {
                    await ProcessCriticalProblemsAsync(AppResources.PermissionNotGrantedText);
                    return false;
                }
                if (!await Check<Permissions.StorageWrite>())
                {
                    await ProcessCriticalProblemsAsync(AppResources.PermissionNotGrantedText);
                    return false;
                }

                // above android 11 check for manage external storage permission
                // https://developer.android.com/preview/privacy/storage?hl=da#all-files-access

                #endregion

                #region Log-Init
                //Delete old log file
                Logger.DeleteLogFile();
                Logger.WriteToLogFile("SplashVM: Done checking permissions");

                Logger.WriteToLogFile($"Starting InitializeAsync. DateTime: {DateTime.Now}. StoredAppVersion: {App.StoredAppVersion}. Current AppVersion: {App.CurrentAppVersion}.");
                Logger.WriteToLogFile($"Data Source: {(LocalSettingsORM.IsReadingDataFromSavefile ? "Savefile" : "Clipboard")}");

                #endregion

                #region Check if App is up to date
                _appValuesNotUpToDate = App.StoredAppVersion != App.CurrentAppVersion;
                Logger.WriteToLogFile($"Starting InitializeAsync. StoredAppVersion != Current AppVersion: {_appValuesNotUpToDate}.");
                #endregion

                #region prevent fuckery
                _ = await EnsureCorrectArtStepAmountAsync();                
                #endregion

                #region Loading SaveFile
                await LoadSavefileAsync();
                #endregion

                await SecureAssets();

                #region Load Artifacts
                Logger.WriteToLogFile("SplashVM: Loading Artifacts");
                ProgressText = "Loading artifacts";

                ArtifactHandler.OnProblemHaving += ProblemHandler;

                bool artsLoaded = ArtifactHandler.LoadArtifacts();
                if (!artsLoaded)
                {
                    ArtifactHandler.OnProblemHaving -= ProblemHandler;

                    // TODO this is temporary!!! Remove this when proper backend available
                    LocalSettingsORM.AppVersion = "";
                    LocalSettingsORM.CurrentTTVersion = "";

                    await ProcessCriticalProblemsAsync("Artifacts could not be initialized");
                }
                Logger.WriteToLogFile("SplashVM: Done loading Artifacts");
                ArtifactHandler.OnProblemHaving -= ProblemHandler;
                #endregion

                #region Load Skills
                Logger.WriteToLogFile("SplashVM: Loading SP Builds");
                ProgressText = "Loading Skills";

                SkillInfoHandler.OnProblemHaving += ProblemHandler;

                bool spbuildsLoaded = SkillInfoHandler.LoadSkills();
                if (!spbuildsLoaded)
                {
                    SkillInfoHandler.OnProblemHaving -= ProblemHandler;

                    // TODO this is temporary!!! Remove this when proper backend available
                    LocalSettingsORM.AppVersion = "";
                    LocalSettingsORM.CurrentTTVersion = "";

                    await ProcessCriticalProblemsAsync("SP builds could not be initialized");
                }
                Logger.WriteToLogFile("SplashVM: Done loading SP builds");
                SkillInfoHandler.OnProblemHaving -= ProblemHandler;

                // call fill skills in order to populate sp recieved when loading from export.
                SkillInfoHandler.FillSkills(App.Save);
                #endregion

                #region Check Artifact Builds
                Logger.WriteToLogFile("SplashVM: checking default builds");
                await CheckDefaultBuilds();
                Logger.WriteToLogFile("SplashVM: Done checking default builds");
                #endregion

                #region Init Equipment
                Logger.WriteToLogFile("SplashVM: checking equipment");
                bool eqInit = EquipmentHandler.Load();
                Logger.WriteToLogFile("SplashVM: done checking equipment");
                #endregion

                #region Checking Purchasement
                Logger.WriteToLogFile("SplashVM: checking purchases");
                ProgressText = AppResources.CheckingPurchasesText;
                await Purchasement.CheckPurchases(true, _dialogService);
                Logger.WriteToLogFile("SplashVM: Done checking purchases");
                #endregion

                #region UpgradeAppVersion
                Logger.WriteToLogFile("SplashVM: Upgrading stored App Version if needed");
                if (_appValuesNotUpToDate)
                {
                    App.ShouldIShowChangelog = true;
                    App.StoredAppVersion = App.CurrentAppVersion;
                    Logger.WriteToLogFile("SplashVM: Upgrading stored App Version");
                }
                #endregion

                #region Load Number Conversion Setting
                Logger.WriteToLogFile("SplashVM: loading conversion settings");
                App.LoadConvertNumbersScientificSetting();
                Logger.WriteToLogFile("SplashVM: done loading conversion settings");
                #endregion

                #region Take daily snapshot
                if (LocalSettingsORM.IsDefaultSavefileSelected)
                {
                    Logger.WriteToLogFile("SplashVM: checking snapshots");
                    SnapshotFactory.OnLogMePlease += Save_OnLogMePlease;
                    
                    bool result = await SnapshotFactory.CreateDailySnapshotAsync();
                    
                    SnapshotFactory.OnLogMePlease -= Save_OnLogMePlease;
                    Logger.WriteToLogFile("SplashVM: done checking snapshots");
                }
                #endregion

                #region Save ClanMessages
                Logger.WriteToLogFile("SplashVM: saving clan messages");
                ClanMessageFactory.OnLogMePlease += Save_OnLogMePlease;
                bool clanMsgResult = await ClanMessageFactory.SaveNewClanMessages();
                ClanMessageFactory.OnLogMePlease -= Save_OnLogMePlease;
                Logger.WriteToLogFile("SplashVM: done saving clan messages");
                #endregion

                #region Check for banned players
                _ = await CheckBannedPlayersAsync();
                #endregion

                #region Auto export
                // auto export
                if (LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    Xamarin.Forms.DependencyService.Get<IStartClanAutoExport>().StartService();
                }
                #endregion

                #region Announcements
                try
                {
                    if(await AnnouncementHandler.DownloadAnnouncementsAsync())
                    {
                        Logger.WriteToLogFile($"downloaded announcements");
                    }
                    else
                    {
                        Logger.WriteToLogFile($"something strange happened with the announcement thingie");
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteToLogFile($"Error with announcement stuff: {ex.Message} {ex.Data}");
                }
                #endregion

                #region ATT
                _ = await Xamarin.Forms.DependencyService.Get<IAskForAttPermission>().AskUserAsync();
                #endregion

                #region Installation Source check
#if RELEASE
                InstallationSourceResult.Init();
                App.InstallationSourceInfo = Xamarin.Forms.DependencyService.Get<IGetInstallationSource>().GetInstallationSource();
                string instLogString = $"Installation source info: Installer: {App.InstallationSourceInfo.Installer ?? "null"} IsOfficial: {App.InstallationSourceInfo.IsOfficialStoreInstallation} Information: {App.InstallationSourceInfo.Information ?? "?"}";
                Logger.WriteToLogFile(instLogString);
                Analytics.TrackEvent("Installation source", App.InstallationSourceInfo.ToDict());
#endif
#if DEBUG
                InstallationSourceResult.Init();
                App.InstallationSourceInfo = new InstallationSourceResult("com.android.vending")
                {
                    Information = "Debugging",
                };
#endif
                #endregion

                SaveFile.OnLogMePlease -= Save_OnLogMePlease;
                ProgressText = "Finished Initialization";
                InitializationCompleted = true;

                return true;
            }
            catch (Exception e)
            {
                await ProcessCriticalProblemsAsync($"Error in SplashVM: {e.Message} \n\n{e.Data}\n\n");
                return false;
            }
        }
#endregion

        #region Navigation override
        /// <summary>
        /// Overrides OnNavigatedTo for async synchronization
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            bool initfinished = await InitializeAsync();

            Logger.WriteToLogFile($"Init finished: {initfinished}");

            base.OnNavigatedTo(parameters);
        }

#endregion

        #region events and delegates
        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(Exception e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public event HoustonWeGotAProblem OnProblemHaving;

        /// <summary>
        /// Processes OnProgressMade from <see cref="DefaultBuildFactory"/>
        /// </summary>
        /// <param name="message"></param>
        private void OnProgressMade(object sender, InformationEventArgs e)
        {
            ProgressText = e.Information;
            Logger.WriteToLogFile($"OnProgressMade {sender.ToString()}: {e.Information}");
        }

        /// <summary>
        /// Logs stuff from Save
        /// </summary>
        /// <param name="message"></param>
        private void Save_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"OnLogMePlease {sender.ToString()}: {e.Information}");

        /// <summary>
        /// Handles Error
        /// </summary>
        /// <param name="e"></param>
        private async void DefaultBuildFactory_OnProblemHaving(object sender, CustErrorEventArgs e) => await ProcessCriticalProblemsAsync("Error", $"{sender.ToString()}: {e.MyException.Message}");

        /// <summary>
        /// Errorhandler
        /// </summary>
        /// <param name="e"></param>
        private async void ProblemHandler(object sender, CustErrorEventArgs e)
        {
            Logger.WriteToLogFile($"{sender.ToString()} Error {e.MyException.Message}: {e.MyException.Data}");
            await ProcessCriticalProblemsAsync(e.MyException.Message);
        }
#endregion
    }
}