namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Changelog item
    /// </summary>
    public class ChangelogItem
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifier of optional parent app update request
        /// </summary>
        public int? ParentAppUpdateRequestId { get; set; }

        /// <summary>
        /// Optional parent app update request
        /// </summary>
        public AppUpdateRequest ParentAppUpdateRequest { get; set; }

        /// <summary>
        /// The type of the changelog
        /// </summary>
        public ChangelogType ItemType { get; set; }

        /// <summary>
        /// Public application version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// User friendly text of changes
        /// </summary>
        public string Changes { get; set; }

        /// <summary>
        /// Hyperlink to some resource
        /// </summary>
        public string Hyperlink { get; set; }
    }
}
