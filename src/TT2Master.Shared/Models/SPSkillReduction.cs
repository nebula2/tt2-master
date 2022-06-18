namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes reductions for a skill
    /// </summary>
    public class SPSkillReduction
    {
        /// <summary>
        /// Talent identifier
        /// </summary>
        public string SkillId { get; set; }
        /// <summary>
        /// Reduction for Ship build
        /// </summary>
        public double ShipReduction { get; set; }

        /// <summary>
        /// Reduction for Pet build
        /// </summary>
        public double PetReduction { get; set; }

        /// <summary>
        /// Reduction for shadow clone build
        /// </summary>
        public double ShadowCloneReduction { get; set; }

        /// <summary>
        /// Reduction for HS build
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
        /// Reduction for pHoM gold
        /// </summary>
        public double PHoMReduction { get; set; }

        /// <summary>
        /// Reduction for Boss gold
        /// </summary>
        public double BossGoldReduction { get; set; }

        /// <summary>
        /// Is this talent relevant for damage output?
        /// </summary>
        public bool IsDmgRelevant { get; set; }

        /// <summary>
        /// Is this talent relevant for gold output?
        /// </summary>
        public bool IsGoldRelevant { get; set; }

        /// <summary>
        /// Is this talent relevant for online playstile?
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// Is this talent relevant for offline playstile?
        /// </summary>
        public bool IsOffline { get; set; }

        /// <summary>
        /// Is the secondary bonus of this talent relevant?
        /// </summary>
        public bool IsSecondaryRelevant { get; set; }

        /// <summary>
        /// String for Logfile
        /// </summary>
        /// <returns></returns>
        public string GetInfoFileString()
        {
            string tmp = "";

            tmp += $"- SkillId: {SkillId}";
            tmp += $"\t- ShipReduction: {ShipReduction}";
            tmp += $"\t- PetReduction: {PetReduction}";
            tmp += $"\t- ShadowCloneReduction: {ShadowCloneReduction}";
            tmp += $"\t- HeavenlyStrikeReduction: {HeavenlyStrikeReduction}";
            tmp += $"\t- ChestersonReduction: {ChestersonReduction}";
            tmp += $"\t- FairyReduction: {FairyReduction}";
            tmp += $"\t- AllGoldReduction: {AllGoldReduction}";
            tmp += $"\t- PHoMReduction: {PHoMReduction}";
            tmp += $"\t- BossGoldReduction: {BossGoldReduction}";
            tmp += $"\t- IsOnline: {IsOnline}";
            tmp += $"\t- IsOffline: {IsOffline}";
            tmp += $"\t- IsDmgRelevant: {IsDmgRelevant}";
            tmp += $"\t- IsGoldRelevant: {IsGoldRelevant}";
            tmp += $"\t- IsSecondaryRelevant: {IsSecondaryRelevant}";

            return tmp;
        }
    }
}
