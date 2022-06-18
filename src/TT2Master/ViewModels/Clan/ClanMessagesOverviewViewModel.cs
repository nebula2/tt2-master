using Plugin.Clipboard;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Clan;
using TT2Master.Model.Navigation;
using TT2Master.Resources;

namespace TT2Master
{
    public class ClanMessagesOverviewViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<ClanMessage> _allMessages = new ObservableCollection<ClanMessage>();

        private ObservableCollection<ClanMessage> _messages = new ObservableCollection<ClanMessage>();
        public ObservableCollection<ClanMessage> Messages { get => _messages; set => SetProperty(ref _messages, value); }

        private ObservableCollection<ClanMessage> _history = new ObservableCollection<ClanMessage>();
        public ObservableCollection<ClanMessage> History { get => _history; set => SetProperty(ref _history, value); }

        private ObservableCollection<ClanMessage> _buildShares = new ObservableCollection<ClanMessage>();
        public ObservableCollection<ClanMessage> BuildShares { get => _buildShares; set => SetProperty(ref _buildShares, value); }

        private ObservableCollection<ClanMessage> _raidMessages = new ObservableCollection<ClanMessage>();
        public ObservableCollection<ClanMessage> RaidMessages { get => _raidMessages; set => SetProperty(ref _raidMessages, value); }

        private ObservableCollection<ClanMessage> _raidResults = new ObservableCollection<ClanMessage>();
        public ObservableCollection<ClanMessage> RaidResults { get => _raidResults; set => SetProperty(ref _raidResults, value); }

        private ObservableCollection<ClanMessage> _tournamentResults = new ObservableCollection<ClanMessage>();
        public ObservableCollection<ClanMessage> TournamentResults { get => _tournamentResults; set => SetProperty(ref _tournamentResults, value); }

        private readonly IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;

        public ICommand EnterDetailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand ExportMessagesCommand { get; private set; }
        public ICommand OpenRaidResultCommand { get; private set; }
        public ICommand OpenTournamentResultCommand { get; private set; }
        public ICommand CopyMessageCommand { get; private set; }

        private int _lastMessageId = 0;
        public int LastMessageId
        {
            get => _lastMessageId;
            set
            {
                if (value < 0)
                {
                    SetProperty(ref _lastMessageId, 0);
                }
                else
                {
                    SetProperty(ref _lastMessageId, value);
                }
            }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        private string _msgHeader = "Msg";
        public string MsgHeader { get => _msgHeader; set => SetProperty(ref _msgHeader, value); }

        private string _buildHeader = "Build";
        public string BuildHeader { get => _buildHeader; set => SetProperty(ref _buildHeader, value); }

        private string _raidHeader = "Raid";
        public string RaidHeader { get => _raidHeader; set => SetProperty(ref _raidHeader, value); }

        private string _raidResultHeader = "RaidResult";
        public string RaidResultHeader { get => _raidResultHeader; set => SetProperty(ref _raidResultHeader, value); }

        private string _tournamentResultHeader = "Tournament";
        public string TournamentResultHeader { get => _tournamentResultHeader; set => SetProperty(ref _tournamentResultHeader, value); }

        private string _memberHeader = "Member";
        public string MemberHeader { get => _memberHeader; set => SetProperty(ref _memberHeader, value); }

        private string _searchString = "";
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (value != _searchString)
                {
                    SetProperty(ref _searchString, value, async () => await ReloadList(false, value));
                    //ReloadList(false, value);
                };
            }
        }
        #endregion

        #region Ctor
        public ClanMessagesOverviewViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.ClanMessagesTitle;

            _dialogService = dialogService;
            _navigationService = navigationService;

            EnterDetailCommand = new DelegateCommand<object>(EnterMember);
            OpenRaidResultCommand = new DelegateCommand<object>(OpenRaidResultExecute);
            OpenTournamentResultCommand = new DelegateCommand<object>(OpenTournamentResultExecute);
            RefreshCommand = new DelegateCommand(async () => await RefreshExecute());
            ExportMessagesCommand = new DelegateCommand(async () => await ExportMessagesAsync());
            CopyMessageCommand = new DelegateCommand<object>(async (o) => await CopyMessageAsync(o));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Opens a Popup for Raid Result
        /// </summary>
        /// <param name="obj"></param>
        private async void OpenRaidResultExecute(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var msg = obj as ClanMessage;
                var item = new RaidResult(msg);

                var result = await _navigationService.NavigateAsync("RaidAttackResultPopupPage", new NavigationParameters() { { "item", item } });

                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Could not navigate to item Error. {e.Message}");
            }
        }

        /// <summary>
        /// Opens a Popup for Tournament Result
        /// </summary>
        /// <param name="obj"></param>
        private async void OpenTournamentResultExecute(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var msg = obj as ClanMessage;
                msg.Message = msg.Message.Replace("TournamentResultsShare: ", "");
                var item = new TournamentResult(msg);

                var result = await _navigationService.NavigateAsync("TournamentResultPopupPage", new NavigationParameters() { { "item", item } });

                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Could not navigate to item Error. {e.Message}");
            }
        }

        /// <summary>
        /// Navigates to SP follower to show and compare build
        /// </summary>
        /// <param name="obj"></param>
        private async void EnterMember(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var msg = obj as ClanMessage;
                var build = new SPBuild(msg);

                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ClanMessagesOverviewPage, SPOptimizerPage>(), new NavigationParameters() { { "build", build } });
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Could not navigate to build Error. {e.Message}");
            }
        }

        /// <summary>
        /// Copies the content of the message to clipboard
        /// </summary>
        /// <param name="obj"></param>
        private async Task CopyMessageAsync(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var msg = obj as ClanMessage;

                if(msg != null)
                {
                    CrossClipboard.Current.SetText(msg.Message);

                    await ToastSender.SendToastAsync(AppResources.Copied, null);
                }
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"CopyMessage ERROR: Could not copy message to clipboard: {e.Message}");
            }
        }

        /// <summary>
        /// Exports the messages
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ExportMessagesAsync()
        {
            var result = await _navigationService.NavigateAsync("ClanMsgExportPopupPage");
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;

        }

        /// <summary>
        /// Handles the Refresh-Command
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RefreshExecute()
        {
            IsRefreshing = true;
            try
            {
                LastMessageId -= 200; //act like you did not get the last 200 messages

                // get new messages from savefile
                await App.Save.Initialize(loadPlayer: true, loadAccountModel: true, loadArtifacts: false, loadSkills: false, loadClan: true, loadTournament: false, loadEquipment: false);

                Logger.WriteToLogFile("SplashVM: saving clan messages");
                ClanMessageFactory.OnLogMePlease += ClanMessageFactory_OnLogMePlease;
                bool clanMsgResult = await ClanMessageFactory.SaveNewClanMessages();
                ClanMessageFactory.OnLogMePlease -= ClanMessageFactory_OnLogMePlease;
                Logger.WriteToLogFile("SplashVM: done saving clan messages");

                bool reloaded = await ReloadList(filterWithSearchString: true);
                IsRefreshing = false;

                return true;
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                return false;
            }
        }

        /// <summary>
        /// Reloads the collections
        /// </summary>
        /// <param name="withDbReload"></param>
        /// <param name="filterStr"></param>
        /// <param name="filterWithSearchString"></param>
        /// <returns></returns>
        private async Task<bool> ReloadList(bool withDbReload = true, string filterStr = null, bool filterWithSearchString = false)
        {
            try
            {
                if (withDbReload)
                {
                    _allMessages = new ObservableCollection<ClanMessage>(await App.DBRepo.GetAllClanMessageAsync());
                }

                string strToFilter = filterWithSearchString ? SearchString : filterStr;

                if (string.IsNullOrWhiteSpace(strToFilter) && _allMessages.Count > 0)
                {
                    Messages = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => x.ClanMessageType == "Message")
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    BuildShares = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => (x.ClanMessageType == "BuildShare"))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    History = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => (x.ClanMessageType != "BuildShare"
                            && x.ClanMessageType != "Message"
                            && !(x.ClanMessageType.ToUpper().IndexOf("RAID") >= 0)
                            && x.ClanMessageType != "MakeItRain"))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    RaidMessages = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => x.ClanMessageType.ToUpper().IndexOf("RAID") >= 0 && x.ClanMessageType != "RaidAttackSummaryShare")
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    RaidResults = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => x.ClanMessageType == "RaidAttackSummaryShare")
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    TournamentResults = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => x.ClanMessageType == "TournamentResultsShare")
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());
                }
                else if (!string.IsNullOrWhiteSpace(strToFilter))
                {
                    Messages = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => x.ClanMessageType == "Message" && x.FilterString.ToUpper().Contains(strToFilter.ToUpper()))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    BuildShares = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => (x.ClanMessageType == "BuildShare" && x.FilterString.ToUpper().Contains(strToFilter.ToUpper())))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    History = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => (x.ClanMessageType != "BuildShare"
                            && x.ClanMessageType != "Message"
                            && !(x.ClanMessageType.ToUpper().IndexOf("RAID") >= 0)
                            && x.ClanMessageType != "MakeItRain")
                            && x.FilterString.ToUpper().Contains(strToFilter.ToUpper()))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    RaidMessages = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => (x.ClanMessageType.ToUpper().IndexOf("RAID") >= 0 && x.ClanMessageType != "RaidAttackSummaryShare") && x.FilterString.ToUpper().Contains(strToFilter.ToUpper()))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    RaidResults = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => (x.ClanMessageType == "RaidAttackSummaryShare") && x.FilterString.ToUpper().Contains(strToFilter.ToUpper()))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());

                    RaidResults = new ObservableCollection<ClanMessage>(_allMessages
                        .Where(x => (x.ClanMessageType == "TournamentResultsShare") && x.FilterString.ToUpper().Contains(strToFilter.ToUpper()))
                        .OrderByDescending(n => n.TimeStamp)
                        .ToList());
                }

                MsgHeader = $"Msg {(Messages == null ? 0 : Messages.Count)}";
                BuildHeader = $"Build {(Messages == null ? 0 : BuildShares.Count)}";
                RaidHeader = $"Raid {(Messages == null ? 0 : RaidMessages.Count)}";
                MemberHeader = $"Member {(Messages == null ? 0 : History.Count)}";
                RaidResultHeader = $"RaidResult {(Messages == null ? 0 : RaidResults.Count)}";
                TournamentResultHeader = $"Tournament {(Messages == null ? 0 : TournamentResults.Count)}";

                LastMessageId = _allMessages.Select(x => x.MessageID).Max();
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanMsgOverview Error in ReloadList: {e.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorLoadingClanMsg, AppResources.OKText);
                return false;
            }
        }

        #endregion

        #region E+D
        /// <summary>
        /// Event handler for Log requests
        /// </summary>
        /// <param name="message"></param>
        private void ClanMessageFactory_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"Info at {sender.ToString()}: {e.Information}");
        #endregion

        #region Navigation
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (App.Save.ThisClan.Messages == null)
                {
                    base.OnNavigatedTo(parameters);
                    return;
                }

                bool cleaned = await ClanMessageFactory.DeleteOldClanMessageAsync();
                bool reloaded = await ReloadList(filterWithSearchString: true);

            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
            }

            base.OnNavigatedTo(parameters);
        } 
        #endregion
    }
}