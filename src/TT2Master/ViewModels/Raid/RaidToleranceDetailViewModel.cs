using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Raid;
using TT2Master.Resources;

namespace TT2Master.ViewModels.Raid
{
    public class RaidToleranceDetailViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        private RaidTolerance _selectedConfig;
        /// <summary>
        /// The current config
        /// </summary>
        public RaidTolerance SelectedConfig
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

        private List<string> _overkillCalculationTypes = Enum.GetNames(typeof(OverkillCalculationType)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Overkill calculation Types
        /// </summary>
        public List<string> OverkillCalculationTypes { get => _overkillCalculationTypes; set => SetProperty(ref _overkillCalculationTypes, value); }
        
        private List<string> _amountCalculationTypes = Enum.GetNames(typeof(AttackAmountCalculationType)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Min Amount calculation Types
        /// </summary>
        public List<string> AmountCalculationTypes { get => _amountCalculationTypes; set => SetProperty(ref _amountCalculationTypes, value); }
        
        private List<string> _averageCalculationTypes = Enum.GetNames(typeof(AverageDamageCalculationType)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Average damage calculation Types
        /// </summary>
        public List<string> AverageCalculationTypes { get => _averageCalculationTypes; set => SetProperty(ref _averageCalculationTypes, value); }

        public ICommand SaveCommand { get; private set; }

        private bool _isNameEditable = true;
        public bool IsNameEditable { get => _isNameEditable; set => SetProperty(ref _isNameEditable, value); }

        private bool _isPercentageCapVisisble = false;
        public bool IsPercentageCapVisisble { get => _isPercentageCapVisisble; set => SetProperty(ref _isPercentageCapVisisble, value); }

        private bool _isWaveInfoTextVisisble = false;
        public bool IsWaveInfoTextVisisble { get => _isWaveInfoTextVisisble; set => SetProperty(ref _isWaveInfoTextVisisble, value); }

        private bool _isAbsoluteAttackVisisble = false;
        public bool IsAbsoluteAttackVisisble { get => _isAbsoluteAttackVisisble; set => SetProperty(ref _isAbsoluteAttackVisisble, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public RaidToleranceDetailViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.Tolerance;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            SaveCommand = new DelegateCommand(async () => await SaveConfigurationAsync());
        }

        private void EnsureConfigIsValid()
        {
            if(SelectedConfig.AmountType == AttackAmountCalculationType.RelativeFromAllAttacksSum && SelectedConfig.AmountTolerance > 2)
            {
                SelectedConfig.AmountTolerance = 2;
            }
        }

        private async Task<bool> SaveConfigurationAsync()
        {
            try
            {
                //Check if Name is filled
                if (string.IsNullOrWhiteSpace(SelectedConfig.Name))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.FillNameFirstText, AppResources.OKText);
                    return false;
                }

                EnsureConfigIsValid();

                // save config
                if (await App.DBRepo.IsRaidToleranceExisting(SelectedConfig.Name))
                {
                    var saveAccepted = await _dialogService.DisplayAlertAsync(AppResources.OKText, AppResources.DoYouWantToOverwrite, AppResources.YesText, AppResources.NoText);

                    //if user does not want to overwrite -> do not save
                    if (!saveAccepted)
                    {
                        return false;
                    }

                    await App.DBRepo.UpdateRaidToleranceAsync(SelectedConfig);
                }
                else
                {
                    int confSavedCount = await App.DBRepo.AddRaidToleranceAsync(SelectedConfig);
                    Logger.WriteToLogFile($"Saved configurations: {confSavedCount}");
                }

                SelectedConfig.IsSaved = true;
                IsNameEditable = false;

                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ChangesSavedText, AppResources.OKText);

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error saving configuration: {e.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);

                return false;
            }
        }
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            // Load configuration if passed in parameters
            if (parameters.ContainsKey("id"))
            {
                SelectedConfig = await App.DBRepo.GetRaidToleranceByID(parameters["id"].ToString());
                SelectedConfig.IsSaved = true;
            }
            else
            {
                SelectedConfig = new RaidTolerance();
            }

            // set if saved
            SelectedConfig.IsSaved = await App.DBRepo.IsRaidToleranceExisting(SelectedConfig.Name);

            if (SelectedConfig.IsSaved)
            {
                IsNameEditable = false;
            }

            SelectedConfig.OnAttackAmountChanged += SelectedConfig_OnAttackAmountChanged;
            SelectedConfig_OnAttackAmountChanged();

            base.OnNavigatedTo(parameters);
        }

        private void SelectedConfig_OnAttackAmountChanged()
        {
            IsPercentageCapVisisble = SelectedConfig.AmountType == AttackAmountCalculationType.RelativeFromAllAttacksSum;
            IsWaveInfoTextVisisble = SelectedConfig.AmountType == AttackAmountCalculationType.AbsoluteInWavesExcludingLastWave
                || SelectedConfig.AmountType == AttackAmountCalculationType.AbsoluteInWavesIncludingLastWave;

            IsAbsoluteAttackVisisble = SelectedConfig.AmountType == AttackAmountCalculationType.AbsoluteInAttacks;
        }
        #endregion
    }
}
