using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Mailing;
using TT2Master.Model.Social;
using TT2Master.Resources;
using Xamarin.Essentials;

namespace TT2Master
{
    public class AboutViewModel : ViewModelBase
    {
        #region Properties
        private string _myMessage;
        public string MyMessage { get => _myMessage; set => SetProperty(ref _myMessage, value); }

        private readonly string _discordLink = @"https://discord.gg/zWw3bgr";

        private ObservableCollection<Credits> _creditsTo = new ObservableCollection<Credits>();
        public ObservableCollection<Credits> CreditsTo { get => _creditsTo; set => SetProperty(ref _creditsTo, value); }

        public ICommand OpenMailCommand { get; private set; }
        public ICommand OpenDiscordCommand { get; private set; }
        #endregion

        #region Ctor
        public AboutViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.AboutMeHeader;

            OpenMailCommand = new DelegateCommand(async () => await OpenMailExecute());
            OpenDiscordCommand = new DelegateCommand(async () => await Launcher.OpenAsync(new Uri(_discordLink)));

            MyMessage = AppResources.AboutMeMessage;

            CreditsTo = new ObservableCollection<Credits>()
            {
                new Credits()
                {
                    Name = "Cauchy",
                    Text = "Thank you for helping in any matter. Without you this app would not be nearly as good as it it today :)",
                },
                new Credits()
                {
                    Name = "VrozaX",
                    Text = "Thank you for translating this app to polish!",
                },
                new Credits()
                {
                    Name = "Soulrise",
                    Text = "Thank you for making tutorials!",
                },
                new Credits()
                {
                    Name = "6mon",
                    Text = "Thank you for translating the app to french!",
                },
            };
        }
        #endregion

        #region Command Methods
        private async Task<bool> OpenMailExecute()
        {
            try
            {
                var sender = new EmailSender();
                bool sent = await sender.SendErrorEmail();

                return sent;
            }
            catch (Exception ex)
            {
                Xamarin.Forms.DependencyService.Get<ISendNotification>().ShowNotification(AppResources.ErrorHeader, ex.Message);
                return false;
            }
        }
        #endregion
    }
}