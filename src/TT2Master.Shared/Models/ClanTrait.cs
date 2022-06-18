namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes ClanExp to level and advanced start from ClanTraitInfo
    /// </summary>
    public class ClanTrait
    {
        /// <summary>
        /// The clan level
        /// </summary>
        public int ClanLevel { get; set; }

        /// <summary>
        /// The clan exp
        /// </summary>
        public double ClanExp { get; set; }

        /// <summary>
        /// Advanced start after prestige
        /// </summary>
        public double AdvancedStart { get; set; }
    }
}
