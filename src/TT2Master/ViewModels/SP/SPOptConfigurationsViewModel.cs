using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Model.SP;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Follow SP Builds
    /// </summary>
    public class SPOptConfigurationsViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private ObservableCollection<SPOptConfiguration> _configurations;
        /// <summary>
        /// The current config
        /// </summary>
        public ObservableCollection<SPOptConfiguration> Configurations
        {
            get => _configurations;
            set
            {
                if (value != _configurations)
                {
                    SetProperty(ref _configurations, value);
                }
            }
        }

        private readonly bool _allFuncsAccess = PurchaseableItems.GetAllFuncsAccess();

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public SPOptConfigurationsViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.SPConfigurationsHeader;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            AddCommand = new DelegateCommand(async () =>
            {
                if(!_allFuncsAccess && Configurations.Count >= 10)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, string.Format(AppResources.OnlyForSupporterItemLimitText, 10), AppResources.OKText);
                    return;
                }

                var result = await _navigationService.NavigateAsync(NavigationConstants.DefaultPath + "SPOptPage/SPOptConfigurationsPage/SPOptConfigurationDetailPage");
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });

            EditCommand = new DelegateCommand<object>(async (o) =>
            {
                if(o == null)
                {
                    return;
                }

                try
                {
                    string param = (o as SPOptConfiguration).Name;

                    var answer = await _dialogService.DisplayActionSheetAsync(AppResources.WhatDoYouWant
                        , AppResources.CancelText
                        , AppResources.DestroyText
                        , new string[] { AppResources.Choose, AppResources.Edit });

                    if(answer == AppResources.Edit)
                    {
                        var result = await _navigationService.NavigateAsync(NavigationConstants.DefaultPath + "SPOptPage/SPOptConfigurationsPage/SPOptConfigurationDetailPage", new NavigationParameters() { { "id", param } });
                        Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                    }
                    else if(answer == AppResources.Choose)
                    {
                        var result = await _navigationService.GoBackAsync(new NavigationParameters() { { "id", param } });
                        Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                    }

                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"SPOptConfig Error: {e.Message}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            });
        }
        #endregion

        #region Helper

        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            //Load Configurations

            var dbConfs = await App.DBRepo.GetAllSPOptConfigurationsAsync();

            Configurations = dbConfs == null 
                ? new ObservableCollection<SPOptConfiguration>()
                : new ObservableCollection<SPOptConfiguration>(dbConfs);

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

