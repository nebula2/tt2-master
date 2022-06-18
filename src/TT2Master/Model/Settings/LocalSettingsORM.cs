using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TT2Master.Model.Arti;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Manages local Settings
    /// </summary>
    public static class LocalSettingsORM
    {
        /// <summary>
        /// Gets the CurrSPBuildName-Setting
        /// </summary>
        /// <returns></returns>
        public static string GetCurrSPBuildName()
        {
            return Application.Current.Properties.ContainsKey("CurrSPBuildName")
                ? Application.Current.Properties["CurrSPBuildName"] == null ? "" : Application.Current.Properties["CurrSPBuildName"].ToString()
                : "";
        }

        /// <summary>
        /// Sets the CurrSPBuildName-Setting
        /// </summary>
        /// <param name="value"></param>
        public static void SetCurrSPBuildName(string value) => Application.Current.Properties["CurrSPBuildName"] = value;

        /// <summary>
        /// Gets the OnlyRelevant-Setting
        /// </summary>
        /// <returns></returns>
        public static bool GetOnlyRelevant()
        {
            return Application.Current.Properties.ContainsKey("OnlyRelevant")
                ? Application.Current.Properties["OnlyRelevant"].ToString() == "TRUE" ? true : false
                : true;
        }

        /// <summary>
        /// Sets the OnlyRelevant setting
        /// </summary>
        /// <param name="value"></param>
        public static void SetOnlyRelevant(bool value) => Application.Current.Properties["OnlyRelevant"] = value ? "TRUE" : "FALSE";

        /// <summary>
        /// Gets the Current Language setting
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLanguage()
        {
            return Application.Current.Properties.ContainsKey("CurrentLanguage")
                ? Application.Current.Properties["CurrentLanguage"].ToString() ?? "en"
                : "en";
        }

        /// <summary>
        /// Sets the current Language setting
        /// </summary>
        /// <param name="value"></param>
        public static void SetCurrentLanguage(string value) => Application.Current.Properties["CurrentLanguage"] = value;

        /// <summary>
        /// Gets the number conversion setting
        /// </summary>
        /// <returns></returns>
        // TODO make this a property
        public static bool GetConvertNumbersScientific()
        {
            return Application.Current.Properties.ContainsKey("ConvertNumbersScientific")
                ? Application.Current.Properties["ConvertNumbersScientific"].ToString() == "TRUE" ? true : false
                : true;
        }

        /// <summary>
        /// Sets the number conversion setting
        /// </summary>
        /// <param name="value"></param>
        public static void SetConvertNumbersScientific(bool value) => Application.Current.Properties["ConvertNumbersScientific"] = value ? "TRUE" : "FALSE";

        /// <summary>
        /// Gets current version of locally stored various assets
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentVariousAssetsVersion() => Application.Current.Properties.ContainsKey("VariousAssetsVersion")
                ? Application.Current.Properties["VariousAssetsVersion"].ToString() ?? "0.0.1"
                : "0.0.1";

        /// <summary>
        /// Gets current version of locally stored various assets
        /// </summary>
        /// <returns></returns>
        public static void SetCurrentVariousAssetsVersion(string value) => Application.Current.Properties["VariousAssetsVersion"] = value;

        /// <summary>
        /// Get Current TT2Master-Version setting
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTTMasterVersion() => ChangelogList.Changelog.Max(x => x.Version).ToString();

        /// <summary>
        /// Sets TT2-Database path setting
        /// </summary>
        /// <param name="value"></param>
        public static void SetTT2DBPath(string value) => Application.Current.Properties["TT2DBPath"] = value;

        /// <summary>
        /// Gets the maximum Snapshot amount setting
        /// </summary>
        /// <returns></returns>
        public static string GetSnapshotAmount()
        {
            return Application.Current.Properties.ContainsKey("SnapshotAmount")
                ? Application.Current.Properties["SnapshotAmount"].ToString() ?? "100"
                : "100";
        }

        /// <summary>
        /// Sets the maximum Snapshot amount setting
        /// </summary>
        /// <param name="value"></param>
        public static void SetSnapshotAmount(string value) => Application.Current.Properties["SnapshotAmount"] = value;

        /// <summary>
        /// Gets the maximum Clanmessage amount setting
        /// </summary>
        /// <returns></returns>
        public static string GetClanMessageAmount()
        {
            return Application.Current.Properties.ContainsKey("ClanMessageAmount")
                ? Application.Current.Properties["ClanMessageAmount"].ToString() ?? "1000"
                : "1000";
        }

        /// <summary>
        /// Sets the maximum ClanMessage amount setting
        /// </summary>
        /// <param name="value"></param>
        public static void SetClanMessageAmount(string value) => Application.Current.Properties["ClanMessageAmount"] = value;

        /// <summary>
        /// Gets the SplashSkillSetting
        /// </summary>
        /// <returns></returns>
        public static string GetSplashSkillSetting()
        {
            return Application.Current.Properties.ContainsKey("SplashSkillSetting")
                ? Application.Current.Properties["SplashSkillSetting"].ToString() ?? "0"
                : "0";
        }

        /// <summary>
        /// Sets the SplashSkillSetting
        /// </summary>
        /// <param name="value"></param>
        public static void SetSplashSkillSetting(string value) => Application.Current.Properties["SplashSkillSetting"] = value;

        /// <summary>
        /// Gets the SplashSnapSetting
        /// </summary>
        /// <returns></returns>
        public static string GetSplashSnapSetting()
        {
            return Application.Current.Properties.ContainsKey("SplashSnapSetting")
                ? Application.Current.Properties["SplashSnapSetting"].ToString() ?? "0"
                : "0";
        }

        /// <summary>
        /// Sets the SplashSnapSetting
        /// </summary>
        /// <param name="value"></param>
        public static void SetSplashSnapSetting(string value) => Application.Current.Properties["SplashSnapSetting"] = value;

        private static ISettings AppSettings => CrossSettings.Current;

        #region Automation
        public static bool IsClanAutoExportNotificationEnabled
        {
            get => AppSettings.GetValueOrDefault("IsClanAutoExportNotificationEnabled", false);
            set => AppSettings.AddOrUpdateValue("IsClanAutoExportNotificationEnabled", value);
        }

        public static int ClanAutoExportSchedule
        {
            get => AppSettings.GetValueOrDefault("ClanAutoExportSchedule", 1);
            set => AppSettings.AddOrUpdateValue("ClanAutoExportSchedule", value);
        }

        public static bool IsClanAutoExport
        {
            get => AppSettings.GetValueOrDefault("IsClanAutoExport", false);
            set => AppSettings.AddOrUpdateValue("IsClanAutoExport", value);
        }

        public static bool IsAutoArtifactCheck
        {
            get => AppSettings.GetValueOrDefault("IsAutoArtifactCheck", true);
            set => AppSettings.AddOrUpdateValue("IsAutoArtifactCheck", value);
        }
        public static bool IsAutoSkillCheck
        {
            get => AppSettings.GetValueOrDefault("IsAutoSkillCheck", true);
            set => AppSettings.AddOrUpdateValue("IsAutoSkillCheck", value);
        }
        public static bool IsAutoEquipCheck
        {
            get => AppSettings.GetValueOrDefault("IsAutoEquipCheck", true);
            set => AppSettings.AddOrUpdateValue("IsAutoEquipCheck", value);
        }

        public static int AutoServiceSchedule
        {
            get => AppSettings.GetValueOrDefault("AutoServiceSchedule", 5);
            set => AppSettings.AddOrUpdateValue("AutoServiceSchedule", value);
        }
        public static bool IsDiamondFairyWished
        {
            get => AppSettings.GetValueOrDefault("IsDiamondFairyWished", true);
            set => AppSettings.AddOrUpdateValue("IsDiamondFairyWished", value);
        }
        public static bool IsFatFairyWished
        {
            get => AppSettings.GetValueOrDefault("IsFatFairyWished", true);
            set => AppSettings.AddOrUpdateValue("IsFatFairyWished", value);
        }
        public static bool IsFreeEquipWished
        {
            get => AppSettings.GetValueOrDefault("IsFreeEquipWished", true);
            set => AppSettings.AddOrUpdateValue("IsFreeEquipWished", value);
        }
        #endregion

        /// <summary>
        /// SPIgnoreNotEnoughSP-Setting
        /// </summary>
        /// <returns></returns>
        public static bool SPIgnoreNotEnoughSP
        {
            get => AppSettings.GetValueOrDefault("SPIgnoreNotEnoughSP", false);
            set => AppSettings.AddOrUpdateValue("SPIgnoreNotEnoughSP", value);
        }

        /// <summary>
        /// SPOverclockAmount-Setting
        /// </summary>
        /// <returns></returns>
        public static int SPOverclockAmount
        {
            get => AppSettings.GetValueOrDefault("SPOverclockAmount", 0);
            set => AppSettings.AddOrUpdateValue("SPOverclockAmount", value);
        }

        /// <summary>
        /// DefaultSPConfiguration-Setting
        /// </summary>
        /// <returns></returns>
        public static string DefaultSPConfiguration
        {
            get => AppSettings.GetValueOrDefault("DefaultSPConfiguration", "");
            set => AppSettings.AddOrUpdateValue("DefaultSPConfiguration", value);
        }

        /// <summary>
        /// Path to the Tap Titans 2 savefile.
        /// <para>Default is DependencyService.Get<ITapTitansPath>().GetFileName()</para>
        /// </summary>
        public static string TT2SavefilePath
        {
            get => AppSettings.GetValueOrDefault("TT2SavefilePath", null);
            set => AppSettings.AddOrUpdateValue("TT2SavefilePath", value);
        }

        /// <summary>
        /// Path to local copy of the Tap Titans 2 savefile.
        /// <para>Default is Xamarin.Forms.DependencyService.Get<IDBPath>().DBPath("mySave.adat")</para>
        /// </summary>
        public static string TT2TempSavefilePath
        {
            get => AppSettings.GetValueOrDefault("TT2TempSavefilePath", null);
            set => AppSettings.AddOrUpdateValue("TT2TempSavefilePath", value);
        }

        /// <summary>
        /// Current TT2-Version setting
        /// </summary>
        /// <returns></returns>
        public static string CurrentTTVersion
        {
            get => AppSettings.GetValueOrDefault("TT2Version", "");
            set => AppSettings.AddOrUpdateValue("TT2Version", value);
        }

        /// <summary>
        /// the deliminator for csv files
        /// </summary>
        /// <returns></returns>
        public static string CsvDelimiter
        {
            get => AppSettings.GetValueOrDefault("CsvDelimiter", ",");
            set => AppSettings.AddOrUpdateValue("CsvDelimiter", value);
        }

        /// <summary>
        /// the locally stored AppVersion (TT2Master)
        /// </summary>
        /// <returns></returns>
        public static string AppVersion
        {
            get => AppSettings.GetValueOrDefault("AppVersion", "Not set");
            set => AppSettings.AddOrUpdateValue("AppVersion", value);
        }

        /// <summary>
        /// UseMasterBoSDisplay-Setting
        /// </summary>
        /// <returns>true, if proper BoS display is wished. false if dumb data is requested</returns>
        public static bool UseMasterBoSDisplay
        {
            get => AppSettings.GetValueOrDefault("UseMasterBoSDisplay", true);
            set => AppSettings.AddOrUpdateValue("UseMasterBoSDisplay", value);
        }

        /// <summary>
        /// IsCreatingSnapshotOnDashboardReload-Setting
        /// </summary>
        /// <returns>true, if a snapshot should be created on dashboard reload</returns>
        public static bool IsCreatingSnapshotOnDashboardReload
        {
            get => AppSettings.GetValueOrDefault("IsCreatingSnapshotOnDashboardReload", false);
            set => AppSettings.AddOrUpdateValue("IsCreatingSnapshotOnDashboardReload", value);
        }

        /// <summary>
        /// DailyAutoSnapshotThreshold-Setting
        /// </summary>
        /// <returns>Amount of automatic daily snapshots</returns>
        public static int DailyAutoSnapshotThreshold
        {
            get => AppSettings.GetValueOrDefault("DailyAutoSnapshotThreshold", 1);
            set => AppSettings.AddOrUpdateValue("DailyAutoSnapshotThreshold", value);
        }


        /// <summary>
        /// LastSnapshotExportFromId-Setting
        /// </summary>
        /// <returns></returns>
        public static int LastSnapshotExportFromId
        {
            get => AppSettings.GetValueOrDefault("LastSnapshotExportFromId", 1);
            set => AppSettings.AddOrUpdateValue("LastSnapshotExportFromId", value);
        }


        /// <summary>
        /// LastSnapshotExportToId-Setting
        /// </summary>
        /// <returns></returns>
        public static int LastSnapshotExportToId
        {
            get => AppSettings.GetValueOrDefault("LastSnapshotExportToId", 1);
            set => AppSettings.AddOrUpdateValue("LastSnapshotExportToId", value);
        }

        #region artifact optimizer view settings
        /// <summary>
        /// ArtOptViewMode-Setting represented by index
        /// </summary>
        /// <returns>prefered view mode for artifact optimizer</returns>
        public static int ArtOptViewModeInt
        {
            get => AppSettings.GetValueOrDefault("ArtOptViewMode", (int)ArtifactOptimizerViewMode.DefaultList);
            set => AppSettings.AddOrUpdateValue("ArtOptViewMode", value);
        }

        /// <summary>
        /// ArtOptViewMode-Setting represented by index
        /// </summary>
        /// <returns>prefered direction mode for artifact optimizer</returns>
        public static int ArtOptDirectionModeInt
        {
            get => AppSettings.GetValueOrDefault("ArtOptDirectionModeInt", (int)ArtifactOptimizerDirectionMode.Row);
            set => AppSettings.AddOrUpdateValue("ArtOptDirectionModeInt", value);
        }

        /// <summary>
        /// ArtOptCellSize-Setting
        /// </summary>
        /// <returns>prefered size for artifact optimizer</returns>
        public static int ArtOptCellSize
        {
            get => AppSettings.GetValueOrDefault("ArtOptCellSize", 50);
            set => AppSettings.AddOrUpdateValue("ArtOptCellSize", value);
        }
        #endregion

        /// <summary>
        /// DefaultShortcutConfig-Setting
        /// </summary>
        /// <returns></returns>
        public static string DefaultShortcutConfig
        {
            get => AppSettings.GetValueOrDefault("DefaultShortcutConfig", "0,0;1,1;2,2;3,3;4,4;5,5;6,6;7,7;8,8;");
            set => AppSettings.AddOrUpdateValue("DefaultShortcutConfig", value);
        }

        /// <summary>
        /// DefaultExportDataSourceShortcutConfig-Setting
        /// </summary>
        /// <returns></returns>
        public static string DefaultExportDataSourceShortcutConfig
        {
            get => AppSettings.GetValueOrDefault("DefaultExportDataSourceShortcutConfig", "0,0;1,1;2,2;4,4;5,5;6,6;");
            set => AppSettings.AddOrUpdateValue("DefaultExportDataSourceShortcutConfig", value);
        }

        /// <summary>
        /// Custom shortcut setting
        /// </summary>
        /// <returns></returns>
        public static string CustomShortcutConfig
        {
            get => AppSettings.GetValueOrDefault("CustomShortcutConfig", null);
            set => AppSettings.AddOrUpdateValue("CustomShortcutConfig", value);
        }

        /// <summary>
        /// AdsClickedTodayCounter-Setting
        /// </summary>
        /// <returns>amount of ads clicked today</returns>
        public static int AdsClickedTodayCounter
        {
            get => AppSettings.GetValueOrDefault("AdsClickedTodayCounter", 0);
            set => AppSettings.AddOrUpdateValue("AdsClickedTodayCounter", value);
        }

        /// <summary>
        /// AdLimitResetDay-Setting
        /// </summary>
        /// <returns>null if limit not reached, else it returns the date in format yyyy-MM-dd when ads can be shown again</returns>
        public static string AdLimitResetDay
        {
            get => AppSettings.GetValueOrDefault("AdLimitResetDay", null);
            set => AppSettings.AddOrUpdateValue("AdLimitResetDay", value);
        }

        /// <summary>
        /// AdCounterResetDay-Setting
        /// </summary>
        /// <returns>null if no ad has been clicked today, else it returns the date in format yyyy-MM-dd when AdsClickedTodayCounter will be reset</returns>
        public static string AdCounterResetDay
        {
            get => AppSettings.GetValueOrDefault("AdCounterResetDay", null);
            set => AppSettings.AddOrUpdateValue("AdCounterResetDay", value);
        }

        /// <summary>
        /// IsReadingDataFromSavefile-Setting
        /// </summary>
        /// <returns>true, if data is read from savefile. false if data is read from export</returns>
        public static bool IsReadingDataFromSavefile
        {
            get => AppSettings.GetValueOrDefault("IsReadingDataFromSavefile", true);
            set => AppSettings.AddOrUpdateValue("IsReadingDataFromSavefile", value);
        }

        /// <summary>
        /// IsDataFromSavefileDescisionMade-Setting
        /// </summary>
        /// <returns>true, if descision made</returns>
        public static bool IsDataFromSavefileDescisionMade
        {
            get => AppSettings.GetValueOrDefault("IsDataFromSavefileDescisionMade", false);
            set => AppSettings.AddOrUpdateValue("IsDataFromSavefileDescisionMade", value);
        }

        /// <summary>
        /// CurrentSavefileString-Setting
        /// </summary>
        /// <returns>null if there is no current save file string</returns>
        public static string CurrentSavefileString
        {
            get => AppSettings.GetValueOrDefault("CurrentSavefileString", null);
            set => AppSettings.AddOrUpdateValue("CurrentSavefileString", value);
        }

        /// <summary>
        /// AbyssalSavefilePath
        /// </summary>
        /// <returns>null if there is no current save file string</returns>
        public static string AbyssalSavefilePath
        {
            get => AppSettings.GetValueOrDefault("AbyssalSavefilePath", null);
            set => AppSettings.AddOrUpdateValue("AbyssalSavefilePath", value);
        }

        /// <summary>
        /// Path to local copy of the Tap Titans Abyss 2 savefile.
        /// <para>Default is Xamarin.Forms.DependencyService.Get<IDBPath>().DBPath("myChallengeSave.adat")</para>
        /// </summary>
        public static string TT2TempAbyssSavefilePath
        {
            get => AppSettings.GetValueOrDefault("TT2TempAbyssSavefilePath", null);
            set => AppSettings.AddOrUpdateValue("TT2TempAbyssSavefilePath", value);
        }

        /// <summary>
        /// Is default savefile selected? (true = yes | no = abyssal
        /// </summary>
        /// <returns>null if there is no current save file string</returns>
        public static bool IsDefaultSavefileSelected
        {
            get => AppSettings.GetValueOrDefault("IsDefaultSavefileSelected", true);
            set => AppSettings.AddOrUpdateValue("IsDefaultSavefileSelected", value);
        }

        /// <summary>
        /// App Tracking Transparency setting
        /// default is 0 (not determined)
        /// </summary>
        public static int ATTUserChoice 
        {
            get => AppSettings.GetValueOrDefault("ATTUserChoice", 0);
            set => AppSettings.AddOrUpdateValue("ATTUserChoice", value);
        }

        /// <summary>
        /// Is a player in tournament?
        /// ONLY FOR CLIPBOARD USERS!!!
        /// </summary>
        public static bool IsPlayerInTournament
        {
            get => AppSettings.GetValueOrDefault("IsPlayerInTournament", false);
            set => AppSettings.AddOrUpdateValue("IsPlayerInTournament", value);
        }

        /// <summary>
        /// LastRaidSeedString
        /// </summary>
        public static string LastRaidSeedString
        {
            get => AppSettings.GetValueOrDefault("LastRaidSeedString", null);
            set => AppSettings.AddOrUpdateValue("LastRaidSeedString", value);
        }

        #region Identity
        public static string AccessToken
        {
            get => AppSettings.GetValueOrDefault("AccessToken", null);
            set => AppSettings.AddOrUpdateValue("AccessToken", value);
        }

        public static DateTime AccessTokenExpiration
        {
            get => AppSettings.GetValueOrDefault("AccessTokenExpiration", DateTime.UtcNow);
            set => AppSettings.AddOrUpdateValue("AccessTokenExpiration", value);
        }

        public static string IdentityToken
        {
            get => AppSettings.GetValueOrDefault("IdentityToken", null);
            set => AppSettings.AddOrUpdateValue("IdentityToken", value);
        }

        public static bool IsUploadingSnapshotsEnabled
        {
            get => AppSettings.GetValueOrDefault("IsUploadingSnapshotsEnabled", false);
            set => AppSettings.AddOrUpdateValue("IsUploadingSnapshotsEnabled", value);
        }

        public static string IdentityUsername
        {
            get => AppSettings.GetValueOrDefault("IdentityUsername", null);
            set => AppSettings.AddOrUpdateValue("IdentityUsername", value);
        }

        #endregion
    }
}