using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Resources;
using TT2Master.Shared.Helper;

namespace TT2Master.Model.Dashboard
{
    public static class DashboardShortcutHandler
    {
        private static bool _isAvailableShortcutsInitialized = false;

        private static List<AvailableShortcut> GetShortcuts()
        {
            var shortcutTypes =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(ShortcutAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<ShortcutAttribute>() };

            var result = new List<AvailableShortcut>();

            foreach (var item in shortcutTypes)
            {
                var att = item.Attributes.First();

                //do not add shortcuts that are not available
                var isDirectSfLoad = LocalSettingsORM.IsReadingDataFromSavefile;
                if (!isDirectSfLoad && !att.IsAvailableWithNewExport)
                {
                    continue;
                }

                result.Add(new AvailableShortcut
                {
                    ShortcutId = att.ShortcutId,
                    LocalizationKey = att.LocalizationKey,
                    Name = att.LocalizationKey.TranslatedString(),
                    ShortcutType = item.Type,
                });
            }

            _isAvailableShortcutsInitialized = true;

            return result;
        }

        public static List<AvailableShortcut> AvailableShortcuts { get; set; } = new List<AvailableShortcut>();

        public static List<DashboardShortcutConfig> ShortcutConfig { get; set; } = new List<DashboardShortcutConfig>();

        private static string GetShortcutConfig()
        {
            if (LocalSettingsORM.IsReadingDataFromSavefile)
            {
                return LocalSettingsORM.CustomShortcutConfig ?? LocalSettingsORM.DefaultShortcutConfig;
            }

            return LocalSettingsORM.DefaultExportDataSourceShortcutConfig;
        }

        public static bool LoadShortcutConfig()
        {
            try
            {
                if (!_isAvailableShortcutsInitialized)
                {
                    AvailableShortcuts = GetShortcuts();
                }

                ShortcutConfig = new List<DashboardShortcutConfig>();
                var s = GetShortcutConfig();

                var configs = s.Split(';');

                foreach (var item in configs)
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }

                    var row = item.Split(',');

                    if(row.Length != 2)
                    {
                        Logger.WriteToLogFile($"ERROR: DashboardShortcutHandler.LoadShortcutConfig(): comma split got != 2 entries {item}");
                    }

                    int sort = JfTypeConverter.ForceInt(row[0]);
                    int id = JfTypeConverter.ForceInt(row[1]);
                    ShortcutConfig.Add(new DashboardShortcutConfig(sort, id, AvailableShortcuts.Where(x => x.ShortcutId == id).First().Name));
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: DashboardShortcutHandler.LoadShortcutConfig(): {ex.Message}");
                return false;
            }
        }

        public static bool RestoreDefault()
        {
            try
            {
                LocalSettingsORM.CustomShortcutConfig = LocalSettingsORM.DefaultShortcutConfig;
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: DashboardShortcutHandler.RestoreDefault(): {ex.Message}");
                return false;
            }
        }

        public static bool Saveconfig(List<DashboardShortcutConfig> config)
        {
            try
            {
                string s = "";

                foreach (var item in config)
                {
                    // rewrite id from name
                    item.ShortcutId = AvailableShortcuts.Where(x => x.Name == item.Name).First().ShortcutId;

                    s += $"{item.SortId},{item.ShortcutId};";
                }

                LocalSettingsORM.CustomShortcutConfig = s;

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: DashboardShortcutHandler: {ex.Message}");
                return false;
            }
        }
    }
}
