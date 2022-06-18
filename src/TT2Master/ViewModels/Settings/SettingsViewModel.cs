using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Interfaces;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Assets;
using TT2Master.Resources;
using TT2Master.Shared.Helper;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Settings
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        /// <summary>
        /// Command for recreating the default Builds
        /// </summary>
        public ICommand ReloadDefaultBuildsCommand { get; private set; }

        public ICommand DeleteOldClanDataCommand { get; private set; }
        public ICommand WriteOptiLogCommand { get; private set; }
        public ICommand DeleteArtSettingsCommand { get; private set; }
        public ICommand ChangeSavefilePathCommand { get; private set; }

        public ICommand DownloadAssetsAgainCommand { get; private set; }

        public ICommand ExportDatabaseFileCommand { get; private set; }

        public ICommand TestAdmobMediationCommand { get; private set; }
        public ICommand ChangeDataSourceCommand { get; private set; }
        public ICommand CheckPurchasesCommand { get; private set; }
        public ICommand ChangeAbyssSavefilePathCommand { get; private set; }

        private string _tTVersion = "";
        public string TTVersion
        {
            get => _tTVersion;
            set
            {
                SetProperty(ref _tTVersion, value);
            }
        }

        private string _variousAssetVersion = "";
        public string VariousAssetVersion { get => _variousAssetVersion; set => SetProperty(ref _variousAssetVersion, value); }

        private bool _scientificConversion;
        public bool ScientificConversion
        {
            get => _scientificConversion;
            set
            {
                SetProperty(ref _scientificConversion, value);
                if (value != App.ConvertNumbersScientific)
                {
                    App.ConvertNumbersScientific = value;
                }
            }
        }

        private ObservableCollection<Language> _languages;
        public ObservableCollection<Language> Languages { get => _languages; set => SetProperty(ref _languages, value); }

        private Language _currentLanguage;
        public Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if(value == null)
                {
                    return;
                }

                if (value != _currentLanguage)
                {
                    LocalSettingsORM.SetCurrentLanguage(value.ShortName);
                    try
                    {
                        var culture = new CultureInfo(value.ShortName);
                        AppResources.Culture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;
                    }
                    catch (Exception e)
                    {
                        Logger.WriteToLogFile($"Could not set CultureInfo after changing the language: {e.Message}");
                    }

                    SetProperty(ref _currentLanguage, value);
                }
            }
        }

        private int _maxSnapshots;
        public int MaxSnapshots
        {
            get => _maxSnapshots;
            set
            {
                int convertedVal = JfTypeConverter.ForceInt(value);
                if (convertedVal != 0)
                {
                    SetProperty(ref _maxSnapshots, convertedVal);
                    LocalSettingsORM.SetSnapshotAmount(value.ToString());
                }
            }
        }

        private string _csvDelimiter;
        public string CsvDelimiter
        {
            get => _csvDelimiter;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _csvDelimiter = ",";
                    return;
                }

                if (value != _csvDelimiter)
                {
                    SetProperty(ref _csvDelimiter, value[0].ToString());
                    LocalSettingsORM.CsvDelimiter = value[0].ToString();
                }
            }
        }

        private string _clanMessageAmount;
        public string ClanMessageAmount
        {
            get => _clanMessageAmount;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (value != _clanMessageAmount)
                {
                    SetProperty(ref _clanMessageAmount, value);
                    LocalSettingsORM.SetClanMessageAmount(value);
                }
            }
        }

        private bool _isCreatingSnapshotOnDashboardReload;
        public bool IsCreatingSnapshotOnDashboardReload
        {
            get => _isCreatingSnapshotOnDashboardReload;
            set
            {
                SetProperty(ref _isCreatingSnapshotOnDashboardReload, value);
                LocalSettingsORM.IsCreatingSnapshotOnDashboardReload = value;
            }
        }

        private int? _dailyAutoSnapshotThreshold;
        public int? DailyAutoSnapshotThreshold 
        { 
            get => _dailyAutoSnapshotThreshold; 
            set 
            {
                if (value != null && value >= 1)
                {
                    SetProperty(ref _dailyAutoSnapshotThreshold, value);
                    LocalSettingsORM.DailyAutoSnapshotThreshold = (int)value;
                }
            }
        }

        private bool _isInDebugMode = false;
        public bool IsInDebugMode { get => _isInDebugMode; set => SetProperty(ref _isInDebugMode, value); }

        private bool _isLoadingDataFromSavefile = LocalSettingsORM.IsReadingDataFromSavefile;
        public bool IsLoadingDataFromSavefile { get => _isLoadingDataFromSavefile; set => SetProperty(ref _isLoadingDataFromSavefile, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public SettingsViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            Title = AppResources.SettingsHeader;

#if DEBUG
            IsInDebugMode = true;
#endif

            ReloadDefaultBuildsCommand = new DelegateCommand(async () => await ReloadDefaultBuildsAsync());
            WriteOptiLogCommand = new DelegateCommand(async () => await WriteOptiLogExecute());
            DeleteOldClanDataCommand = new DelegateCommand(async () => await DeleteOldClanDataExecute());
            DeleteArtSettingsCommand = new DelegateCommand(async () => await DeleteArtSettingsAsync());
            ChangeSavefilePathCommand = new DelegateCommand(async () => await ChangeSavefilePathAsync());
            ChangeAbyssSavefilePathCommand = new DelegateCommand(async () => await ChangeAbyssSavefilePathAsync());
            DownloadAssetsAgainCommand = new DelegateCommand(async () => await DownloadAssetsAgainAsync());
            ExportDatabaseFileCommand = new DelegateCommand(async () => await ExportDatabaseFileCommandAsync());
            CheckPurchasesCommand = new DelegateCommand(async () => await CheckPurchasesAsync());

            ChangeDataSourceCommand = new DelegateCommand(async () => await ChangeDataSourceCommandAsync());

            TestAdmobMediationCommand = new DelegateCommand(async () =>
            {
                try
                {
                    Xamarin.Forms.DependencyService.Get<ITestAdmobMediation>().StartMediationTest();
                }
                catch (Exception ex)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                }
            });
        }
        #endregion

        #region Command Methods
        private async Task<bool> CheckPurchasesAsync()
        {
            try
            {
                if(!await Purchasement.CheckPurchases(false, _dialogService))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorOccuredText, AppResources.CouldNotUpdatePurchasementInformation, AppResources.OKText);

                    return false;
                }


                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.PurchasementInformationUpdated, AppResources.OKText);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"SettingsVM.CheckPurchasesAsync() Error: {ex.Message}");

                await _dialogService.DisplayAlertAsync(AppResources.ErrorOccuredText, AppResources.CouldNotUpdatePurchasementInformation, AppResources.OKText);

                return false;
            }
        }

        private async Task<bool> ChangeDataSourceCommandAsync()
        {
            try
            {
                if(Device.RuntimePlatform == Device.iOS)
                {
                    return true;
                }

                bool result;

                if (LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    result = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader
                        , AppResources.ChangeFromSavefileToClipboardSourceQuestion
                        , AppResources.Yes
                        , AppResources.No);
                }
                else
                {
                    result = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader
                        , AppResources.ChangeFromSavefileToClipboardSourceQuestion
                        , AppResources.Yes
                        , AppResources.No);
                }

                if (result)
                {
                    LocalSettingsORM.IsReadingDataFromSavefile = !LocalSettingsORM.IsReadingDataFromSavefile;
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.PleaseRestartTheAppForChangesToTakeEffect, AppResources.OKText);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"SettingsVM.ChangeDataSourceCommandAsync() Error: {ex.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }
        }

        private async Task<bool> ExportDatabaseFileCommandAsync()
        {
            try
            {
                string dbName = "tt2master.db3";
                string destination = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDocumentName(dbName);
                var dbPath = Xamarin.Forms.DependencyService.Get<IDBPath>().DBPath(dbName);

                FileHelper.CopyFileSafely(dbPath, destination);

                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.FileSavedToText + $" {destination}", AppResources.OKText);

                return true;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);

                Logger.WriteToLogFile($"SettingsVM.ExportDatabaseFileCommandAsync() Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> DownloadAssetsAgainAsync()
        {
            bool result = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.DownloadAssetsAgainText, AppResources.OKText, AppResources.CancelText);

            if (!result)
            {
                return true;
            }

            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.NoInternet, AppResources.OKText);
                    return false;
                }

                // force download
                AssetManager.OnError += AssetManager_OnError;
                AssetManager.OnLogMePlease += AssetManager_OnLogMePlease;
                bool success = await AssetManager.CheckAndDownloadAssetsAsync(true);
                AssetManager.OnError -= AssetManager_OnError;
                AssetManager.OnLogMePlease -= AssetManager_OnLogMePlease;

                SetAssetStates();

                if (success)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.FilesDownloadedText, AppResources.OKText, AppResources.CancelText);
                    return true;
                }

                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                return false;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);

                Logger.WriteToLogFile($"SettingsVM.DownloadAssetsAgainAsync() Error: {ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// Changes savefile path
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ChangeSavefilePathAsync()
        {
            try
            {
                string f = LocalSettingsORM.TT2SavefilePath ?? Xamarin.Forms.DependencyService.Get<ITapTitansPath>().GetFileName();

                //create filepicker
                Logger.WriteToLogFile($"SettingsVM: opening filepicker");
                var file = await Xamarin.Forms.DependencyService.Get<Interfaces.IFilePicker>().PickFile();

                //check if file was picked
                if (file == null)
                {
                    Logger.WriteToLogFile($"SettingsVM: file is null");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotCatchFileText, AppResources.OKText);
                    return false;
                }

                if (file.FilePath == null)
                {
                    Logger.WriteToLogFile($"SettingsVM: filepath is null");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotGetFilepathText, AppResources.OKText);
                    return false;
                }

                Logger.WriteToLogFile($"SettingsVM: User picked a file. Converting {file.FilePath}");

                //Convert Uri-String
                f = file.FilePath.Contains(@"content://") ?
                    Xamarin.Forms.DependencyService.Get<ITapTitansPath>().ProcessPathString(file.FilePath)
                    : file.FilePath;

                Logger.WriteToLogFile($"SettingsVM: Converted to {f}");

                //check if conversion worked and file exists
                if (!File.Exists(f))
                {
                    Logger.WriteToLogFile($"SettingsVM: {f} does not exist or not having permission on it");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotGetFilepathText, AppResources.OKText);
                    return false;
                }

                //if success - store value to settings
                LocalSettingsORM.TT2SavefilePath = f;
                Logger.WriteToLogFile($"SettingsVM: stored {f} as TT2SavefilePath-Setting");
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SavefilePathChangedSuccessfully, AppResources.OKText);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"SettingsVM: Error picking new savefile path: {ex.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                return false;
            }
        }

        /// <summary>
        /// Changes savefile path
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ChangeAbyssSavefilePathAsync()
        {
            try
            {
                string f = LocalSettingsORM.AbyssalSavefilePath ?? Xamarin.Forms.DependencyService.Get<ITapTitansPath>().GetAbyssalFileName();

                //create filepicker
                Logger.WriteToLogFile($"SettingsVM: opening filepicker");
                var file = await Xamarin.Forms.DependencyService.Get<IFilePicker>().PickFile();

                //check if file was picked
                if (file == null)
                {
                    Logger.WriteToLogFile($"SettingsVM: file is null");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotCatchFileText, AppResources.OKText);
                    return false;
                }

                if (file.FilePath == null)
                {
                    Logger.WriteToLogFile($"SettingsVM: filepath is null");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotGetFilepathText, AppResources.OKText);
                    return false;
                }

                Logger.WriteToLogFile($"SettingsVM: User picked a file. Converting {file.FilePath}");

                //Convert Uri-String
                f = file.FilePath.Contains(@"content://") ?
                    Xamarin.Forms.DependencyService.Get<ITapTitansPath>().ProcessPathString(file.FilePath)
                    : file.FilePath;

                Logger.WriteToLogFile($"SettingsVM: Converted to {f}");

                //check if conversion worked and file exists
                if (!File.Exists(f))
                {
                    Logger.WriteToLogFile($"SettingsVM: {f} does not exist or not having permission on it");
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.CouldNotGetFilepathText, AppResources.OKText);
                    return false;
                }

                //if success - store value to settings
                LocalSettingsORM.AbyssalSavefilePath = f;
                Logger.WriteToLogFile($"SettingsVM: stored {f} as TT2SavefilePath-Setting");
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SavefilePathChangedSuccessfully, AppResources.OKText);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"SettingsVM: Error picking new savefile path: {ex.Message}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                return false;
            }
        }

        private async Task<bool> DeleteOldClanDataExecute()
        {
            int recordCount = await SnapshotFactory.DeleteOldClanDataAsync();
            int msgCount = await ClanMessageFactory.DeleteOldClanDataAsync();

            await ToastSender.SendToastAsync($"{AppResources.DeletedText} {recordCount + msgCount}", _dialogService);

            return true;
        }

        /// <summary>
        /// Execute for <see cref="DeleteArtSettingsCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DeleteArtSettingsAsync()
        {
            int result = await App.DBRepo.DeleteArtOptSettingsID("1");

            if (result > 0)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.DeletedText, AppResources.OKText);
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ErrorOccuredText, AppResources.OKText);
            }

            return true;
        }

        /// <summary>
        /// Reloads the default builds by recreating them
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadDefaultBuildsAsync()
        {
            DefaultBuildFactory.OnProblemHaving += DefaultBuildFactory_OnProblemHaving;
            DefaultBuildFactory.OnLogMePlease += InfoFileHandler_OnLogMePlease;

            bool success = await DefaultBuildFactory.RecreateAllDefaultBuildsAsync();

            if (success)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.BuildsRestoredText, AppResources.OKText);
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ProblemRestoringDefaultBuildsText, AppResources.OKText);
                DefaultBuildFactory.OnProblemHaving -= DefaultBuildFactory_OnProblemHaving;
                return false;
            }

            DefaultBuildFactory.OnProblemHaving -= DefaultBuildFactory_OnProblemHaving;
            DefaultBuildFactory.OnLogMePlease -= InfoFileHandler_OnLogMePlease;
            return true;
        }

        /// <summary>
        /// Execute for <see cref="WriteOptiLogCommand"/>
        /// </summary>
        private async Task<bool> WriteOptiLogExecute()
        {
            if (OptimizeLogger.WriteLog)
            {
                bool answer = await _dialogService.DisplayAlertAsync(AppResources.LoggingHeader
                    , Device.RuntimePlatform == Device.Android ? AppResources.LoggingActiveText : "Activate diagnostic logging?"
                    , AppResources.YesText
                    , AppResources.NoText);

                if (answer)
                {
                    OptimizeLogger.WriteLog = false;
                }

                return true; ;
            }
            else
            {
                bool answer = await _dialogService.DisplayAlertAsync(AppResources.LoggingHeader
                    , string.Format(AppResources.LoggingInactiveText, OptimizeLogger.LogName), AppResources.YesText, AppResources.NoText);

                if (answer)
                {
                    OptimizeLogger.WriteLog = true;
                }

                return true;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Populates properties from AppSettings
        /// </summary>
        private void LoadSettings()
        {
            //Conversion
            ScientificConversion = App.ConvertNumbersScientific;

            //SnapshotAmount
            MaxSnapshots = JfTypeConverter.ForceInt(LocalSettingsORM.GetSnapshotAmount());

            CsvDelimiter = LocalSettingsORM.CsvDelimiter;

            ClanMessageAmount = LocalSettingsORM.GetClanMessageAmount();

            IsCreatingSnapshotOnDashboardReload = LocalSettingsORM.IsCreatingSnapshotOnDashboardReload;
            DailyAutoSnapshotThreshold = LocalSettingsORM.DailyAutoSnapshotThreshold;
        }

        private void SetAssetStates()
        {
            var inf = AssetManager.AssetTypes.Where(x => x.Identifier == "InfoFiles").FirstOrDefault();
            var div = AssetManager.AssetTypes.Where(x => x.Identifier == "Various").FirstOrDefault();

            TTVersion = $"{inf?.CurrentVersion ?? "?"} - {inf?.AssetState.ToString() ?? "?"}";
            VariousAssetVersion = $"{div.CurrentVersion ?? "?"} - {div?.AssetState.ToString() ?? "?"}";
        }
        #endregion

        #region E + D
        private void AssetManager_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"SettingsVM.DownloadAssetsAgainAsync(): {e.Information}");
        private void AssetManager_OnError(object sender, CustErrorEventArgs e) => Logger.WriteToLogFile($"SettingsVM.DownloadAssetsAgainAsync() Error: {e.MyException.Message}");

        /// <summary>
        /// Handles Error
        /// </summary>
        /// <param name="e"></param>
        private async void DefaultBuildFactory_OnProblemHaving(object sender, CustErrorEventArgs e) => await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, $"{e.MyException.Message}", AppResources.OKText);

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            SetAssetStates();

            Languages = new ObservableCollection<Language>(SupportedLanguages.Languages);
            CurrentLanguage = Languages.Where(x => x.ShortName == LocalSettingsORM.GetCurrentLanguage()).FirstOrDefault();

            LoadSettings();

            base.OnNavigatedTo(parameters);
        }

        private void InfoFileHandler_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"Information at {sender.ToString()}: {e.Information}");
        #endregion
    }
}