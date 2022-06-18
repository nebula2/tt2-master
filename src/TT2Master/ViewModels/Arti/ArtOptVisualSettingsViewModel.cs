using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Model.Arti;
using TT2Master.Resources;

namespace TT2Master.ViewModels.Arti
{
    public class ArtOptVisualSettingsViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;


        private ArtifactOptimizerViewMode _artViewMode;
        public ArtifactOptimizerViewMode ArtViewMode
        {
            get => _artViewMode;
            set
            {
                if (value != _artViewMode && value >= 0)
                {
                    SetProperty(ref _artViewMode, value);
                }

                IsDefaultViewActive = value == ArtifactOptimizerViewMode.DefaultList;
            }
        }

        private List<string> _artViewModes = Enum.GetNames(typeof(ArtifactOptimizerViewMode)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available artifact view modes
        /// </summary>
        public List<string> ArtViewModes { get => _artViewModes; set => SetProperty(ref _artViewModes, value); }
        
        private ArtifactOptimizerDirectionMode _artDirectionMode;
        public ArtifactOptimizerDirectionMode ArtDirectionMode
        {
            get => _artDirectionMode;
            set
            {
                if (value != _artDirectionMode && value >= 0)
                {
                    SetProperty(ref _artDirectionMode, value);
                }
            }
        }

        private List<string> _artDirectionModes = Enum.GetNames(typeof(ArtifactOptimizerDirectionMode)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available artifact direction modes
        /// </summary>
        public List<string> ArtDirectionModes { get => _artDirectionModes; set => SetProperty(ref _artDirectionModes, value); }


        private int _cellSize;
        public int CellSize { get => _cellSize; set => SetProperty(ref _cellSize, value); }

        private bool _isDefaultViewActive;
        public bool IsDefaultViewActive { get => _isDefaultViewActive; set => SetProperty(ref _isDefaultViewActive, value); }

        private ObservableCollection<ArtifactToOptimize> _optimizeList = new ObservableCollection<ArtifactToOptimize>(ArtifactToOptimizeMockData.MockData);
        public ObservableCollection<ArtifactToOptimize> OptimizeList { get => _optimizeList; set => SetProperty(ref _optimizeList, value); }

        public ICommand SaveCommand { get; private set; }
        public ICommand RevertToDefaultCommand { get; private set; }
        #endregion

        #region Ctor
        public ArtOptVisualSettingsViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            Title = AppResources.SettingsHeader;

            SaveCommand = new DelegateCommand(async () => await SaveSettingsAsync());
            RevertToDefaultCommand = new DelegateCommand(Revert);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Loads settings
        /// </summary>
        private void LoadSettings()
        {
            ArtViewMode = (ArtifactOptimizerViewMode)LocalSettingsORM.ArtOptViewModeInt;
            ArtDirectionMode = (ArtifactOptimizerDirectionMode)LocalSettingsORM.ArtOptDirectionModeInt;
            CellSize = LocalSettingsORM.ArtOptCellSize;

            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// Resets to default
        /// </summary>
        private void Revert()
        {
            ArtViewMode = ArtifactOptimizerViewMode.DefaultList;
            ArtDirectionMode = ArtifactOptimizerDirectionMode.Row;
            CellSize = 50;
        }

        /// <summary>
        /// Saves settings and notifies user
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveSettingsAsync()
        {
            LocalSettingsORM.ArtOptViewModeInt = (int)ArtViewMode;
            LocalSettingsORM.ArtOptDirectionModeInt = (int)ArtDirectionMode;
            LocalSettingsORM.ArtOptCellSize = CellSize;

            await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ChangesSavedText, AppResources.OKText);

            return true;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// When navigating to this - load some stuff
        /// </summary>
        /// <param name="parameters">Artifact</param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadSettings();

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}