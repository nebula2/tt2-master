using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Model.Arti;
using TT2Master.Resources;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master
{
    public class EditBuildViewModel : ViewModelBase
    {
        #region Properties
        private readonly bool _allFuncsAccess = PurchaseableItems.GetAllFuncsAccess();

        private ArtifactBuild _build;
        public ArtifactBuild Build { get => _build; set => SetProperty(ref _build, value); }

        private List<string> _goldSources = Enum.GetNames(typeof(GoldType)).Select(x => x.TranslatedString()).ToList<string>();
        public List<string> GoldSources { get => _goldSources; set => SetProperty(ref _goldSources, value); }

        private bool _isNewBuild = true;
        public bool IsNewBuild { get => _isNewBuild; set => SetProperty(ref _isNewBuild, value); }

        private bool _changesMade = false;
        public bool ChangesMade { get => _changesMade; set => SetProperty(ref _changesMade, value); }

        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private bool _isExportVisible = true;
        public bool IsExportVisible { get => _isExportVisible; set => SetProperty(ref _isExportVisible, value); }
        #endregion

        #region ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public EditBuildViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.EditBuildHeader;

            IsExportVisible = Device.RuntimePlatform == Device.Android;

            Build = new ArtifactBuild();
            Build.PropertyChanged += Build_PropertyChanged;

            _dialogService = dialogService;

            DeleteCommand = new DelegateCommand(DeleteExecute);
            SaveCommand = new DelegateCommand(async () => await SaveExecuteAsync());
            CopyCommand = new DelegateCommand(async () => await CopyExecuteAsync());
            ExportCommand = new DelegateCommand(async () => await ExportExecuteAsync());
        }
        #endregion

        #region CommandMethods
        /// <summary>
        /// Action for <see cref="DeleteCommand"/>
        /// </summary>
        private async void DeleteExecute()
        {
            //Don't delete if not allowed or new
            if (!Build.Editable || IsNewBuild)
            {
                return;
            }

            bool answer = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SureToDeleteText, AppResources.YesText, AppResources.NoText);

            if (answer)
            {
                //Delete weights
                int deletedWeightsCount = 0;
                foreach (var item in Build.CategoryWeights)
                {
                    deletedWeightsCount += await App.DBRepo.DeleteArtifactWeightByName(item.BuildAndArtifact);
                }

                //Delete Ignos
                int deletedIgnos = 0;
                foreach (var item in Build.ArtsIgnored)
                {
                    if(item.IsIgnored)
                    {
                        deletedIgnos += await App.DBRepo.DeleteArtifactBuildIgnoByNameAsync(Build.Name);
                    }
                }

                //Delete Build
                _ = await App.DBRepo.DeleteArtifactBuildByName(Build.Name);

                //Display result
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.DeletedText, AppResources.OKText);

                //Leave edit
                await NavigationService.GoBackAsync();
            }
        }

        /// <summary>
        /// Predicate for <see cref="DeleteCommand"/>
        /// </summary>
        /// <returns></returns>
        private bool DeleteCanExecute() => Build.Editable && !IsNewBuild;

        /// <summary>
        /// Action for <see cref="SaveCommand"/>
        /// </summary>
        private async Task<bool> SaveExecuteAsync()
        {
            #region Checks
            //Check if Name is filled
            if (string.IsNullOrWhiteSpace(Build.Name))
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.FillNameFirstText, AppResources.OKText);
                return false;
            }

            //Builds with _ as first letter are reserved for default builds.
            if (Build.Name[0] == '_' && IsNewBuild)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.BuildFirstLetterRestrictionText, AppResources.OKText);
                return false;
            }

            bool saveAccepted = true; //is this save accepted?

            //Check if build exists
            var maybeBuild = await App.DBRepo.GetArtifactBuildByName(Build.Name);
            if (maybeBuild != null)
            {
                if (maybeBuild.Name == Build.Name)
                {
                    saveAccepted = await _dialogService.DisplayAlertAsync(AppResources.OKText, AppResources.BuildExistsText, AppResources.YesText, AppResources.NoText);

                    //if user does not want to overwrite -> do not save
                    if (!saveAccepted)
                    {
                        return false;
                    }
                }
            }
            #endregion

            #region Save
            //save build
            int buildSaved = await App.DBRepo.UpdateArtifactBuildAsync(Build);

            //Save weights
            int weightsSaved = 0;
            foreach (var item in Build.CategoryWeights)
            {
                weightsSaved += await App.DBRepo.UpdateArtifactWeightAsync(item);
            }

            //Save ignos
            int ignosSaved = 0;
            await App.DBRepo.DeleteArtifactBuildIgnoByBuildAsync(Build.Name);

            foreach (var igno in Build.ArtsIgnored)
            {
                //Only store ignored stuff
                if (igno.IsIgnored)
                {
                    ignosSaved += await App.DBRepo.UpdateArtifactBuildIgnoAsync(igno);
                }
            }

            //this is no more a new build and there are no changes anymore
            IsNewBuild = false;
            ChangesMade = false;

            await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ChangesSavedText, AppResources.OKText);

            return true; 
            #endregion
        }

        /// <summary>
        /// Action for <see cref="CopyCommand"/>
        /// </summary>
        private async Task<bool> CopyExecuteAsync()
        {
            var builds = await App.DBRepo.GetAllArtifactBuildAsync();

            if (!_allFuncsAccess && builds.Count >= 10)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, string.Format(AppResources.OnlyForSupporterItemLimitText, 10), AppResources.OKText);
                return false;
            }

            if (ChangesMade)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SaveFirstText, AppResources.OKText);
                return false;
            }

            Build.Name = "";
            Build.Editable = true;
            ChangesMade = true;

            bool fuckDis = await FillRemainingWeights();
            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));


            return true;
        }

        /// <summary>
        /// Predicate for <see cref="SaveCommand"/>
        /// </summary>
        /// <returns></returns>
        private bool SaveCanExecute() => Build.Editable;

        /// <summary>
        /// Action for <see cref="ExportCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ExportExecuteAsync()
        {
            if (!_allFuncsAccess)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.OnlyForSupporterInfoText, AppResources.OKText);
                return false;
            }

            if (ChangesMade)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SaveFirstText, AppResources.OKText);
                return false;
            }

            BuildSharer.OnProblemHaving += BuildSharer_OnProblemHaving;

            string filename = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPathName(Build.Name + ".build");

            if (!BuildSharer.ExportBuild(Build, filename))
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.FileNotExportedText, AppResources.OKText);
                BuildSharer.OnProblemHaving -= BuildSharer_OnProblemHaving;
                return false;
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.FileSavedToText + $"{filename}", AppResources.OKText);
            }

            BuildSharer.OnProblemHaving -= BuildSharer_OnProblemHaving;
            return true;
        }

        #endregion

        #region Helper
        /// <summary>
        /// Pass Property Changed Events from build object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Build_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangesMade = true;
            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// Process BuildSharer-Error
        /// </summary>
        /// <param name="e"></param>
        private async void BuildSharer_OnProblemHaving(Exception e) => await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, e.Message, AppResources.OKText);

        /// <summary>
        /// Sets the Categoryweights for the current Build
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetCategoryWeightsAsync()
        {
            try
            {
                //stored weights
                var cats = await App.DBRepo.GetAllArtifactWeightAsync(Build.Name);

                Build.CategoryWeights = cats;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the Artifacts to ignore for current build
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetArtifactsIgnoredAsync()
        {
            try
            {
                Build.ArtsIgnored = await App.DBRepo.GetAllArtifactBuildIgnoAsync(Build.Name);
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
                return true;
            }
            catch (Exception)
            {
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
                Build.PropertyChanged += Build_PropertyChanged;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Fills weights
        /// </summary>
        private async Task<bool> FillRemainingWeights()
        {
            var weightList = Build.CategoryWeights;

            int fuckCounter = 0;

            //Default Categories
            foreach (string item in ArtifactHandler.Artifacts.Select(x => x.ID).Distinct())
            {
                if (weightList.Where(x => x.ArtifactId == item).Count() == 0)
                {
                    weightList.Add(new ArtifactWeight(Build.Name, item));
                    fuckCounter++;
                }
            }

            Build.CategoryWeights = weightList;
            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));


            if(fuckCounter > 0)
            {
                await _dialogService.DisplayAlertAsync(AppResources.AttentionHeader, AppResources.SomeWeightsNotLoadedText, AppResources.OKText);
            }

            return true;
        }
        #endregion

        private void FillArtifactsBehindWeights()
        {
            if(ArtifactHandler.Artifacts == null)
            {
                ArtifactHandler.LoadArtifacts();
                ArtifactHandler.FillArtifacts(App.Save);
            }
            else if (ArtifactHandler.Artifacts.Count == 0)
            {
                ArtifactHandler.LoadArtifacts();
                ArtifactHandler.FillArtifacts(App.Save);
            }

            foreach (var item in Build.CategoryWeights)
            {
                item.ArtifactBehind = ArtifactHandler.Artifacts.Where(x => x.ID == item.ArtifactId).FirstOrDefault();
            }

            // sort items
            Build.CategoryWeights = Build.CategoryWeights.OrderBy(x => x.ArtifactBehind.SortIndex).ToList();
        }

        #region Override
        /// <summary>
        /// When navigating to this - load some stuff
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!string.IsNullOrEmpty((string)parameters["build"]))
            {
                IsNewBuild = false;

                bool buildDone = await SetBuildAsync((string)parameters["build"]);
                bool weightsDone = await SetCategoryWeightsAsync();
                bool ignosDone = await SetArtifactsIgnoredAsync();
            }

            bool filled = await FillRemainingWeights();

            FillArtifactsBehindWeights();

            Build = Build;
            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));

            ChangesMade = IsNewBuild;

            base.OnNavigatedTo(parameters);
        }

        /// <summary>
        /// Navigating from this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedFrom(INavigationParameters parameters)
        {
            if (ChangesMade)
            {
                bool answer = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.WantToSaveBuildText, AppResources.YesText, AppResources.NoText);

                if (answer)
                {
                    bool saved = await SaveExecuteAsync();
                }
            }

            base.OnNavigatedFrom(parameters);
        } 
        #endregion
    }
}