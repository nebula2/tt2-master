using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of Hero Base Damage Types
    /// </summary>
    public enum HeroBaseType
    {
        /// <summary>
        /// Ground type hero
        /// </summary>
        [Description("Ground")]
        Ground,
        /// <summary>
        /// Flying type hero
        /// </summary>
        [Description("Flying")]
        Flying,
        /// <summary>
        /// Not set
        /// </summary>
        [Description("NotSet")]
        NotSet
    }
}
