using System;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Equipment (see EquipmentInfo)
    /// </summary>
    public class Equipment
    {
        #region Properties
        #region Base
        /// <summary>
        /// Identifies object in EquipmentInfo
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        /// The index of this object in the Infofile
        /// </summary>
        public int SortingIndex { get; set; }

        /// <summary>
        /// The category this object belongs to
        /// </summary>
        public string EquipmentCategory { get; set; }
        /// <summary>
        /// Primary Bonus Type
        /// </summary>
        public string BonusType { get; set; }
        /// <summary>
        /// How rare is this? The greater this value the less is the chance to get it
        /// </summary>
        public int Rarity { get; set; }
        /// <summary>
        /// Start amount of Bonus
        /// </summary>
        public int AttributeBaseAmount { get; set; }
        /// <summary>
        /// Attribute Base Multiplier
        /// </summary>
        public double AttributeBaseMult { get; set; }
        /// <summary>
        /// Increasement per level
        /// </summary>
        public double AttributeBaseInc { get; set; }
        /// <summary>
        /// Some exponent
        /// </summary>
        public double AttributeExp1 { get; set; }
        /// <summary>
        /// Some exponent
        /// </summary>
        public double AttributeExp2 { get; set; }
        /// <summary>
        /// Exponent base
        /// </summary>
        public double AttributeExpBase { get; set; }
        /// <summary>
        /// Event or default? If none you cannot get it?
        /// </summary>
        [Obsolete("Removed with 5.9.0")]
        public string EquipmentSource { get; set; }
        /// <summary>
        /// If true, this object is only available during event
        /// </summary>
        public bool LimitedTime { get; set; }
        /// <summary>
        /// If not "None" this object belongs to a set
        /// </summary>
        public string EquipmentSet { get; set; }
        #endregion

        #region Owned

        /// <summary>
        /// Transmog-Identifier
        /// </summary>
        public string LookId { get; set; }
        /// <summary>
        /// Unique ID when owned
        /// </summary>
        public int UniqueId { get; set; }
        /// <summary>
        /// True if currently equipped
        /// </summary>
        public bool Equipped { get; set; }
        /// <summary>
        /// True if locked. If locked, you cannot sell this object
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// True if not visible. Never saw this being true. Maybe this is true while equip is getting loaded
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// The level of this object. Value after dot is determining the secondary bonuses
        /// </summary>
        public double Level { get; set; }
        /// <summary>
        /// True if equip is new
        /// </summary>
        public bool IsNew { get; set; }

        public double PrimaryBonusEff { get; set; }
        public double SecBonusEff1 { get; set; }
        public double SecBonusEff2 { get; set; }
        public double SecBonusEff3 { get; set; }

        /// <summary>
        /// Secondary Bonus 1
        /// </summary>
        public string SecondaryBonus1 { get; set; } = "";

        /// <summary>
        /// Secondary Bonus stat 1
        /// </summary>
        public double SecondaryBonusStat1 { get; set; }

        /// <summary>
        /// Secondary Bonus 2
        /// </summary>
        public string SecondaryBonus2 { get; set; } = "";

        /// <summary>
        /// Secondary Bonus stat 2
        /// </summary>
        public double SecondaryBonusStat2 { get; set; }

        /// <summary>
        /// Secondary Bonus 3
        /// </summary>
        public string SecondaryBonus3 { get; set; } = "";

        /// <summary>
        /// Secondary Bonus stat 3
        /// </summary>
        public double SecondaryBonusStat3 { get; set; }

        /// <summary>
        /// Overall, build dependent efficiency
        /// </summary>
        public double EfficiencyValue { get; set; }
        #endregion

        #region Visualization
        public string RarityColor { get; set; } = "";

        public string EquippedColor { get; set; } = "";

        public string LevelDisplay { get; set; } = "";

        public string EffDisplay { get; set; } = "";

        public string Name { get; set; } = "";

        public string BonusTypeDisplay { get; set; } = "";

        public string SecondaryBonus1Display { get; set; } = "";
        public string SecondaryBonus2Display { get; set; } = "";
        public string SecondaryBonus3Display { get; set; } = "";
        #endregion
        #endregion

        #region public methods
        /// <summary>
        /// String for Logfile
        /// </summary>
        /// <returns></returns>
        public string GetInfoFileString()
        {
            string tmp = "";

            tmp += $"- EquipmentId: {EquipmentId}";
            tmp += $"\t- SortingIndex: {SortingIndex}";
            tmp += $"\t- EquipmentCategory: {EquipmentCategory}";
            tmp += $"\t- BonusType: {BonusType}";
            tmp += $"\t- Rarity: {Rarity}";
            tmp += $"\t- AttributeBaseAmount: {AttributeBaseAmount}";
            tmp += $"\t- AttributeBaseInc: {AttributeBaseInc}";
            tmp += $"\t- AttributeExp1: {AttributeExp1}";
            tmp += $"\t- AttributeExp2: {AttributeExp2}";
            tmp += $"\t- AttributeExpBase: {AttributeExpBase}";
            //tmp += $"\t- EquipmentSource: {EquipmentSource}";
            tmp += $"\t- LimitedTime: {LimitedTime}";
            tmp += $"\t- EquipmentSet: {EquipmentSet}";

            return tmp;
        }

        /// <summary>
        /// String for Optimized
        /// </summary>
        /// <returns></returns>
        public string GetOptInfoFileString()
        {
            string tmp = "";

            tmp += $"- EquipmentId: {EquipmentId}";
            tmp += $"\t- SortingIndex: {SortingIndex}";
            tmp += $"\t- EquipmentCategory: {EquipmentCategory}\n";
            tmp += $"\t- BonusType: {BonusType}";
            tmp += $"\t- Rarity: {Rarity}";
            tmp += $"\t- AttributeBaseAmount: {AttributeBaseAmount}";
            tmp += $"\t- AttributeBaseInc: {AttributeBaseInc}";
            tmp += $"\t- AttributeExp1: {AttributeExp1}";
            tmp += $"\t- AttributeExp2: {AttributeExp2}";
            tmp += $"\t- AttributeExpBase: {AttributeExpBase}";
            //tmp += $"\t- EquipmentSource: {EquipmentSource}";
            tmp += $"\t- LimitedTime: {LimitedTime}";
            tmp += $"\t- EquipmentSet: {EquipmentSet}";

            tmp += "\n";

            tmp += $"- Level: {Level}";
            tmp += $"\t- Primary Bonus  : {PrimaryBonusEff}";
            tmp += $"\t- SecondaryBonus1: {SecondaryBonus1}";
            tmp += $"\t- SecondaryBonusStat1: {SecondaryBonusStat1}";
            tmp += $"\t- SecondaryBonus2: {SecondaryBonus2}";
            tmp += $"\t- SecondaryBonusStat2: {SecondaryBonusStat2}";
            tmp += $"\t- SecondaryBonus3: {SecondaryBonus3}";
            tmp += $"\t- SecondaryBonusStat3: {SecondaryBonusStat3}";
            tmp += $"\t- EfficiencyValue: {EfficiencyValue}";

            return tmp;
        }

        /// <summary>
        /// String for tests
        /// </summary>
        /// <returns></returns>
        public string GetTestInfoString()
        {
            string tmp = "";

            tmp += $"EquipmentId: {EquipmentId} \tLevel: {Level}\n";
            tmp += $"\t- BonusType: {BonusType} \tPrimaryBonusEff: {PrimaryBonusEff}\n";
            tmp += $"\t- SecondaryBonus1: {SecondaryBonus1} \tSecBonusEff1: {SecBonusEff1}\n";
            tmp += $"\t- SecondaryBonus2: {SecondaryBonus2} \tSecBonusEff2: {SecBonusEff2}\n";
            tmp += $"\t- SecondaryBonus3: {SecondaryBonus3} \tSecBonusEff3: {SecBonusEff3}\n";
            tmp += $"\t- EfficiencyValue: {EfficiencyValue}\n";

            return tmp;
        }

        /// <summary>
        /// Returns the build independent primary bonus efficiency
        /// </summary>
        /// <returns></returns>
        public double PrimaryBonusEfficiency()
        {
            double result;
            try
            {
                result = AttributeBaseAmount + AttributeBaseInc * (Math.Pow(Level, AttributeExp1) + Math.Pow(AttributeExpBase, Math.Pow(Level, AttributeExp2)));
                // AttributeBase + (PowerBase + Level * PowerInc) ^ PowerExp
            }
            catch (Exception)
            {
                result = 123456;
            }

            return result;
        }

        public void SetPurePrimaryEfficiencyOnly() => PrimaryBonusEff = PrimaryBonusEfficiency();
        #endregion

        #region Private methods
        public string GetRarityColor()
        {
            return Rarity switch
            {
                1 => LimitedTime
                    ? "#880f96"  // Event
                    : "#FFFFFF", // Normal
                2 => "#4286F4",
                3 => "#f3f93b",
                _ => "#f47f1f",
            };
        }

        public string GetEquippedColor()
        {
            return Equipped ? "#344860" : "#1A2634";
        }
        #endregion
    }
}
