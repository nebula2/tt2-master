using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Resources;
using Xamarin.Essentials;

namespace TT2Master
{
    /// <summary>
    /// Viewmodel for Info-Stuff
    /// </summary>
    public class LinksInfoVM : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Link to usage video in german
        /// </summary>
        private readonly string _usageVideoGER = @"https://www.youtube.com/watch?v=fueTZEvBQrc&feature=youtu.be";

        /// <summary>
        /// Link to usage video in english
        /// </summary>
        private readonly string _usageVideoENG = @"https://www.youtube.com/watch?v=lTN-h8BePc4&feature=youtu.be";

        /// <summary>
        /// Playlist with all videos
        /// </summary>
        private readonly string _tt2MasterPlaylist = @"https://www.youtube.com/watch?v=lTN-h8BePc4&list=PLTL_wnKrwtMwwJmUWRO6IPJvim5Qsy9tU";

        /// <summary>
        /// Link to tt2master reddit page
        /// </summary>
        private readonly string _tT2MasterRedditPage = @"https://www.reddit.com/r/TapTitans2/comments/8x1izv/tt2master_artifact_optimizer_for_android/";

        private readonly string _soulrideChannel = @"https://www.youtube.com/channel/UCX1eJQPgKVau2gXkARvkTBA";

        /// <summary>
        /// Command to open usage video
        /// </summary>
        public ICommand UsageVideoCommand { get; private set; }

        /// <summary>
        /// Command to open TT2Master reddit page
        /// </summary>
        public ICommand OpenTT2MasterRedditPageCommand { get; private set; }

        public ICommand OpenPlaylistCommand { get; private set; }
        public ICommand OpenSoulriseCommand { get; private set; }

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public LinksInfoVM(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            Title = AppResources.LinksHeader;

            UsageVideoCommand = new DelegateCommand(async () => await UsageVideoExecute());
            OpenTT2MasterRedditPageCommand = new DelegateCommand(async () => await Launcher.OpenAsync(new Uri(_tT2MasterRedditPage)));
            OpenPlaylistCommand = new DelegateCommand(async () => await Launcher.OpenAsync(new Uri(_tt2MasterPlaylist)));
            OpenSoulriseCommand = new DelegateCommand(async () => await Launcher.OpenAsync(new Uri(_soulrideChannel)));
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Video for usage
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UsageVideoExecute()
        {
            string response = await _dialogService.DisplayActionSheetAsync(AppResources.OpenVideoText
                , AppResources.CancelText
                , AppResources.DestroyText
                , new string[] { "English", "German" });

            switch (response)
            {
                case ("English"):
                    await Launcher.OpenAsync(new Uri(_usageVideoENG));
                    break;
                case ("German"):
                    await Launcher.OpenAsync(new Uri(_usageVideoGER));
                    break;
                default:
                    break;
            }

            return true;
        }

        #endregion
    }
}