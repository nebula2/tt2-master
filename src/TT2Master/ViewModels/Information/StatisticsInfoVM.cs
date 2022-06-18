using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Viewmodel for Info-Stuff
    /// </summary>
    public class StatisticsInfoVM : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Header
        /// </summary>
        public string HeaderText => AppResources.Statistics;

        /// <summary>
        /// Introductional text
        /// </summary>
        public string Introduction => AppResources.StatisticsIntroduction;

        /// <summary>
        /// First image
        /// </summary>
        public string ImageOne => @"howto_statistics_1";

        /// <summary>
        /// Description for first image
        /// </summary>
        public string ImageOneDescription => AppResources.StatisticsImgOneDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageTwo => @"howto_statistics_2";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageTwoDescription => AppResources.StatisticsImgTwoDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageThree => @"howto_statistics_3";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageThreeDescription => AppResources.StatisticsImgThreeDesc;
        #endregion

        #region Ctor
        public StatisticsInfoVM(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.HowTo;

        }
        #endregion

        #region Command Methods

        #endregion
    }
}