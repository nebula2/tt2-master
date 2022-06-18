using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master
{
    public class EquipSetViewModel : ViewModelBase
    {
        #region Properties
        private List<EquipmentSet> _mySets = new List<EquipmentSet>();
        /// <summary>
        /// All Equipment I own
        /// </summary>
        public List<EquipmentSet> MySets { get => _mySets; set => SetProperty(ref _mySets, value); }

        private List<EquipmentSet> _myMyths = new List<EquipmentSet>();
        /// <summary>
        /// Sword-Equip
        /// </summary>
        public List<EquipmentSet> MyMyths { get => _myMyths; set => SetProperty(ref _myMyths, value); }

        private List<EquipmentSet> _myLegi = new List<EquipmentSet>();
        /// <summary>
        /// Chest-Equip
        /// </summary>
        public List<EquipmentSet> MyLegi { get => _myLegi; set => SetProperty(ref _myLegi, value); }

        private List<EquipmentSet> _myRare = new List<EquipmentSet>();
        /// <summary>
        /// Hat-Equip
        /// </summary>
        public List<EquipmentSet> MyRare { get => _myRare; set => SetProperty(ref _myRare, value); }

        private List<EquipmentSet> _myEvent = new List<EquipmentSet>();
        /// <summary>
        /// Aura-Equip
        /// </summary>
        public List<EquipmentSet> MyEvent { get => _myEvent; set => SetProperty(ref _myEvent, value); }

        private INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        private EquipAdvSettings _advSettings = new EquipAdvSettings();
        public EquipAdvSettings AdvSettings { get => _advSettings; set => SetProperty(ref _advSettings, value); }

        private EquipAdvSettings _currentSettings = new EquipAdvSettings();
        public EquipAdvSettings CurrentSettings { get => _currentSettings; set => SetProperty(ref _currentSettings, value); }

        /// <summary>
        /// Command to enter the tabbed Artifact
        /// </summary>
        public ICommand EnterDetailCommand { get; private set; }

        public ICommand GoToSettingsCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public EquipSetViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.EquipAdvisor;

            EnterDetailCommand = new DelegateCommand<object>(EnterDetailExecute);
            GoToSettingsCommand = new DelegateCommand(async () => await GoToSettingsAsync());
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Navigates to <see cref="ArtOptSettingsPopupPage"/> async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GoToSettingsAsync()
        {
            var result = await _navigationService.NavigateAsync("EquipAdvSetPopupPage", new NavigationParameters() { { "id", AdvSettings.ID } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }

        /// <summary>
        /// Action for <see cref="EnterDetailCommand"/>
        /// </summary>
        private void EnterDetailExecute(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var art = obj as Equipment;

            NavigationService.NavigateAsync("EquipDetailPopupPage", new NavigationParameters() { { "item", art.UniqueId.ToString() } });
        }
        #endregion

        #region private methods
        /// <summary>
        /// Populates lists in each equipment category ordered by efficiency
        /// </summary>
        private void BuildLists()
        {
            MyMyths = MySets.Where(x => x.SetType == "Mythic").ToList();
            MyLegi  = MySets.Where(x => x.SetType == "Legendary").ToList();
            MyRare  = MySets.Where(x => x.SetType == "Rare").ToList();
            MyEvent = MySets.Where(x => x.SetType == "Event").ToList();
        }
        #endregion

        #region Navigation overrides
        /// <summary>
        /// When navigating to this, load equipment stuff
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            EquipmentHandler.OnLogMePlease += EquipmentHandler_OnLogMePlease;
            EquipmentHandler.FillEquipment(App.Save);
            bool setsLoaded = EquipmentHandler.LoadSetInformation(App.Save);

            MySets = EquipmentHandler.EquipmentSets;

            BuildLists();

            base.OnNavigatedTo(parameters);
        }

        /// <summary>
        /// When navigating from this, unload Logger
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            EquipmentHandler.OnLogMePlease -= EquipmentHandler_OnLogMePlease;

            base.OnNavigatedFrom(parameters);
        }
        #endregion

        #region E + D
        private void EquipmentHandler_OnLogMePlease(string message) => Logger.WriteToLogFile(message);

        #endregion
    }
}