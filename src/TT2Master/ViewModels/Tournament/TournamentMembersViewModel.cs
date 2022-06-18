using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Model.Tournament;
using TT2Master.Resources;

namespace TT2Master
{
    public class TournamentMembersViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<Player> _members = new ObservableCollection<Player>();
        public ObservableCollection<Player> Members { get => _members; set => SetProperty(ref _members, value); }

        private readonly INavigationService _navigationService;

        public ICommand EnterDetailCommand { get; private set; }
        public ICommand CompareCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public TournamentMembersViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.Tournament;

            _navigationService = navigationService;

            EnterDetailCommand = new DelegateCommand<object>(EnterMember);
            CompareCommand = new DelegateCommand<object>(async (o) => await CompareExecute(o));
        }
        #endregion

        /// <summary>
        /// Command to compare two member
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private async Task<bool> CompareExecute(object o)
        {
            try
            {
                if (o == null)
                {
                    return false;
                }

                var player = o as Player;

                var result = await _navigationService.NavigateAsync("PlayerComparePickerPopupPage"
                    , new NavigationParameters() { { "tournament", true }, { "source", player.PlayerId } });

                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

                return true;
            }
            catch (System.Exception e)
            {
                Logger.WriteToLogFile($"Member comaprison error: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Enters the Details to the member which has been tapped
        /// </summary>
        /// <param name="obj"></param>
        private async void EnterMember(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var player = obj as Player;

            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<TournamentMembersPage, ClanMemberDetailPage>()
                , new NavigationParameters() { { "member", player.PlayerId }, { "isTournament", true } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Members = new ObservableCollection<Player>(TournamentHandler.TM.Members);

                // Sort the List
                if (Members != null)
                {
                    Members.OrderByDescending(x => x.ClanRank);
                }
            }
            catch (System.Exception e)
            {
                Logger.WriteToLogFile($"Navigating to TournamentMember Error: {e.Message}");
                throw;
            }

            base.OnNavigatedTo(parameters);
        }
    }
}