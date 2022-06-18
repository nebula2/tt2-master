using System;
using System.Collections.Generic;
using SQLite;
using System.Text;

namespace TT2Master.Model.Raid
{
    /// <summary>
    /// Describes a Clan Raid result which can be exported from Tap Titans to clipboard
    /// this class extends the possibility to be stored in db and references <see cref="ClanRaid"/> as a parent
    /// </summary>
    [Table("CLANRAIDRESULT")]
    public class ClanRaidResult
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// ID of <see cref="ClanRaid"/>
        /// </summary>
        public int ParentId { get; set; }

        #region CSV Properties
        public string PlayerName { get; set; }
        public string PlayerCode { get; set; }
        public int TotalRaidAttacks { get; set; }
        public int TitanNumber { get; set; }
        public string TitanName { get; set; }
        public double TitanDamage { get; set; }

        public double ArmorHead { get; set; }
        public double ArmorTorso { get; set; }
        public double ArmorLeftArm { get; set; }
        public double ArmorRightArm { get; set; }
        public double ArmorLeftHand { get; set; }
        public double ArmorRightHand { get; set; }
        public double ArmorLeftLeg { get; set; }
        public double ArmorRightLeg { get; set; }

        public double BodyHead { get; set; }
        public double BodyTorso { get; set; }
        public double BodyLeftArm { get; set; }
        public double BodyRightArm { get; set; }
        public double BodyLeftHand { get; set; }
        public double BodyRightHand { get; set; }
        public double BodyLeftLeg { get; set; }
        public double BodyRightLeg { get; set; }

        public double SkeletonHead { get; set; }
        public double SkeletonTorso { get; set; }
        public double SkeletonLeftArm { get; set; }
        public double SkeletonRightArm { get; set; }
        public double SkeletonLeftHand { get; set; }
        public double SkeletonRightHand { get; set; }
        public double SkeletonLeftLeg { get; set; }
        public double SkeletonRightLeg { get; set; }
        #endregion

        #region Analysis properties
        [Ignore]
        public double OverkillAmount { get; set; }

        [Ignore]
        public double OverkillPercentage { get; set; }

        [Ignore]
        public bool IsOneOfWorstOverkills { get; set; }

        [Ignore]
        public bool IsOneOfWorstParticipents { get; set; }

        [Ignore]
        public bool IsBelowMinAverageDamage { get; set; }

        [Ignore]
        public double AverageDamage { get; set; }

        [Ignore]
        public double OverkillHead { get; set; }

        [Ignore]
        public double OverkillTorso { get; set; }

        [Ignore]
        public double OverkillLeftArm { get; set; }

        [Ignore]
        public double OverkillRightArm { get; set; }

        [Ignore]
        public double OverkillLeftHand { get; set; }

        [Ignore]
        public double OverkillRightHand { get; set; }

        [Ignore]
        public double OverkillLeftLeg { get; set; }

        [Ignore]
        public double OverkillRightLeg { get; set; } 
        #endregion
    }
}
