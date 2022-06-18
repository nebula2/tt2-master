using Prism.Commands;
using Prism.Navigation;
using System.Windows.Input;
using TT2Master.Loggers;

namespace TT2Master
{
    public class RaidAttackResultPopupViewModel : ViewModelBase
    {
        #region Member
        private RaidResult _item;
        /// <summary>
        /// Current Raid Attack Result item
        /// </summary>
        public RaidResult Item
        {
            get => _item;
            set
            {
                if (value != _item)
                {
                    SetProperty(ref _item, value);
                }
            }
        }

        private INavigationService _navigationService;

        public ICommand CloseCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public RaidAttackResultPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            CloseCommand = new DelegateCommand(async () => await _navigationService.GoBackAsync());
        }
        #endregion

        #region CommandMethods

        #endregion

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.ContainsKey("item"))
            {
                Logger.WriteToLogFile("RaidAttackResult.OnNavigatedTo: No item given. Creating new ");
                Item = new RaidResult();
            }
            else
            {
                Item = parameters["item"] as RaidResult;
            }

            base.OnNavigatedTo(parameters);
        }
    }
}