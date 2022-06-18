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
    public class RaidToleranceOverviewViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private ObservableCollection<RaidTolerance> _items;
        /// <summary>
        /// The current config
        /// </summary>
        public ObservableCollection<RaidTolerance> Items
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
        public ICommand RenameCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public RaidToleranceOverviewViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.Tolerances;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<RaidToleranceOverviewPage, RaidToleranceDetailPage>());
                
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
                    var param = (o as RaidTolerance).Name;

                    var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<RaidToleranceOverviewPage, RaidToleranceDetailPage>()
                        , new NavigationParameters() { { "id", param } });
                    Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"RaidToleranceOverviewViewModel Error: {e.Message}");
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
                    var param = (o as RaidTolerance).Name;

                    await App.DBRepo.DeleteRaidToleranceByID(param);
    
                    await ReloadItems();
    
                    Xamarin.Forms.DependencyService.Get<ISendNotification>().ShowNotification(AppResources.Tolerance, AppResources.DeletedText);
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"RaidToleranceOverviewViewModel Error: {e.Message}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            });

            RenameCommand = new DelegateCommand<object>(async (o) =>
            {
                if(o == null)
                {
                    return;
                }

                try
                {
                    var param = (o as RaidTolerance);

                    var result = await _navigationService.NavigateAsync("RenameClanRaidStuffPopupPage", new NavigationParameters() { { "tolerance", param } });
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
                var tmpItems = await App.DBRepo.GetAllRaidTolerancesAsync();

                if(tmpItems == null)
                {
                    tmpItems = new List<RaidTolerance>();
                };

                tmpItems = tmpItems.OrderBy(x => x.Name).ToList();

                Items = new ObservableCollection<RaidTolerance>(tmpItems);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"RaidToleranceOverviewViewModel Error: {ex.Message}");
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
