using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Resources;

namespace TT2Master
{
    /// <summary>
    /// ViewModel for automatic clan member export background service
    /// </summary>
    public class ClanAutoExportViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Command to save settings and start the service
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        private bool _isAutoExportWished;
        /// <summary>
        /// Is automatic export wished?
        /// </summary>
        public bool IsAutoExportWished { get => _isAutoExportWished; set => SetProperty(ref _isAutoExportWished, value); }
        
        private bool _isAutoExportNotificationWished;
        /// <summary>
        /// Do you want to be notified when data is exported?
        /// </summary>
        public bool IsAutoExportNotificationWished { get => _isAutoExportNotificationWished; set => SetProperty(ref _isAutoExportNotificationWished, value); }

        private int _autoExportSchedule;
        /// <summary>
        /// Export time interval
        /// </summary>
        public int AutoExportSchedule { get => _autoExportSchedule; set => SetProperty(ref _autoExportSchedule, value); }

        /// <summary>
        /// Dialog service
        /// </summary>
        private readonly IPageDialogService _dialogService;

        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public ClanAutoExportViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.ClanAutoExportTitle;

            _dialogService = dialogService;

            SaveCommand = new DelegateCommand(async () => await SaveExecute());

        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Action for SaveCommand
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveExecute()
        {
            SaveSettings();

            // start or stop service
            bool success = Xamarin.Forms.DependencyService.Get<IStartClanAutoExport>().StartService(true);

            await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, success ? AppResources.Saved : AppResources.ErrorOccuredText, AppResources.OKText);

            return true;
        }

        #endregion

        #region Helper
        /// <summary>
        /// Sets properties from AppSettings
        /// </summary>
        private void LoadSettings()
        {
            IsAutoExportWished = LocalSettingsORM.IsClanAutoExport;
            AutoExportSchedule = LocalSettingsORM.ClanAutoExportSchedule;
            IsAutoExportNotificationWished = LocalSettingsORM.IsClanAutoExportNotificationEnabled;
        }

        /// <summary>
        /// Saves values to AppSettings
        /// </summary>
        private void SaveSettings()
        {
            LocalSettingsORM.IsClanAutoExport = IsAutoExportWished;
            LocalSettingsORM.IsClanAutoExportNotificationEnabled = IsAutoExportNotificationWished;
            LocalSettingsORM.ClanAutoExportSchedule = AutoExportSchedule;
        }
        #endregion

        #region Navigation overrides
        /// <summary>
        /// Gets fired when navigation to this is finished
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadSettings();

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}