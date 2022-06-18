using Microcharts;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json.Linq;
using Plugin.Clipboard;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Interfaces;
using TT2Master.Loggers;
using TT2Master.Model;
using TT2Master.Model.Arti;
using TT2Master.Model.Assets;
using TT2Master.Model.Dashboard;
using TT2Master.Model.DataSource;
using TT2Master.Model.Information;
using TT2Master.Model.Navigation;
using TT2Master.Model.Tournament;
using TT2Master.Resources;
using TT2Master.Shared.Models;
using TT2Master.Views.Dashboard;
using TT2Master.Views.Information;
using TT2Master.Views.Reporting;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TT2Master
{
    public class DashboardViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<DashboardItem> _infoItems;
        public ObservableCollection<DashboardItem> InfoItems { get => _infoItems; set => SetProperty(ref _infoItems, value); }

        private ObservableCollection<DashboardShortcut> _shortcuts = new ObservableCollection<DashboardShortcut>();
        public ObservableCollection<DashboardShortcut> Shortcuts { get => _shortcuts; set => SetProperty(ref _shortcuts, value); }

        private string _playerName;
        public string PlayerName { get => _playerName; set => SetProperty(ref _playerName, value); }

        private string _clanMemberText;
        public string ClanMemberText { get => _clanMemberText; set => SetProperty(ref _clanMemberText, value); }

        private double _currentStage = 1;
        public double CurrentStage { get => _currentStage; set => SetProperty(ref _currentStage, value); }

        private double _maxStage = 1;
        public double MaxStage { get => _maxStage; set => SetProperty(ref _maxStage, value); }

        private string _currMsPercentageString = "0 %";
        public string CurrMsPercentageString
        {
            get => _currMsPercentageString;
            set => SetProperty(ref _currMsPercentageString, value);
        }

        private double _currMsPercentage = 0;
        public double CurrMsPercentage
        {
            get => _currMsPercentage;
            set => SetProperty(ref _currMsPercentage, value);
        }

        private double _prestigeToday = 0;
        public double PrestigeToday { get => _prestigeToday; set => SetProperty(ref _prestigeToday, value); }

        private string _prestigeTodayText = "";
        public string PrestigeTodayText { get => _prestigeTodayText; set => SetProperty(ref _prestigeTodayText, value); }

        private bool _hasUnseenAnnouncements = false;
        public bool HasUnseenAnnouncements { get => _hasUnseenAnnouncements; set => SetProperty(ref _hasUnseenAnnouncements, value); }
        
        private bool _hasNoUnseenAnnouncements = false;
        public bool HasNoUnseenAnnouncements { get => _hasNoUnseenAnnouncements; set => SetProperty(ref _hasNoUnseenAnnouncements, value); }

        private bool _isLoadingDataFromSavefile;
        public bool IsLoadingDataFromSavefile { get => _isLoadingDataFromSavefile; set => SetProperty(ref _isLoadingDataFromSavefile, value); }

        private bool _isShowingClipboardDataImportInfo;
        public bool IsShowingClipboardDataImportInfo { get => _isShowingClipboardDataImportInfo; set => SetProperty(ref _isShowingClipboardDataImportInfo, value); }

        private INavigationService _navigationService;
        private IPageDialogService _dialogService;

        public ICommand NavigateCommand { get; private set; }
        public ICommand EnterClanCommand { get; private set; }
        public ICommand ReloadCommand { get; private set; }
        public ICommand OpenWarningsCommand { get; private set; }
        public ICommand GoToSettingsCommand { get; private set; }
        public ICommand ResetPrestigeCounterCommand { get; private set; }
        public ICommand ImportSfFromClipboardCommand { get; private set; }
        public ICommand ChangeSfCommand { get; private set; }
        
        public ICommand AnnouncementCommand { get; private set; }

        private bool _smallAdVisible = false;
        public bool SmallAdVisible { get => _smallAdVisible; set => SetProperty(ref _smallAdVisible, value); }

        private int _smallAdHeightRequest;
        public int SmallAdHeightRequest { get => _smallAdHeightRequest; set => SetProperty(ref _smallAdHeightRequest, value); }

        #region ProgressBoard
        private string _msIncreaseText = "";
        public string MsIncreaseText { get => _msIncreaseText; set => SetProperty(ref _msIncreaseText, value); }

        private List<ChartEntry> _entries;
        public List<ChartEntry> Entries { get => _entries; set => SetProperty(ref _entries, value); }

        private LineChart _lChart;
        public LineChart LChart { get => _lChart; set => SetProperty(ref _lChart, value); }

        private int _statTimeId = 0;
        public int StatTimeId
        {
            get => _statTimeId;
            set => SetProperty(ref _statTimeId, value);
        }

        private bool _isWarningVisible = false;
        public bool IsWarningVisible { get => _isWarningVisible; set => SetProperty(ref _isWarningVisible, value); }

        private List<StatTime> _times = StatTimes.Times;
        public List<StatTime> Times { get => _times; set => SetProperty(ref _times, value); }

        private List<Snapshot> _snapshots = new List<Snapshot>();
        #endregion

        /// <summary>
        /// Big Ad
        /// </summary>
        private IAdInterstitial _adInterstitial;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public DashboardViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _adInterstitial = Xamarin.Forms.DependencyService.Get<IAdInterstitial>();

            Title = AppResources.DashboardHeader;

            //Only show small ad when noad or supporter was not purchased
            SmallAdVisible = PurchaseableItems.GetSmallAdVisible();
            SmallAdHeightRequest = SmallAdVisible
                ? 50
                : 0;

            InitializeCommands();
        }
        #endregion

        #region Command Methods
        private void InitializeCommands()
        {
            NavigateCommand = new DelegateCommand<object>(async (o) => await NavigateAsync(o));

            ReloadCommand = new DelegateCommand(async () => await ReloadAsync(true));
            EnterClanCommand = new DelegateCommand(async () => await EnterClanAsync());
            OpenWarningsCommand = new DelegateCommand(async () => await OpenWarningsAsync());
            GoToSettingsCommand = new DelegateCommand(async () => await NavigateToSettingsAsync());
            ResetPrestigeCounterCommand = new DelegateCommand(async () => await ResetPrestigeCounterAsync());
            AnnouncementCommand = new DelegateCommand(async () => await NavigateToAnnouncementsAsync());
            ImportSfFromClipboardCommand = new DelegateCommand(async () => await ImportSfFromClipboardAsync());
            ChangeSfCommand = new DelegateCommand(async () => await ChangeSfExecuteAsync());
        }

        private async Task<bool> NavigateAsync(object o)
        {
            if (o == null)
            {
                return false;
            }

            var item = o as DashboardItem;

            var result = await _navigationService.NavigateAsync(item.Destination);
            var success = (result as Prism.Navigation.NavigationResult).Success;
            Logger.WriteToLogFile($"Navigation Result: \n{success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return success;
        }

        private async Task<bool> OpenWarningsAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<DashboardPage, AssetInfoPage>());

            var success = (result as Prism.Navigation.NavigationResult).Success;
            Logger.WriteToLogFile($"Navigation Result: \n{success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return success;
        }

        private async Task<bool> NavigateToSettingsAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<DashboardPage, DashboardConfigPage>());

            var success = (result as Prism.Navigation.NavigationResult).Success;
            Logger.WriteToLogFile($"Navigation Result: \n{success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return success;
        }

        private async Task<bool> ResetPrestigeCounterAsync()
        {
            // check if there is anything to reset
            if (PrestigeToday == 0)
            {
                return true;
            }

            // promt for reset
            var response = await _dialogService.DisplayAlertAsync(AppResources.Choose, AppResources.DoYouWantToResetPrestigeAmount, AppResources.YesText, AppResources.No);

            try
            {
                // reset by making previous snapshot amount equal to current state
                if (_snapshots.Count == 1)
                {
                    Logger.WriteToLogFile("ERROR ResetPrestigeCounterCommand: there is only one snapshot");
                    return true;
                }

                var sn = _snapshots.OrderByDescending(x => x.Timestamp).Take(2).ToList().Last();

                var memberSn = sn.MemberSnapshotItems.Where(x => x.PlayerId == App.Save.ThisPlayer.PlayerId).First();

                memberSn.PrestigeCount = App.Save.ThisPlayer.PrestigeCount;
                await App.DBRepo.UpdateMemberSnapshotItemAsync(memberSn);

                // reload
                await ReloadAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR ResetPrestigeCounterCommand: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }
        }

        private async Task<bool> NavigateToAnnouncementsAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<DashboardPage, ChangesPage>());

            var success = (result as Prism.Navigation.NavigationResult).Success;
            Logger.WriteToLogFile($"Navigation Result dashboard -> announcement: \n{success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return success;
        }

        private async Task<bool> ImportSfFromClipboardAsync()
        {
            try
            {
                var importer = new ClipboardSfImporter(_dialogService);

                var result = await importer.ImportSfFromClipboardAsync(false);

                if (result.IsSuccessful)
                {
                    // reload stuff and force snapshot creation (ReloadAsync)
                    await ReloadAsync(true, true);
                }

                Analytics.TrackEvent("Module used", new Dictionary<string, string> { { "Name", "Import SF from clipboard" } });

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ImportSfFromClipboardAsync ERROR: {ex.Message} - {ex.Data}");
                return false;
            }
        }

        private async Task<bool> ChangeSfExecuteAsync()
        {
            try
            {
                string msg = LocalSettingsORM.IsDefaultSavefileSelected
                    ? "Want to switch to abyssal save?"
                    : "Want to switch to default save?";

                var result = await _dialogService.DisplayAlertAsync("Change savefile", msg, AppResources.Yes, AppResources.No);

                if (!result)
                {
                    return true;
                }

                LocalSettingsORM.IsDefaultSavefileSelected = !LocalSettingsORM.IsDefaultSavefileSelected;

                // todo ensure savefile is existing
                var sfh = new SavefileHandler(_dialogService);
                sfh.OnLogMePlease += SnapshotFactory_OnLogMePlease;
                sfh.OnProblemHaving += Sfh_OnProblemHaving;
                var success = await sfh.LoadSavefileAsync();
                sfh.OnLogMePlease -= SnapshotFactory_OnLogMePlease;
                sfh.OnProblemHaving -= Sfh_OnProblemHaving;

                if (!success)
                {
                    LocalSettingsORM.IsDefaultSavefileSelected = !LocalSettingsORM.IsDefaultSavefileSelected;
                    return false;
                }

                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.PleaseRestartTheAppForChangesToTakeEffect, AppResources.OKText);

                // reload stuff and force snapshot creation (ReloadAsync)
                await ReloadAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ChangeSfExecuteAsync ERROR: {ex.Message} - {ex.Data}");
                return false;
            }
        }

        /// <summary>
        /// Reloads everything
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadAsync(bool reloadAll = false, bool createSnapshot = false)
        {
            if (reloadAll)
            {
                Logger.WriteToLogFile($"ReloadAsync. true passed.");
                bool saveOk = await App.Save.Initialize(loadAccountModel: false);
                App.Save.CalculateCurrentRelics();
                
                #region Snapshot creation
                if (LocalSettingsORM.IsDefaultSavefileSelected && (LocalSettingsORM.IsCreatingSnapshotOnDashboardReload || createSnapshot))
                {
                    Logger.WriteToLogFile("DashboardViewModel: checking snapshots");
                    SnapshotFactory.OnLogMePlease += SnapshotFactory_OnLogMePlease;
                    bool result = await SnapshotFactory.CreateDailySnapshotAsync();
                    SnapshotFactory.OnLogMePlease -= SnapshotFactory_OnLogMePlease;
                    Logger.WriteToLogFile("DashboardViewModel: done checking snapshots");
                }
                #endregion
            }


            await LoadShortcuts();

            IsWarningVisible = AssetManager.AssetTypes.Where(x => x.AssetState != AssetDownloadResult.SuccessfulAssetDownload).Count() > 0;

            InitProgressBar();

            InitLists();

            bool listinit = await UpdateChartsView();

            Logger.WriteToLogFile($"ReloadAsync finished");
            return true;
        }

        private async Task<bool> EnterClanAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath < DashboardPage, ClanOverviewPage>());
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }
        #endregion

        #region Helper
        private void ShowBigAd()
        {
            Logger.WriteToLogFile($"Dashboard ShowBigAd start");
            if (!PurchaseableItems.GetBigAdVisible())
            {
                Logger.WriteToLogFile($"Dashboard ShowBigAd: due to purchasements i will not show any ad.");
                LogAdvertisementEventToAppCenter("Ad not shown because of purchasements");
                return;
            }

            if (!Xamarin.Forms.DependencyService.Get<IGetWindowMode>().IsFullScreen())
            {
                Logger.WriteToLogFile($"Dashboard ShowBigAd: App is not in full screen so i will not show any ad.");
                LogAdvertisementEventToAppCenter("Ad not shown because not in full screen");
                return;
            }

            Logger.WriteToLogFile($"Dashboard ShowBigAd going to load and show ad");
            LogAdvertisementEventToAppCenter();

            _adInterstitial.LoadAd();
            _adInterstitial.ShowAd();
        }

        private void InitShortcuts()
        {
            try
            {
                Shortcuts = new ObservableCollection<DashboardShortcut>();

                DashboardShortcutHandler.LoadShortcutConfig();

                foreach (var item in DashboardShortcutHandler.ShortcutConfig)
                {
                    // get shortcut item
                    var sc = DashboardShortcutHandler.AvailableShortcuts.Where(x => x.ShortcutId == item.ShortcutId).FirstOrDefault();

                    if (sc == null)
                    {
                        Logger.WriteToLogFile($"ERROR: InitShortcuts: could not find shortcut with id {item.ShortcutId}");
                        continue;
                    }

                    var ctorParams = new object[]
                    {
                        _navigationService,
                        _dialogService,
                    };

                    var scInstance = Activator.CreateInstance(sc.ShortcutType, ctorParams);

                    Shortcuts.Add((DashboardShortcut)scInstance);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: InitShortcuts: {ex.Message}\n{ex.Data}");
            }
        }

        private async Task<bool> LoadShortcuts()
        {
            try
            {
                foreach (var item in Shortcuts)
                {
                    if(item.LoadItem != null)
                    {
                        await item.LoadItem();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: LoadShortcuts: {ex.Message}\n{ex.Data}");
                return false;
            }
        }

        /// <summary>
        /// Updates the Charts
        /// </summary>
        private async Task<bool> UpdateChartsView()
        {
            try
            {
                Logger.WriteToLogFile($"UpdateChartsView: Getting snapshots");
                _snapshots = await App.DBRepo.GetAllSnapshotAsync();

                Logger.WriteToLogFile($"UpdateChartsView: Received {_snapshots.Count} snapshots.");

                // get relevant snapshots grouped by date and ordered by timestamp ascending
                var relevantSnaps = _snapshots
                    .Where(x => x.Timestamp >= DateTime.Now.AddDays(-Times[StatTimeId].Days))
                    .GroupBy(g => g.Timestamp.ToString("yyyyMMdd"))
                    .Select(s => s.First())
                    .OrderBy(n => n.Timestamp).ToList();

                Logger.WriteToLogFile($"UpdateChartsView: got {relevantSnaps.Count} relevant snapshots. loading details");
                foreach (var item in relevantSnaps)
                {
                    item.MemberSnapshotItems = await App.DBRepo.GetAllMemberSnapshotItemAsync(item.ID);
                }
                Logger.WriteToLogFile($"UpdateChartsView: fetched all member snapshots from db");

                _snapshots = relevantSnaps;

                Entries = new List<ChartEntry>();
                var userSnaps = new List<MemberSnapshotItem>();

                //get item for this clanmember
                for (int i = 0; i < relevantSnaps.Count; i++)
                {
                    var user = relevantSnaps[i].MemberSnapshotItems?.Where(x => x.PlayerId == App.Save.ThisPlayer?.PlayerId).FirstOrDefault();

                    if (user == null)
                    {
                        Logger.WriteToLogFile($"UpdateChartsView: user is null for snapshot {i}");
                        continue;
                    }
                    else
                    {
                        Logger.WriteToLogFile($"UpdateChartsView: added user");
                        userSnaps.Add(user);
                    }
                }

                Logger.WriteToLogFile($"UpdateChartsView: populating entries with userSnaps ({userSnaps.Count})");

                //populate entries
                for (int i = 0; i < userSnaps.Count; i++)
                {
                    Logger.WriteToLogFile($"UpdateChartsView: adding entry {i}");
                    //Add entry
                    Entries.Add(new ChartEntry((float)userSnaps[i].StageMax)
                    {
                        Color = SKColor.Parse("#23B391"),
                        ValueLabel = userSnaps[i].StageMax.ToString(),
                        Label = relevantSnaps[i].Timestamp.ToString("MM.dd"),
                    });
                }

                Logger.WriteToLogFile($"UpdateChartsView: populating LChart");

                LChart = new LineChart()
                {
                    Entries = Entries,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelOrientation = Orientation.Horizontal,
                    Margin = 30,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };

                if (userSnaps.Count > 0)
                {
                    double minVal = userSnaps[0].StageMax;
                    double maxVal = userSnaps[userSnaps.Count - 1].StageMax;

                    LChart.MinValue = (float)minVal;
                    LChart.MaxValue = (float)maxVal;

                    Logger.WriteToLogFile($"UpdateChartsView: Setting MsIncreaseText with {maxVal} - {minVal}");

                    MsIncreaseText = string.Format(AppResources.IncreasedXStages, (maxVal - minVal));
                }
                else
                {
                    MsIncreaseText = string.Format(AppResources.IncreasedXStages, "?");
                }

                Logger.WriteToLogFile($"UpdateChartsView: Going to set todays prestige amount");
                bool prestigeAmountInit = await SetTodaysPrestigeCountAsync();

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Dash.UpdateChartsView Error:{e.Message}");
                return false;
            }
        }

        private async Task<bool> SetTodaysPrestigeCountAsync()
        {
            try
            {
                if (!IsLoadingDataFromSavefile)
                {
                    return true;
                }

                var lastSnap = await App.DBRepo.GetLastNotTodaySnapshotAsync();

                if (lastSnap == null)
                {
                    PrestigeToday = 0;
                    PrestigeTodayText = AppResources.NoPrestigeData;
                }

                else if (lastSnap.ID < 1)
                {
                    PrestigeToday = 0;
                    PrestigeTodayText = AppResources.NoPrestigeData;
                }
                else
                {
                    //get member data from snapshot
                    lastSnap.MemberSnapshotItems = await App.DBRepo.GetAllMemberSnapshotItemAsync(lastSnap.ID);

                    if (lastSnap.MemberSnapshotItems == null)
                    {
                        Logger.WriteToLogFile($"SetTodaysPrestigeCountAsync Error. Could not get lastSnap.MemberSnapshotItems: {App.DBRepo.StatusMessage}");
                        PrestigeToday = 0;
                        PrestigeTodayText = AppResources.NoPrestigeData;
                        return false;
                    }

                    // get relevant member snapshot
                    var relSnap = lastSnap.MemberSnapshotItems.Where(x => x.PlayerId == App.Save.ThisPlayer?.PlayerId).FirstOrDefault();

                    if (relSnap == null)
                    {
                        Logger.WriteToLogFile($"SetTodaysPrestigeCountAsync Error. Could not get relSnap: {App.DBRepo.StatusMessage}");
                        return false;
                    }

                    if (relSnap.PrestigeCount == 0)
                    {
                        PrestigeToday = 0;
                        PrestigeTodayText = AppResources.NoPrestigeData;
                        return false;
                    }

                    //set prestige count
                    PrestigeToday = relSnap == null || App.Save.ThisPlayer == null ? 0 : App.Save.ThisPlayer.PrestigeCount - relSnap.PrestigeCount;

                    if((DateTime.Now - lastSnap.Timestamp).TotalDays == 1)
                    {
                        PrestigeTodayText = string.Format(AppResources.PrestigeTodayText, PrestigeToday.ToString());
                    }
                    else
                    {
                        PrestigeTodayText = string.Format(AppResources.PrestigeSinceLastRecord, PrestigeToday.ToString());
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"SetTodaysPrestigeCountAsync Error: {e.Message}");
                return false;
            }
        }

        private void InitLists()
        {
            if (!IsLoadingDataFromSavefile)
            {
                return;
            }

            InfoItems = new ObservableCollection<DashboardItem>();

            var archHandler = new AchievmentHandler();
            archHandler.ReloadAchievments();

            foreach (var item in archHandler.Achievments)
            {
                if (!item.DailyAchievementCollected)
                {
                    InfoItems.Add(new DashboardItem()
                    {
                        Area = DashboardArea.ShortTerm,
                        Header = item.CurrentDailyAchievement,
                        Content = $"{item.CurrentDailyAchievementProgress} / {item.CurrentDailyAchievementTotal}",
                        Destination = "DailyQuestPage",
                        Icon = "",
                        ID = item.CurrentDailyAchievement,
                    });
                }
            }

            if (archHandler.Achievments.Where(x => !x.DailyAchievementCollected).Count() == 0)
            {
                InfoItems.Add(new DashboardItem()
                {
                    Area = DashboardArea.ShortTerm,
                    Header = "Daily Achievements completed",
                    Content = "",
                    Destination = "DailyQuestPage",
                    Icon = "",
                    ID = "DailyCompleted",
                });
            }

            try
            {
                string dropsStr = App.Save.SaveObject["EquipmentModel"]["extraEquipmentDrops"].ToString();

                if (dropsStr == null)
                {
                    InfoItems.Add(new DashboardItem()
                    {
                        Area = DashboardArea.Information,
                        Header = "Equip Drops: Got all",
                        Content = "",
                        Destination = "DailyDropPage",
                        Icon = "",
                        ID = "DropsCompleted",
                    });
                }
                else
                {
                    var dropsObj = JObject.Parse(dropsStr);
                    var dropsToken = dropsObj.GetValue("$content");
                    int[] dropsArray = dropsToken.Values<int>().ToArray();

                    int[] drops = dropsArray.Distinct().ToArray();

                    if(drops.Count() == 0)
                    {
                        InfoItems.Add(new DashboardItem()
                        {
                            Area = DashboardArea.Information,
                            Header = "Equip Drops: Got all",
                            Content = "",
                            Destination = "DailyDropPage",
                            Icon = "",
                            ID = "DropsCompleted",
                        });
                    }
                    else
                    {
                        InfoItems.Add(new DashboardItem
                        {
                            Area = DashboardArea.Information,
                            Header = "Equip Drops:",
                            Content = string.Join("\n", drops),
                            Destination = "DailyDropPage",
                            Icon = "",
                            ID = "DropsCompleted",
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Dash.LoadDrops Error:{e.Message}");
            }
        }

        private void InitProgressBar()
        {
            try
            {
                CurrentStage = App.Save.ThisPlayer?.CurrentStage ?? 0;
                MaxStage = App.Save.ThisPlayer?.StageMax ?? 0;

                double startingStage = App.Save.StartingStage > 0 ? App.Save.StartingStage : FormulaHelper.StartingStage();

                double stageFrom = Math.Max(App.Save.ThisPlayer?.CurrentStage ?? 0 - startingStage, 1);
                double stageTo = Math.Max(App.Save.ThisPlayer?.StageMax ?? 0 - startingStage, 1);

                CurrMsPercentage = stageFrom / stageTo;
                CurrMsPercentageString = $"{(int)(stageFrom * 100 / stageTo)} %";

            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Dash.InitProgBar Error:{e.Message}");
            }
        }

        private async Task<bool> ShowUpdateRequiredAnnouncementAsync()
        {
            HasUnseenAnnouncements = AnnouncementHandler.IsHavingUnseenItems;
            HasNoUnseenAnnouncements = !HasUnseenAnnouncements;

            if (!AnnouncementHandler.IsHavingUpdateRequiredItem)
            {
                return true;
            }

            // show announcement popup
            var result = await _navigationService.NavigateAsync("AnnouncementPopupPage");
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            if(!result.Success)
            {
                // could not navigate. at least show some info and get outta here
                var response = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, "There is a new version available. Please update this app", AppResources.OKText, AppResources.CancelText);
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().CloseApplication();
            }

            return true;
        }

        private void LoadClanInfo()
        {
            // values to set
            try
            {
                PlayerName = App.Save.ThisPlayer?.PlayerName ?? "";

                if (App.Save.ThisClan == null)
                {
                    ClanMemberText = "0";
                }
                else if (App.Save.ThisClan.ClanMember == null)
                {
                    ClanMemberText = "0";
                }
                else
                {
                    ClanMemberText = App.Save.ThisClan.ClanMember.Count.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Dash.PlayerName and ClanmemberText:{e.Message}");
            }
        }


        private void LogAdvertisementEventToAppCenter(string eventName = "Advertisement request")
        {
            try
            {
                var dict = new Dictionary<string, string>()
            {
                {"OS", Device.RuntimePlatform },
                {"Data Source", LocalSettingsORM.IsReadingDataFromSavefile ? "Savefile" : "Clipboard" },
            };
                Analytics.TrackEvent("Advertisement request", dict);
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR Could not log advertisement event: {ex.Message} - {ex.Data}");
            }
        }
        #endregion

        #region E + D
        private void SnapshotFactory_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => Logger.WriteToLogFile($"{sender} - {e.Information}");

        private async void Sfh_OnProblemHaving(object sender, Helpers.CustErrorEventArgs e)
        {
            Logger.WriteToLogFile($"{sender.ToString()} ERROR: {e.MyException.Message} - {e.MyException.Data}");
            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters != null && parameters.ContainsKey("loadAds"))
            {
                //Only show ad directly when safe file is data source
                if (LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    ShowBigAd();
                }
            }

            IsLoadingDataFromSavefile = LocalSettingsORM.IsReadingDataFromSavefile;
            IsShowingClipboardDataImportInfo = !App.Save.IsHavingSavefileContent && !LocalSettingsORM.IsReadingDataFromSavefile;

            Logger.WriteToLogFile($"DashboardVM Loading ClanInfo");
            LoadClanInfo();

            Logger.WriteToLogFile($"DashboardVM Initializing Shortcuts");
            InitShortcuts();

            Logger.WriteToLogFile($"DashboardVM Doing one reload to have current data");
            bool isLoaded = await ReloadAsync();

            // show required announcements
            try
            {
                Logger.WriteToLogFile($"DashboardVM Checking for required updates");
                await ShowUpdateRequiredAnnouncementAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"DashboardVM ERROR OnNavigatedTo -> ShowUpdateRequiredAnnouncementAsync logic:{ex.Message}\n{ex.Data}");
            }

            // check if we need to see changelog
            try
            {
                Logger.WriteToLogFile($"DashboardVM Checking if i need to show changelog");
                if (App.ShouldIShowChangelog)
                {
                    var result = await _navigationService.NavigateAsync("ChangelogPopupPage", new NavigationParameters() { { "id", ChangelogList.Changelog.Max(x => x.Version) } });
                    Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                    App.ShouldIShowChangelog = false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"DashboardVM ERROR OnNavigatedTo -> Changelog/ Advertisement logic:{ex.Message}\n{ex.Data}");
            }

            // check if we need to make popup due to shitty asset state
            try
            {
                Logger.WriteToLogFile($"DashboardVM Checking if I need to make a popup due to invalid asset state");
                if (!App.HasShownAssetError && AssetManager.AssetTypes.Where(x => x.AssetState != AssetDownloadResult.SuccessfulAssetDownload).Count() > 0)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.Problem, AppResources.CheckAssetState, AppResources.OKText);
                    App.HasShownAssetError = true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"DashboardVM ERROR OnNavigatedTo -> Asset unsuccessful download logic:{ex.Message}\n{ex.Data}");
            }

#if RELEASE
            try
            {
                Logger.WriteToLogFile($"DashboardVM Checking installation source");
                if (!App.InstallationSourceInfo.IsOfficialStoreInstallation)
                {
                    Logger.WriteToLogFile($"-> invalid");
                    await _dialogService.DisplayAlertAsync(AppResources.Caution, "it seems like this app has been installed from an unknown source. \r\nAccess will be restricted!", AppResources.OKText);
                }
                Logger.WriteToLogFile($"-> valid");
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"Error showing unofficial info: {ex.Message}");
            } 
#endif

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}