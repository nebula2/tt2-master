using Prism.Commands;
using Prism.Navigation;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TT2Master.Interfaces;
using TT2Master.Loggers;
using TT2Master.Model.Information;
using TT2Master.Resources;
using TT2Master.Shared.Helper;
using Xamarin.Essentials;

namespace TT2Master.ViewModels.Information
{
    public class AnnouncementPopupVM : ViewModelBase
    {
        #region Member
        private DbAnnouncement _currentItem = new DbAnnouncement();
        public DbAnnouncement CurrentItem { get => _currentItem; set => SetProperty(ref _currentItem, value); }

        public ICommand CloseCommand { get; private set; }

        private readonly INavigationService _navigationService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public AnnouncementPopupVM(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            CloseCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.GoBackAsync();
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });
        }
        #endregion

        #region Override
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (!parameters.ContainsKey("id"))
                {
                    Logger.WriteToLogFile("OnNavigatedTo: No id given. Loading latest Update required announcement");
                    CurrentItem = AnnouncementHandler.Announcements.Where(x => x.IsUpdateRequired)?.OrderByDescending(x => x.ID).FirstOrDefault();
                }
                else
                {
                    Logger.WriteToLogFile("OnNavigatedTo: id given. loading from resources");

                    var idToLoad = JfTypeConverter.ForceInt(parameters["id"].ToString());

                    CurrentItem = AnnouncementHandler.Announcements.Where(x => x.ID == idToLoad).FirstOrDefault();

                    await App.DBRepo.UpdateAnnouncementAsSeenByIdAsync(CurrentItem.ID);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"OnNavigatedTo: ERROR {ex.Message} {ex.Data}");
            }

            if(CurrentItem == null)
            {
                CurrentItem = new DbAnnouncement { ID = -1, Header = "Not found", Body = "not found", IsUpdateRequired = false };
            }

            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));

            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            if (AnnouncementHandler.IsUpdateRequired(CurrentItem))
            {
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().CloseApplication();
            }

            base.OnNavigatedFrom(parameters);
        }
        #endregion
    }
}