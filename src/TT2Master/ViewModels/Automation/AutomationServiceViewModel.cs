using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Windows.Input;
using TT2Master.Resources;

namespace TT2Master
{
    /// <summary>
    /// ViewModel for automatic clan member export background service
    /// </summary>
    public class AutomationServiceViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Command to save settings and start the service
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Command to start the foreground service
        /// </summary>
        public ICommand StartCommand { get; private set; }

        private bool _isArtifactCheckWished;
        /// <summary>
        /// Do you want to check for artifacts?
        /// </summary>
        public bool IsArtifactCheckWished { get => _isArtifactCheckWished; set => SetProperty(ref _isArtifactCheckWished, value); }

        private bool _isSkillCheckWished;
        /// <summary>
        /// Do you want to check for skills?
        /// </summary>
        public bool IsSkillCheckWished { get => _isSkillCheckWished; set => SetProperty(ref _isSkillCheckWished, value); }

        private bool _isEquipCheckWished;
        /// <summary>
        /// Do you want to check for Equipment?
        /// </summary>
        public bool IsEquipCheckWished { get => _isEquipCheckWished; set => SetProperty(ref _isEquipCheckWished, value); }

        private int _autoExportSchedule;
        /// <summary>
        /// Export time interval
        /// </summary>
        public int AutoExportSchedule { get => _autoExportSchedule; set => SetProperty(ref _autoExportSchedule, value); }

        private bool _isDiamondFairyWished;
        public bool IsDiamondFairyWished { get => _isDiamondFairyWished; set => SetProperty(ref _isDiamondFairyWished, value); }

        private bool _isFatFairyWished;
        public bool IsFatFairyWished { get => _isFatFairyWished; set => SetProperty(ref _isFatFairyWished, value); }

        private bool _isFreeEquipWished;
        public bool IsFreeEquipWished { get => _isFreeEquipWished; set => SetProperty(ref _isFreeEquipWished, value); }

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
        public AutomationServiceViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.AutomationService;

            _dialogService = dialogService;

            SaveCommand = new DelegateCommand(() => SaveSettings());

            StartCommand = new DelegateCommand(async () =>
            {
                SaveSettings();

                // start or stop service
                bool success = Xamarin.Forms.DependencyService.Get<IAutomationService>().StartService();

                await _dialogService.DisplayAlertAsync(AppResources.StartService, success ? AppResources.ServiceStartedText : AppResources.ErrorOccuredText, AppResources.OKText);
            });
        }
        #endregion

        #region Helper
        /// <summary>
        /// Sets properties from AppSettings
        /// </summary>
        private void LoadSettings()
        {
            IsArtifactCheckWished = LocalSettingsORM.IsAutoArtifactCheck;
            IsSkillCheckWished = LocalSettingsORM.IsAutoSkillCheck;
            IsEquipCheckWished = LocalSettingsORM.IsAutoEquipCheck;
            AutoExportSchedule = LocalSettingsORM.AutoServiceSchedule;

            IsDiamondFairyWished = LocalSettingsORM.IsDiamondFairyWished;
            IsFatFairyWished     = LocalSettingsORM.IsFatFairyWished;
            IsFreeEquipWished    = LocalSettingsORM.IsFreeEquipWished;
        }

        /// <summary>
        /// Saves values to AppSettings
        /// </summary>
        private void SaveSettings()
        {
            LocalSettingsORM.IsAutoArtifactCheck = IsArtifactCheckWished;
            LocalSettingsORM.IsAutoSkillCheck = IsSkillCheckWished;
            LocalSettingsORM.IsAutoEquipCheck = IsEquipCheckWished;
            LocalSettingsORM.AutoServiceSchedule = AutoExportSchedule;

            LocalSettingsORM.IsDiamondFairyWished = IsDiamondFairyWished;
            LocalSettingsORM.IsFatFairyWished = IsFatFairyWished;
            LocalSettingsORM.IsFreeEquipWished = IsFreeEquipWished;
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