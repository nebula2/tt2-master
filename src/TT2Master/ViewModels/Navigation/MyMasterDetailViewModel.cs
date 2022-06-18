using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Navigation;
using TT2Master.Resources;
using TT2Master.Views.Equip;
using TT2Master.Views.Raid;
using TT2Master.Views.Reporting;

namespace TT2Master
{
    public class MyMasterDetailViewModel : ViewModelBase
    {
        #region Properties
        readonly INavigationService _navigationService;
        public ICommand NavigateCommand { get; private set; }
        public ICommand ExpandCommand { get; private set; }
        public ICommand EnterSettingsCommand { get; private set; }

        private ObservableCollection<MenuItemGroup> _allItemsList = new ObservableCollection<MenuItemGroup>();

        private ObservableCollection<MenuItemGroup> _menuList = new ObservableCollection<MenuItemGroup>();
        public ObservableCollection<MenuItemGroup> MenuList { get => _menuList; set => SetProperty(ref _menuList, value); }
        #endregion

        #region Ctor
        public MyMasterDetailViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            NavigateCommand = new DelegateCommand<object>(async (o) =>
            {
                if (o == null)
                {
                    return;
                }

                var item = o as MasterPageItem;

                await _navigationService.NavigateAsync(NavigationConstants.DefaultPath + item.Destination);
            });

            EnterSettingsCommand = new DelegateCommand(async () => await EnterSettingsAsync());

            ExpandCommand = new DelegateCommand<object>((o) =>
           {
               if (o == null)
               {
                   return;
               }

               var item = o as MenuItemGroup;
               _allItemsList.Where(x => x.Title == item.Title).FirstOrDefault().InvertExpanded();
               UpdateListContent();
           });

            InitMenuList();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Load List of Master Page Item
        /// </summary>
        private void InitMenuList()
        {
            _allItemsList = new ObservableCollection<MenuItemGroup>()
            {
                new MenuItemGroup("Dashboard", true)
                {
                    new MasterPageItem()
                    {
                        Title = "Dashboard",
                        Destination = typeof(DashboardPage).Name,
                        Icon = "\uf009",
                    },
                },
                new MenuItemGroup(AppResources.MenuOptimizerTitle, false)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.MenuOptimizerTitle,
                        Destination = typeof(ArtOptImageGridPage).Name,
                        Icon = "\uf1ec",
                    },
                    new MasterPageItem()
                    {
                        Title = "Builds",
                        Destination = typeof(BuildsPage).Name,
                        Icon = "\uf24e",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.MenuArtifactOverviewTitle,
                        Destination = typeof(ArtifactOverviewPage).Name,
                        Icon = "\uf02d",
                    },
                },
                new MenuItemGroup(AppResources.Skills, false)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.SPOptimizer,
                        Destination = typeof(SPOptPage).Name,
                        Icon = "\uf471",
                    },
                    new MasterPageItem()
                    {
                        Title = "SP Follower",
                        Destination = typeof(SPOptimizerPage).Name,
                        IsEnabled = LocalSettingsORM.IsReadingDataFromSavefile,
                    },
                    new MasterPageItem()
                    {
                        Title = "SP Builds",
                        Destination = typeof(SPBuildsPage).Name,
                        Icon = "\uf0e3",
                        IsEnabled = LocalSettingsORM.IsReadingDataFromSavefile,
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.SPSplashCalculator,
                        Destination = typeof(SPSplashCalculatorPage).Name,
                        Icon = "",
                        IsEnabled = LocalSettingsORM.IsReadingDataFromSavefile,
                    },
                },
                new MenuItemGroup(AppResources.Equipment, false)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.EquipAdvisor,
                        Destination = typeof(EquipAdvisorPage).Name,
                        Icon = "\uf21d",
                        IsEnabled = LocalSettingsORM.IsReadingDataFromSavefile,
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.CraftingAdvisor,
                        Destination = typeof(CraftingAdvisorPage).Name,
                        Icon = "\uf6e3",
                    },
                },
                new MenuItemGroup(AppResources.Clan, false, LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.ClanTitle,
                        Destination = typeof(ClanOverviewPage).Name,
                        Icon = "\uf1ae",
                    },

                    new MasterPageItem()
                    {
                        Title = AppResources.ClanMessagesTitle,
                        Destination = typeof(ClanMessagesOverviewPage).Name,
                        Icon = "",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.ClanMemberTitle,
                        Destination = typeof(ClanMemberOverviewPage).Name,
                        Icon = "",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.BannedPlayersTitle,
                        Destination = typeof(BannedPlayerPage).Name,
                        Icon = "",
                    },
                },
                new MenuItemGroup(AppResources.RaidHeader, false)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.ClanRaidHeader,
                        Destination = typeof(ClanRaidOverviewPage).Name,
                        Icon = "",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.Strategies,
                        Destination = typeof(RaidStrategyOverviewPage).Name,
                        Icon = "",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.Tolerances,
                        Destination = typeof(RaidToleranceOverviewPage).Name,
                        Icon = "",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.RaidSeedHeader,
                        Destination = typeof(RaidSeedPage).Name,
                        Icon = "",
                    },
                },
                new MenuItemGroup(AppResources.Reporting, false, LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.Reporting,
                        Destination = typeof(ReportPage).Name,
                    }
                },
                new MenuItemGroup(AppResources.Automation, false, LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.ClanAutoExportTitle,
                        Destination = typeof(ClanAutoExportPage).Name,
                        Icon = "\uf56e",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.AutomationService,
                        Destination = typeof(AutomationServicePage).Name,
                        Icon = "\uf00c",
                    },
                },
                new MenuItemGroup(AppResources.Things, false)
                {
                    new MasterPageItem()
                    {
                        Title = AppResources.Statistics,
                        Destination = typeof(StatisticsPage).Name,
                        Icon = "\uf201",
                        //IsEnabled = LocalSettingsORM.IsReadingDataFromSavefile,
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.ExportTitle,
                        Destination = typeof(ExportPage).Name,
                        Icon = "\uf56e",
                        //IsEnabled = LocalSettingsORM.IsReadingDataFromSavefile,
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.MenuPurchaseTitle,
                        Destination = typeof(PurchasePage).Name,
                        Icon = "\uf155",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.MenuInfoTitle,
                        Destination = typeof(InfoPage).Name,
                        Icon = "\uf558",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.MenuAboutTitle,
                        Destination = typeof(AboutPage).Name,
                        Icon = "\uf129",
                    },
                    new MasterPageItem()
                    {
                        Title = AppResources.MenuPrivacyPolicyTitle,
                        Destination = typeof(PrivacyPolicyPage).Name,
                        Icon = "",
                    },
                },
            };

            UpdateListContent();
        }

        private void UpdateListContent()
        {
            MenuList = new ObservableCollection<MenuItemGroup>();
            foreach (var item in _allItemsList)
            {
                if (!item.IsEnabled)
                {
                    continue;
                }

                var grp = new MenuItemGroup(item.Title, item.Expanded)
                {
                    ItemCount = item.ItemCount
                };
                if (item.Expanded)
                {
                    foreach (var child in item)
                    {
                        if (child.IsEnabled)
                        {
                            grp.Add(child);
                        }
                    }
                }

                MenuList.Add(grp);
            }
        }

        private async Task<bool> EnterSettingsAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.NavigationPath< SettingsPage>());
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }
        #endregion

        #region E + D
        public override void OnNavigatedFrom(INavigationParameters parameters) => base.OnNavigatedFrom(parameters);

        public override void OnNavigatedTo(INavigationParameters parameters) => base.OnNavigatedTo(parameters);
        #endregion
    }
}