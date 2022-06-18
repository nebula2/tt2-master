using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Windows.Input;
using TT2Master.Resources;
using Xamarin.Essentials;

namespace TT2Master
{
    /// <summary>
    /// Viewmodel for Info-Stuff
    /// </summary>
    public class WidgetInfoVM : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Path to widget reddit page
        /// </summary>
        private readonly string _widgetRedditPage = @"https://www.reddit.com/r/TapTitans2/comments/98keu6/tt2master_tournament_widget/";

        /// <summary>
        /// Command for opening the widget reddit page
        /// </summary>
        public ICommand OpenWidgetRedditPageCommand { get; private set; }

        /// <summary>
        /// Widget description
        /// </summary>
        public string WidgetDescription => AppResources.WidgetDescriptionText;
        /// <summary>
        /// Path to widget picture
        /// </summary>
        public string WidgetImagePath => $"widget_pic";

        /// <summary>
        /// Widget description
        /// </summary>
        public string WidgetOutsideDescription => AppResources.WidgetOutsideDescription;
        /// <summary>
        /// Path to widget picture
        /// </summary>
        public string WidgetOutsideImagePath => $"widget_outtaTourney";
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public WidgetInfoVM(INavigationService navigationService) : base(navigationService)
        {
            Title = "Widget";
            OpenWidgetRedditPageCommand = new DelegateCommand(async () => await Launcher.OpenAsync(new Uri(_widgetRedditPage)));
        }
        #endregion

        #region Command Methods

        #endregion
    }
}