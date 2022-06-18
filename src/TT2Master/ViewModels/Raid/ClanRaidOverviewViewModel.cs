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
    public class ClanRaidOverviewViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private ObservableCollection<ClanRaid> _items;
        /// <summary>
        /// The current config
        /// </summary>
        public ObservableCollection<ClanRaid> Items
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

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public ClanRaidOverviewViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.ClanRaidHeader;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ClanRaidOverviewPage, ClanRaidDetailPage>());
                
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
                    int param = (o as ClanRaid).ID;

                    var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ClanRaidOverviewPage, ClanRaidDetailPage>()
                        , new NavigationParameters() { { "id", param } });
                    Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ClanRaidOverviewViewModel Error: {e.Message}");
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
                    int param = (o as ClanRaid).ID;

                    await App.DBRepo.DeleteClanRaidByID(param);
    
                    await ReloadItems();
    
                    Xamarin.Forms.DependencyService.Get<ISendNotification>().ShowNotification(AppResources.Raid, AppResources.DeletedText);
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ClanRaidOverviewViewModel Error: {e.Message}");
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
                var tmpItems = await App.DBRepo.GetAllClanRaidsAsync();

                if(tmpItems == null)
                {
                    tmpItems = new List<ClanRaid>();
                };

                tmpItems = tmpItems.OrderByDescending(x => x.ID).ToList();

                Items = new ObservableCollection<ClanRaid>(tmpItems);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ClanRaidOverviewViewModel Error: {ex.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
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
