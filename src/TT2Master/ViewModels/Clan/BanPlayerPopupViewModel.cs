using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    public class BanPlayerPopupViewModel : ViewModelBase
    {
        #region Member
        private Player _currentPlayer;
        /// <summary>
        /// Input from User
        /// </summary>
        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (value != _currentPlayer)
                {
                    SetProperty(ref _currentPlayer, value);
                }
            }
        }

        private BannedPlayer _bannedPlayer = new BannedPlayer();
        public BannedPlayer BannedPlayer { get => _bannedPlayer; set => SetProperty(ref _bannedPlayer, value); }

        /// <summary>
        /// Command to save
        /// </summary>
        public ICommand SaveCommand { get; set; }

        private INavigationService _navigationService; 
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public BanPlayerPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            SaveCommand = new DelegateCommand(async () => await SaveExecuteAsync());
        }
        #endregion

        #region CommandMethods
        /// <summary>
        /// Command to Save ban
        /// </summary>
        private async Task<bool> SaveExecuteAsync()
        {
            if(BannedPlayer == null || string.IsNullOrEmpty(BannedPlayer.ID))
            {
                return false;
            }
            
            //store value
            await App.DBRepo.AddBannedPlayerAsync(BannedPlayer);

            //leave this shit - i am done
            var result = await _navigationService.GoBackAsync(new NavigationParameters() { { "member", BannedPlayer.Name } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }

        #endregion

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters["dude"].ToString()))
            {
                CurrentPlayer = new Player();
            }
            else
            {
                CurrentPlayer = App.Save.ThisClan.ClanMember.Where(x => x.PlayerName == parameters["dude"].ToString()).FirstOrDefault();
                BannedPlayer = new BannedPlayer(CurrentPlayer.PlayerId, CurrentPlayer.PlayerName);
                Title = CurrentPlayer.PlayerName;
            }

            base.OnNavigatedTo(parameters);
        }
    }
}