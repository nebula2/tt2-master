using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Model.Raid;
using TT2Master.Resources;
using TT2Master.Shared.Raids;
using TT2Master.Views.Raid;
using Xamarin.Forms;

namespace TT2Master.ViewModels.Raid
{
    public class RaidSeedViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        private ObservableCollection<GroupedRaidSeedEnemyViewModel> _shadowCopy = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();

        private ObservableCollection<GroupedRaidSeedEnemyViewModel> _items;
        public ObservableCollection<GroupedRaidSeedEnemyViewModel> Items
        {
            get => _items;
            set
            {
                if (value != _items)
                {
                    SetProperty(ref _items, value);
                }
            }
        }

        public ICommand LoadCommand { get; private set; }
        public ICommand ExpandCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public RaidSeedViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.RaidSeedHeader;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            LoadCommand = new DelegateCommand(async () =>
            {
                var jsonStr = await GetRaidSeedString();
                _ = await LoadSeedsFromJsonString(jsonStr);
            });

            ExpandCommand = new DelegateCommand<object>((o) =>
            {
                if (o == null)
                {
                    return;
                }

                var item = o as GroupedRaidSeedEnemyViewModel;
                _shadowCopy.Where(x => x.LongName == item.LongName).FirstOrDefault().InvertExpanded();
                UpdateListContent();
            });
        }
        #endregion

        #region Helper
        private async Task<string> GetRaidSeedString()
        {
            var file = await Xamarin.Forms.DependencyService.Get<Interfaces.IFilePicker>().PickFile();

            if (file == null)
            {
                Logger.WriteToLogFile("RaidSeedViewModel: file is null");
                return null;
            }

            var filePath = file.FilePath.Contains(@"content://") ?
                Xamarin.Forms.DependencyService.Get<ITapTitansPath>().ProcessPathString(file.FilePath)
                : file.FilePath;

            if (!File.Exists(filePath))
            {
                Logger.WriteToLogFile($"RaidSeedViewModel ERROR: {filePath} does not exist!");
                return null;
            }

            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"RaidSeedViewModel ERROR: Could not read from {filePath}\n{ex}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return null;
            }
        }

        private async Task<bool> ReloadItems()
        {
            var lastStr = LocalSettingsORM.LastRaidSeedString;

            return await LoadSeedsFromJsonString(lastStr);
        }

        private async Task<bool> LoadSeedsFromJsonString(string s)
        {
            if (s == null)
            {
                Items = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();
                _shadowCopy = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();
                return true;
            };

            var parser = new RaidSeedParser();
            try
            {
                if (!parser.LoadSeedsFromJsonString(s))
                {
                    Items = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();
                    _shadowCopy = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();
                    LocalSettingsORM.LastRaidSeedString = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"RaidSeedViewModel Error: {ex}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                Items = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();
                _shadowCopy = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();
                LocalSettingsORM.LastRaidSeedString = null;
                return false;
            }

            var tmpItems = new List<RaidSeedEnemyViewModel>();

            parser.Seeds.ForEach(seed =>
            {
                foreach (var enemy in seed.Enemies)
                {
                    tmpItems.Add(new RaidSeedEnemyViewModel(seed, enemy));
                }
            });

            List<(string code, string name, int tier, int level)> tmp = tmpItems
                .Select(x => (x.GetShortString(), x.GetLongString(), x.Tier, x.Level))
                .Distinct()
                .OrderBy(x => x.Tier)
                .ThenBy(x => x.Level)
                .ToList();
            _shadowCopy = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();

            foreach (var item in tmp)
            {
                var group = new GroupedRaidSeedEnemyViewModel(item.name, item.code);

                foreach (var child in tmpItems.Where(x => x.GetShortString() == group.ShortName).ToList())
                {
                    group.Add(child);
                }

                _shadowCopy.Add(group);
            }

            LocalSettingsORM.LastRaidSeedString = s;
            UpdateListContent();
            return true;
        }

        private void UpdateListContent()
        {
            Items = new ObservableCollection<GroupedRaidSeedEnemyViewModel>();
            foreach (var item in _shadowCopy)
            {
                var grp = new GroupedRaidSeedEnemyViewModel(item.LongName, item.ShortName, item.Expanded);
                if (item.Expanded)
                {
                    foreach (var child in item)
                    {
                        grp.Add(child);
                    }
                }

                Items.Add(grp);
            }
        }
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await ReloadItems();

            base.OnNavigatedTo(parameters);
        }

        /// <summary>
        /// When navigating away
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }
        #endregion
    }
}
