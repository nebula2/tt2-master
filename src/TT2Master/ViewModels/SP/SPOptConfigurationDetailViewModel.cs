using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.SP;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// Follow SP Builds
    /// </summary>
    public class SPOptConfigurationDetailViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;
        private readonly bool _allFuncsAccess = PurchaseableItems.GetAllFuncsAccess();

        private SPOptConfiguration _selectedConfig;
        /// <summary>
        /// The current config
        /// </summary>
        public SPOptConfiguration SelectedConfig
        {
            get => _selectedConfig;
            set
            {
                if (value != _selectedConfig)
                {
                    SetProperty(ref _selectedConfig, value);
                }
            }
        }

        private List<string> _modeTypes = Enum.GetNames(typeof(SPOptMode)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Mode Types
        /// </summary>
        public List<string> ModeTypes { get => _modeTypes; set => SetProperty(ref _modeTypes, value); }

        private List<string> _dmgTypes = Enum.GetNames(typeof(SPOptDamageSource)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Mode Types
        /// </summary>
        public List<string> DmgTypes { get => _dmgTypes; set => SetProperty(ref _dmgTypes, value); }

        private List<string> _goldTypes = Enum.GetNames(typeof(GoldType)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Mode Types
        /// </summary>
        public List<string> GoldTypes { get => _goldTypes; set => SetProperty(ref _goldTypes, value); }

        private bool _isDefault;
        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                if (ConfigurationSaved)
                {
                    SetProperty(ref _isDefault, value);
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand ConfigInfoCommand { get; private set; }
        public ICommand IsDefaultInfoCommand { get; private set; }
        public ICommand ModeInfoCommand { get; private set; }
        public ICommand DamageSourceInfoCommand { get; private set; }
        public ICommand GoldSourceInfoCommand { get; private set; }
        public ICommand PushingTypeInfoCommand { get; private set; }

        private bool _configurationSaved = false;
        public bool ConfigurationSaved
        {
            get => _configurationSaved;
            set
            {
                SetProperty(ref _configurationSaved, value);
                IsNameEditable = !value;
            }
        }

        private bool _isNameEditable = true;
        public bool IsNameEditable { get => _isNameEditable; set => SetProperty(ref _isNameEditable, value); }

        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public SPOptConfigurationDetailViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.SPConfigurationsHeader;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            UndoCommand = new DelegateCommand<object>(UndoCustomSkillSettings);
            SaveCommand = new DelegateCommand(async () => await SaveConfigurationAsync());
            DeleteCommand = new DelegateCommand(async () => await DeleteConfigurationAsync());

            ConfigInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.Configuration, AppResources.SPConfigurationDescription));
            IsDefaultInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.Default, AppResources.SPConfigDefaultDescription));
            ModeInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.Mode, AppResources.SPConfigModeDescription));
            DamageSourceInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.DamageSource, AppResources.SPDamageSourceDescription));
            GoldSourceInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.GoldSourceHeader, AppResources.GoldSourceDescription));
            PushingTypeInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.PushingTypeHeader, AppResources.PushTypeInfoText));
        }

        private async Task<bool> SaveConfigurationAsync()
        {
            try
            {
                App.DBRepo.StatusMessage = "";
                var confs = await App.DBRepo.GetAllSPOptConfigurationsAsync();

                if (!_allFuncsAccess && confs.Count >= 10)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, string.Format(AppResources.OnlyForSupporterItemLimitText, 10), AppResources.OKText);
                    return false;
                }

                // save config
                if (await App.DBRepo.GetSPOptConfigurationSavedByID(SelectedConfig.Name))
                {
                    var delChildsAmount = await App.DBRepo.DeleteSPOptSkillSettingBySId(SelectedConfig.Name);
                    Logger.WriteToLogFile($"Deleted {delChildsAmount} configuration childs. db-status: {App.DBRepo.StatusMessage}");
                    await App.DBRepo.DeleteSPOptConfigurationByID(SelectedConfig.Name);
                    Logger.WriteToLogFile($"Deleted configuration. db-status: {App.DBRepo.StatusMessage}");
                }

                int confSavedCount = await App.DBRepo.AddSPOptConfigurationAsync(SelectedConfig);
                Logger.WriteToLogFile($"Saved configurations: {confSavedCount}. db-status: {App.DBRepo.StatusMessage}");

                //save childs
                int childSavedCount = await App.DBRepo.AddNewSPOptSkillSettingListAsync(SelectedConfig.SkillSettings.ToList());
                Logger.WriteToLogFile($"Saved configuration childs: {childSavedCount}. db-status: {App.DBRepo.StatusMessage}");

                //save IsDefault
                if (IsDefault)
                {
                    LocalSettingsORM.DefaultSPConfiguration = SelectedConfig.Name;
                }

                ConfigurationSaved = true;

                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ChangesSavedText, AppResources.OKText);

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error saving configuration: {e.Message}. db-status: {App.DBRepo.StatusMessage}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);

                return false;
            }
        }

        private async Task<bool> DeleteConfigurationAsync()
        {
            if (!ConfigurationSaved)
            {
                return true;
            }

            try
            {
                bool answer = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SureToDeleteText, AppResources.YesText, AppResources.NoText);

                if (!answer)
                {
                    return true;
                }

                // delete childs
                await App.DBRepo.DeleteSPOptSkillSettingBySId(SelectedConfig.Name);

                await App.DBRepo.DeleteSPOptConfigurationByID(SelectedConfig.Name);

                // run
                await _navigationService.GoBackAsync();
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error deleting configuration: {e.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }
        }

        private void UndoCustomSkillSettings(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var item = obj as SPOptSkillSetting;

                item.UndoCustomValues();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error in Undo: {e.Message}");
            }
        }

        /// <summary>
        /// Displays a Popup Message to the user
        /// </summary>
        /// <param name="header"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private async Task<bool> OpenPopupInfoAsync(string header, string info)
        {
            await _dialogService.DisplayAlertAsync(header, info, AppResources.OKText);

            return true;
        }

        private void SelectedConfig_OnGoldSourceChanged() => SPOptConfigFactory.ResetGoldWeight(SelectedConfig, App.Save);

        private void SelectedConfig_OnDmgSourceChanged() => SPOptConfigFactory.ResetDamageWeight(SelectedConfig, App.Save);
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            // Load configuration if passed in parameters
            SelectedConfig = await SPOptConfigFactory.LoadConfigAsync(App.DBRepo, App.Save, parameters.ContainsKey("id") ? parameters["id"].ToString() : null, true);

            string defConfig = LocalSettingsORM.DefaultSPConfiguration ?? "";

            // check if default
            if (SelectedConfig.Name == defConfig)
            {
                IsDefault = true;
            }

            // set existant
            if (await App.DBRepo.GetSPOptConfigurationSavedByID(SelectedConfig.Name))
            {
                ConfigurationSaved = true;
            }

            // Abo on dmg source change to load the correct reductions
            SelectedConfig.OnDmgSourceChanged += SelectedConfig_OnDmgSourceChanged;
            SelectedConfig.OnGoldSourceChanged += SelectedConfig_OnGoldSourceChanged;

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}