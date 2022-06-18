namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Equipment reduction for efficiency calculation
    /// </summary>
    public class EquipmentReduction
    {
        /// <summary>
        /// Boost Identifier
        /// </summary>
        public string BoostId { get; set; }
        /// <summary>
        /// Reduction for ship based damage
        /// </summary>
        public double ShipReduction { get; set; }
        /// <summary>
        /// Reduction for tap based damage
        /// </summary>
        public double TapReduction { get; set; }
        /// <summary>
        /// Reduction for pet based damage
        /// </summary>
        public double PetReduction { get; set; }
        /// <summary>
        /// Reduction for shadow clone based damage
        /// </summary>
        public double ShadowCloneReduction { get; set; }
        /// <summary>
        /// Reduction for heavenly strike based damage
        /// </summary>
        public double HeavenlyStrikeReduction { get; set; }
        /// <summary>
        /// Reduction for chesterson gold
        /// </summary>
        public double ChestersonReduction { get; set; }
        /// <summary>
        /// Reduction for fairy gold
        /// </summary>
        public double FairyReduction { get; set; }
        /// <summary>
        /// Reduction for all gold
        /// </summary>
        public double AllGoldReduction { get; set; }
        /// <summary>
        /// Reduction for pet heart of midas gold
        /// </summary>
        public double PHoMReduction { get; set; }
        /// <summary>
        /// Reduction for boss gold
        /// </summary>
        public double BossGoldReduction { get; set; }

        /// <summary>
        /// String for Logfile
        /// </summary>
        /// <returns></returns>
        public string GetInfoFileString()
        {
            string tmp = "";

            tmp += $"- BoostId: {BoostId}";
            tmp += $"\t- ShipReduction: {ShipReduction}";
            tmp += $"\t- TapReduction: {TapReduction}";
            tmp += $"\t- PetReduction: {PetReduction}";
            tmp += $"\t- ShadowCloneReduction: {ShadowCloneReduction}";
            tmp += $"\t- HeavenlyStrikeReduction: {HeavenlyStrikeReduction}";
            tmp += $"\t- ChestersonReduction: {ChestersonReduction}";
            tmp += $"\t- FairyReduction: {FairyReduction}";
            tmp += $"\t- AllGoldReduction: {AllGoldReduction}";
            tmp += $"\t- PHoMReduction: {PHoMReduction}";
            tmp += $"\t- BossGoldReduction: {BossGoldReduction}";

            return tmp;
        }
    }
}
