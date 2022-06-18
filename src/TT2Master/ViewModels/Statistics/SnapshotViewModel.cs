using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Helper;
using TT2Master.Views.Identity;
using Xamarin.Forms;

namespace TT2Master
{
    public class SnapshotViewModel : ViewModelBase
    {
        #region Properties
        private Snapshot _currentSnapshot = new Snapshot();
        public Snapshot CurrentSnapshot { get => _currentSnapshot; set => SetProperty(ref _currentSnapshot, value); }

        private ObservableCollection<MemberSnapshotItem> _snapshots = new ObservableCollection<MemberSnapshotItem>();
        /// <summary>
        /// List of stored snapshots
        /// </summary>
        public ObservableCollection<MemberSnapshotItem> Snapshots { get => _snapshots; set => SetProperty(ref _snapshots, value); }

        /// <summary>
        /// Command to export <see cref="Snapshots"/>
        /// </summary>
        public ICommand ExportCommand { get; private set; }

        /// <summary>
        /// Command to see snaphot in a report
        /// </summary>
        public ICommand ReportCommand { get; private set; }
        
        /// <summary>
        /// Command to delete <see cref="Snapshots"/>
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Secret command to navigate to login
        /// </summary>
        public ICommand GoToIdentityCommand { get; private set; }
        
        public ICommand UpsertSnapshotsCommand { get; private set; }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        INavigationService _navigationService;


        public static int LastLoadedId { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public SnapshotViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;

            ExportCommand = new DelegateCommand(async () => await ExportExecute());
            ReportCommand = new DelegateCommand(async () => await ReportExecute());
            DeleteCommand = new DelegateCommand(async () => await DeleteExecute());
            GoToIdentityCommand = new DelegateCommand(async () => 
            {
                var result = await _navigationService.NavigateAsync(nameof(IdentityConnectPage));
                Logger.WriteToLogFile($"Navigating to Identity Page: {result?.Success} : {result?.Exception?.Message}");
            });

            UpsertSnapshotsCommand = new DelegateCommand(async () => 
            {
                try
                {
                    Logger.WriteToLogFile("Starting upsert");
                    var api = new TT2WebMaster.WebMasterService(App.DBRepo);

                    var transferredAmount = await api.UpsertPlayerSnapshotsAsync();
                    if (transferredAmount == 0)
                    {
                        Logger.WriteToLogFile($"Error upserting: did not return true");
                        await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                        return;
                    }

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, $"{transferredAmount} items upped", AppResources.OKText);
                }
                catch (Exception ex)
                {
                    Logger.WriteToLogFile($"Error upserting: {ex.Message} - {ex.Data} - {ex.StackTrace}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            });
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Execute for <see cref="ExportCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ExportExecute()
        {
            var result = await _navigationService.NavigateAsync("ClanExportPopupPage", new NavigationParameters() { { "id", CurrentSnapshot.ID } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return true;
        }

        /// <summary>
        /// Execute for <see cref="ReportCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReportExecute()
        {

            string response = await _dialogService.DisplayActionSheetAsync(AppResources.Reporting
                , AppResources.CancelText
                , AppResources.DestroyText
                , Model.Reporting.Reports.AvailableStandardReports.Select(x => x.Name).ToArray());

            if(response == AppResources.CancelText || response == AppResources.DestroyText)
            {
                return true;
            }

            var choice = Model.Reporting.Reports.AvailableStandardReports.Where(x => x.Name == response).FirstOrDefault();

            if(choice == null)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }

            var result = await _navigationService.NavigateAsync(choice.Destination, new NavigationParameters() { { "id", CurrentSnapshot.ID } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return true;
        }

        /// <summary>
        /// Deletes the snapshot from DB and navigates back
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DeleteExecute()
        {
            int detDel = await App.DBRepo.DeleteMemberSnapshotItemBySId(CurrentSnapshot.ID);
            int del = await App.DBRepo.DeleteSnapshotByID(CurrentSnapshot.ID);

            var result = await _navigationService.GoBackAsync();
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Reloads <see cref="Snapshots"/> from DB and sorts the list
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadSnapshotsAsync()
        {
            Snapshots = new ObservableCollection<MemberSnapshotItem>(await App.DBRepo.GetAllMemberSnapshotItemAsync(CurrentSnapshot.ID));
            Snapshots.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId);

            return true;
        }

        #endregion

        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.ContainsKey("id") && LastLoadedId == 0)
            {
                CurrentSnapshot = new Snapshot();
                Snapshots = new ObservableCollection<MemberSnapshotItem>();
            }
            else
            {
                LastLoadedId = parameters.ContainsKey("id") ? JfTypeConverter.ForceInt(parameters["id"].ToString()) : LastLoadedId;

                //Load from DB
                CurrentSnapshot = await App.DBRepo.GetSnapshotByID(LastLoadedId);

                if (CurrentSnapshot == null)
                {
                    CurrentSnapshot = new Snapshot();
                    Snapshots = new ObservableCollection<MemberSnapshotItem>();
                }
            }
            bool snLoaded = await ReloadSnapshotsAsync();

            Title = $"ID {LastLoadedId}";

            base.OnNavigatedTo(parameters);
        }
    }
}