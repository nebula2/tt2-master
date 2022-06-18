using Microsoft.AppCenter.Analytics;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.DataSource;
using TT2Master.Model.Equip;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master
{
    public class EquipAdvisorViewModel : ViewModelBase
    {
        #region Properties
        private List<Equipment> _myEquipment = new List<Equipment>();
        /// <summary>
        /// All Equipment I own
        /// </summary>
        public List<Equipment> MyEquipment { get => _myEquipment; set => SetProperty(ref _myEquipment, value); }

        private List<Equipment> _mySwords = new List<Equipment>();
        /// <summary>
        /// Sword-Equip
        /// </summary>
        public List<Equipment> MySwords { get => _mySwords; set => SetProperty(ref _mySwords, value); }

        private List<Equipment> _myChests = new List<Equipment>();
        /// <summary>
        /// Chest-Equip
        /// </summary>
        public List<Equipment> MyChests { get => _myChests; set => SetProperty(ref _myChests, value); }

        private List<Equipment> _myHats = new List<Equipment>();
        /// <summary>
        /// Hat-Equip
        /// </summary>
        public List<Equipment> MyHats { get => _myHats; set => SetProperty(ref _myHats, value); }

        private List<Equipment> _myAuras = new List<Equipment>();
        /// <summary>
        /// Aura-Equip
        /// </summary>
        public List<Equipment> MyAuras { get => _myAuras; set => SetProperty(ref _myAuras, value); }

        private List<Equipment> _mySlashs = new List<Equipment>();
        /// <summary>
        /// Aura-Equip
        /// </summary>
        public List<Equipment> MySlashs { get => _mySlashs; set => SetProperty(ref _mySlashs, value); }

        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        private EquipOptimizer _equipOptimizer;

        /// <summary>
        /// Command to enter the tabbed Artifact
        /// </summary>
        public ICommand EnterDetailCommand { get; private set; }

        public ICommand GoToSettingsCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        private bool _isRefreshing = false;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public EquipAdvisorViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _equipOptimizer = new EquipOptimizer(App.DBRepo, App.Save);


            Title = AppResources.EquipAdvisor;

            EnterDetailCommand = new DelegateCommand<object>(EnterDetailExecute);
            GoToSettingsCommand = new DelegateCommand(async () => await GoToSettingsAsync());
            RefreshCommand = new DelegateCommand(async () => await RefreshExecute());
        }
        #endregion

        #region Command Methods

        private async Task<bool> RefreshExecute()
        {
            IsRefreshing = true;
            try
            {
                if (!await ImportSfFromClipboardAsync())
                {
                    IsRefreshing = false;
                    return false;
                }

                await _equipOptimizer.ReloadList();
                MyEquipment = _equipOptimizer.MyEquipment;
                MySwords = _equipOptimizer.MySwords;
                MyHats = _equipOptimizer.MyHats;
                MyChests = _equipOptimizer.MyChests;
                MyAuras = _equipOptimizer.MyAuras;
                MySlashs = _equipOptimizer.MySlashs;

                IsRefreshing = false;

                return true;
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                return false;
            }
        }

        /// <summary>
        /// Navigates to <see cref="ArtOptSettingsPopupPage"/> async
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GoToSettingsAsync()
        {
            var result = await _navigationService.NavigateAsync("EquipAdvSetPopupPage", new NavigationParameters() { { "id", _equipOptimizer.CurrentSettings.ID } });
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

        private async Task<bool> ImportSfFromClipboardAsync()
        {
            if (LocalSettingsORM.IsReadingDataFromSavefile)
            {
                return true;
            }

            try
            {
                var importer = new ClipboardSfImporter(_dialogService);

                var result = await importer.ImportSfFromClipboardAsync(false);

                return result.IsSuccessful;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ImportSfFromClipboardAsync ERROR: {ex.Message} - {ex.Data}");
                return false;
            }
        }

        #region Navigation overrides
        /// <summary>
        /// When navigating to this, load equipment stuff
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Logger.WriteToLogFile("EquipAdvisorViewModel.OnNavigatedTo");
            Analytics.TrackEvent("Module used", new Dictionary<string, string> { { "Name", "Equip Advisor" } });

            await RefreshExecute();

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