using SQLite;

namespace TT2Master
{
    [Table("PLAYER")]
    public class Player
    {
        /// <summary>
        /// Private player ID for identification
        /// </summary>
        [PrimaryKey]
        public string PlayerId { get; set; }

        /// <summary>
        /// Hashed PlayerId for queries
        /// </summary>
        public string PlayerIdHash { get; set; }

        /// <summary>
        /// The ID for Ads
        /// </summary>
        public string AdId { get; set; } = "";

        /// <summary>
        /// Public player name
        /// </summary>
        [MaxLength(250)]
        public string PlayerName { get; set; }

        /// <summary>
        /// Current Stage
        /// </summary>
        public double CurrentStage { get; set; }

        /// <summary>
        /// Max reached Stage
        /// </summary>
        public double StageMax { get; set; }

        public int ClanRank { get; set; }

        /// <summary>
        /// Amount of collected Artifacts
        /// </summary>
        public int ArtifactCount { get; set; }

        public string ClanName { get; set; }

        /// <summary>
        /// Titan Points earned through Tourneys
        /// </summary>
        public double TitanPoints { get; set; }

        /// <summary>
        /// Current Clan (code)
        /// </summary>
        [MaxLength(10)]
        public string ClanCurrent { get; set; }

        /// <summary>
        /// Role in Clan
        /// </summary>
        [MaxLength(50)]
        public string ClanRole { get; set; }

        /// <summary>
        /// The amount of tournament parcipitation
        /// </summary>
        public int TournamentCount { get; set; }

        /// <summary>
        /// Highest Rank reached in a tournament
        /// </summary>
        public int TournamentMaxRank { get; set; }

        /// <summary>
        /// Country code 
        /// </summary>
        [MaxLength(10)]
        public string CountryCode { get; set; }

        /// <summary>
        /// The amount of crates shared
        /// </summary>
        public int ClanCratesShared { get; set; }

        /// <summary>
        /// The Token used to auth on http requests
        /// </summary>
        [Unique]
        public string AuthToken { get; set; }

        /// <summary>
        /// Is this me?
        /// </summary>
        public bool IsMe { get; set; }

        /// <summary>
        /// Time of last update
        /// </summary>
        public string LastTimestamp { get; set; }

        /// <summary>
        /// Is this player banned by you?
        /// </summary>
        [Ignore]
        public bool IsBanned { get; set; } = false;

        /// <summary>
        /// Amount of prestige
        /// </summary>
        public double PrestigeCount { get; set; }

        #region Raid
        /// <summary>
        /// Total raid experience
        /// </summary>
        public double RaidTotalXP { get; set; }

        /// <summary>
        /// raid attacks
        /// </summary>
        public int RaidAttackCount { get; set; }

        /// <summary>
        /// Tickets collected this week
        /// </summary>
        public int WeeklyTicketCount { get; set; }

        /// <summary>
        /// total raid tickets collected
        /// </summary>
        public int RaidTicketsCollected { get; set; }

        /// <summary>
        /// Raid cards level in total
        /// </summary>
        public int RaidTotalCardLevel { get; set; }

        /// <summary>
        /// I have no idea
        /// </summary>
        public int RaidUniqueSkillCount { get; set; }

        /// <summary>
        /// Raid base damage
        /// </summary>
        public int RaidBaseDamage { get; set; }

        /// <summary>
        /// Players raid level
        /// </summary>
        public int RaidPlayerLevel { get; set; }

        /// <summary>
        /// Amount of dust this player has spent
        /// </summary>
        public int DustSpent { get; set; }
        #endregion

        #region 3.0 new Props
        /// <summary>
        /// Amount of completed sets
        /// </summary>
        public int EquipmentSetCount { get; set; }
        /// <summary>
        /// Amount of spent shards 
        /// </summary>
        public int CraftingShardsSpent { get; set; }
        /// <summary>
        /// Amount of pets
        /// </summary>
        public int TotalPetLevels { get; set; }
        /// <summary>
        /// Amount of skill points
        /// </summary>
        public int TotalSkillPoints { get; set; }
        /// <summary>
        /// Amount of helper (hero) weapons
        /// </summary>
        public int TotalHelperWeapons { get; set; }
        /// <summary>
        /// Amount of helper (hero) scrolls
        /// </summary>
        public int TotalHelperScrolls { get; set; }
        #endregion

        /// <summary>
        /// Undisputed wins amount
        /// </summary>
        public int UndisputedWins { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Player()
        {

        }
    }
}