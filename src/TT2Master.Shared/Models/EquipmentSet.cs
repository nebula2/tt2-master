namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Equipment set
    /// </summary>
    public class EquipmentSet
    {
        /// <summary>
        /// Set identifier
        /// </summary>
        public string Set { get; set; }
        /// <summary>
        /// available to public?
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Set type
        /// </summary>
        public string SetType { get; set; } = "";
        /// <summary>
        /// Crafting cost for first part
        /// </summary>
        public int CraftCost1 { get; set; }
        /// <summary>
        /// Crafting cost for second part
        /// </summary>
        public int CraftCost2 { get; set; }
        /// <summary>
        /// Crafting cost for third part
        /// </summary>
        public int CraftCost3 { get; set; }
        /// <summary>
        /// Crafting cost for fourth part
        /// </summary>
        public int CraftCost4 { get; set; }
        /// <summary>
        /// crafting cost for fifth part
        /// </summary>
        public int CraftCost5 { get; set; }
        /// <summary>
        /// required max stage to get this set
        /// </summary>
        public int StageReq { get; set; }

        /// <summary>
        /// Bonus type 1
        /// </summary>
        public string BonusType1 { get; set; } = "";
        /// <summary>
        /// Bonus amount 1
        /// </summary>
        public double Amount1 { get; set; }
        /// <summary>
        /// Increment 1
        /// </summary>
        public double Inc1 { get; set; }
        /// <summary>
        /// Exponent 1
        /// </summary>
        public double Expo1 { get; set; }

        /// <summary>
        /// Bonus type 2
        /// </summary>
        public string BonusType2 { get; set; } = "";
        /// <summary>
        /// Bonus amount 2
        /// </summary>
        public double Amount2 { get; set; }
        /// <summary>
        /// increment 2
        /// </summary>
        public double Inc2 { get; set; }
        /// <summary>
        /// Exponent 2
        /// </summary>
        public double Expo2 { get; set; }

        /// <summary>
        /// Bonus type 3
        /// </summary>
        public string BonusType3 { get; set; } = "";
        /// <summary>
        /// Bonus amount 3
        /// </summary>
        public double Amount3 { get; set; }
        /// <summary>
        /// Increment 3
        /// </summary>
        public double Inc3 { get; set; }
        /// <summary>
        /// Exponent 3
        /// </summary>
        public double Expo3 { get; set; }

        /// <summary>
        /// Is this set completed by the user?
        /// </summary>
        public bool Completed { get; set; } = true;
    }
}
