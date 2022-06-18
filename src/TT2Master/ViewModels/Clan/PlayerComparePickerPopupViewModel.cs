using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Model.Tournament;

namespace TT2Master
{
    /// <summary>
    /// Pick a target and go back. Give comparison properties in navigationparameters
    /// </summary>
    public class PlayerComparePickerPopupViewModel : ViewModelBase
    {
        #region Member
        private string _sourcePlayerId = "";
        public string SourcePlayerId { get => _sourcePlayerId; set => SetProperty(ref _sourcePlayerId, value); }

        private Player _targetPlayer = new Player();
        public Player TargetPlayer { get => _targetPlayer; set => SetProperty(ref _targetPlayer, value); }

        private bool _isTournamentComparison = false;
        public bool IsTournamentComparison { get => _isTournamentComparison; set => SetProperty(ref _isTournamentComparison, value); }

        private ObservableCollection<Player> _players = new ObservableCollection<Player>();
        public ObservableCollection<Player> Players { get => _players; set => SetProperty(ref _players, value); }

        /// <summary>
        /// Command to save
        /// </summary>
        public ICommand SaveCommand { get; set; }

        private readonly INavigationService _navigationService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public PlayerComparePickerPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            SaveCommand = new DelegateCommand<object>(SaveExecuteAsync);
        }
        #endregion

        #region CommandMethods
        /// <summary>
        /// Command to compare players
        /// </summary>
        private async void SaveExecuteAsync(object obj)
        {
            if (obj == null)
            {
                return;
            }

            TargetPlayer = obj as Player;

            var result = await _navigationService.NavigateAsync(
                NavigationConstants.ChildNavigationPath<TournamentMembersPage, MemberComparisonPage>()
                , new NavigationParameters
                {
                    { "source", SourcePlayerId },
                    { "target", TargetPlayer.PlayerId } ,
                    { "tournament", IsTournamentComparison },
                });

            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return;
        }

        #endregion

        #region Private Methods
        private void LoadTargetPlayers()
        {
            if (IsTournamentComparison)
            {
                Players = new ObservableCollection<Player>(TournamentHandler.TM.Members.Where(x => x.PlayerId != SourcePlayerId).ToList());

                // Sort the List
                if (Players != null)
                {
                    Players.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId);
                }
            }
            else
            {
                // get all players except source player
                Players = new ObservableCollection<Player>(App.Save.ThisClan.ClanMember.Where(x => x.PlayerId != SourcePlayerId).ToList());

                // Sort the List
                if (Players != null)
                {
                    Players.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId);
                }
            }
        }
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this - load some stuff
        /// </summary>
        /// <param name="parameters">Artifact</param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("tournament"))
                {
                    IsTournamentComparison = true;
                }

                if (parameters.ContainsKey("source"))
                {
                    SourcePlayerId = parameters["source"].ToString();
                }

                LoadTargetPlayers();
            }
            catch (System.Exception e)
            {
                Logger.WriteToLogFile($"PlayerComparePicker.OnNavigatedTo Error: {e.Message}");
                throw;
            }

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}