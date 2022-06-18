using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a row of "RaidLevelInfo.csv"
    /// </summary>
    public class RaidLevelInfo
    {
        public int TierID { get; set; }
        public int LevelID { get; set; }
        public string AreaID { get; set; }
        public string EnemyIDs { get; set; }
        public double TicketCost { get; set; }
        public double TicketClanReward { get; set; }
        public double XPClanReward { get; set; }
        public double XPPlayerReward { get; set; }
        public double DustPlayerReward { get; set; }
        public double CardPlayerReward { get; set; }
        public double ScrollPlayerReward { get; set; }
        public double FortuneScrollPlayerReward { get; set; }
        public double HolidayCurrencyPerAttack { get; set; }
        public double AttacksPerReset { get; set; }
        public double BaseHP { get; set; }
        public double TitanCount { get; set; }
        public bool HasArmor { get; set; }
        public double AreaBonuses { get; set; }
        public double EnemyBonuses { get; set; }
        public double TotalHP { get; set; }
        public double HPRatio { get; set; }
        //public double HPRatioV36 { get; set; }
        public double ClanXPCost { get; set; }
    }
}
