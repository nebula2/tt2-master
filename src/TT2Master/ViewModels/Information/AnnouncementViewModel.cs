using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Information;
using TT2Master.Model.Navigation;
using TT2Master.Resources;
using TT2Master.Views.Information;
using Xamarin.Essentials;

namespace TT2Master.ViewModels.Information
{
    /// <summary>
    /// Viewmodel for Info-Stuff
    /// </summary>
    public class AnnouncementViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<DbAnnouncement> _ann;
        public ObservableCollection<DbAnnouncement> Ann { get => _ann; set => SetProperty(ref _ann, value); }

        public ICommand EditCommand { get; private set; }
        public ICommand MarkAsReadCommand { get; private set; }

        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public AnnouncementViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = "Announcements";

            EditCommand = new DelegateCommand<object>(async (o) =>
            {
                if (o == null)
                {
                    return;
                }

                try
                {
                    var param = (o as DbAnnouncement).ID;

                    var result = await _navigationService.NavigateAsync("AnnouncementPopupPage"
                        , new NavigationParameters() { { "id", param } });
                    Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"AnnouncementViewModel Error: {e.Message}");
                }
            });

            MarkAsReadCommand = new DelegateCommand(async () =>
            {
                try
                {
                    var unseen = Ann.Where(x => !x.IsSeen).ToList();

                    foreach (var item in unseen)
                    {
                        item.IsSeen = true;
                        await App.DBRepo.UpsertDbAnnouncementAsync(item);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteToLogFile($"MarkAsReadCommand ERROR: {ex.Message} {ex.Data}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            });
        }
        #endregion

        #region Command Methods

        #endregion

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await AnnouncementHandler.UpdateLocalAnnouncementsAsync();

            Ann = new ObservableCollection<DbAnnouncement>(AnnouncementHandler.Announcements?.OrderByDescending(x => x.ID));

            base.OnNavigatedTo(parameters);
        }
    }
}