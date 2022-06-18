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
    public class RaidStrategyDetailViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        private RaidStrategy _selectedConfig;
        /// <summary>
        /// The current config
        /// </summary>
        public RaidStrategy SelectedConfig
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

        private List<string> _attackTypes = Enum.GetNames(typeof(EnemyAttackType)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Mode Types
        /// </summary>
        public List<string> AttackTypes { get => _attackTypes; set => SetProperty(ref _attackTypes, value); }

        private List<string> _enemies;
        /// <summary>
        /// Available Mode Types
        /// </summary>
        public List<string> Enemies { get => _enemies; set => SetProperty(ref _enemies, value); }

        public ICommand SaveCommand { get; private set; }
        
        public ICommand LeftShoulderCommand { get; private set; }
        public ICommand HeadCommand { get; private set; }
        public ICommand RightShoulderCommand { get; private set; }
        public ICommand LeftArmCommand { get; private set; }
        public ICommand TorsoCommand { get; private set; }
        public ICommand RightArmCommand { get; private set; }
        public ICommand LeftLegCommand { get; private set; }
        public ICommand RightLegCommand { get; private set; }

        private bool _isNameEditable = true;
        public bool IsNameEditable { get => _isNameEditable; set => SetProperty(ref _isNameEditable, value); }

        private string _imagePath;
        public string ImagePath { get => _imagePath; set => SetProperty(ref _imagePath, value); }
        
        #region Attack type images

        private string _leftShoulderAttackImage;
        public string LeftShoulderAttackImage { get => _leftShoulderAttackImage; set => SetProperty(ref _leftShoulderAttackImage, value); }
        
        private string _headAttackImage;
        public string HeadAttackImage { get => _headAttackImage; set => SetProperty(ref _headAttackImage, value); }
        
        private string _rightShoulderAttackImage;
        public string RightShoulderAttackImage { get => _rightShoulderAttackImage; set => SetProperty(ref _rightShoulderAttackImage, value); }
        
        private string _leftArmAttackImage;
        public string LeftArmAttackImage { get => _leftArmAttackImage; set => SetProperty(ref _leftArmAttackImage, value); }
        
        private string _torsoAttackImage;
        public string TorsoAttackImage { get => _torsoAttackImage; set => SetProperty(ref _torsoAttackImage, value); }
        
        private string _rightArmAttackImage;
        public string RightArmAttackImage { get => _rightArmAttackImage; set => SetProperty(ref _rightArmAttackImage, value); }
        
        private string _leftLegAttackImage;
        public string LeftLegAttackImage { get => _leftLegAttackImage; set => SetProperty(ref _leftLegAttackImage, value); }
        
        private string _rightLegAttackImage;
        public string RightLegAttackImage { get => _rightLegAttackImage; set => SetProperty(ref _rightLegAttackImage, value); }

        #endregion
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public RaidStrategyDetailViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.Strategy;

            InitializeCommands();

            Enemies = GetEnemyNames();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            SaveCommand = new DelegateCommand(async () => await SaveConfigurationAsync());

            LeftShoulderCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("LeftShoulder"));
            HeadCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("Head"));
            RightShoulderCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("RightShoulder"));
            LeftArmCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("LeftArm"));
            TorsoCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("Torso"));
            RightArmCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("RightArm"));
            LeftLegCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("LeftLeg"));
            RightLegCommand = new DelegateCommand<string>(async (o) => await SetAttacktypeAsync("RightLeg"));
        }

        private void ReloadAttackImages()
        {
            LeftShoulderAttackImage = GetImageFromAttackType(SelectedConfig.LeftShoulder);
            HeadAttackImage = GetImageFromAttackType(SelectedConfig.Head);
            RightShoulderAttackImage = GetImageFromAttackType(SelectedConfig.RightShoulder);
            LeftArmAttackImage = GetImageFromAttackType(SelectedConfig.LeftHand);
            TorsoAttackImage = GetImageFromAttackType(SelectedConfig.Torso);
            RightArmAttackImage = GetImageFromAttackType(SelectedConfig.RightHand);
            LeftLegAttackImage = GetImageFromAttackType(SelectedConfig.LeftLeg);
            RightLegAttackImage = GetImageFromAttackType(SelectedConfig.RightLeg);
        }

        private EnemyAttackType GetAttackTypeFromString(string s)
        {
            if(s == AppResources.No)
            {
                return EnemyAttackType.No;
            }

            if(s == AppResources.VM)
            {
                return EnemyAttackType.VM;
            }

            if(s == AppResources.ToP)
            {
                return EnemyAttackType.ToP;
            }

            if (s == AppResources.One)
            {
                return EnemyAttackType.One;
            }
            if (s == AppResources.Two)
            {
                return EnemyAttackType.Two;
            }
            if (s == AppResources.Three)
            {
                return EnemyAttackType.Three;
            }
            if (s == AppResources.Four)
            {
                return EnemyAttackType.Four;
            }
            if (s == AppResources.Five)
            {
                return EnemyAttackType.Five;
            }
            if (s == AppResources.Six)
            {
                return EnemyAttackType.Six;
            }
            if (s == AppResources.Seven)
            {
                return EnemyAttackType.Seven;
            }
            if (s == AppResources.Eight)
            {
                return EnemyAttackType.Eight;
            }

            return EnemyAttackType.Yes;
        }

        private string GetImageFromAttackType(EnemyAttackType attackType)
        {
            switch (attackType)
            {
                case EnemyAttackType.Yes:
                    return "yes";
                case EnemyAttackType.No:
                    return "no";
                case EnemyAttackType.VM:
                    return "FinisherAttack";
                case EnemyAttackType.ToP:
                    return "TotemFairySkill";
                case EnemyAttackType.One:
                    return "One";
                case EnemyAttackType.Two:
                    return "Two";
                case EnemyAttackType.Three:
                    return "Three";
                case EnemyAttackType.Four:
                    return "Four";
                case EnemyAttackType.Five:
                    return "Five";
                case EnemyAttackType.Six:
                    return "Six";
                case EnemyAttackType.Seven:
                    return "Seven";
                case EnemyAttackType.Eight:
                    return "Eight";
                default:
                    return "yes";
            }
        }

        private async Task<bool> SetAttacktypeAsync(string o)
        {
            try
            {
                var choices = Enum.GetNames(typeof(EnemyAttackType)).Select(x => x.TranslatedString()).ToArray<string>();
                var result = await _dialogService.DisplayActionSheetAsync(AppResources.Choose
                    , AppResources.CancelText
                    , AppResources.DestroyText
                    , choices);

                if(result == null || result == AppResources.CancelText || result == AppResources.DestroyText)
                {
                    return true;
                }

                switch (o)
                {
                    case "LeftShoulder":
                        SelectedConfig.LeftShoulder = GetAttackTypeFromString(result);
                    break;
                    case "Head":
                        SelectedConfig.Head = GetAttackTypeFromString(result);
                        break;
                    case "RightShoulder":
                        SelectedConfig.RightShoulder = GetAttackTypeFromString(result);
                        break;
                    case "LeftArm":
                        SelectedConfig.LeftHand = GetAttackTypeFromString(result);
                        break;
                    case "Torso":
                        SelectedConfig.Torso = GetAttackTypeFromString(result);
                        break;
                    case "RightArm":
                        SelectedConfig.RightHand = GetAttackTypeFromString(result);
                        break;
                    case "LeftLeg":
                        SelectedConfig.LeftLeg = GetAttackTypeFromString(result);
                        break;
                    case "RightLeg":
                        SelectedConfig.RightLeg = GetAttackTypeFromString(result);
                        break;
                    default:
                        break;
                }

                ReloadAttackImages();

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR SetAttacktypeAsync {o}: {ex.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
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

                // save config
                if (await App.DBRepo.IsRaidStrategyExisting(SelectedConfig.Name))
                {
                    var saveAccepted = await _dialogService.DisplayAlertAsync(AppResources.OKText, AppResources.DoYouWantToOverwrite, AppResources.YesText, AppResources.NoText);

                    //if user does not want to overwrite -> do not save
                    if (!saveAccepted)
                    {
                        return false;
                    }

                    await App.DBRepo.UpdateRaidStrategyAsync(SelectedConfig);
                }
                else
                {
                    int confSavedCount = await App.DBRepo.AddRaidStrategyAsync(SelectedConfig);
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

        private void SelectedConfig_OnEnemyNameChanged(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                return;
            }

            RaidInfoHandler.LoadRaidInfos();

            SelectedConfig.EnemyId = RaidInfoHandler.EnemyInfos.Where(x => x.Name == newValue).FirstOrDefault()?.EnemyId;

            ImagePath = SelectedConfig.ImagePath == "?" ? "notfound" : SelectedConfig.ImagePath;
        }

        private void FillEnemyNameFromId()
        {
            SelectedConfig.EnemyName = RaidInfoHandler.EnemyInfos.Where(x => x.EnemyId == SelectedConfig.EnemyId).FirstOrDefault()?.Name;
        }

        private List<string> GetEnemyNames()
        {
            RaidInfoHandler.LoadRaidInfos();

            return RaidInfoHandler.EnemyInfos.Select(x => x.Name).ToList();
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
                SelectedConfig = await App.DBRepo.GetRaidStrategyByID(parameters["id"].ToString());
                SelectedConfig.IsSaved = true;
            }
            else
            {
                SelectedConfig = new RaidStrategy();
            }

            ReloadAttackImages();

            // set if saved
            SelectedConfig.IsSaved = await App.DBRepo.IsRaidStrategyExisting(SelectedConfig.Name);

            if (SelectedConfig.IsSaved)
            {
                IsNameEditable = false;
            }

            // Abo on enemy name change to load the correct id
            SelectedConfig.OnEnemyNameChanged += SelectedConfig_OnEnemyNameChanged;

            FillEnemyNameFromId();

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}
