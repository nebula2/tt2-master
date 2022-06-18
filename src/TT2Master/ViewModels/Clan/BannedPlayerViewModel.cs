using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Resources;

namespace TT2Master
{
    public class BannedPlayerViewModel : ViewModelBase
    {
        #region Properties
        private List<BannedPlayer> _bannedPlayers = new List<BannedPlayer>();
        public List<BannedPlayer> BannedPlayers { get => _bannedPlayers; set => SetProperty(ref _bannedPlayers, value); }

        public ICommand RemoveCommand { get; private set; }

        private IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;
        #endregion

        #region Ctor
        public BannedPlayerViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.BannedPlayersTitle;

            RemoveCommand = new DelegateCommand<object>(RemoveAsync);

            _dialogService = dialogService;
            _navigationService = navigationService;
        }
        #endregion

        #region Command methods
        private async void RemoveAsync(object obj)
        {
            if (obj == null)
            {
                return;
            }

            bool choice = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SureToDeleteText, AppResources.OKText, AppResources.CancelText);

            if (!choice)
            {
                return;
            }


            var player = obj as BannedPlayer;

            int result = await App.DBRepo.DeleteBannedPlayerByID(player.ID);

            if(result > 0)
            {
                await ToastSender.SendToastAsync(AppResources.DeletedText, _dialogService);
            }
            else
            {
                await ToastSender.SendToastAsync(AppResources.NotDeletedText, _dialogService);
            }

            await ReloadList();

            return;
        }
        #endregion

        private async Task<bool> ReloadList()
        {
            BannedPlayers = await App.DBRepo.GetAllBannedPlayersAsync();
            return true;
        }

        #region Overrides
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await ReloadList();

            base.OnNavigatedTo(parameters);
        } 
        #endregion
    }
}