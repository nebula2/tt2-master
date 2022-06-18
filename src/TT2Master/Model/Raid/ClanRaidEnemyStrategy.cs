using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TT2Master.Model.Raid
{
    /// <summary>
    /// Relationship between <see cref="ClanRaid"/> and <see cref="RaidStrategy"/>
    /// </summary>
    [Table("CLANRAIDENEMYSTRATEGY")]
    public class ClanRaidEnemyStrategy
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int ClanRaidId { get; set; }

        public string RaidStrategyId { get; set; }
    }
}
