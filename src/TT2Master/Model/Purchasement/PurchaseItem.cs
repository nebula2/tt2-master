namespace TT2Master
{
    /// <summary>
    /// Describes a purchaseable item
    /// </summary>
    public class PurchaseItem
    {
        /// <summary>
        /// Identifier (must be the same as in google play console)
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// True if this item has been purchased already
        /// </summary>
        public bool IsPurchased { get; set; }
    }
}