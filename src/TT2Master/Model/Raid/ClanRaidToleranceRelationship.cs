using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TT2Master.Model.Raid
{
    /// <summary>
    /// Relationship between <see cref="ClanRaid"/> and <see cref="RaidTolerance"/>
    /// </summary>
    [Table("CLANRAIDTOLERANCERELATIONSHIP")]
    public class ClanRaidToleranceRelationship
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int ClanRaidId { get; set; }

        public string RaidToleranceId { get; set; }
    }
}
