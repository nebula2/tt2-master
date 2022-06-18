using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Resources;
using TT2Master.TT2WebMaster;
using Xamarin.Forms;

namespace TT2Master.ViewModels.Identity
{
    public class IdentityConnectViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;


        private bool _isLogoutVisible;
        public bool IsLogoutVisible { get => _isLogoutVisible; set => SetProperty(ref _isLogoutVisible, value); }

        private bool _isLoginVisible = true;
        public bool IsLoginVisible { get => _isLoginVisible; set => SetProperty(ref _isLoginVisible, value); }

        private bool _isUploadingSnapshotsEnabled;
        public bool IsUploadingSnapshotsEnabled 
        { 
            get => _isUploadingSnapshotsEnabled;
            set
            {
                SetProperty(ref _isUploadingSnapshotsEnabled, value);
                LocalSettingsORM.IsUploadingSnapshotsEnabled = value;
            }
        }

        public ICommand LoginCommand { get; private set; }

        public ICommand LogoutCommand { get; private set; }

        private readonly WebMasterService _webMasterService;

        public IdentityConnectViewModel(INavigationService navigationService, IPageDialogService dialogService) 
            : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = "Web";

            _webMasterService = new WebMasterService(App.DBRepo);
            IsUploadingSnapshotsEnabled = LocalSettingsORM.IsUploadingSnapshotsEnabled;

            LoginCommand = new DelegateCommand(async () => await ConnectToIdentityServerAsync());
            LogoutCommand = new DelegateCommand(async () => await DisconnectFromIdentityServerAsync());
        }

        private async Task ConnectToIdentityServerAsync()
        {
            if (!await _webMasterService.ConnectAsync())
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return;
            }

            IsLogoutVisible = true;
            IsLoginVisible = false;
        }

        private async Task DisconnectFromIdentityServerAsync()
        {
            if (!await _webMasterService.DisconnectAsync())
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return;
            }

            IsLogoutVisible = false;
            IsLoginVisible = true;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            //await ConnectToIdentityServerAsync();


            base.OnNavigatedTo(parameters);
        }
    }
}
