using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Navigation;
using TT2Master.Resources;

namespace TT2Master
{
    public class ArtifactOverviewViewModel : ViewModelBase
    {
        #region Properties
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
        /// Reloads Artifact Levels
        /// </summary>
        public ICommand ReloadArtifactsCommand { get; private set; }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        /// <summary>
        /// Command to enter the tabbed Artifact
        /// </summary>
        public ICommand EnterDetailCommand { get; private set; }
        #endregion

        #region Ctor
        public ArtifactOverviewViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            Title = AppResources.ArtifactOverviewHeader;

            OptimizeList = new ObservableCollection<ArtifactToOptimize>();

            ReloadArtifactsCommand = new DelegateCommand(async () => await ReloadArtifactsAsync());
            EnterDetailCommand = new DelegateCommand<object>(async (o) => await EnterDetailExecute(o));
        }
        #endregion

        /// <summary>
        /// Action for <see cref="EnterDetailCommand"/>
        /// </summary>
        private async Task<bool> EnterDetailExecute(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            try
            {
                var art = obj as ArtifactToOptimize;

                await NavigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<ArtifactOverviewPage, ArtifactDetailPage>(), new NavigationParameters() { { "art", art.ID } });
            }
            catch (Exception)
            { }

            return true;
        }

        /// <summary>
        /// When ArtifactConstants crashes
        /// </summary>
        /// <param name="e"></param>
        private async void ArtifactConstants_OnProblemHaving(object sender, CustErrorEventArgs e)
        {
            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, $"{e.MyException.Message}", AppResources.OKText);
            Logger.WriteToLogFile($"ARTIFACT ERROR {sender.ToString()} -> {e.MyException.Message} \n{e.MyException.Data}");
        }

        /// <summary>
        /// Reloads Artifacts Async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadArtifactsAsync()
        {
            //Load Save file -> Only Arts
            if (!await App.Save.Initialize(loadPlayer: false, loadAccountModel: false, loadSkills: false, loadClan: false, loadTournament: false, loadEquipment: false))
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotLoadFileText, AppResources.OKText);
                return false;
            }

            //Load Artifacts
            ArtifactHandler.OnProblemHaving += ArtifactConstants_OnProblemHaving;

            if (!ArtifactHandler.LoadArtifacts())
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotFillArtifactsText, AppResources.OKText);
                ArtifactHandler.OnProblemHaving -= ArtifactConstants_OnProblemHaving;
                return false;
            }

            ArtifactHandler.FillArtifacts(App.Save);

            ArtifactHandler.OnProblemHaving -= ArtifactConstants_OnProblemHaving;

            //InitArtifacts();
            CalculateArtifactLTR();

            return true;
        }

        /// <summary>
        /// Action for <see cref="CalculateCommand"/>
        /// </summary>
        /// <returns></returns>
        private void CalculateArtifactLTR()
        {
            //Reload Arts
            InitArtifacts();

            for (int i = 0; i < OptimizeList.Count; i++)
            {
                var tmp = OptimizeList[i];


                #region Set active Ratio (weight) and CurrPercentage
                try
                {
                    double perc = LocalSettingsORM.UseMasterBoSDisplay
                    ? ArtifactHandler.CalculateLifeTimeSpentPercentage(tmp.RelicsSpent)
                    : ArtifactHandler.CalculateLifeTimeSpentPercentageForDummies(tmp.RelicsSpent);

                    tmp.CurrPercentage = Math.Round(perc, 2);
                }
                catch (Exception)
                {
                    tmp.CurrPercentage = 0;
                }
                #endregion

                //Calculate Percentage
                double w = OptimizeList[i].CostToLevel(OptimizeList[i].Amount);

                OptimizeList[i].InPercent = Math.Round(w * 100 / App.Save.CurrentRelics, 2, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Initializes List of Artifacts
        /// </summary>
        private void InitArtifacts()
        {
            OptimizeList = new ObservableCollection<ArtifactToOptimize>();

            for (int i = 0; i < ArtifactHandler.Artifacts.Count; i++)
            {
                OptimizeList.Add(new ArtifactToOptimize(ArtifactHandler.Artifacts[i]));
            }
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            bool done = await ReloadArtifactsAsync();
            Logger.WriteToLogFile($"Navigating to ArtifactOverviewViewModel: {done}");
            base.OnNavigatedTo(parameters);
        }
    }
}

