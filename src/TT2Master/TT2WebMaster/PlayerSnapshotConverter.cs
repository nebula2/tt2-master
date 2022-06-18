using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.TT2WebMaster
{
    public static class PlayerSnapshotConverter
    {
        public static PlayerSnapshotDto GetPlayerSnapshotDto(MemberSnapshotItem memberSnapshot, DateTime createdDate)
        {
            return new PlayerSnapshotDto
            {
                CreatedDate = createdDate,
                PlayerId = memberSnapshot.PlayerId,
                Name = memberSnapshot.Name,
                ArtifactCount = memberSnapshot.ArtifactCount,
                ClanRank = memberSnapshot.ClanRank,
                ClanRole = memberSnapshot.ClanRole,
                CraftingShardsSpent = memberSnapshot.CraftingShardsSpent,
                EquipmentSetCount = memberSnapshot.EquipmentSetCount,
                LastTimestamp = memberSnapshot.LastTimestamp,
                PrestigeCount = memberSnapshot.PrestigeCount,
                RaidAttackCount = memberSnapshot.RaidAttackCount,
                RaidBaseDamage = memberSnapshot.RaidBaseDamage,
                RaidPlayerLevel = memberSnapshot.RaidPlayerLevel,
                RaidTicketsCollected = memberSnapshot.RaidTicketsCollected,
                RaidTotalCardLevel = memberSnapshot.RaidTotalCardLevel,
                RaidTotalXP = memberSnapshot.RaidTotalXP,
                RaidUniqueSkillCount = memberSnapshot.RaidUniqueSkillCount,
                StageMax = memberSnapshot.StageMax,
                TitanPoints = memberSnapshot.TitanPoints,
                TotalHelperScrolls = memberSnapshot.TotalHelperScrolls,
                TotalHelperWeapons = memberSnapshot.TotalHelperWeapons,
                TotalPetLevels = memberSnapshot.TotalPetLevels,
                TotalSkillPoints = memberSnapshot.TotalSkillPoints,
                TournamentCount = memberSnapshot.TournamentCount,
                UndisputedWins = memberSnapshot.UndisputedWins,
                WeeklyTicketCount = memberSnapshot.WeeklyTicketCount,
                LocalAppId = memberSnapshot.Id,
            };
        }
    }

    public class PlayerSnapshotDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// App user if existant/ found
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Players identifier
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// Max stage
        /// </summary>
        public double StageMax { get; set; }

        /// <summary>
        /// Amount of TP earned
        /// </summary>
        public double TitanPoints { get; set; }

        /// <summary>
        /// Amount of tournaments
        /// </summary>
        public int TournamentCount { get; set; }

        /// <summary>
        /// Amount of collected Artifacts
        /// </summary>
        public int ArtifactCount { get; set; }

        /// <summary>
        /// Playername
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Amount of prestiges this player has made
        /// </summary>
        public double PrestigeCount { get; set; }

        /// <summary>
        /// Rank in clan
        /// </summary>
        public int ClanRank { get; set; }

        /// <summary>
        /// Time of last update
        /// </summary>
        public string LastTimestamp { get; set; }

        /// <summary>
        /// Role in clan
        /// </summary>
        public string ClanRole { get; set; }

        /// <summary>
        /// Absolved tickets this week
        /// </summary>
        public int WeeklyTicketCount { get; set; }

        /// <summary>
        /// Amount of raid attacks
        /// </summary>
        public int RaidAttackCount { get; set; }

        /// <summary>
        /// Total raid xp
        /// </summary>
        public double RaidTotalXP { get; set; }

        /// <summary>
        /// Total raid tickets this player collected
        /// </summary>
        public int RaidTicketsCollected { get; set; }

        /// <summary>
        /// Total raid card level
        /// </summary>
        public int RaidTotalCardLevel { get; set; }

        /// <summary>
        /// I have no idea
        /// </summary>
        public int RaidUniqueSkillCount { get; set; }

        /// <summary>
        /// Base damage in raid
        /// </summary>
        public int RaidBaseDamage { get; set; }

        /// <summary>
        /// Raid level
        /// </summary>
        public int RaidPlayerLevel { get; set; }

        /// <summary>
        /// Amount of completed equipment sets
        /// </summary>
        public int EquipmentSetCount { get; set; }

        /// <summary>
        /// Amount of spent crafting shards
        /// </summary>
        public int CraftingShardsSpent { get; set; }

        /// <summary>
        /// Amount of pets this player owns
        /// </summary>
        public int TotalPetLevels { get; set; }

        /// <summary>
        /// Amount of skill points this player owns
        /// </summary>
        public int TotalSkillPoints { get; set; }

        /// <summary>
        /// Amount of helper (hero) weapons this player owns
        /// </summary>
        public int TotalHelperWeapons { get; set; }

        /// <summary>
        /// Amount of helper (hero) scrolls this player owns
        /// </summary>
        public int TotalHelperScrolls { get; set; }

        /// <summary>
        /// Amount of Tournaments where you placed first
        /// </summary>
        public int UndisputedWins { get; set; }

        public string LocalAppId { get; set; }
    }
}
