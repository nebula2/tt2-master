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
    public class ImportChatBuildsInfoVM : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Header
        /// </summary>
        public string HeaderText => AppResources.ImportBuildFromChat;

        /// <summary>
        /// Introductional text
        /// </summary>
        public string Introduction => AppResources.ImportBuildFromChatDesc;

        /// <summary>
        /// First image
        /// </summary>
        public string ImageOne => @"howtoBuildImport_1";

        /// <summary>
        /// Description for first image
        /// </summary>
        public string ImageOneDescription => AppResources.ImportBuildImgOneDesc;

        /// <summary>
        /// Second image
        /// </summary>
        public string ImageTwo => @"howtoBuildImport_2";

        /// <summary>
        /// Description for second image
        /// </summary>
        public string ImageTwoDescription => AppResources.ImportBuildImgTwoDesc;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public ImportChatBuildsInfoVM(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.HowTo;
        }
        #endregion

        #region Command Methods
        
        #endregion
    }
}