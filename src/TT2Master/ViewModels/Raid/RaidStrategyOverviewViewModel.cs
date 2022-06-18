using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Model.Raid;
using TT2Master.Resources;
using TT2Master.Views.Raid;
using Xamarin.Forms;

namespace TT2Master.ViewModels.Raid
{
    public class RaidStrategyOverviewViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private ObservableCollection<RaidStrategy> _items;
        /// <summary>
        /// The current config
        /// </summary>
        public ObservableCollection<RaidStrategy> Items
        {
            get => _items;
            set
            {
                if (value != _items)
                {
                    SetProperty(ref _items, value);
                }
            }
        }

        private ObservableCollection<GroupedClanRaidStrategy> _raidStrategyGrouping;

        public ObservableCollection<GroupedClanRaidStrategy> RaidStrategyGrouping { get => _raidStrategyGrouping; set => SetProperty(ref _raidStrategyGrouping, value); }

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public ICommand RenameCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public RaidStrategyOverviewViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.Strategies;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<RaidStrategyOverviewPage, RaidStrategyDetailPage>());
                
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });

            EditCommand = new DelegateCommand<object>(async (o) =>
            {
                if (o == null)
                {
                    return;
                }

                try
                {
                    string param = (o as RaidStrategy).Name;

                    var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<RaidStrategyOverviewPage, RaidStrategyDetailPage>()
                        , new NavigationParameters() { { "id", param } });
                    Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"RaidStrategyOverviewViewModel Error: {e.Message}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            });

            DeleteCommand = new DelegateCommand<object>(async (o) =>
            {
                if (o == null)
                {
                    return;
                }

                try
                {
                    string param = (o as RaidStrategy).Name;

                    await App.DBRepo.DeleteRaidStrategyByID(param);

                    await ReloadItems();

                    Xamarin.Forms.DependencyService.Get<ISendNotification>().ShowNotification(AppResources.Strategy, AppResources.DeletedText);
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ClanRaidOverviewViewModel Error: {e.Message}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            });

            RenameCommand = new DelegateCommand<object>(async (o) =>
            {
                if (o == null)
                {
                    return;
                }

                try
                {
                    var param = (o as RaidStrategy);

                    var result = await _navigationService.NavigateAsync("RenameClanRaidStuffPopupPage", new NavigationParameters() { { "strategy", param } });
                    Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                }
                catch (Exception ex)
                {
                    Logger.WriteToLogFile($"RaidToleranceOverviewViewModel Error RenameCommand: {ex.Message}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            });
        }
        #endregion

        #region Helper
        private async Task<bool> ReloadItems()
        {
            try
            {
                var lst = await App.DBRepo.GetAllRaidStrategiesAsync();

                Items = new ObservableCollection<RaidStrategy>(lst);
                LoadEnemyNames();

                List<(string code, string name)> tmp = Items.Select(x => (x.EnemyName[0].ToString().ToUpper(), x.EnemyName)).Distinct().ToList();
                RaidStrategyGrouping = new ObservableCollection<GroupedClanRaidStrategy>();

                foreach (var item in tmp.OrderBy(x => x.name).ToList())
                {
                    var group = new GroupedClanRaidStrategy { LongName = item.name, ShortName = item.code };

                    foreach (var child in Items.Where(x => x.EnemyName == group.LongName).OrderBy(x => x.Name).ToList())
                    {
                        group.Add(child);
                    }

                    RaidStrategyGrouping.Add(group);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"RaidStrategyOverviewViewModel Error: {ex.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }
        }

        private void LoadEnemyNames()
        {
            RaidInfoHandler.LoadRaidInfos();

            foreach (var item in Items)
            {
                item.EnemyName = RaidInfoHandler.EnemyInfos.Where(x => x.EnemyId == item.EnemyId).FirstOrDefault()?.Name ?? "?";
            }
        }
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await ReloadItems();

            base.OnNavigatedTo(parameters);
        }

        /// <summary>
        /// When navigating away
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }
        #endregion
    }
}
