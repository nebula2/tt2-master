using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of Hero Damage Types - what kind of damage
    /// </summary>
    public enum HeroDmgType
    {
        /// <summary>
        /// Melee hero
        /// </summary>
        [Description("Melee")]
        Melee,
        /// <summary>
        /// Ranged hero
        /// </summary>
        [Description("Ranged")]
        Ranged,
        /// <summary>
        /// Spell hero
        /// </summary>
        [Description("Spell")]
        Spell,
        /// <summary>
        /// Not set
        /// </summary>
        [Description("NotSet")]
        NotSet
    }
}
