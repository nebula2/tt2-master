using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Models;
using Xamarin.Essentials;

namespace TT2Master
{
    /// <summary>
    /// Viewmodel for Info-Stuff
    /// </summary>
    public class ChangelogInfoVM : ViewModelBase
    {
        #region Properties
        private ObservableCollection<ChangelogItem> _changelog = new ObservableCollection<ChangelogItem>();
        public ObservableCollection<ChangelogItem> Changelog { get => _changelog; set => SetProperty(ref _changelog, value); }

        public ICommand TapCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public ChangelogInfoVM(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.Changelog;

            TapCommand = new DelegateCommand<object>(TapExecute);
        }
        #endregion

        #region Command Methods
        private void TapExecute(object url)
        {
            try
            {
                string link = (url as ChangelogItem).Hyperlink;

                if (string.IsNullOrWhiteSpace(link))
                {
                    return;
                }

                Launcher.OpenAsync(new Uri((url as ChangelogItem).Hyperlink));
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Changelog Error: {e.Message}");
            }
        }
        #endregion

        #region private Methods
        private void LoadChangelog()
        {
            try
            {
                var log = ChangelogList.Changelog;

                Changelog = new ObservableCollection<ChangelogItem>(log.OrderByDescending(x => x.Version).ToList());
            }
            catch (Exception e)
            {
                Changelog = new ObservableCollection<ChangelogItem>()
                {
                    new ChangelogItem()
                    {
                        Version = 0,
                        Changes = "?",
                        Hyperlink = "",
                    },
                };
                Logger.WriteToLogFile($"LoadChangelog Exception: {e.Message}");
            }

        }
        #endregion

        #region E + D
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadChangelog();
            base.OnNavigatedFrom(parameters);
        }
        #endregion
    }
}