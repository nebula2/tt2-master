using Microsoft.AppCenter.Analytics;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.DataSource;
using TT2Master.Model.Equip;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master.ViewModels.Equip
{
    public class CraftingAdvisorViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<Equipment> _allEquipment = new ObservableCollection<Equipment>();
        /// <summary>
        /// All craftable equipment
        /// </summary>
        public ObservableCollection<Equipment> AllEquipment { get => _allEquipment; set => SetProperty(ref _allEquipment, value); }

        private ObservableCollection<Equipment> _filteredEquipment = new ObservableCollection<Equipment>();
        /// <summary>
        /// Filtered Equip
        /// </summary>
        public ObservableCollection<Equipment> FilteredEquipment { get => _filteredEquipment; set => SetProperty(ref _filteredEquipment, value); }

        private ObservableCollection<string> _equipCategories = new ObservableCollection<string>();
        /// <summary>
        /// Equipment Categories like helm or aura
        /// </summary>
        public ObservableCollection<string> EquipCategories { get => _equipCategories; set => SetProperty(ref _equipCategories, value); }

        private ObservableCollection<string> _boostTypes = new ObservableCollection<string>();
        /// <summary>
        /// Primary boost types
        /// </summary>
        public ObservableCollection<string> BoostTypes { get => _boostTypes; set => SetProperty(ref _boostTypes, value); }

        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        private bool _isRefreshing = false;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        private string _categoryChoice = "";
        public string CategoryChoice { get => _categoryChoice; set => SetProperty(ref _categoryChoice, value, async () => await CategoryChoiceChangedAsync()); }

        private string _boostChoice = "";
        public string BoostChoice { get => _boostChoice; set => SetProperty(ref _boostChoice, value, async () => await ReloadListsAsync()); }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public CraftingAdvisorViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.CraftingAdvisor;
        }
        #endregion

        #region private methods
        private async Task<bool> CategoryChoiceChangedAsync()
        {
            BoostTypes = new ObservableCollection<string>(AllEquipment.Where(x => x.EquipmentCategory == CategoryChoice)
                    .Select(x => x.BonusType).Distinct().ToList());
            BoostChoice = BoostTypes.FirstOrDefault();

            await ReloadListsAsync();

            return true;
        }

        private async Task<bool> InitializeAsync()
        {
            try
            {
                // load all craftable equip and choices
                EquipmentHandler.FillEquipment(App.Save);
                bool setsLoaded = EquipmentHandler.LoadSetInformation(App.Save);

                var completedSets = EquipmentHandler.EquipmentSets.Where(x => x.Completed).Select(x => x.Set);

                AllEquipment = new ObservableCollection<Equipment>(EquipmentHandler.ExistingEquipment.Where(x =>
                    //x.EquipmentSource == "Default"
                       x.EquipmentSet != "None"
                    && !x.LimitedTime
                    && completedSets.Contains(x.EquipmentSet)).ToList());

                if (AllEquipment == null || AllEquipment.Count == 0)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.NoCraftableEquipmentFoundText, AppResources.OKText);
                    return false;
                }

                foreach (var item in AllEquipment)
                {
                    item.Level = 10;
                    item.SetPurePrimaryEfficiencyOnly();
                    item.SetVisualProperties();
                }

                EquipCategories = new ObservableCollection<string>(AllEquipment.Select(x => x.EquipmentCategory).Distinct().ToList());
                CategoryChoice = EquipCategories.FirstOrDefault();
                
                BoostTypes = new ObservableCollection<string>(AllEquipment.Where(x => x.EquipmentCategory == CategoryChoice)
                    .Select(x => x.BonusType).Distinct().ToList());
                BoostChoice = BoostTypes.FirstOrDefault();

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"CraftingAdvisor.InitializeAsync ERROR: {ex.Message} - {ex.Data}");
                return false;
            }
        }

        private async Task<bool> ReloadListsAsync()
        {
            if (string.IsNullOrEmpty(BoostChoice) || string.IsNullOrEmpty(CategoryChoice))
            {
                FilteredEquipment = new ObservableCollection<Equipment>(AllEquipment.ToList());
                return true;
            }

            try
            {
                // filter and sort by eff desc
                var filtered = AllEquipment.Where(x =>
                    x.EquipmentCategory == CategoryChoice && x.BonusType == BoostChoice).ToList();

                FilteredEquipment = new ObservableCollection<Equipment>(filtered.OrderByDescending(x => x.PrimaryBonusEff).ToList());

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"CraftingAdvisorViewModel.ReloadListsAsync ERROR: {ex.Message} - {ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }
        }
        #endregion

        #region Navigation overrides
        /// <summary>
        /// When navigating to this, load equipment stuff
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Logger.WriteToLogFile("CraftingAdvisor.OnNavigatedTo");
            Analytics.TrackEvent("Module used", new Dictionary<string, string> { { "Name", "Crafting Advisor" } });

            EquipmentHandler.OnLogMePlease += EquipmentHandler_OnLogMePlease;

            if(!await InitializeAsync())
            {
                await _navigationService.GoBackAsync();
            }
            else
            {
                base.OnNavigatedTo(parameters);
            }
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