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
    public class EditBuildsInfoVM : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Header
        /// </summary>
        public string HeaderText => AppResources.EditBuildHeader;

        /// <summary>
        /// Introductional text
        /// </summary>
        public string Introduction => AppResources.EditBuildIntroduction;

        /// <summary>
        /// First image
        /// </summary>
        public string ImageOne => @"howto_editBuild_1";

        /// <summary>
        /// Description for first image
        /// </summary>
        public string ImageOneDescription => AppResources.EditBuildImgOneDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageTwo => @"howto_editBuild_2";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageTwoDescription => AppResources.EditBuildImgTwoDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageThree => @"howto_editBuild_3";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageThreeDescription => AppResources.EditBuildImgThreeDesc;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public EditBuildsInfoVM(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.HowTo;
        }
        #endregion

        #region Command Methods

        #endregion
    }
}