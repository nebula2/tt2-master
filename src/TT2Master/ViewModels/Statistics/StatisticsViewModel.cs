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
using TT2Master.Model.Navigation;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    public class StatisticsViewModel : ViewModelBase
    {
        #region Properties
        private bool _isWorking = false;
        /// <summary>
        /// Indicator for a longer working thread.
        /// </summary>
        public bool IsWorking { get => _isWorking; set => SetProperty(ref _isWorking, value); }

        private ObservableCollection<Snapshot> _snapshots = new ObservableCollection<Snapshot>();
        /// <summary>
        /// List of stored snapshots
        /// </summary>
        public ObservableCollection<Snapshot> Snapshots { get => _snapshots; set => SetProperty(ref _snapshots, value); }

        /// <summary>
        /// Command to make a new <see cref="Snapshot"/>
        /// </summary>
        public ICommand SnapshotCommand { get; private set; }

        /// <summary>
        /// Command to enter a Snapshot
        /// </summary>
        public ICommand EnterSnapshotCommand { get; private set; }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        INavigationService _navigationService;

        #endregion

        #region Ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public StatisticsViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;

            Title = AppResources.Statistics;

            SnapshotCommand = new DelegateCommand(async() => await SnapshotExecute());
            EnterSnapshotCommand = new DelegateCommand<object>(EnterSnapshot);
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Execute for <see cref="SnapshotCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SnapshotExecute()
        {
            IsWorking = true;
            bool sn = await CreateSnapshotAsync();

            bool snLoaded = await ReloadSnapshotsAsync();
            IsWorking = false;

            return true;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Creates a snapshot and saves it to the database async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CreateSnapshotAsync()
        {
            SnapshotFactory.OnLogMePlease += SnapshotFactory_OnLogMePlease;
            bool result = await SnapshotFactory.CreateSnapshotAsync(true);
            SnapshotFactory.OnLogMePlease -= SnapshotFactory_OnLogMePlease;
            return result;
        }

        /// <summary>
        /// Reloads <see cref="Snapshots"/> from DB and sorts the list
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadSnapshotsAsync()
        {
            var items = await App.DBRepo.GetAllSnapshotAsync();
            items.OrderByDescending(x => x.ID);

            Snapshots = new ObservableCollection<Snapshot>();
            foreach (var item in items)
            {
                Snapshots.Add(item);
            }

            return true;
        }

        /// <summary>
        /// Deletes old <see cref="Snapshots"/> from DB
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DeleteOldSnapshotsAsync()
        {
            SnapshotFactory.OnLogMePlease += SnapshotFactory_OnLogMePlease;
            bool result = await SnapshotFactory.DeleteOldSnapshotsAsync();
            SnapshotFactory.OnLogMePlease -= SnapshotFactory_OnLogMePlease;
            return result;
        }

        /// <summary>
        /// Enters a snapshot async
        /// </summary>
        /// <param name="obj"></param>
        private async void EnterSnapshot(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var item = obj as Snapshot;

            var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<StatisticsPage, SnapshotPage>(), new NavigationParameters() { { "id", item.ID} });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
        }

        #endregion

        /// <summary>
        /// Handles Log-Requests from <see cref="SnapshotFactory"/>
        /// </summary>
        /// <param name="message"></param>
        private void SnapshotFactory_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"{sender.ToString()}: {e.Information}");

        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            bool snCleaned = await DeleteOldSnapshotsAsync();

            bool snLoaded = await ReloadSnapshotsAsync();

            base.OnNavigatedTo(parameters);
        }
    }
}