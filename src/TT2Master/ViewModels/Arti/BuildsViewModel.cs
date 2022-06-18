using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Model.Arti;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    public class BuildsViewModel : ViewModelBase
    {
        #region Properties

        private readonly bool _allFuncsAccess = PurchaseableItems.GetAllFuncsAccess();

        private ObservableCollection<ArtifactBuild> _builds = new ObservableCollection<ArtifactBuild>();
        /// <summary>
        /// List of Builds
        /// </summary>
        public ObservableCollection<ArtifactBuild> Builds { get => _builds; set => SetProperty(ref _builds, value); }

        private bool _isImportVisible = true;
        public bool IsImportVisible { get => _isImportVisible; set => SetProperty(ref _isImportVisible, value); }


        /// <summary>
        /// Command to edit the selected Build
        /// </summary>
        public ICommand EditBuildCommand { get; private set; }

        /// <summary>
        /// Command for adding a new build
        /// </summary>
        public ICommand AddBuildCommand { get; private set; }

        public ICommand ImportBuildCommand { get; private set; }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private IPageDialogService _dialogService; 
        #endregion

        #region ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public BuildsViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            IsImportVisible = Device.RuntimePlatform == Device.Android;

            Title = AppResources.BuildsHeader;

            EditBuildCommand = new DelegateCommand<object>(EditBuildExecute);
            AddBuildCommand = new DelegateCommand(async () => await AddBuildExecute());
            ImportBuildCommand = new DelegateCommand(async () => await ImportBuildExecuteAsync());
        }

        #endregion

        #region Command Methods
        /// <summary>
        /// Action for <see cref="EditBuildCommand"/>
        /// </summary>
        private void EditBuildExecute(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var build = obj as ArtifactBuild;

            NavigationService.NavigateAsync("EditBuildPage", new NavigationParameters() { { "build", obj.ToString() } });
        }

        /// <summary>
        /// Action for <see cref="AddBuildCommand"/>
        /// </summary>
        private async Task<bool> AddBuildExecute()
        {
            if (!_allFuncsAccess && Builds.Count >= 10)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, string.Format(AppResources.OnlyForSupporterItemLimitText, 10), AppResources.OKText);
                return false;
            }
            
            await NavigationService.NavigateAsync("EditBuildPage");
            return true;
        }

        /// <summary>
        /// Imports a build (<see cref="ImportBuildCommand"/>)
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ImportBuildExecuteAsync()
        {
            if (!_allFuncsAccess)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.OnlyForSupporterInfoText, AppResources.OKText);
                return false;
            }

            BuildSharer.OnProblemHaving += BuildSharer_OnProblemHaving;

            bool buildImported = await BuildSharer.ImportBuildAsync();

            if (!buildImported)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.BuildNotImportedText, AppResources.OKText);
                BuildSharer.OnProblemHaving -= BuildSharer_OnProblemHaving;
                return false;
            }

            //Alert User
            await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.BuildImportedText, AppResources.OKText);

            //Reload List
            Builds = new ObservableCollection<ArtifactBuild>(await App.DBRepo.GetAllArtifactBuildAsync());

            BuildSharer.OnProblemHaving -= BuildSharer_OnProblemHaving;
            return true;
        } 
        #endregion

        #region Helper
        /// <summary>
        /// Handles BuildSharer errors
        /// </summary>
        /// <param name="e"></param>
        private async void BuildSharer_OnProblemHaving(Exception e) => await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, e.Message, AppResources.OKText);
        #endregion

        #region Override
        /// <summary>
        /// When Navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Builds = new ObservableCollection<ArtifactBuild>(await App.DBRepo.GetAllArtifactBuildAsync());

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}