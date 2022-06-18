namespace TT2Master
{
    /// <summary>
    /// Model for .csv-Export of Clan Members
    /// Deliminator is a comma
    /// </summary>
    public class ExportClanMember
    {
        #region Props
        /// <summary>
        /// delimiter
        /// </summary>
        private static readonly string _del = LocalSettingsORM.CsvDelimiter;

        /// <summary>
        /// Identifier
        /// </summary>
        public string ID { get; set; } = "";
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// MS
        /// </summary>
        public double StageMax { get; set; } = 0;
        /// <summary>
        /// Amount of arts
        /// </summary>
        public int ArtifactCount { get; set; } = 0;
        /// <summary>
        /// Amount of tournaments
        /// </summary>
        public int TournamentCount { get; set; } = 0;
        /// <summary>
        /// Amount of titan points
        /// </summary>
        public double TitanPoints { get; set; } = 0;
        /// <summary>
        /// Role in clan
        /// </summary>
        public string ClanRole { get; set; } = "";
        /// <summary>
        /// Last seen
        /// </summary>
        public string LastTimestamp { get; set; } = "?";


        #region Raid
        /// <summary>
        /// total raid exp
        /// </summary>
        public double RaidTotalXP { get; set; } = 0;

        /// <summary>
        /// Total raid attacks
        /// </summary>
        public int RaidAttackCount { get; set; } = 0;

        /// <summary>
        /// tickets collected this week
        /// </summary>
        public int WeeklyTicketCount { get; set; } = 0;

        /// <summary>
        /// total tickets collected
        /// </summary>
        public int RaidTicketsCollected { get; set; } = 0;

        /// <summary>
        /// total raid card level
        /// </summary>
        public int RaidTotalCardLevel { get; set; } = 0;

        /// <summary>
        /// I have no idea
        /// </summary>
        public int RaidUniqueSkillCount { get; set; } = 0;

        /// <summary>
        /// raid base damage
        /// </summary>
        public int RaidBaseDamage { get; set; } = 0;

        /// <summary>
        /// raid level
        /// </summary>
        public int RaidPlayerLevel { get; set; } = 0;
        #endregion

        #region 3.0 new Props
        /// <summary>
        /// completed sets
        /// </summary>
        public int EquipmentSetCount { get; set; } = 0;
        /// <summary>
        /// CP
        /// </summary>
        public int CraftingShardsSpent { get; set; } = 0;
        /// <summary>
        /// Pets
        /// </summary>
        public int TotalPetLevels { get; set; } = 0;
        /// <summary>
        /// SP
        /// </summary>
        public int TotalSkillPoints { get; set; } = 0;
        /// <summary>
        /// Weapons
        /// </summary>
        public int TotalHelperWeapons { get; set; } = 0;
        /// <summary>
        /// Scrolls
        /// </summary>
        public int TotalHelperScrolls { get; set; } = 0;

        public string SaveDate { get; set; } = "";
        #endregion

        public int UndisputedWins { get; set; }

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportClanMember()
        {

        }

        /// <summary>
        /// Returns csv header line
        /// </summary>
        /// <returns></returns>
        public static string GetHeaderLine() => $"ID{_del}Name{_del}MS{_del}Weekly Morale Count{_del}Total Morale count{_del}Raid attack count{_del}Total Raid XP{_del}Artifacts{_del}Tournaments{_del}TP{_del}Role{_del}Last Time Online\n";

        /// <summary>
        /// Converts properties into a string variable to directly write into a csv-row
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{ID}{_del}{Name.Replace(_del, "")}{_del}{StageMax}{_del}{_del}{WeeklyTicketCount}{_del}{RaidTicketsCollected}{_del}{RaidAttackCount}{RaidTotalXP}{_del}{_del}{ArtifactCount}{_del}{TournamentCount}{_del}{TitanPoints}{_del}{ClanRole}{_del}{LastTimestamp}\n";
    }
}
