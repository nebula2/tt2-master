using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of snap choice for splashing skill
    /// </summary>
    public enum SplashSkillEnum
    {
        /// <summary>
        /// Eternal Darkness (Shadow clone)
        /// </summary>
        [Description("EternalDarkness")]
        EternalDarkness = 0,
        /// <summary>
        /// Heavenly Strike (HS)
        /// </summary>
        [Description("HeavenlyStrike")]
        HeavenlyStrike = 1,
        /// <summary>
        /// Lightning Burst (Pet)
        /// </summary>
        [Description("LightningBurst")]
        LightningBurst = 2,
    }
}
