using Prism.Commands;
using Prism.Navigation;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Models;
using Xamarin.Essentials;

namespace TT2Master
{
    public class ChangelogPopupVM : ViewModelBase
    {
        #region Member
        private ChangelogItem _currentItem = new ChangelogItem();
        public ChangelogItem CurrentItem { get => _currentItem; set => SetProperty(ref _currentItem, value); }

        private bool _isShowingLink = false;
        public bool IsShowingLink { get => _isShowingLink; set => SetProperty(ref _isShowingLink, value); }
        public ICommand CloseCommand { get; private set; }
        public ICommand TapCommand { get; private set; }


        private readonly INavigationService _navigationService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public ChangelogPopupVM(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            Title = AppResources.Changelog;

            CloseCommand = new DelegateCommand(async () =>
            {
                var result = await _navigationService.GoBackAsync();
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            });

            TapCommand = new DelegateCommand<object>(TapExecute);

        }
        #endregion

        private void TapExecute(object url)
        {
            try
            {
                string link = CurrentItem.Hyperlink;

                if (string.IsNullOrWhiteSpace(link))
                {
                    return;
                }

                Launcher.OpenAsync(new Uri(CurrentItem.Hyperlink));
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Changelog Error: {e.Message}");
            }
        }

        #region Override
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.ContainsKey("id"))
            {
                Logger.WriteToLogFile("ChangelogPopup.OnNavigatedTo: No id given. Creating new CurrentSettings");
                CurrentItem = new ChangelogItem();
            }
            else
            {
                Logger.WriteToLogFile("ChangelogPopup.OnNavigatedTo: id given. loading from resources");
                //Load from DB
                CurrentItem = ChangelogList.Changelog.Where(x => x.Version.ToString() == (parameters["id"].ToString())).FirstOrDefault();

                if (CurrentItem == null)
                {

                    Logger.WriteToLogFile("ChangelogPopup.OnNavigatedTo: No item found");
                    CurrentItem = new ChangelogItem();
                }
                else
                {
                    Logger.WriteToLogFile($"EquipAdvSet.OnNavigatedTo: loaded item {CurrentItem.Version}");
                }
            }

            IsShowingLink = !string.IsNullOrWhiteSpace(CurrentItem.Hyperlink);

            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}