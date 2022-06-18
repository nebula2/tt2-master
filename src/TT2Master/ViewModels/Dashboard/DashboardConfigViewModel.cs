using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Dashboard;
using TT2Master.Resources;

namespace TT2Master.ViewModels.Dashboard
{
    public class DashboardConfigViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<DashboardShortcutConfig> _shortcutConfig = new ObservableCollection<DashboardShortcutConfig>();
        /// <summary>
        /// Shortcut config
        /// </summary>
        public ObservableCollection<DashboardShortcutConfig> ShortcutConfig { get => _shortcutConfig; set => SetProperty(ref _shortcutConfig, value); }

        private ObservableCollection<AvailableShortcut> _availableShortcuts = new ObservableCollection<AvailableShortcut>();
        /// <summary>
        /// available shortcuts
        /// </summary>
        public ObservableCollection<AvailableShortcut> AvailableShortcuts { get => _availableShortcuts; set => SetProperty(ref _availableShortcuts, value); }

        private ObservableCollection<string> _shortcutLookups;
        public ObservableCollection<string> ShortcutLookups { get => _shortcutLookups; set => SetProperty(ref _shortcutLookups, value); }

        public ICommand SaveCommand { get; private set; }
        public ICommand RestoreDefaultCommand { get; private set; }
        public ICommand SortUpCommand { get; private set; }
        public ICommand SortDownCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand AddCommand { get; private set; }

        private readonly IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;

        #endregion

        #region Ctor
        public DashboardConfigViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.SettingsHeader;

            _dialogService = dialogService;
            _navigationService = navigationService;

            SaveCommand = new DelegateCommand(async () =>
            {
                // clean up list

                try
                {
                    foreach (var item in ShortcutConfig)
                    {
                        if (string.IsNullOrWhiteSpace(item.Name))
                        {
                            ShortcutConfig.Remove(item);
                        }
                    }

                    for (int i = 0; i < ShortcutConfig.Count; i++)
                    {
                        ShortcutConfig[i].SortId = i;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteToLogFile($"ERROR SaveCommand {ex.Message}\n{ex.Data}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }

                // save 
                if (!DashboardShortcutHandler.Saveconfig(ShortcutConfig.ToList()))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
                else
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ChangesSavedText, AppResources.OKText);
                }
            });

            SortUpCommand = new DelegateCommand<object>(SortUp);
            SortDownCommand = new DelegateCommand<object>(SortDown);
            RemoveCommand = new DelegateCommand<object>(async (o) => 
            {
                if (o == null) return;

                try
                {
                    ShortcutConfig.Remove(o as DashboardShortcutConfig);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.WriteToLogFile($"ERROR: RemoveAsync: {ex.Message}\n{ex.Data}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                    return;
                }
            });

            AddCommand = new DelegateCommand(() => 
            {
                ShortcutConfig.Add(new DashboardShortcutConfig(ShortcutConfig.Count, 0, null));
            });

            RestoreDefaultCommand = new DelegateCommand(() =>
            {
                DashboardShortcutHandler.RestoreDefault();
                LoadData();
            });
        }
        #endregion

        #region Command methods
        /// <summary>
        /// Decreases the sort-id
        /// </summary>
        /// <param name="obj"></param>
        private void SortUp(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var item = obj as DashboardShortcutConfig;

                // find item in collection
                int currentSortId = 0;
                int posInList = 0;

                for (int i = 0; i < ShortcutConfig.Count; i++)
                {
                    if (ShortcutConfig[i].SortId == item.SortId)
                    {
                        posInList = i;
                        currentSortId = ShortcutConfig[i].SortId;
                        break;
                    }
                }

                // return if it cannot decrease
                if (currentSortId <= 0)
                {
                    return;
                }

                ShortcutConfig[posInList].SortId--;
                ShortcutConfig[posInList - 1].SortId++;

                ShortcutConfig = new ObservableCollection<DashboardShortcutConfig>(ShortcutConfig.OrderBy(x => x.SortId).ToList());
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error in DashboardConfigViewModel sorting up: {e.Message}");
            }
        }

        /// <summary>
        /// Increases the sort-id
        /// </summary>
        /// <param name="obj"></param>
        private void SortDown(object obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                var item = obj as DashboardShortcutConfig;

                // find item in collection
                int currentSortId = 0;
                int posInList = 0;

                for (int i = 0; i < ShortcutConfig.Count; i++)
                {
                    if (ShortcutConfig[i].SortId == item.SortId)
                    {
                        posInList = i;
                        currentSortId = ShortcutConfig[i].SortId;
                        break;
                    }
                }

                // return if it cannot increase
                if (currentSortId >= ShortcutConfig.Count)
                {
                    return;
                }

                ShortcutConfig[posInList].SortId++;
                ShortcutConfig[posInList + 1].SortId--;

                ShortcutConfig = new ObservableCollection<DashboardShortcutConfig>(ShortcutConfig.OrderBy(x => x.SortId).ToList());
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error in DashboardConfigViewModel sorting down: {e.Message}");
            }
        }

        #endregion

        #region Helper
        private void LoadData()
        {
            DashboardShortcutHandler.LoadShortcutConfig();
            ShortcutConfig = new ObservableCollection<DashboardShortcutConfig>(DashboardShortcutHandler.ShortcutConfig);
            AvailableShortcuts = new ObservableCollection<AvailableShortcut>(DashboardShortcutHandler.AvailableShortcuts);
            ShortcutLookups = new ObservableCollection<string>(AvailableShortcuts.Select(x => x.Name).ToList());
        }
        #endregion

        #region Overrides
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ERROR DashboardConfigViewModel: {e.Message}");
            }

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}
