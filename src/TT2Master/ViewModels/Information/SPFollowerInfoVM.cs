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
    public class SPFollowerInfoVM : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Header
        /// </summary>
        public string HeaderText => AppResources.SPFollowerHeader;

        /// <summary>
        /// Introductional text
        /// </summary>
        public string Introduction => AppResources.SPFollowerIntroduction;

        /// <summary>
        /// First image
        /// </summary>
        public string ImageOne => @"howto_spfollow_1";

        /// <summary>
        /// Description for first image
        /// </summary>
        public string ImageOneDescription => AppResources.SPFollowerImgOneDesc;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public SPFollowerInfoVM(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.HowTo;
        }
        #endregion

        #region Command Methods

        #endregion
    }
}