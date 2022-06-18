using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// Class for handling/ manipulating Snapshots
    /// </summary>
    public static class SnapshotFactory
    {
        /// <summary>
        /// Creates a snapshot and saves it to the database async
        /// </summary>
        /// <returns>bool indicating if something was saved?</returns>
        public static async Task<bool> CreateSnapshotAsync(bool selfCreated = false)
        {
            //get nex snapshot-ID
            int nextId = await App.DBRepo.GetNextSnapshotIdAsync();

            OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: nextId is {nextId}"));

            //create snapshot
            string clanToSave = "";
            if (string.IsNullOrWhiteSpace(App.Save.ThisPlayer?.ClanCurrent))
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: This Players clan is null or white space. storing clan-independent snapshot"));
            }
            else
            {
                clanToSave = App.Save.ThisPlayer?.ClanCurrent;
            }

            var snapshot = new Snapshot(nextId, SnapshotType.Member, DateTime.Now, clanToSave);

            if (selfCreated)
            {
                snapshot.SelfMade = true;
            }

            OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: create data for each member"));

            if (App.Save.ThisClan == null)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: clan is null. not saving"));
                return false;
            }

            if (App.Save.ThisClan.ClanMember == null)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: clanmember is null. initializing object"));
                App.Save.ThisClan.ClanMember = new List<Player>();
            }

            foreach (var item in App.Save.ThisClan.ClanMember)
            {
                try
                {
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: working on {item.PlayerName}"));

                    snapshot.MemberSnapshotItems.Add(new MemberSnapshotItem()
                    {
                        SnapshotId = nextId,
                        PlayerId = item.PlayerId,
                        StageMax = item.StageMax,
                        WeeklyTicketCount = item.WeeklyTicketCount,
                        RaidAttackCount = item.RaidAttackCount,
                        RaidTotalXP = item.RaidTotalXP,
                        RaidTicketsCollected = item.RaidTicketsCollected,
                        RaidTotalCardLevel = item.RaidTotalCardLevel,
                        RaidPlayerLevel = item.RaidPlayerLevel,
                        RaidBaseDamage = item.RaidBaseDamage,
                        RaidUniqueSkillCount = item.RaidUniqueSkillCount,
                        CraftingShardsSpent = item.CraftingShardsSpent,
                        EquipmentSetCount = item.EquipmentSetCount,
                        TotalHelperScrolls = item.TotalHelperScrolls,
                        TotalHelperWeapons = item.TotalHelperWeapons,
                        TotalPetLevels = item.TotalPetLevels,
                        TotalSkillPoints = item.TotalSkillPoints,
                        ClanRank = item.ClanRank,
                        LastTimestamp = item.LastTimestamp,
                        TitanPoints = item.TitanPoints,
                        TournamentCount = item.TournamentCount,
                        ArtifactCount = item.ArtifactCount,
                        Name = item.PlayerName,
                        PrestigeCount = item.PrestigeCount,
                        ClanRole = item.ClanRole,
                        UndisputedWins = item.UndisputedWins,
                    });
                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync Error: {ex.Message}"));
                }
            }

            OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: created snapshot"));

            //save items to database
            try
            {
                int snSaved = await App.DBRepo.AddNewSnapshotAsync(snapshot);
                int snmSaved = await App.DBRepo.AddNewMemberSnapshotItemListAsync(snapshot.MemberSnapshotItems);

                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: snSaved is {snSaved} and snmSaved is {snmSaved}"));

                return snSaved > 0 && snapshot.MemberSnapshotItems.Count > 0 ? snmSaved > 0 : true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: error saving snapshot: {ex.Message}"));
                return false;
            }
        }

        /// <summary>
        /// Creates a snapshot and saves it to the database async
        /// </summary>
        /// <returns>bool indicating if something was saved?</returns>
        public static async Task<bool> CreateSnapshotAsync(DBRepository repository, Clan clan, bool selfCreated = false)
        {
            if(clan == null)
            {
                return false;
            }
            
            //get nex snapshot-ID
            int nextId = await repository.GetNextSnapshotIdAsync();

            OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: nextId is {nextId}"));

            //create snapshot

            var snapshot = new Snapshot(nextId, SnapshotType.Member, DateTime.Now, clan.Name);

            if (selfCreated)
            {
                snapshot.SelfMade = true;
            }

            OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: create data for each member"));

            if (clan.ClanMember == null)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: clanmember is null. initializing object"));
                clan.ClanMember = new List<Player>();
            }

            foreach (var item in clan.ClanMember)
            {
                try
                {
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: working on {item.PlayerName}"));

                    snapshot.MemberSnapshotItems.Add(new MemberSnapshotItem()
                    {
                        SnapshotId = nextId,
                        PlayerId = item.PlayerId,
                        StageMax = item.StageMax,
                        WeeklyTicketCount = item.WeeklyTicketCount,
                        RaidAttackCount = item.RaidAttackCount,
                        RaidTotalXP = item.RaidTotalXP,
                        RaidTicketsCollected = item.RaidTicketsCollected,
                        RaidTotalCardLevel = item.RaidTotalCardLevel,
                        RaidPlayerLevel = item.RaidPlayerLevel,
                        RaidBaseDamage = item.RaidBaseDamage,
                        RaidUniqueSkillCount = item.RaidUniqueSkillCount,
                        CraftingShardsSpent = item.CraftingShardsSpent,
                        EquipmentSetCount = item.EquipmentSetCount,
                        TotalHelperScrolls = item.TotalHelperScrolls,
                        TotalHelperWeapons = item.TotalHelperWeapons,
                        TotalPetLevels = item.TotalPetLevels,
                        TotalSkillPoints = item.TotalSkillPoints,
                        ClanRank = item.ClanRank,
                        LastTimestamp = item.LastTimestamp,
                        TitanPoints = item.TitanPoints,
                        TournamentCount = item.TournamentCount,
                        ArtifactCount = item.ArtifactCount,
                        Name = item.PlayerName,
                        PrestigeCount = item.PrestigeCount,
                        ClanRole = item.ClanRole,
                        UndisputedWins = item.UndisputedWins,
                    });
                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync Error: {ex.Message}"));
                }
            }

            OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: created snapshot"));

            //save items to database
            try
            {
                int snSaved = await repository.AddNewSnapshotAsync(snapshot);
                int snmSaved = await repository.AddNewMemberSnapshotItemListAsync(snapshot.MemberSnapshotItems);

                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: snSaved is {snSaved} and snmSaved is {snmSaved}"));

                return snSaved > 0 && snapshot.MemberSnapshotItems.Count > 0 ? snmSaved > 0 : true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"SnapshotFactory.CreateSnapshotAsync: error saving snapshot: {ex.Message}"));
                return false;
            }
        }

        /// <summary>
        /// Deletes old <see cref="Snapshots"/> from DB
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteOldSnapshotsAsync()
        {
            try
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"DeleteOldSnapshotsAsync"));
                int maxAmount = JfTypeConverter.ForceInt(LocalSettingsORM.GetSnapshotAmount());

                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"DeleteOldSnapshotsAsync: maxAmount: {maxAmount}"));

                var items = await App.DBRepo.GetAllSnapshotAsync();

                if (items.Count <= maxAmount)
                {
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"DeleteOldSnapshotsAsync: {items.Count} items in DB. less than maxAmount. Done"));
                    return true;
                }

                var delList = items.OrderBy(x => x.ID).Take(items.Count - maxAmount).ToList();

                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"DeleteOldSnapshotsAsync: having {delList.Count} items to delete"));

                int del = await App.DBRepo.DeleteSnapshots(delList);

                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"DeleteOldSnapshotsAsync: deleted {del} items. Done"));

                return true;
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"DeleteOldSnapshotsAsync Error: {e.Message}"));
                return false;
            }
        }

        /// <summary>
        /// Creates a daily snapshot and deletes other snapshots that where taken this day
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CreateDailySnapshotAsync()
        {
            try
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs("CreateDailySnapshotAsync. Entry"));

                var todays = await App.DBRepo.GetTodaysSnapshotAsync();

                if (todays == null || todays.Count == 0)
                {
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"did not find any snapshot for today"));

                    return await CreateSnapshotAsync();
                }

                if (todays.Count > LocalSettingsORM.DailyAutoSnapshotThreshold)
                {
                    int del = await App.DBRepo.DeleteSnapshots(todays.Where(x => !x.SelfMade)
                        .OrderByDescending(x => x.Timestamp)
                        .Take(LocalSettingsORM.DailyAutoSnapshotThreshold).ToList());
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"found {del} snapshots for today. deleting some"));
                }

                return await CreateSnapshotAsync();
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"CreateDailySnapshot Error: {e.Message}"));

                return false;
            }
        }

        /// <summary>
        /// Creates a daily snapshot and deletes other snapshots that where taken this day
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CreateDailySnapshotAsync(DBRepository repository, Clan clan)
        {
            if(repository == null || clan == null)
            {
                return false;
            }

            try
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs("CreateDailySnapshotAsync. Entry"));

                var todays = await repository.GetTodaysSnapshotAsync();



                if (todays == null || todays.Count == 0)
                {
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"did not find any snapshot for today"));

                    return await CreateSnapshotAsync(repository, clan);
                }

                if (todays.Count > LocalSettingsORM.DailyAutoSnapshotThreshold)
                {
                    var del = await App.DBRepo.DeleteSnapshots(todays.Where(x => !x.SelfMade)
                        .OrderByDescending(x => x.Timestamp)
                        .Take(LocalSettingsORM.DailyAutoSnapshotThreshold).ToList());
                    OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"found {del} snapshots for today. deleting some"));
                }

                return await CreateSnapshotAsync(repository, clan);
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SnapshotFactory", new InformationEventArgs($"CreateDailySnapshot Error: {e.Message}"));

                return false;
            }
        }

        /// <summary>
        /// Deletes old clan data by checking the current clancode
        /// </summary>
        /// <returns></returns>
        public static async Task<int> DeleteOldClanDataAsync() => await App.DBRepo.DeleteOldClanData(App.Save.ThisPlayer.ClanCurrent);

        #region Private Methods

        #endregion

        #region E+D
        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when I think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;
        #endregion
    }
}
