using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Droid.Loggers;
using TT2Master.Helpers;

namespace TT2Master.Droid.Automation
{
    /// <summary>
    /// Exports snapshots
    /// </summary>
    public class SnapshotExport
    {
        /// <summary>
        /// Creates a snapshot and then exports it
        /// </summary>
        /// <returns>True if success</returns>
        public async Task<bool> ExportSnapshotAsync()
        {
            // Work is happening asynchronously
            try
            {
                //check if export is wished 
                if (!LocalSettingsORM.IsClanAutoExport)
                {
                    return true;
                }

                ClanExportLogger.DeleteLogFile();
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): START");

                // create dbRepo
                var h = new DBPathHelper();
                var dBRepo = new DBRepository(h.DBPath("tt2master.db3"));

                // read save file and get clan
                var save = new SaveFile(dBRepo);
                SaveFile.OnError += SaveFile_OnError;
                SaveFile.OnLogMePlease += SaveFile_OnLogMePlease;
                SaveFile.OnProgressMade += SaveFile_OnLogMePlease;

                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): Initializing savefile start");
                bool isSaveOkay = await save.Initialize(loadPlayer: true, loadAccountModel: true, loadClan: true);
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): Initializing savefile done");

                SaveFile.OnError -= SaveFile_OnError;
                SaveFile.OnLogMePlease -= SaveFile_OnLogMePlease;
                SaveFile.OnProgressMade -= SaveFile_OnLogMePlease;

                #region create snapshot
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): Creating snapshot");
                bool isCreated = await CreateSnapshot(dBRepo, save.ThisClan);
                #endregion

                #region get latest snapshot
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): getTodaysSnapshotAsync()");

                var lst = await dBRepo.GetTodaysSnapshotAsync();

                if (lst == null || lst.Count == 0)
                {
                    ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync() ERROR: no snapshots available for today. cancelling");
                    return false;
                }

                var snapshot = lst.Aggregate((i1, i2) => i1.Timestamp > i2.Timestamp ? i1 : i2);

                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): snapshot winner is {snapshot.ID}"); 
                #endregion

                // receive details for snapshot
                snapshot.MemberSnapshotItems = await dBRepo.GetAllMemberSnapshotItemAsync(snapshot.ID);

                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): received {snapshot.MemberSnapshotItems.Count} member snapshot items");

                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): Getting export properties");

                #region export snapshot
                // get export setting
                var settings = await GetExportProps(dBRepo);

                // close db
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): closing DB");
                _ = await dBRepo.Close();

                // prepare export string
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): getting clan csv string");
                string strToExport = GetClanCSVString(snapshot, settings);

                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): exporting data");

                // export
                string fileDate = snapshot.Timestamp.ToString("yyyyMMdd");
                string name = $"{fileDate}_clanmember.csv";

                var dirHelper = new DirectoryHelper();
                string filePath = dirHelper.GetDownloadPathName(name);
                string dir = dirHelper.GetDownloadPath();

                FileHelper.DeleteFile(dir, filePath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // write
                using (var sw = new StreamWriter(filePath, false, Encoding.Default))
                {
                    sw.Write(strToExport);
                    sw.Close();
                }

                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): finished exporting data");

                #endregion

                #region Notification
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): creating notification if wished");
                if (LocalSettingsORM.IsClanAutoExportNotificationEnabled)
                {
                    ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): notify user");
                    var notification = new NotificationRequest
                    {
                        NotificationId = 100,
                        Title = "Clan Export",
                        Description = "Data exported",
                    };
                    
                    NotificationCenter.Current.Show(notification);
                    //_ = await NotificationCenter.Current.Show(notification);


                    //LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.ic_stat_notifications_active;
                    //CrossLocalNotifications.Current.Show("Clan Export", "data exported", 102, DateTime.Now);
                }
                else
                {
                    ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync(): notification not wished");
                }
                #endregion
            }
            catch (Exception ex)
            {
                ClanExportLogger.WriteToLogFile($"SnapshotExport.ExportSnapshotAsync() ERROR: {ex.Message}\n{ex.Data}");
                return false;
            }

            return true;
        }

        #region Helper
        private async Task<List<CsvExportProperty>> GetExportProps(DBRepository repository)
        {
            var allProps = ExportProperyLists.GetClanMemberProperties();

            var savedProps = await repository.GetAllCsvExportPropertiesAsync("ClanMember");

            if (savedProps == null)
            {
                return allProps;
            }
            if (savedProps.Count == 0)
            {
                return allProps;
            }

            // set stored values
            foreach (var item in allProps)
            {
                item.ExportReference = "ClanMember";
                // get saved props setting
                var savedItem = savedProps.Where(x => x.ID == item.ID).FirstOrDefault();

                if (savedItem == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(savedItem.Identifier))
                {
                    continue;
                }

                item.IsExportWished = savedItem.IsExportWished;
                item.SortId = savedItem.SortId;
            }

            return allProps.OrderBy(x => x.SortId).ToList();
        }

        private string GetHeaderLine(List<CsvExportProperty> exportProperties)
        {
            string line = "";
            string del = LocalSettingsORM.CsvDelimiter;

            var expList = exportProperties.Where(x => x.IsExportWished).ToList();
            for (int i = 0; i < expList.Count; i++)
            {
                if (expList[i].IsExportWished)
                {
                    line += expList[i].DisplayName + (i + 1 == expList.Count ? "" : del);
                }
            }

            line += "\n";

            return line;
        }

        private List<ExportClanMember> GetExportList(Snapshot snapshot)
        {
            var exp = new List<ExportClanMember>();

            // populate from snapshot
            foreach (var item in snapshot.MemberSnapshotItems)
            {
                exp.Add(new ExportClanMember()
                {
                    ID = item.PlayerId,
                    Name = item.Name,
                    StageMax = item.StageMax,
                    WeeklyTicketCount = item.WeeklyTicketCount,
                    RaidAttackCount = item.RaidAttackCount,
                    ArtifactCount = item.ArtifactCount,
                    RaidTicketsCollected = item.RaidTicketsCollected,
                    RaidTotalXP = item.RaidTotalXP,
                    TournamentCount = item.TournamentCount,
                    CraftingShardsSpent = item.CraftingShardsSpent,
                    TotalSkillPoints = item.TotalSkillPoints,
                    TotalPetLevels = item.TotalPetLevels,
                    TotalHelperWeapons = item.TotalHelperWeapons,
                    TotalHelperScrolls = item.TotalHelperScrolls,
                    EquipmentSetCount = item.EquipmentSetCount,
                    RaidUniqueSkillCount = item.RaidUniqueSkillCount,
                    RaidBaseDamage = item.RaidBaseDamage,
                    RaidPlayerLevel = item.RaidPlayerLevel,
                    RaidTotalCardLevel = item.RaidTotalCardLevel,
                    TitanPoints = item.TitanPoints,
                    ClanRole = item.ClanRole,
                    LastTimestamp = item.LastTimestamp,
                    SaveDate = $"{ snapshot.Timestamp:yyyy.MM.dd}",
                });
            }


            return exp;
        }

        /// <summary>
        /// Little export of clan member information
        /// </summary>
        private string GetClanCSVString(Snapshot snapshot, List<CsvExportProperty> exportProperties)
        {
            try
            {
                var exp = GetExportList(snapshot);

                var mitVals = new List<List<CsvExportProperty>>();
                var props = typeof(ExportClanMember).GetProperties();
                string del = LocalSettingsORM.CsvDelimiter;
                var expList = exportProperties.Where(x => x.IsExportWished).ToList();


                foreach (var item in exp)
                {
                    var tmp = new List<CsvExportProperty>();

                    foreach (var expProp in expList)
                    {
                        tmp.Add(
                            new CsvExportProperty()
                            {
                                DisplayName = expProp.DisplayName,
                                IsExportWished = expProp.IsExportWished,
                                ExportReference = expProp.ExportReference,
                                Identifier = expProp.Identifier,
                                SortId = expProp.SortId,
                                PrintValue = props.Where(x => x.Name == expProp.ID).FirstOrDefault().GetValue(item) ?? "",
                            });
                    }

                    mitVals.Add(tmp);
                }

                // build export string
                string expStr = GetHeaderLine(exportProperties);

                foreach (var item in mitVals)
                {
                    string line = "";

                    for (int i = 0; i < item.Count; i++)
                    {
                        line += item[i].PrintValue + (i + 1 == item.Count ? "" : del);
                    }
                    expStr += line + "\n";
                }

                return expStr;
            }
            catch (Exception e)
            {
                ClanExportLogger.WriteToLogFile($"SnapshotExport.GetClanCSVString() error: {e.Message}");
                return "????";
            }
        }

        private async Task<bool> CreateSnapshot(DBRepository repository, Clan clan)
        {
            if (repository == null)
            {
                return false;
            }

            try
            {
                ClanExportLogger.WriteToLogFile("SnapshotExport.CreateSnapshot(): checking snapshots");
                SnapshotFactory.OnLogMePlease += SnapshotFactory_OnLogMePlease;
                bool result = await SnapshotFactory.CreateDailySnapshotAsync(repository, clan);
                SnapshotFactory.OnLogMePlease -= SnapshotFactory_OnLogMePlease;
                ClanExportLogger.WriteToLogFile("SnapshotExport.CreateSnapshot(): done checking snapshots");

                return result;
            }
            catch (Exception ex)
            {
                ClanExportLogger.WriteToLogFile($"SnapshotExport.CreateSnapshot() ERROR: {ex.Message}\n{ex.Data}");
                return false;
            }
        }
        #endregion

        #region E + D
        private void SaveFile_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => ClanExportLogger.WriteToLogFile($"SnapshotExport.SaveFile_OnLogMePlease: {e.Information}");
        private void SaveFile_OnError(object sender, Helpers.CustErrorEventArgs e) => ClanExportLogger.WriteToLogFile($"SnapshotExport.SaveFile_OnError: {e.MyException.Message}");
        private void SnapshotFactory_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => ClanExportLogger.WriteToLogFile($"SnapshotExport.SnapshotFactory_OnLogMePlease: {e.Information}");
        #endregion
    }
}