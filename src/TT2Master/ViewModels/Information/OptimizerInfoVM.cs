using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Viewmodel for Info-Stuff
    /// </summary>
    public class OptimizerInfoVM : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Header
        /// </summary>
        public string HeaderText => AppResources.MenuOptimizerTitle;

        /// <summary>
        /// Introductional text
        /// </summary>
        public string Introduction => AppResources.OptimizerIntroduction;

        /// <summary>
        /// First image
        /// </summary>
        public string ImageOne => @"howto_optimizer_1";

        /// <summary>
        /// Description for first image
        /// </summary>
        public string ImageOneDescription => AppResources.OptimizerImgOneDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageTwo => @"howto_optimizer_2";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageTwoDescription => AppResources.OptimizerImgTwoDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageThree => @"howto_optimizer_3";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageThreeDescription => AppResources.OptimizerImgThreeDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageFour => @"howto_optimizer_4";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageFourDescription => AppResources.OptimizerImgFourDesc;
        #endregion

        #region Ctor
        public OptimizerInfoVM(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.HowTo;
        }
        #endregion
    }
}