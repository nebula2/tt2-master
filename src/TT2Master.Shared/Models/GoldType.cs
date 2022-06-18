using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of Gold types (gold sources for income)
    /// </summary>
    public enum GoldType
    {
        /// <summary>
        /// Pet Heart of Midas
        /// </summary>
        [Description("pHoM")]
        pHoM,
        /// <summary>
        /// Boss gold
        /// </summary>
        [Description("BossGold")]
        BossGold,
        /// <summary>
        /// Chesterson gold
        /// </summary>
        [Description("ChestersonGold")]
        ChestersonGold,
        /// <summary>
        /// Normal monster gold
        /// </summary>
        [Description("NormalMobGold")]
        NormalMobGold,
        /// <summary>
        /// Fairy gold
        /// </summary>
        [Description("FairyGold")]
        FairyGold
    }
}
