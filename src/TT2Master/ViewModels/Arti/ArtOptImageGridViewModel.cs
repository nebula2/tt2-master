using Microsoft.AppCenter.Analytics;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model;
using TT2Master.Model.Arti;
using TT2Master.Model.DataSource;
using TT2Master.Model.Navigation;
using TT2Master.Resources;
using TT2Master.Views.Arti;

namespace TT2Master
{
    /// <summary>
    /// VM for Artifact Optimization
    /// </summary>
    public class ArtOptImageGridViewModel : ViewModelBase
    {
        #region Properties
        #region Optimizer Settings
        private ArtifactOptimization _optimization;
        public ArtifactOptimization Optimization { get => _optimization; set => SetProperty(ref _optimization, value); }

        public bool IsDefaultViewActive { get; set; }

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
            }
        }

        private int _cellSize;
        public int CellSize { get => _cellSize; set => SetProperty(ref _cellSize, value); }
        #endregion

        #region Commands
        /// <summary>
        /// Reloads Artifact Levels
        /// </summary>
        public ICommand ReloadArtifactLevelCommand { get; private set; }

        /// <summary>
        /// Navigate to Artifact Optimizer Settings Popup
        /// </summary>
        public ICommand GoToSettingsCommand { get; private set; }

        /// <summary>
        /// Navigate to Artifact Optimizer View Settings Popup
        /// </summary>
        public ICommand GoToViewSettingsCommand { get; private set; }

        /// <summary>
        /// Help Command
        /// </summary>
        public ICommand HelpCommand { get; private set; }
        
        public ICommand MarkAsDoneCommand { get; private set; }

        #endregion

        #region Others
        private bool _isRefreshing = false;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        private ObservableCollection<ArtifactToOptimize> _optimizeList;
        /// <summary>
        /// List of Artifacts to optimize
        /// </summary>
        public ObservableCollection<ArtifactToOptimize> OptimizeList
        {
            get => _optimizeList;
            set
            {
                if (value != _optimizeList)
                {
                    SetProperty(ref _optimizeList, value);
                }
            }
        }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private readonly INavigationService _navigationService;
        #endregion
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public ArtOptImageGridViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Optimization = new ArtifactOptimization(null, null, App.Save, true, _dialogService);
            Optimization.OnLogMePlease += Optimization_OnLogMePlease;
            if (OptimizeLogger.WriteLog)
            {
                Optimization.OnOptiLogMePlease += Optimization_OnOptiLogMePlease;
            }

            Title = AppResources.OptimizerHeader;

            InitCommands();
        }
        #endregion

        #region General Helper

        private void LoadVisualSettings()
        {
            IsDefaultViewActive = (ArtifactOptimizerViewMode)LocalSettingsORM.ArtOptViewModeInt == ArtifactOptimizerViewMode.DefaultList;
            ArtViewMode = (ArtifactOptimizerViewMode)LocalSettingsORM.ArtOptViewModeInt;
            ArtDirectionMode = (ArtifactOptimizerDirectionMode)LocalSettingsORM.ArtOptDirectionModeInt;
            CellSize = LocalSettingsORM.ArtOptCellSize;

            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// Initializes Commands
        /// </summary>
        private void InitCommands()
        {
            ReloadArtifactLevelCommand = new DelegateCommand(async () => await ReloadDataAndArtifactLevelAsync());
            GoToSettingsCommand = new DelegateCommand(async () => await GoToSettingsAsync());
            GoToViewSettingsCommand = new DelegateCommand(async () => await GoToViewSettingsAsync());
            HelpCommand = new DelegateCommand(async () => await HelpAsync());
            MarkAsDoneCommand = new DelegateCommand<object>(MarkAsDone);
        }
        #endregion

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

        #region Command Methods

        private void MarkAsDone(object o)
        {
            if(o == null)
            {
                return;
            }

            if(!(o is ArtifactToOptimize artifact))
            {
                return;
            }

            artifact.IsTaggedAsDone = !artifact.IsTaggedAsDone;
        }

        /// <summary>
        /// Reloads Artifact Levels Async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadArtifactLevelAsync()
        {
            IsRefreshing = true;

            var result = await Optimization.GetOptimizedList();

            OptimizeList = new ObservableCollection<ArtifactToOptimize>(result.OptimizedList);

            if (result.IsMessageNeeded)
            {
                await _dialogService.DisplayAlertAsync(result.Header, result.Content, AppResources.OKText);
            }

            IsRefreshing = false;
            return true;
        }

        private async Task<bool> ReloadDataAndArtifactLevelAsync() 
        {
            IsRefreshing = true;

            if (!await ImportSfFromClipboardAsync())
            {
                IsRefreshing = false;
                return false;
            }

            return await ReloadArtifactLevelAsync();
        }

        /// <summary>
        /// Navigates to <see cref="ArtOptSettingsPopupPage"/> async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GoToSettingsAsync()
        {
            var result = await _navigationService.NavigateAsync(nameof(ArtOptSettingsPopupPage), new NavigationParameters() { { "id", Optimization.ArtOptSettings.ID } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }

        /// <summary>
        /// Navigates to <see cref="ArtOptVisualSettingsPage"/> async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GoToViewSettingsAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ArtOptImageGridPage, ArtOptVisualSettingsPage>());
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }

        /// <summary>
        /// Navigates to <see cref="InfoPage"/> async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> HelpAsync()
        {
            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ArtOptImageGridPage, InfoPage>());
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }
        #endregion

        #region E + D
        private void Optimization_OnOptiLogMePlease(object sender, Helpers.InformationEventArgs e) => OptimizeLogger.WriteToLogFile($"{sender} - {e.Information}");

        private void Optimization_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => Logger.WriteToLogFile($"{sender} - {e.Information}");

        /// <summary>
        /// When Navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Logger.WriteToLogFile($"OptVM.OnNavigatedTo.");
                Analytics.TrackEvent("Module used", new Dictionary<string, string> { { "Name", "Artifact Optimizer" } });

                //Load Settings
                #region CurrentSettings
                if (!parameters.ContainsKey("id"))
                {
                    Logger.WriteToLogFile($"OptVM.OnNavigatedTo: id was not given. Loading it");

                    Optimization.ArtOptSettings = await App.DBRepo.GetArtOptSettingsByID("1");

                    if (Optimization.ArtOptSettings == null)
                    {
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: CurrentSettings is null. Creating new one");
                        Optimization.ArtOptSettings = new ArtOptSettings()
                        {
                            ID = "1",
                            BoSRoyalty = 50,
                            BosTourneyRoyalty = 50,
                            Build = "_SHIP",
                            HeroBaseTypeInt = 0,
                            HeroDamageInt = 0,
                            IsClickSuggestionEnabled = true,
                            LifeTimeSpentPercentageOnAmount = 5,
                            MaxArtifactAmount = 30,
                            MinEfficiency = 1.05,
                            StepAmountId = 6,
                        };
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: CurrentSettings is now {Optimization.ArtOptSettings.ToString()}");
                    }
                    else
                    {
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: CurrentSettings was in db and is now {Optimization.ArtOptSettings.ToString()}");
                    }
                }
                else
                {
                    //Load from DB
                    Logger.WriteToLogFile($"OptVM.OnNavigatedTo: id was given. Loading it from db");
                    Optimization.ArtOptSettings = await App.DBRepo.GetArtOptSettingsByID(parameters["id"].ToString());

                    if (Optimization.ArtOptSettings == null)
                    {
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: current settings is null. Probably not in DB yet. Storing it");
                        Optimization.ArtOptSettings = new ArtOptSettings();
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: CurrentSettings is now {Optimization.ArtOptSettings.ToString()}");

                        int setSaved = await App.DBRepo.AddArtOptSettingsAsync(Optimization.ArtOptSettings);
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: result of storing: {setSaved}");
                    }
                    else
                    {
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: CurrentSettings was in db and is now {Optimization.ArtOptSettings.ToString()}");
                    }
                }

                Logger.WriteToLogFile($"OptVM.OnNavigatedTo: checking if CurrentSettings values are valid");

                bool settingsOkay = Optimization.ArtOptSettings.CheckIfValuesAreValid();

                if (!settingsOkay)
                {
                    Logger.WriteToLogFile($"OptVM.OnNavigatedTo: Settings are not okay");

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.PleaseCheckSettings, AppResources.OKText);
                }
                #endregion

                else
                {
                    #region CurrentBuild
                    if (string.IsNullOrWhiteSpace(Optimization.ArtOptSettings.Build))
                    {
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: Setting current build to _SHIP because somehow it is currently null or whitespace");

                        Optimization.ArtOptSettings.Build = "_SHIP";
                    }

                    Logger.WriteToLogFile($"OptVM.OnNavigatedTo: settings are okay.\n{Optimization.ArtOptSettings.ToString()}\nGetting currentBuild {Optimization.ArtOptSettings.Build}");
                    //Load default build and select it
                    Optimization.ArtBuild = await App.DBRepo.GetArtifactBuildByName(Optimization.ArtOptSettings.Build);

                    if (Optimization.ArtBuild != null)
                    {
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: currentBuild {Optimization.ArtBuild.Name} was loaded from db");

                        Optimization.ArtBuild.CategoryWeights = await App.DBRepo.GetAllArtifactWeightAsync(Optimization.ArtBuild.Name);
                        Optimization.ArtBuild.ArtsIgnored = await App.DBRepo.GetAllArtifactBuildIgnoAsync(Optimization.ArtBuild.Name);
                    }
                    else
                    {
                        Logger.WriteToLogFile($"OptVM.OnNavigatedTo: Could not load CurrentBuild from Settings. Displaying a dialog to let user know");

                        await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.PleaseCheckSettings, AppResources.OKText);
                    }
                    #endregion

                   OptimizeLogger.WriteToLogFile($"OptVM.OnNavigatedTo: Gold Source is: {Optimization.ArtBuild.GoldSource}");

                    //Calculate
                    await ReloadArtifactLevelAsync();
                }

                LoadVisualSettings();
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile(ex.Message);
            }

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}