using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    public class SPBuildsViewModel : ViewModelBase
    {
        #region Properties
        private readonly bool _allFuncsAccess = PurchaseableItems.GetAllFuncsAccess();

        private List<SPBuild> _builds;
        /// <summary>
        /// List of Builds
        /// </summary>
        public List<SPBuild> Builds { get => _builds; set => SetProperty(ref _builds, value); }

        /// <summary>
        /// Command to edit the selected Build
        /// </summary>
        public ICommand OpenOptionsCommand { get; private set; }

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

        #region Ctor
        public SPBuildsViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            Title = "SP Builds";

            Builds = new List<SPBuild>();
            OpenOptionsCommand = new DelegateCommand<object>(OpenOptionsCommandExecute);
            AddBuildCommand = new DelegateCommand(async () => await AddBuildExecute());
            ImportBuildCommand = new DelegateCommand(async () => await ImportBuildExecuteAsync());
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Action for <see cref="EditBuildCommand"/>
        /// </summary>
        private async void OpenOptionsCommandExecute(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var build = obj as SPBuild;

            bool response = await _dialogService.DisplayAlertAsync("Export Build", AppResources.ExportQuestionText, AppResources.YesText, AppResources.NoText);

            if (!response)
            {
                return;
            }

            try
            {
                string filename = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPathName(build.ID + ".csv");

                //get milestones
                build.Milestones = await App.DBRepo.GetAllSPBuildMilestoneAsync(build.ID);

                //get milestoneitems
                foreach (var ms in build.Milestones)
                {
                    ms.MilestoneItems = await App.DBRepo.GetAllSPBuildMilestoneItemAsync(ms.Build, ms.Milestone);
                }

                SPBuildSharer.OnProblemHaving += BuildSharer_OnProblemHaving;
                SPBuildSharer.ExportBuild(build, filename);
                SPBuildSharer.OnProblemHaving -= BuildSharer_OnProblemHaving;

                await _dialogService.DisplayAlertAsync(AppResources.FinishedText, AppResources.ExportedToText + $"{filename}", AppResources.OKText);
            }
            catch (Exception ex)
            {
                SPBuildSharer.OnProblemHaving -= BuildSharer_OnProblemHaving;
                await _dialogService.DisplayAlertAsync("Error on export", ex.Message, "ok");
            }
        }

        /// <summary>
        /// Action for <see cref="AddBuildCommand"/>
        /// </summary>
        private async Task<bool> AddBuildExecute()
        {
            if (_allFuncsAccess)
            {
                await NavigationService.NavigateAsync("EditSPBuildPage");
                return true;
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.OnlyForSupporterInfoText, AppResources.OKText);
                return false;
            }
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

            return true;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Handles BuildSharer errors
        /// </summary>
        /// <param name="e"></param>
        private async void BuildSharer_OnProblemHaving(object sender, CustErrorEventArgs e)
        {
            Logger.WriteToLogFile($"{sender.ToString()} ERROR: {e.MyException.Message}\n{e.MyException.Data}");
            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, e.MyException.Message, AppResources.OKText);
        }
        #endregion

        #region Override
        /// <summary>
        /// When Navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Builds = await App.DBRepo.GetAllSPBuildAsync();

            //Check if there is a current build
            if(Builds.Count == 0)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotFindText, AppResources.OKText);
            }
        }
        #endregion
    }
}

