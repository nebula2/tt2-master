using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Resources;

namespace TT2Master
{
    /// <summary>
    /// Viewmodel for Info-Stuff
    /// </summary>
    public class InfoViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<InfoItem> _infoList;
        /// <summary>
        /// List of informational pages
        /// </summary>
        public ObservableCollection<InfoItem> InfoList { get => _infoList; set => SetProperty(ref _infoList, value); }

        /// <summary>
        /// Navigationservice to navigate to infopages
        /// </summary>
        readonly INavigationService _navigationService;

        /// <summary>
        /// Command to navigate to child info page
        /// </summary>
        public ICommand NavigateCommand { get; private set; }

        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public InfoViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            Title = AppResources.InfoHeader;

            NavigateCommand = new DelegateCommand<object>(async (o) => await NavigateAsync(o));

            InitInfoList();
        }
        #endregion

        /// <summary>
        /// Initializes List of child info pages
        /// </summary>
        private void InitInfoList()
        {
            InfoList = new ObservableCollection<InfoItem>(new List<InfoItem>()
            {
                new InfoItem()
                {
                    Title = AppResources.LinksHeader,
                    Destination = "LinksInfoPage",
                },
                new InfoItem()
                {
                    Title = AppResources.HowToLogHeader,
                    Destination = "HowToLogPage",
                },
                new InfoItem()
                {
                    Title = AppResources.MenuOptimizerTitle,
                    Destination = "OptimizerInfoPage",
                },
                new InfoItem()
                {
                    Title = AppResources.EditBuildHeader,
                    Destination = "EditBuildsInfoPage",
                },
                //new InfoItem()
                //{
                //    Title = "SP " + AppResources.OptimizerHeader,
                //    Destination = "SPFollowerInfoPage",
                //},
                new InfoItem()
                {
                    Title = AppResources.Statistics,
                    Destination = "StatisticsInfoPage",
                },
                new InfoItem()
                {
                    Title = AppResources.Changelog,
                    Destination = "ChangelogInfoPage",
                },
            });

            if (LocalSettingsORM.IsReadingDataFromSavefile)
            {
                InfoList.Add(new InfoItem
                {
                    Title = AppResources.ImportBuildFromChat,
                    Destination = "ImportChatBuildsInfoPage",
                });
            }
        }

        #region Command Methods
        /// <summary>
        /// Execute for <see cref="NavigateCommand"/>
        /// </summary>
        /// <param name="obj"></param>
        private async Task<bool> NavigateAsync(object obj)
        {
            if (obj == null)
            {
                return true;
            }

            var item = obj as InfoItem;

            var result = await _navigationService.NavigateAsync(item.Destination);
            Logger.WriteToLogFile($"Navigation to {item.Destination} Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return result?.Success ?? false;
        }
        #endregion
    }
}