using Prism.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Resources;
using Xamarin.Essentials;

namespace TT2Master.Model
{
    public class SavefileHandler
    {
        private IPageDialogService _dialogService;

        public SavefileHandler(IPageDialogService  dialogService)
        {
            _dialogService = dialogService;
        }

        public void InitPaths()
        {
            if (string.IsNullOrWhiteSpace(LocalSettingsORM.TT2TempSavefilePath))
            {
                LocalSettingsORM.TT2TempSavefilePath = Xamarin.Forms.DependencyService.Get<IDBPath>().DBPath("mySave.adat");
            }
            if (string.IsNullOrWhiteSpace(LocalSettingsORM.TT2TempAbyssSavefilePath))
                LocalSettingsORM.TT2TempAbyssSavefilePath = Xamarin.Forms.DependencyService.Get<IDBPath>().DBPath("myChallengeSave.adat");
        }

        /// <summary>
        /// Copies savefile and loads data from copy
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadSavefileAsync(bool isInRecursion = false)
        {
            if (!LocalSettingsORM.IsReadingDataFromSavefile)
            {
                try
                {
                    SaveFile.OnProgressMade += SaveFile_OnProgressMade;
                    SaveFile.OnError += SaveFile_OnError;
                    SaveFile.OnLogMePlease += SaveFile_OnLogMePlease;

                    bool exportOk = await App.Save.Initialize(loadAccountModel: false);

                    SaveFile.OnProgressMade -= SaveFile_OnProgressMade;
                    SaveFile.OnError -= SaveFile_OnError;
                    SaveFile.OnLogMePlease -= SaveFile_OnLogMePlease;
                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"LoadSavefileAsync ERROR: {ex.Message}"));
                }

                return true;
            }

            OnLogMePlease?.Invoke(this, new InformationEventArgs("LoadSavefileAsync: Loading Savefile"));
            OnProgressMade?.Invoke(this, new InformationEventArgs(AppResources.LoadingSavefileText));

            // secure copy path for savefile
            LocalSettingsORM.TT2TempSavefilePath = Xamarin.Forms.DependencyService.Get<IDBPath>().DBPath("mySave.adat");
            LocalSettingsORM.TT2TempAbyssSavefilePath = Xamarin.Forms.DependencyService.Get<IDBPath>().DBPath("myChallengeSave.adat");

            // Load Save file
            SaveFile.OnProgressMade += SaveFile_OnProgressMade;
            SaveFile.OnError += SaveFile_OnError;
            SaveFile.OnLogMePlease += SaveFile_OnLogMePlease;

            #region Secure Save file path

            OnLogMePlease?.Invoke(this, new InformationEventArgs("LoadSavefileAsync: Getting Savefile filename"));

            // get full name of file to decrypt - then decrypt it
            string f = GetSavefilePath();

            OnLogMePlease?.Invoke(this, new InformationEventArgs($"LoadSavefileAsync: f is currently: {f}"));

            // check for file permissions
            if (!HasRequiredSavefilePermissions(f))
            {
                f = await LetUserPickSaveFileAsync(f, true);
                if (isInRecursion) return false;
                if (f == null && !!LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    await LoadSavefileAsync(true);
                }
            }

            // check if file exists
            if (!File.Exists(f) || !f.EndsWith(".adat"))
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"LoadSavefileAsync: {f} does not exist. Asking user to search"));
                f = await LetUserPickSaveFileAsync(f, false);
                if (isInRecursion) return false;
                if (f == null && !!LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    await LoadSavefileAsync(true);
                }
            }

            // if success - store value to settings
            SetSavefilePath(f);
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"LoadSavefileAsync: stored {f} as savefile path-Setting"));
            #endregion

            bool saveOk = await App.Save.Initialize(loadAccountModel: false);
            
            SaveFile.OnProgressMade -= SaveFile_OnProgressMade;
            SaveFile.OnError -= SaveFile_OnError;
            SaveFile.OnLogMePlease -= SaveFile_OnLogMePlease;
            
            if (!saveOk)
            {
                OnProblemHaving?.Invoke(this, new CustErrorEventArgs(new Exception("Save file could not be loaded")));
                return false;
            }

            OnLogMePlease?.Invoke(this, new InformationEventArgs("LoadSavefileAsync: Done loading savefile"));
            return true;
        }

        /// <summary>
        /// Lets user pick a savefile
        /// </summary>
        /// <param name="f">current filepath</param>
        /// <param name="isMissingAccess">may it be that access is missing? if yes we will display another message</param>
        /// <returns></returns>
        private async Task<string> LetUserPickSaveFileAsync(string f, bool isMissingAccess)
        {
            // ask User if he wants to find the file
            await Task.Yield();

            string header = isMissingAccess ? AppResources.CouldNotAccessFileTitle : AppResources.CouldNotFindFileText;
            string body = isMissingAccess ? AppResources.CouldAccessSavefileText : AppResources.CouldNotFindSavefileText;

            bool answer = await _dialogService.DisplayAlertAsync(header, string.Format(body, f), AppResources.YesText, AppResources.NoText);

            // user does not want
            if (!answer)
            {
                if(!await AskUserToChangeDataSourceToClipboard())
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: user does not want to search"));
                    OnProblemHaving?.Invoke(this, new CustErrorEventArgs(new Exception(AppResources.CouldNotCatchFileText)));
                    return f;
                }
                else
                {
                    return null;
                }
            }

            // create filepicker
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: opening filepicker"));
            var file = await Xamarin.Forms.DependencyService.Get<Interfaces.IFilePicker>().PickFile();

            // check if filepath is there
            if (file?.FilePath != null)
            {
                if (!file.FilePath.EndsWith(".adat"))
                {
                    if(!await AskUserToChangeDataSourceToClipboard())
                    {
                        OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: {file.FilePath} does not end with .adat"));
                        OnProblemHaving?.Invoke(this, new CustErrorEventArgs(new Exception(AppResources.WrongFileTypeSelectedAdat)));
                        return f;
                    }
                    else
                    {
                        return null;
                    }
                }

                OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: User picked a file. Converting {file.FilePath}"));

                // Convert Uri-String
                f = file.FilePath.Contains(@"content://") ?
                    Xamarin.Forms.DependencyService.Get<ITapTitansPath>().ProcessPathString(file.FilePath)
                    : file.FilePath;

                OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: Converted to {f}"));

                // check if conversion worked and file exists
                if (!File.Exists(f))
                {
                    if(!await AskUserToChangeDataSourceToClipboard())
                    {
                        OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: {f} does not exist or not having permission on it"));
                        OnProblemHaving?.Invoke(this, new CustErrorEventArgs(new Exception(AppResources.CouldNotGetFilepathText)));
                    }
                }
            }
            else
            {
                if(!await AskUserToChangeDataSourceToClipboard())
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: filepath is null"));
                    OnProblemHaving?.Invoke(this, new CustErrorEventArgs(new Exception(AppResources.CouldNotGetFilepathText)));
                }
            }

            return f;
        }

        private async Task<bool> AskUserToChangeDataSourceToClipboard()
        {
            bool datasourceAnswer = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader
                    , AppResources.ChangeToClipboardDataToFixSafeFileIssuesQuestion
                    , AppResources.Yes
                    , AppResources.No);

            if (datasourceAnswer)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"LetUserPickSaveFileAsync: user changed data source"));

                LocalSettingsORM.IsReadingDataFromSavefile = false;

                OnProblemHaving?.Invoke(this, new CustErrorEventArgs(new Exception(AppResources.PleaseRestartTheAppForChangesToTakeEffect)));
            }
            
            return datasourceAnswer;
        }

        /// <summary>
        /// Checks if we can copy the file
        /// </summary>
        /// <param name="file">file to check access for</param>
        /// <returns>true if all good else false</returns>
        private bool HasRequiredSavefilePermissions(string file)
        {
            try
            {
                File.Copy(file, Path.Combine(FileSystem.CacheDirectory, "smellyFart.tmp"), true);
                return true;
            }
            catch (System.Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"HasRequiredSavefilePermissions ERROR: {ex.Message}"));
                return false;
            }
        }

        public static string GetSavefilePath()
        {
            return LocalSettingsORM.IsDefaultSavefileSelected
                ? LocalSettingsORM.TT2SavefilePath ?? Xamarin.Forms.DependencyService.Get<ITapTitansPath>().GetFileName()
                : LocalSettingsORM.AbyssalSavefilePath ?? Xamarin.Forms.DependencyService.Get<ITapTitansPath>().GetAbyssalFileName();
        }

        public static void SetSavefilePath(string f)
        {
            if (LocalSettingsORM.IsDefaultSavefileSelected)
            {
                LocalSettingsORM.TT2SavefilePath = f;
            }
            else
            {
                LocalSettingsORM.AbyssalSavefilePath = f;
            }
        }

        #region events and delegates
        private void SaveFile_OnLogMePlease(object sender, InformationEventArgs e) => OnLogMePlease?.Invoke(sender, e);

        private void SaveFile_OnError(object sender, CustErrorEventArgs e) => OnProblemHaving?.Invoke(this, e);

        private void SaveFile_OnProgressMade(object sender, InformationEventArgs e) => OnProgressMade?.Invoke(sender, e);

        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Raised when i made progress
        /// </summary>
        public event ProgressCarrier OnProgressMade;

        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(object sender, CustErrorEventArgs e);

        public event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}
