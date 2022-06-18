using SQLite;
using System;
using System.Runtime.CompilerServices;

namespace TT2Master
{
    /// <summary>
    /// Describes a snapshot item for a clan member
    /// </summary>
    [Table("MEMBERSNAPSHOTITEM")]
    public class MemberSnapshotItem
    {
        #region Properties
        /// <summary>
        /// Identifier
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// Snapshot-ID
        /// </summary>
        private int _snapshotId;
        /// <summary>
        /// Identifier of parent
        /// </summary>
        public int SnapshotId
        {
            get => _snapshotId;
            set
            {
                _snapshotId = value;
                if (!string.IsNullOrWhiteSpace(PlayerId))
                {
                    SetId(value, PlayerId);
                }
            }
        }

        /// <summary>
        /// Player-ID
        /// </summary>
        private string _playerId;
        /// <summary>
        /// Players identifier
        /// </summary>
        public string PlayerId
        {
            get => _playerId;
            set
            {
                _playerId = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetId(SnapshotId, value);
                }
            }
        }

        /// <summary>
        /// MS reached
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

        #region Raid
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
        #endregion

        #region 3.0 new Props
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

        /// <summary>
        /// External ID in TT2WebMaster
        /// </summary>
        public Guid ExternalId { get; set; }
        #endregion
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public MemberSnapshotItem()
        {

        }
        #endregion

        #region Helper
        /// <summary>
        /// Sets the <see cref="Id"/> from snapshotid and playerid
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="player"></param>
        private void SetId(int snapshot, string player) => Id = $"{SnapshotId.ToString()},{PlayerId}";
        #endregion

        #region Public Functions
        public Player ConvertToPlayer()
        {
            return new Player
            {
                ArtifactCount = this.ArtifactCount,
                ClanRank = this.ClanRank,
                ClanRole = this.ClanRole,
                CraftingShardsSpent = this.CraftingShardsSpent,
                EquipmentSetCount = this.EquipmentSetCount,
                LastTimestamp = this.LastTimestamp,
                PlayerId = this.PlayerId,
                PlayerName = this.Name,
                PrestigeCount = this.PrestigeCount,
                RaidAttackCount = this.RaidAttackCount,
                RaidBaseDamage = this.RaidBaseDamage,
                RaidPlayerLevel = this.RaidPlayerLevel,
                RaidTicketsCollected = this.RaidTicketsCollected,
                RaidTotalCardLevel = this.RaidTotalCardLevel,
                RaidTotalXP = this.RaidTotalXP,
                RaidUniqueSkillCount = this.RaidUniqueSkillCount,
                StageMax = this.StageMax,
                TitanPoints = this.TitanPoints,
                TotalHelperScrolls = this.TotalHelperScrolls,
                TotalHelperWeapons = this.TotalHelperWeapons,
                TotalPetLevels = this.TotalPetLevels,
                TotalSkillPoints = this.TotalSkillPoints,
                TournamentCount = this.TournamentCount,
                UndisputedWins = this.UndisputedWins,
                WeeklyTicketCount = this.WeeklyTicketCount,
            };
        }
        #endregion
    }
}