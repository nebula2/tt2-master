using Prism.Navigation;

namespace TT2Master
{
    public class PlayerViewModel : ViewModelBase
    {
        /// <summary>
        /// My Player
        /// </summary>
        private Player _Me;

        /// <summary>
        /// My Player
        /// </summary>
        public Player Me
        {
            get => _Me;
            set => SetProperty(ref _Me, value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public PlayerViewModel(INavigationService navigationService) 
            : base (navigationService)
        {
            Title = "Player";
            LoadProfileDataAsync();
        }

        /// <summary>
        /// Load My Profile
        /// </summary>
        private async void LoadProfileDataAsync()
        {
            Me = await App.DBRepo.GetMyPlayer();
        }
    }
}
