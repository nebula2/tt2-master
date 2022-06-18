using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master.ViewModels.Arti
{
    public class ArtOptSettingsPopupViewModel : ViewModelBase
    {
        #region Member
        private bool _settingsWereSaved = true;

        private ArtOptSettings _currentSettings;
        /// <summary>
        /// Input from User
        /// </summary>
        public ArtOptSettings CurrentSettings
        {
            get => _currentSettings;
            set
            {
                if (value != _currentSettings)
                {
                    SetProperty(ref _currentSettings, value);
                }
            }
        }

        private readonly INavigationService _navigationService;

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private IPageDialogService _dialogService;

        private string _selectedBuild;
        /// <summary>
        /// Currently selected Build
        /// </summary>
        public string SelectedBuild
        {
            get => _selectedBuild;
            set
            {
                if (value == null)
                {
                    return;
                }

                SetProperty(ref _selectedBuild, value);
            }
        }

        private string[] _availableBuilds = { };
        /// <summary>
        /// Array of Builds available for this player
        /// </summary>
        public string[] AvailableBuilds { get => _availableBuilds; set => SetProperty(ref _availableBuilds, value); }

        private int _stepAmountId = 0;
        /// <summary>
        /// Step amount for relic level increase
        /// </summary>
        public int StepAmountId
        {
            get => _stepAmountId; set => SetProperty(ref _stepAmountId, value);
        }

        private List<ArtStepAmount> _stepAmountOptions = ArtStepAmounts.StepAmounts;
        /// <summary>
        /// Step Options for relic level increase
        /// </summary>
        public List<ArtStepAmount> StepAmountOptions { get => _stepAmountOptions; set => SetProperty(ref _stepAmountOptions, value); }

        private double _lifeTimeSpentPercentageOnAmount;
        /// <summary>
        /// To calculate Upgrade amount under consideration of Lifetime
        /// </summary>
        public double LifeTimeSpentPercentageOnAmount
        {
            get => _lifeTimeSpentPercentageOnAmount;
            set => SetProperty(ref _lifeTimeSpentPercentageOnAmount, value);
        }

        private double _boSRoyalty;
        /// <summary>
        /// BoS Royalty. The Percantage you want to spend on BoS
        /// </summary>
        public double BoSRoyalty
        {
            get => _boSRoyalty;
            set
            {
                if (value != _boSRoyalty)
                {
                    SetProperty(ref _boSRoyalty, value);
                }
            }
        }

        private double _boSTourneyRoyalty;
        /// <summary>
        /// BoS Royalty. The Percantage you want to spend on BoS in Tourney
        /// </summary>
        public double BoSTourneyRoyalty
        {
            get => _boSTourneyRoyalty;
            set
            {
                if (value != _boSTourneyRoyalty)
                {
                    SetProperty(ref _boSTourneyRoyalty, value);
                }
            }
        }

        private double _minEfficiency;
        /// <summary>
        /// MinEfficiency before optimization stops
        /// </summary>
        public double MinEfficiency
        {
            get => _minEfficiency;
            set
            {
                if (value != _minEfficiency)
                {
                    SetProperty(ref _minEfficiency, Math.Max(value, 1.01));
                }
            }
        }

        private int _maxArtifactAmount;
        /// <summary>
        /// MaxArtifactAmount before optimization stops
        /// </summary>
        public int MaxArtifactAmount
        {
            get => _maxArtifactAmount;
            set
            {
                if (value != _maxArtifactAmount)
                {
                    SetProperty(ref _maxArtifactAmount, value);
                }
            }
        }

        private bool _isClickEnabled;
        public bool IsClickEnabled { get => _isClickEnabled; set => SetProperty(ref _isClickEnabled, value); }

        private HeroDmgType _heroDamage;
        public HeroDmgType HeroDamage
        {
            get => _heroDamage;
            set => SetProperty(ref _heroDamage, value);
        }

        private HeroBaseType _heroBaseType;
        public HeroBaseType HeroBaseType { get => _heroBaseType; set => SetProperty(ref _heroBaseType, value); }

        private List<string> _heroDmg = Enum.GetNames(typeof(HeroDmgType)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Hero Damage Types
        /// </summary>
        public List<string> HeroDmg { get => _heroDmg; set => SetProperty(ref _heroDmg, value); }

        private List<string> _heroBaseTypes = Enum.GetNames(typeof(HeroBaseType)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Hero Base Types
        /// </summary>
        public List<string> HeroBaseTypes { get => _heroBaseTypes; set => SetProperty(ref _heroBaseTypes, value); }

        private ArtifactBuild _build;
        public ArtifactBuild Build { get => _build; set => SetProperty(ref _build, value); }

        private GoldType _currGoldType = GoldType.pHoM;
        public GoldType CurrGoldType { get => _currGoldType; set => SetProperty(ref _currGoldType, value); }

        private List<string> _goldSources = Enum.GetNames(typeof(GoldType)).Select(x => x.TranslatedString()).ToList<string>();
        public List<string> GoldSources { get => _goldSources; set => SetProperty(ref _goldSources, value); }

        private bool _hasHerosMaxed;
        public bool HasHerosMaxed { get => _hasHerosMaxed; set => SetProperty(ref _hasHerosMaxed, value); }

        private bool _isPlayerInTournament;
        public bool IsPlayerInTournament { get => _isPlayerInTournament; set => SetProperty(ref _isPlayerInTournament, value); }

        private bool _isLoadingDataFromSavefile = true;

        public bool IsLoadingDataFromSavefile { get => _isLoadingDataFromSavefile; set => SetProperty(ref _isLoadingDataFromSavefile, value); }
        
        #region Commands
        /// <summary>
        /// Command to save
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// Command for opening BoS-Info
        /// </summary>
        public ICommand BoSInfoCommand { get; private set; }

        /// <summary>
        /// Command for opening BoST-Info
        /// </summary>
        public ICommand BoSTInfoCommand { get; private set; }

        /// <summary>
        /// Lifetime % Info Command
        /// </summary>
        public ICommand LTPInfoCommand { get; private set; }

        public ICommand BuildInfoCommand { get; private set; }
        public ICommand PTInfoCommand { get; private set; }
        public ICommand SAInfoCommand { get; private set; }
        public ICommand HDTInfoCommand { get; private set; }
        public ICommand HRInfoCommand { get; private set; }
        public ICommand ClickInfoCommand { get; private set; }
        public ICommand GoldTypeInfoCommand { get; private set; }
        public ICommand MaxArtInfoCommand { get; private set; }
        public ICommand MinEffInfoCommand { get; private set; }
        public ICommand HasHerosMaxedInfoCommand { get; private set; }
        #endregion
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public ArtOptSettingsPopupViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.SettingsHeader;

            _navigationService = navigationService;
            _dialogService = dialogService;

            IsLoadingDataFromSavefile = LocalSettingsORM.IsReadingDataFromSavefile;

            InitCommands();
        }
        #endregion

        #region CommandMethods
        /// <summary>
        /// Command to Save category
        /// </summary>
        private async Task<bool> SaveExecuteAsync()
        {
            try
            {
                Logger.WriteToLogFile("ArtOpt.SaveExecuteAsync: Going to save");
                FillSettingsFromProperties();

                //Make some checks
                if (!CurrentSettings.ValuesComplete())
                {
                    Logger.WriteToLogFile($"ArtOpt.SaveExecuteAsync: values are not complete.\n{CurrentSettings.ToString()}");

                    await ToastSender.SendToastAsync(AppResources.NotAllValuesEntered, _dialogService);
                }

                //store value
                Logger.WriteToLogFile($"ArtOpt.SaveExecuteAsync: going to save settings");
                int res = await App.DBRepo.UpdateArtOptSettingsAsync(CurrentSettings);

                //store gold shit
                if (_settingsWereSaved)
                {
                    Logger.WriteToLogFile($"ArtOpt.SaveExecuteAsync: settings were saved. setting gold source to build");

                    //Set gold source to build
                    Build.GoldSource = CurrGoldType;
                    //save build
                    int buildSaved = await App.DBRepo.UpdateArtifactBuildAsync(Build);
                }

                Logger.WriteToLogFile($"ArtOpt.SaveExecuteAsync: saved {res}.\n{CurrentSettings.ToString()}\nGoing back");
                //leave this shit - i am done
                var result = await _navigationService.GoBackAsync(new NavigationParameters() { { "id", CurrentSettings.ID } });
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ArtOptSettings Error: {e.Message}");
                return false;
            }
        }

        #region Info
        /// <summary>
        /// Action for <see cref="BoSInfoCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BoSInfoExecute() => await OpenPopupInfoAsync("BoS Royalty", AppResources.BosInfoText);

        /// <summary>
        /// Action for <see cref="BoSTInfoCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BoSTInfoExecute() => await OpenPopupInfoAsync("BoS T-Royalty", AppResources.BosTourneyInfoText);

        /// <summary>
        /// Action for <see cref="BoSInfoCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> LTPInfoExecute() => await OpenPopupInfoAsync(AppResources.LifeTimeHeader, AppResources.LifeTimeInfoText);

        private async Task<bool> BuildInfoExecute() => await OpenPopupInfoAsync("Build", AppResources.BuildInfoText);

        private async Task<bool> PTInfoExecute() => await OpenPopupInfoAsync(AppResources.PushingTypeHeader, AppResources.PushTypeInfoText);

        private async Task<bool> SAInfoExecute() => await OpenPopupInfoAsync(AppResources.StepAmountHeader, AppResources.StepAmountInfoText);

        private async Task<bool> HDInfoExecute() => await OpenPopupInfoAsync(AppResources.HeroDamageTypeHeader, AppResources.HeroDamageInfoText);

        private async Task<bool> HRInfoExecute() => await OpenPopupInfoAsync(AppResources.RangeHeader, AppResources.HeroRangeInfoText);

        private async Task<bool> ClickInfoExecute() => await OpenPopupInfoAsync(AppResources.ClickSuggestionInfoHeader, AppResources.ClickSuggestionInfoText);

        private async Task<bool> ClickGoldExecute() => await OpenPopupInfoAsync(AppResources.GoldTypeInfoHeader, AppResources.ClickGoldTypeInfoText);
        private async Task<bool> MaxArtifactAmountExecute() => await OpenPopupInfoAsync(AppResources.MaxArtifactAmountOption, AppResources.MaxArtifactAmountInfoText);
        private async Task<bool> MinEfficiencyExecute() => await OpenPopupInfoAsync(AppResources.MinEfficiencyOption, AppResources.MinEfficiencyInfoText);
        #endregion
        #endregion

        #region Helper
        /// <summary>
        /// Initializes Commands
        /// </summary>
        private void InitCommands()
        {
            SaveCommand = new DelegateCommand(async () => await SaveExecuteAsync());

            BoSInfoCommand = new DelegateCommand(async () => await BoSInfoExecute());
            BoSTInfoCommand = new DelegateCommand(async () => await BoSTInfoExecute());
            LTPInfoCommand = new DelegateCommand(async () => await LTPInfoExecute());
            BuildInfoCommand = new DelegateCommand(async () => await BuildInfoExecute());
            PTInfoCommand = new DelegateCommand(async () => await PTInfoExecute());
            SAInfoCommand = new DelegateCommand(async () => await SAInfoExecute());
            HDTInfoCommand = new DelegateCommand(async () => await HDInfoExecute());
            HRInfoCommand = new DelegateCommand(async () => await HRInfoExecute());
            ClickInfoCommand = new DelegateCommand(async () => await ClickInfoExecute());
            GoldTypeInfoCommand = new DelegateCommand(async () => await ClickGoldExecute());
            MaxArtInfoCommand = new DelegateCommand(async () => await MaxArtifactAmountExecute());
            MinEffInfoCommand = new DelegateCommand(async () => await MinEfficiencyExecute());
            HasHerosMaxedInfoCommand = new DelegateCommand(async () => 
            {
                await OpenPopupInfoAsync(AppResources.HasHerosMaxedOption, AppResources.HasHerosMaxedInfoText);
            });
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

        #region Population
        /// <summary>
        /// Fills Settings from Properties
        /// </summary>
        private void FillSettingsFromProperties()
        {
            Logger.WriteToLogFile("Filling settings from properties");
            CurrentSettings = new ArtOptSettings()
            {
                BoSRoyalty = BoSRoyalty,
                BosTourneyRoyalty = BoSTourneyRoyalty,
                HeroBaseTypeInt = (int)HeroBaseType,
                HeroDamageInt = (int)HeroDamage,
                LifeTimeSpentPercentageOnAmount = LifeTimeSpentPercentageOnAmount < 5 ? 5 : LifeTimeSpentPercentageOnAmount,
                Build = SelectedBuild,
                IsClickSuggestionEnabled = IsClickEnabled,
                MaxArtifactAmount = MaxArtifactAmount,
                MinEfficiency = MinEfficiency,
                HasHerosMaxed = HasHerosMaxed,
            };

            try
            {
                LocalSettingsORM.IsPlayerInTournament = IsPlayerInTournament;

                if (StepAmountId > ArtStepAmounts.StepAmounts.Count)
                {
                    StepAmountId = 0;
                }

                CurrentSettings.StepAmountId = ArtStepAmounts.StepAmounts[StepAmountId].ID;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ArtOptSettings.FillSettingsFromProperties: Error {ex.Message}");

                CurrentSettings.StepAmountId = 0;
            }

            Logger.WriteToLogFile("done doing that");
        }

        /// <summary>
        /// Fills Properties from Settings
        /// </summary>
        private async Task<bool> FillPropertiesFromSettingsAsync()
        {
            Logger.WriteToLogFile("Filling properties from settings");
            try
            {
                var sa = ArtStepAmounts.StepAmounts.Where(x => x.ID == CurrentSettings.StepAmountId)?.FirstOrDefault();
                StepAmountId = sa == null ? 0 : ArtStepAmounts.StepAmounts.FindIndex(x => x.ID == sa.ID);

                HeroBaseType = (HeroBaseType)CurrentSettings.HeroBaseTypeInt;
                HeroDamage = (HeroDmgType)CurrentSettings.HeroDamageInt;
                SelectedBuild = CurrentSettings.Build;
                IsClickEnabled = CurrentSettings.IsClickSuggestionEnabled;

                IsClickEnabled = true;

                MaxArtifactAmount = CurrentSettings.MaxArtifactAmount <= 0 ? 30 : CurrentSettings.MaxArtifactAmount;
                MinEfficiency = Math.Max(CurrentSettings.MinEfficiency,1.01);

                BoSRoyalty = CurrentSettings.BoSRoyalty;
                BoSTourneyRoyalty = CurrentSettings.BosTourneyRoyalty;

                HasHerosMaxed = CurrentSettings.HasHerosMaxed;
                IsPlayerInTournament = LocalSettingsORM.IsPlayerInTournament;

                Logger.WriteToLogFile("done doing that");

                //Load gold source from current build
                if (_settingsWereSaved)
                {
                    Logger.WriteToLogFile("settings were saved. getting build");

                    bool buildDone = await SetBuildAsync(CurrentSettings.Build);

                    if (buildDone)
                    {
                        Logger.WriteToLogFile($"setting gold source to {Build.GoldSource}");

                        CurrGoldType = Build.GoldSource;
                    }
                    else
                    {
                        Logger.WriteToLogFile($"could not get build");
                        _settingsWereSaved = false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ArtOptSettings.FillPropsFromSettings: Error {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sets the Build async
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        private async Task<bool> SetBuildAsync(string build)
        {
            try
            {
                Build = await App.DBRepo.GetArtifactBuildByName(build);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion 
        #endregion

        #region Navigation
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            //Load Settings
            AvailableBuilds = await App.DBRepo.GetArtifactBuildNamesArrayAsync();

            if (AvailableBuilds == null || AvailableBuilds.Length == 0)
            {
                _settingsWereSaved = false;
                Logger.WriteToLogFile("ArtOptSettings.OnNavigatedTo: AvailableBuilds null or Length == 0");
            }

            if (!parameters.ContainsKey("id"))
            {
                _settingsWereSaved = false;
                Logger.WriteToLogFile("ArtOptSettings.OnNavigatedTo: No id given. Creating new CurrentSettings");
                CurrentSettings = new ArtOptSettings();
            }
            else
            {
                Logger.WriteToLogFile("ArtOptSettings.OnNavigatedTo: id given. loading from db");
                //Load from DB
                CurrentSettings = await App.DBRepo.GetArtOptSettingsByID(parameters["id"].ToString());

                if (CurrentSettings == null)
                {
                    _settingsWereSaved = false;
                    Logger.WriteToLogFile("ArtOptSettings.OnNavigatedTo: No currentSettings got from db");
                    CurrentSettings = new ArtOptSettings();
                }
                else
                {
                    Logger.WriteToLogFile($"ArtOptSettings.OnNavigatedTo: loaded currentsettings from db.\n{_currentSettings.ToString()}");
                }
            }

            bool propFilled = await FillPropertiesFromSettingsAsync();

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}