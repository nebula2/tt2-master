using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Plugin.Popups;
using System;
using System.Globalization;
using System.Linq;
using TT2Master.Resources;
using TT2Master.ViewModels.Arti;
using TT2Master.ViewModels.Assets;
using TT2Master.ViewModels.Dashboard;
using TT2Master.ViewModels.Raid;
using TT2Master.ViewModels.Reporting;
using TT2Master.Views.Arti;
using TT2Master.Views.Dashboard;
using TT2Master.Views.Raid;
using TT2Master.Views.Reporting;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using TT2Master.Views.Information;
using TT2Master.ViewModels.Information;
using TT2Master.Model;
using System.Threading;
using TT2Master.Views.Equip;
using TT2Master.ViewModels.Equip;
using TT2Master.Views.Clan;
using TT2Master.Views.Identity;
using TT2Master.ViewModels.Identity;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TT2Master
{
     /// <summary>
    /// Father of evil
    /// </summary>
    public partial class App : PrismApplication
    {
        #region Properties
        /// <summary>
        /// Savefile
        /// </summary>
        public static SaveFile Save { get; private set; }
        /// <summary>
        /// Path to database
        /// </summary>
        public static string DBFilePath { get; private set; }
        /// <summary>
        /// Database repository
        /// </summary>
        public static DBRepository DBRepo { get; private set; }
        private static string _currentTTVersion;
        /// <summary>
        /// Current version of Tap Titans
        /// </summary>
        public static string CurrentTTVersion
        {
            get => _currentTTVersion;
            set => _currentTTVersion = value;
        }

        private static bool _isUserOnline = true;
        /// <summary>
        /// Is the user online?
        /// </summary>
        public static bool IsUserOnline { get => _isUserOnline; set => _isUserOnline = value; }

        private static string _storedAppVersion = "";
        /// <summary>
        /// Stored app version
        /// </summary>
        public static string StoredAppVersion
        {
            get => _storedAppVersion;
            set
            {
                _storedAppVersion = value;
                SaveAppVersion(value);
            }
        }

        private static bool _shouldIShowChangelog = false;
        /// <summary>
        /// if true show Changelog
        /// </summary>
        public static bool ShouldIShowChangelog { get => _shouldIShowChangelog; set => _shouldIShowChangelog = value; }

        private static Language _currentLanguage;
        /// <summary>
        /// The current language
        /// </summary>
        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    SaveCurrentLanguage(value);
                }
            }
        }

        /// <summary>
        /// The current app version
        /// </summary>
        public static string CurrentAppVersion { get; private set; } = LocalSettingsORM.GetCurrentTTMasterVersion();

        private static bool _convertNumbersScientific = true;
        /// <summary>
        /// True, if numbers should be converted to scientific
        /// </summary>
        public static bool ConvertNumbersScientific
        {
            get => _convertNumbersScientific;
            set
            {
                _convertNumbersScientific = value;
                SaveConvertNumbersScientific(value);
            }
        }

        /// <summary>
        /// Has the asset error been shown yet?
        /// </summary>
        public static bool HasShownAssetError { get; set; } = false;

        /// <summary>
        /// True if the big ad has been shown in this session
        /// </summary>
        public static bool HaveIShownTheBigAd { get; set; } = false;

        public static InstallationSourceResult InstallationSourceInfo { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public App() : this(null) { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="initializer"></param>
        public App(IPlatformInitializer initializer = null) : base(initializer) { }
        #endregion

        #region Helper

        /// <summary>
        /// Loads AppVersion from Settings
        /// </summary>
        private static void LoadAppVersion() => StoredAppVersion = LocalSettingsORM.AppVersion;

        /// <summary>
        /// Saves Value from StoredAppVersion to Settings
        /// </summary>
        private static void SaveAppVersion(string value) => LocalSettingsORM.AppVersion = value;

        /// <summary>
        /// Saves Value from ConvertNumbersScientific
        /// </summary>
        private static void SaveConvertNumbersScientific(bool value) => LocalSettingsORM.SetConvertNumbersScientific(value);

        /// <summary>
        /// saves given language in appsettings
        /// </summary>
        /// <param name="value"></param>
        private static void SaveCurrentLanguage(Language value)
        {
            LocalSettingsORM.SetCurrentLanguage(value.ShortName);
            ResetLanguage(value);
        }

        /// <summary>
        /// Loads the current language from appsettings
        /// </summary>
        public static void LoadCurrentLanguage()
        {
            CurrentLanguage = SupportedLanguages.Languages.Where(x => x.ShortName == LocalSettingsORM.GetCurrentLanguage()).FirstOrDefault();

            ResetLanguage(CurrentLanguage);
        }

        /// <summary>
        /// Resets the language
        /// </summary>
        /// <param name="value"></param>
        private static void ResetLanguage(Language value)
        {
            try
            {
                var culture = new CultureInfo(value.ShortName);
                AppResources.Culture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Loads ConvertNumbersScientific from Settings
        /// </summary>
        public static void LoadConvertNumbersScientificSetting() => ConvertNumbersScientific = LocalSettingsORM.GetConvertNumbersScientific();

        /// <summary>
        /// Loads CurrentTTVersion from Settings
        /// </summary>
        private static void LoadCurrentTTVersion() => CurrentTTVersion = LocalSettingsORM.CurrentTTVersion;


        protected override async void OnInitialized()
        {
            InitializeComponent();

            Init();

            await NavigationService.NavigateAsync("SplashPage");
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=65d63e4b-5ae2-4847-9ece-a9182f1b84ee;" +
                  "uwp={Your UWP App secret here};" +
                  "ios=963d33bb-f13a-4dc2-9b17-824d9fd87962;",
                  typeof(Analytics), typeof(Crashes));

            HaveIShownTheBigAd = false;

            base.OnStart();
        }

        protected override void OnResume()
        {
            HaveIShownTheBigAd = false;
            base.OnResume();
        }

        /// <summary>
        /// Initialize application wide stuff that should only be handled here
        /// </summary>
        private static void Init()
        {
            DBFilePath = DependencyService.Get<IDBPath>().DBPath("tt2master.db3");
            DBRepo = new DBRepository(DBFilePath);

            Save = new SaveFile(DBRepo);

            LoadCurrentTTVersion();
            LoadAppVersion();
            LoadCurrentLanguage();

            Xamarin.Forms.DataGrid.DataGridComponent.Init();
        }

        /// <summary>
        /// Register all stuff
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Always give view and viewmodel so no recursion is used (performance)

            #region Navigation
            containerRegistry.RegisterForNavigation<DefaultNavigationPage, DefaultNavigationViewModel>();
            containerRegistry.RegisterForNavigation<MyMasterDetailPage, MyMasterDetailViewModel>();
            containerRegistry.RegisterForNavigation<SplashPage, SplashViewModel>();
            #endregion

            #region Dashboard
            containerRegistry.RegisterForNavigation<DashboardPage, DashboardViewModel>();
            containerRegistry.RegisterForNavigation<DashboardConfigPage, DashboardConfigViewModel>();
            #endregion

            #region Artifact
            containerRegistry.RegisterForNavigation<BuildsPage, BuildsViewModel>();
            containerRegistry.RegisterForNavigation<EditBuildPage, EditBuildViewModel>();
            containerRegistry.RegisterForNavigation<ArtOptImageGridPage, ArtOptImageGridViewModel>();
            containerRegistry.RegisterForNavigation<ArtOptVisualSettingsPage, ArtOptVisualSettingsViewModel>();
            containerRegistry.RegisterForNavigation<ArtifactOverviewPage, ArtifactOverviewViewModel>();
            containerRegistry.RegisterForNavigation<ArtifactDetailPage, ArtifactDetailViewModel>();
            #endregion

            #region SP
            containerRegistry.RegisterForNavigation<SPOptimizerPage, SPOptimizerViewModel>();
            containerRegistry.RegisterForNavigation<SPOptPage, SPOptViewModel>();
            containerRegistry.RegisterForNavigation<SPOptResultPage, SPOptResultViewModel>();
            containerRegistry.RegisterForNavigation<SPBuildsPage, SPBuildsViewModel>();
            containerRegistry.RegisterForNavigation<SPSplashCalculatorPage, SPSplashCalculatorViewModel>();
            containerRegistry.RegisterForNavigation<SPOptConfigurationsPage, SPOptConfigurationsViewModel>();
            containerRegistry.RegisterForNavigation<SPOptConfigurationDetailPage, SPOptConfigurationDetailViewModel>();
            #endregion

            #region Clan
            containerRegistry.RegisterForNavigation<ClanOverviewPage, ClanOverviewViewModel>();
            containerRegistry.RegisterForNavigation<ClanMemberOverviewPage, ClanMemberOverviewViewModel>();
            containerRegistry.RegisterForNavigation<ClanMemberDetailPage, ClanMemberDetailViewModel>();
            containerRegistry.RegisterForNavigation<ClanMessagesOverviewPage, ClanMessagesOverviewViewModel>();
            containerRegistry.RegisterForNavigation<BannedPlayerPage, BannedPlayerViewModel>();
            containerRegistry.RegisterForNavigation<MemberComparisonPage, MemberComparisonViewModel>();
            #endregion

            #region Statistics
            containerRegistry.RegisterForNavigation<StatisticsPage, StatisticsViewModel>();
            containerRegistry.RegisterForNavigation<SnapshotPage, SnapshotViewModel>();
            #endregion

            #region Equipment
            containerRegistry.RegisterForNavigation<EquipAdvisorPage, EquipAdvisorViewModel>();
            containerRegistry.RegisterForNavigation<CraftingAdvisorPage, CraftingAdvisorViewModel>();
            #endregion

            #region Automation
            containerRegistry.RegisterForNavigation<ClanAutoExportPage, ClanAutoExportViewModel>();
            containerRegistry.RegisterForNavigation<AutomationServicePage, AutomationServiceViewModel>();
            #endregion

            #region Identity
            containerRegistry.RegisterForNavigation<IdentityConnectPage, IdentityConnectViewModel>();
            #endregion

            #region Informational
            containerRegistry.RegisterForNavigation<InfoPage, InfoViewModel>();
            containerRegistry.RegisterForNavigation<ChangesPage, ChangesViewModel>();
            containerRegistry.RegisterForNavigation<ChangelogInfoPage, ChangelogInfoVM>();
            containerRegistry.RegisterForNavigation<EditBuildsInfoPage, EditBuildsInfoVM>();
            containerRegistry.RegisterForNavigation<ImportChatBuildsInfoPage, ImportChatBuildsInfoVM>();
            containerRegistry.RegisterForNavigation<LinksInfoPage, LinksInfoVM>();
            containerRegistry.RegisterForNavigation<OptimizerInfoPage, OptimizerInfoVM>();
            containerRegistry.RegisterForNavigation<SPFollowerInfoPage, SPFollowerInfoVM>();
            containerRegistry.RegisterForNavigation<StatisticsInfoPage, StatisticsInfoVM>();
            containerRegistry.RegisterForNavigation<WidgetInfoPage, WidgetInfoVM>();
            containerRegistry.RegisterForNavigation<HowToLogPage, HowToLogViewModel>();
            #endregion

            #region Tournament
            containerRegistry.RegisterForNavigation<TournamentMembersPage, TournamentMembersViewModel>();
            #endregion

            #region Reporting
            containerRegistry.RegisterForNavigation<ReportPage, ReportViewModel>();
            containerRegistry.RegisterForNavigation<ClanMemberCompleteReportPage, ClanMemberCompleteReportViewModel>();
            containerRegistry.RegisterForNavigation<ClanMemberRaidStatsReportPage, ClanMemberRaidStatsReportViewModel>();
            containerRegistry.RegisterForNavigation<ClanMemberBaseStatsReportPage, ClanMemberBaseStatsReportViewModel>();

            #endregion

            #region Raids
            containerRegistry.RegisterForNavigation<ClanRaidDetailPage, ClanRaidDetailViewModel>();
            containerRegistry.RegisterForNavigation<ClanRaidOverviewPage, ClanRaidOverviewViewModel>();
            containerRegistry.RegisterForNavigation<RaidStrategyOverviewPage, RaidStrategyOverviewViewModel>();
            containerRegistry.RegisterForNavigation<RaidStrategyDetailPage, RaidStrategyDetailViewModel>();
            containerRegistry.RegisterForNavigation<RaidToleranceOverviewPage, RaidToleranceOverviewViewModel>();
            containerRegistry.RegisterForNavigation<RaidToleranceDetailPage, RaidToleranceDetailViewModel>();
            containerRegistry.RegisterForNavigation<RaidSeedPage, RaidSeedViewModel>();
            #endregion

            #region Others
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<AboutPage, AboutViewModel>();
            containerRegistry.RegisterForNavigation<PurchasePage, PurchaseViewModel>();
            containerRegistry.RegisterForNavigation<PrivacyPolicyPage, PrivacyPolicyViewModel>();
            containerRegistry.RegisterForNavigation<ExportPage, ExportViewModel>();
            containerRegistry.RegisterForNavigation<AssetInfoPage, AssetInfoViewModel>();
            containerRegistry.RegisterForNavigation<AnnouncementPage, AnnouncementViewModel>();
            containerRegistry.RegisterForNavigation<AnnouncementPopupPage, AnnouncementPopupVM>();
            #endregion

            #region Popups
            // This updates INavigationService and registers PopupNavigation.Instance
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterForNavigation<MyPopupPage>();
            containerRegistry.RegisterForNavigation<ChangelogPopupPage, ChangelogPopupVM>();
            containerRegistry.RegisterForNavigation<BanPlayerPopupPage, BanPlayerPopupViewModel>();
            containerRegistry.RegisterForNavigation<ArtOptSettingsPopupPage, ArtOptSettingsPopupViewModel>();
            containerRegistry.RegisterForNavigation<EquipDetailPopupPage, EquipDetailPopupViewModel>();
            containerRegistry.RegisterForNavigation<EquipAdvSetPopupPage, EquipAdvSetPopupViewModel>();
            containerRegistry.RegisterForNavigation<ClanMsgExportPopupPage, ClanMsgExportViewModel>();
            containerRegistry.RegisterForNavigation<ClanExportPopupPage, ClanExportViewModel>();
            containerRegistry.RegisterForNavigation<RaidAttackResultPopupPage, RaidAttackResultPopupViewModel>();
            containerRegistry.RegisterForNavigation<PlayerComparePickerPopupPage, PlayerComparePickerPopupViewModel>();
            containerRegistry.RegisterForNavigation<RenameClanRaidStuffPopupPage, RenameClanRaidStuffPopupViewModel>();
            containerRegistry.RegisterForNavigation<TournamentResultPopupPage, TournamentResultPopupViewModel>();
            #endregion
        }
        #endregion
    }
}