using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Assets;
using TT2Master.Model.Reporting;
using TT2Master.Resources;
using System.Reflection;

namespace TT2Master.ViewModels.Reporting
{
    /// <summary>
    /// Settings
    /// </summary>
    public class ReportViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private ObservableCollection<Report> _reports = new ObservableCollection<Report>(TT2Master.Model.Reporting.Reports.AvailableStandardReports);

        public ObservableCollection<Report> Reports { get => _reports; set => SetProperty(ref _reports, value); }


        public ICommand EnterDetailCommand { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public ReportViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            Title = AppResources.Reporting;

            EnterDetailCommand = new DelegateCommand<object>(EnterDetail);
        }
        #endregion

        #region Command Methods
        private void EnterDetail(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var build = obj as Report;

            NavigationService.NavigateAsync(build.Destination);
        }
        #endregion

        #region Private Methods

        #endregion

        #region E + D
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}