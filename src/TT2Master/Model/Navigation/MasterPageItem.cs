namespace TT2Master
{
    /// <summary>
    /// Describes an item for MasterDetailPage
    /// </summary>
    public class MasterPageItem
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Icon
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Destination this leads to
        /// </summary>
        public string Destination { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}