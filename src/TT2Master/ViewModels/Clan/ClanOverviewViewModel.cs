using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Resources;

namespace TT2Master
{
    public class ClanOverviewViewModel : ViewModelBase
    {
        #region Properties
        private Clan _myClan = new Clan(App.Save.ThisClan);
        public Clan MyClan { get => _myClan; set => SetProperty(ref _myClan, value); }

        private int _msgCount;
        public int MsgCount { get => _msgCount; set => SetProperty(ref _msgCount, value); }

        private string _advancedStart;

        public string AdvancedStart { get => _advancedStart; set => SetProperty(ref _advancedStart, value); }

        private IPageDialogService _dialogService;
        private INavigationService _navigationService;

        public ICommand EnterMemberCommand { get; private set; }
        public ICommand EnterMessagesCommand { get; private set; }
        #endregion

        #region Ctor
        public ClanOverviewViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.ClanTitle;

            _dialogService = dialogService;
            _navigationService = navigationService;

            EnterMemberCommand = new DelegateCommand<object>(EnterMember);
            EnterMessagesCommand = new DelegateCommand(async () => await EnterMessagesAsync());
        }
        #endregion

        private async Task<bool> EnterMessagesAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ClanOverviewPage, ClanMessagesOverviewPage>());
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }

        private async void EnterMember(object obj)
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath < ClanOverviewPage, ClanMemberOverviewPage>());
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                var msgs = await App.DBRepo.GetAllClanMessageAsync();
                MsgCount = msgs.Count;

                AdvancedStart = $"{(FormulaHelper.GetClanAdvancedStart() * 100):N2}%";
            }
            catch (Exception)
            {
            }

            base.OnNavigatedTo(parameters);
        }
    }
}