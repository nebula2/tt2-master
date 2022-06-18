namespace TT2Master.Shared.Models
{
    /// <summary>
    /// EquipmentEnhancementComboInfo item
    /// </summary>
    public class EquipEnhanceCombo
    {
        #region Properties
        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Rare 1
        /// </summary>
        public string Rare1 { get; set; }
        /// <summary>
        /// Legendary 1
        /// </summary>
        public string Legendary1 { get; set; }
        /// <summary>
        /// Legendary 2
        /// </summary>
        public string Legendary2 { get; set; }
        /// <summary>
        /// Mythic 1
        /// </summary>
        public string Mythic1 { get; set; }
        /// <summary>
        /// Mythic 2
        /// </summary>
        public string Mythic2 { get; set; }
        /// <summary>
        /// Mythic 3
        /// </summary>
        public string Mythic3 { get; set; }

        /// <summary>
        /// Rare 1
        /// </summary>
        public double R1 { get; set; }
        /// <summary>
        /// Rare 2
        /// </summary>
        public double R2 { get; set; }
        /// <summary>
        /// Rare 3
        /// </summary>
        public double R3 { get; set; }

        /// <summary>
        /// Legendary 1
        /// </summary>
        public double L1 { get; set; }
        /// <summary>
        /// Legendary 2
        /// </summary>
        public double L2 { get; set; }
        /// <summary>
        /// Legendary 3
        /// </summary>
        public double L3 { get; set; }

        /// <summary>
        /// Mythic 1
        /// </summary>
        public double M1 { get; set; }
        /// <summary>
        /// Mythic 2
        /// </summary>
        public double M2 { get; set; }
        /// <summary>
        /// Mythic 3
        /// </summary>
        public double M3 { get; set; }
        #endregion

        #region public Methods
        /// <summary>
        /// String for Logfile
        /// </summary>
        /// <returns></returns>
        public string GetInfoFileString()
        {
            string tmp = "";

            tmp += $"- Index: {Index}";
            tmp += $"\t- Category: {Category}";
            tmp += $"\t- Rare1: {Rare1}";
            tmp += $"\t- Legendary1: {Legendary1}";
            tmp += $"\t- Legendary2: {Legendary2}";
            tmp += $"\t- Mythic1: {Mythic1}";
            tmp += $"\t- Mythic2: {Mythic2}";
            tmp += $"\t- Mythic3: {Mythic3}";
            tmp += $"\t- R1: {R1}";
            tmp += $"\t- R2: {R2}";
            tmp += $"\t- R3: {R3}";
            tmp += $"\t- L1: {L1}";
            tmp += $"\t- L2: {L2}";
            tmp += $"\t- L3: {L3}";
            tmp += $"\t- M1: {M1}";
            tmp += $"\t- M2: {M2}";
            tmp += $"\t- M3: {M3}";

            return tmp;
        }
        #endregion
    }
}
