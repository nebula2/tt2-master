namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Scaling information for equipment
    /// </summary>
    public class EquipEnhScalingInfo
    {
        /// <summary>
        /// Bonus type
        /// </summary>
        public string BonusType { get; set; }
        /// <summary>
        /// Sprite index
        /// </summary>
        public int SpriteIndex { get; set; }
        /// <summary>
        /// Attribute base
        /// </summary>
        public double AttributeBase { get; set; }
        /// <summary>
        /// Power base
        /// </summary>
        public int PowerBase { get; set; }
        /// <summary>
        /// Power increase
        /// </summary>
        public double PowerInc { get; set; }
        /// <summary>
        /// Power exponent
        /// </summary>
        public double PowerExp { get; set; }
    }
}
