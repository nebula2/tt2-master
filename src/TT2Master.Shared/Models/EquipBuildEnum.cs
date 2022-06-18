using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of equipment builds
    /// </summary>
    public enum EquipBuildEnum
    {
        /// <summary>
        /// Ship damage based build
        /// </summary>
        [Description("Ship")]
        Ship = 0,
        /// <summary>
        /// Tap damage based build
        /// </summary>
        [Description("Tap")]
        Tap = 1,
        /// <summary>
        /// Pet damage based build
        /// </summary>
        [Description("Pet")]
        Pet = 2,
        /// <summary>
        /// Shadow Clone damage based build
        /// </summary>
        [Description("SC")]
        SC = 3,
        /// <summary>
        /// Heavenly Strike damage based build
        /// </summary>
        [Description("HS")]
        HS = 4,
    }
}
