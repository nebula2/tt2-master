using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Resources;

namespace TT2Master
{
    public class ClanMemberOverviewViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<Player> _member = new ObservableCollection<Player>();
        public ObservableCollection<Player> Member { get => _member; set => SetProperty(ref _member, value); }

        private readonly INavigationService _navigationService;

        public ICommand EnterDetailCommand { get; private set; }
        public ICommand ExportClanCSVCommand { get; private set; }
        public ICommand CompareCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public ClanMemberOverviewViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.ClanMemberTitle;

            _navigationService = navigationService;

            EnterDetailCommand = new DelegateCommand<object>(EnterMember);
            ExportClanCSVCommand = new DelegateCommand(async () => await ExportClanDataExecute());
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
                    , new NavigationParameters() { { "source", player.PlayerId } });

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

            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ClanMemberOverviewPage, ClanMemberDetailPage>(), new NavigationParameters() { { "member", player.PlayerName } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
        }

        private async Task<bool> ExportClanDataExecute()
        {
            var result = await _navigationService.NavigateAsync("ClanExportPopupPage");
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Member = new ObservableCollection<Player>(App.Save.ThisClan.ClanMember);

            // Sort the List
            if (Member != null)
            {
                Member.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId);
            }

            #region Passed players for comparison
            if (parameters.ContainsKey("source") && parameters.ContainsKey("target"))
            {
                try
                {
                }
                catch (System.Exception e)
                {
                    Logger.WriteToLogFile($"ClanMemberOverview Error: Could not compare {e.Message}");
                }
            }
            #endregion

            base.OnNavigatedTo(parameters);
        }
    }
}