using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.DataSource;
using TT2Master.Model.Navigation;
using TT2Master.Model.SP;
using TT2Master.Resources;
using TT2Master.Shared.Models;
using Xamarin.Essentials;

namespace TT2Master
{
    /// <summary>
    /// SP Optimizer Entry Point
    /// </summary>
    public class SPOptViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;

        private int _configAmount = 0;
        public int ConfigAmount
        {
            get => _configAmount;
            set
            {
                SetProperty(ref _configAmount, value);
                NeedToShowConfigInit = value == 0;
            }
        }

        private bool _needToShowConfigInit;
        public bool NeedToShowConfigInit { get => _needToShowConfigInit; set => SetProperty(ref _needToShowConfigInit, value); }

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

        private string _currentSP;
        /// <summary>
        /// Current SP you have
        /// </summary>
        public string CurrentSP
        {
            get => _currentSP;
            set
            {
                if (value != _currentSP)
                {
                    SetProperty(ref _currentSP, value);
                }
            }
        }

        private string _availableSP;
        /// <summary>
        /// Available SP you can spend
        /// </summary>
        public string AvailableSP
        {
            get => _availableSP;
            set
            {
                if (value != _availableSP)
                {
                    SetProperty(ref _availableSP, value);
                }
            }
        }

        private string _underConstructionHeader;
        public string UnderConstructionHeader { get => _underConstructionHeader; set => SetProperty(ref _underConstructionHeader, value); }

        private string _underConstructionBody;
        public string UnderConstructionBody { get => _underConstructionBody; set => SetProperty(ref _underConstructionBody, value); }

        private List<string> _modeTypes = Enum.GetNames(typeof(SPOptMode)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Mode Types
        /// </summary>
        public List<string> ModeTypes { get => _modeTypes; set => SetProperty(ref _modeTypes, value); }

        public ICommand OpenMmlhOptimizerCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand ReloadCommand { get; private set; }
        public ICommand GoToSettingsCommand { get; private set; }
        public ICommand CreateInitialConfigCommand { get; private set; }
        public ICommand ConfigInfoCommand { get; private set; }
        public ICommand IsDefaultInfoCommand { get; private set; }
        public ICommand ModeInfoCommand { get; private set; }
        public ICommand DamageSourceInfoCommand { get; private set; }
        public ICommand GoldSourceInfoCommand { get; private set; }
        public ICommand PushingTypeInfoCommand { get; private set; }
        public ICommand CurrSPInfoCommand { get; private set; }
        public ICommand AvailableSPInfoCommand { get; private set; }
        public ICommand SpOverclockInfoCommand { get; private set; }
        public ICommand IgnoredIfNotEnoughSPCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private bool _isDefault;
        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                SetProperty(ref _isDefault, value);
                if (value && SelectedConfig != null)
                {
                    if (!string.IsNullOrWhiteSpace(SelectedConfig.Name))
                    {
                        LocalSettingsORM.DefaultSPConfiguration = SelectedConfig.Name;
                    }
                }
            }
        }

        private int _spOverclock = LocalSettingsORM.SPOverclockAmount;
        public int SPOverclock
        {
            get => _spOverclock;
            set
            {
                if (SelectedConfig != null)
                {
                    SetProperty(ref _spOverclock, value);
                    LocalSettingsORM.SPOverclockAmount = value;
                }
            }
        }

        private bool _isIgnoredIfNotEnoughSP = false;
        public bool IgnoredIfNotEnoughSP
        {
            get => _isIgnoredIfNotEnoughSP;
            set
            {
                SetProperty(ref _isIgnoredIfNotEnoughSP, value);
                if (SelectedConfig != null)
                {
                    if (!string.IsNullOrWhiteSpace(SelectedConfig.Name))
                    {
                        LocalSettingsORM.SPIgnoreNotEnoughSP = value;
                    }
                }
            }
        }

        public static string LastLoadedId { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public SPOptViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.SPOptimizer;

            InitializeCommands();

            UnderConstructionHeader = "Under construction";
            UnderConstructionBody = "<p>Hello there<br/><br/>" +
                "It will still take some time until the SP Optimizer is usable again.<br/><br/>" +
                "Until this is done please use MMLH's Optimizer - it is well maintained and easy to use.</p><br/>" +
                "<p>Use the button below to open the google sheet.<br/><br/>" +
                "<b>Thank you MMLH for helping out :)</b></p><br/><br/>";
            OpenMmlhOptimizerCommand = new DelegateCommand(async () => await Launcher.OpenAsync("https://tinyurl.com/spoptimiser"));
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            ConfigInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.Configuration, AppResources.SPConfigurationDescription));
            IsDefaultInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.Default, AppResources.SPConfigDefaultDescription));
            ModeInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.Mode, AppResources.SPConfigModeDescription));
            DamageSourceInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.DamageSource, AppResources.SPDamageSourceDescription));
            GoldSourceInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.GoldSourceHeader, AppResources.GoldSourceDescription));
            PushingTypeInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.PushingTypeHeader, AppResources.PushTypeInfoText));
            CurrSPInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.CurrentSP, AppResources.CurrSPInfoText));
            AvailableSPInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.AvailableSPText, AppResources.AvailableSPInfoText));
            SpOverclockInfoCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.SPOverclockAmount, AppResources.SPOverclockInfoText));
            IgnoredIfNotEnoughSPCommand = new DelegateCommand(async () => await OpenPopupInfoAsync(AppResources.IsIgnoredIfNotEnoughSP, AppResources.IsIgnoredIfNotEnoughSPText));
            GoToSettingsCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<SPOptPage, SPOptConfigurationsPage>());
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });

            HelpCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<SPOptPage, InfoPage>());
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });

            StartCommand = new DelegateCommand(async () =>
            {
                // if config is bullshit do nothing
                if (SelectedConfig == null)
                {
                    return;
                }

                if (!await App.DBRepo.GetSPOptConfigurationSavedByID(SelectedConfig.Name))
                {
                    return;
                }

                // save mode
                await App.DBRepo.DeleteSPOptConfigurationByID(SelectedConfig.Name);
                int confSavedCount = await App.DBRepo.AddSPOptConfigurationAsync(SelectedConfig);

                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<SPOptPage, SPOptResultPage>(), new NavigationParameters() { { "id", SelectedConfig.Name } });
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });

            CreateInitialConfigCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.DefaultPath + "SPOptPage/SPOptConfigurationsPage/SPOptConfigurationDetailPage");
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });

            ReloadCommand = new DelegateCommand(async () =>
            {
                if (!await ImportSfFromClipboardAsync())
                {
                    return;
                }

                SaveFile.OnError += SaveFile_OnError;
                //Load Save file -> Only Arts
                if (!await App.Save.Initialize(loadPlayer: false, loadAccountModel: false, loadArtifacts: false, loadSkills: true, loadClan: false, loadTournament: false, loadEquipment: false))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotLoadFileText, AppResources.OKText);
                }

                SaveFile.OnError -= SaveFile_OnError;

                LoadSPValues();
            });
        }
        #endregion

        #region Private Methods

        private async Task<bool> ImportSfFromClipboardAsync()
        {
            if (LocalSettingsORM.IsReadingDataFromSavefile)
            {
                return true;
            }

            try
            {
                var importer = new ClipboardSfImporter(_dialogService);

                var result = await importer.ImportSfFromClipboardAsync(false);

                return result.IsSuccessful;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ImportSfFromClipboardAsync ERROR: {ex.Message} - {ex.Data}");
                return false;
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

        private void LoadSPValues()
        {
            try
            {
                //Set current SP and available SP from Save file
                CurrentSP = SaveFile.SPReceived.ToString();
                AvailableSP = (SaveFile.SPReceived - SaveFile.SPSpent).ToString();

            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Could not load SP values: {e.Message}");
                CurrentSP = "?";
                AvailableSP = "?";
            }
        }

        /// <summary>
        /// Does checks for default configuration
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckDefaultConfigurations()
        {
            try
            {
                // Delete default configuration if not existant
                string def = LocalSettingsORM.DefaultSPConfiguration ?? "";

                if (!string.IsNullOrWhiteSpace(def) && string.IsNullOrWhiteSpace(LastLoadedId))
                {
                    if (!await App.DBRepo.GetSPOptConfigurationSavedByID(def))
                    {
                        LocalSettingsORM.DefaultSPConfiguration = null;
                    }
                }

                // If there is only one configuration, set it as default
                ConfigAmount = await App.DBRepo.GetSPOptConfigurationsAmountAsync();

                if (ConfigAmount == 1)
                {
                    var configs = await App.DBRepo.GetAllSPOptConfigurationsAsync();
                    if (configs != null)
                    {
                        if (configs.Count == 1)
                        {
                            LocalSettingsORM.DefaultSPConfiguration = configs[0].Name;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"CheckConfigurations Error: {e.Message}");
                return false;
            }
        }
        #endregion

        #region E + D
        private void SaveFile_OnError(object sender, Helpers.CustErrorEventArgs e) => Logger.WriteToLogFile($"SPOptViewModel SaveFile Error: {e.MyException.Message}");
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await CheckDefaultConfigurations();

            // Load last configuration. if passed in parameters load that. else load default one
            LastLoadedId = parameters.ContainsKey("id")
                ? parameters["id"].ToString()
                : !string.IsNullOrWhiteSpace(LastLoadedId)
                    ? LastLoadedId
                    : LocalSettingsORM.DefaultSPConfiguration ?? "";

            SelectedConfig = await SPOptConfigFactory.LoadConfigAsync(App.DBRepo, App.Save, LastLoadedId);

            if (!string.IsNullOrWhiteSpace(SelectedConfig.Name) && SelectedConfig.Name == (LocalSettingsORM.DefaultSPConfiguration ?? ""))
            {
                IsDefault = true;
            }

            LoadSPValues();

            IgnoredIfNotEnoughSP = LocalSettingsORM.SPIgnoreNotEnoughSP;

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}