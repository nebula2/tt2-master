namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Item which describes the purchasement cost of an artifact or enchantment
    /// </summary>
    public class ArtifactCost
    {
        /// <summary>
        /// Count - owned arts + owned enchantments + 1 == next price
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Amount of relics you need to spend for the next artifact/ enchantment
        /// </summary>
        public double RelicCost { get; set; }
    }
}
