using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of snap choice for splashing
    /// </summary>
    public enum SplashSnapEnum
    {
        /// <summary>
        /// Without snap
        /// </summary>
        [Description("WithoutSnap")]
        WithoutSnap = 0,
        /// <summary>
        /// With one snap active
        /// </summary>
        [Description("SingleSnap")]
        SingleSnap = 1,
        /// <summary>
        /// With two snaps active
        /// </summary>
        [Description("DoubleSnap")]
        DoubleSnap = 2,
    }
}
