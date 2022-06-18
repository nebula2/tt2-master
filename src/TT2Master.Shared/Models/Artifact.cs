using System;
using TT2Master.Shared.Helper;

namespace TT2Master.Shared.Models
{
    public class Artifact
    {
        #region Properties
        /// <summary>
        /// The ID Tap Titans uses
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The numeric ID
        /// </summary>
        public int NumericID => GetNumericID();

        /// <summary>
        /// Integer for Sorting. The lesser this value, the higher up this Artifact should be shown
        /// </summary>
        public int SortIndex { get; set; }

        public string InternalName { get; set; }

        /// <summary>
        /// The general Tier this Artifact has
        /// <para/> goes from E, D, C, B, A up to S (the best)
        /// </summary>
        public int GeneralTier { get; set; }

        /// <summary>
        /// Current Level of Artifact
        /// </summary>
        public double Level { get; set; }

        /// <summary>
        /// Lifetime spent Relics on this
        /// </summary>
        public double RelicsSpent { get; set; }

        /// <summary>
        /// Name for User
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description for User
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Maximum Level this Artifact can have
        /// </summary>
        public double MaxLevel { get; set; }

        /// <summary>
        /// The Effect this Artifact has - in short
        /// </summary>
        public string Effect { get; set; }

        /// <summary>
        /// The gain on effect per Level
        /// </summary>
        public double EffectPerLevel { get; set; }

        public double GrowthExpo { get; set; }

        public double DamageBonus { get; set; }

        public double CostCoefficient { get; set; }

        public double CostExpo { get; set; }

        public string ImagePath => GetImagePath();

        public int DiscoveryPool { get; set; }
        public int EnchantmentPool { get; set; }
        public double EnchantmentMagnitude { get; set; }
        public double EnchantmentMult { get; set; }
        public string BonusIcon { get; set; }

        public double EnchantmentLevel { get; set; }
        #endregion

        private string GetImagePath(bool onlyId = false)
        {
            return onlyId
                ? NumericID > 99 ? @"notfound" : ID
                : NumericID > 99 ? @"notfound" : $"{ID}";
        }

        /// <summary>
        /// cuts out the number from ID and returnes its value as numeric
        /// </summary>
        /// <returns></returns>
        private int GetNumericID()
        {
            string tmp = ID.ToUpper();
            int start = 8;
            int end = ID.Length - start;
            string dafuq = tmp.Substring(start, end);
            int result = JfTypeConverter.ForceInt(dafuq);

            return result;
        }

        #region ctor
        public Artifact() { }
        #endregion

        #region public methods

        public string GetImageSourceId() => GetImagePath(true);

        public string GetInfoFileString()
        {
            string tmp = "";

            tmp += $"- ID: {ID}";
            tmp += $"\t- MaxLevel: {MaxLevel}";
            tmp += $"\t- Effect: {Effect}";
            tmp += $"\t- EffectPerLevel: {EffectPerLevel}";
            tmp += $"\t- GrowthExpo: {GrowthExpo}";
            tmp += $"\t- DamageBonus: {DamageBonus}";
            tmp += $"\t- CostCoefficient: {CostCoefficient}";
            tmp += $"\t- CostExpo: {CostExpo}";
            tmp += $"\t- Description: {Description}";
            tmp += $"\t- SortIndex: {SortIndex}";

            return tmp;
        }

        /// <summary>
        /// Get Cost for Upgrade
        /// </summary>
        /// <returns></returns>
        public double Cost() => CostCoefficient * Math.Pow(Level, CostExpo);

        /// <summary>
        /// Current Cost Sum
        /// </summary>
        /// <returns></returns>
        public double CurrentCostSum() => (CostCoefficient / (1 + CostExpo)) * Math.Pow(Level, 1 + CostExpo);

        public double ADBoost(double lvlUpAmount = 0) => DamageBonus * (lvlUpAmount > 0 ? lvlUpAmount : Level);

        /// <summary>
        /// ToString output for Artifact
        /// </summary>ArtifactHandler: setting default categories
        /// <returns></returns>
        public override string ToString() => $"{InternalName}: {Level}";

        public double CostAtLevel(double level)
        {
            double result = (CostCoefficient / (1 + CostExpo)) * Math.Pow(Level + level, 1 + CostExpo);

            return result;
        }

        /// <summary>
        /// How many level do I get for my money?
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public double LevelForMoney(double money)
        {
            if (money <= 0)
            {
                return 0;
            }

            double CostExpoIncreased = CostExpo + 1;

            return
                Math.Pow(
                    (money + CostCoefficient / CostExpoIncreased * Math.Pow(Level, CostExpoIncreased)) / (CostCoefficient / CostExpoIncreased)
                    , 1 / CostExpoIncreased
               ) - Level;
        }

        /// <summary>
        /// How many level do I get for my money?
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public double LevelForRelics(double relics)
        {
            return relics <= 0
                ? 0
                : Math.Min(Math.Floor(Math.Pow(Math.Pow(relics + CostCoefficient / (CostExpo + 1) * Level, (CostExpo + 1)) / (CostCoefficient / (CostExpo + 1)), (1 / (CostExpo + 1)))), MaxLevel);
        }

        /// <summary>
        /// Returnes the Cost of Relics to desired level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public double CostToLevel(double level) => CostAtLevel(level) - CurrentCostSum();
        #endregion
    }
}
