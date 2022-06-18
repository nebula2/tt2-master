namespace TT2Master.Shared.Models
{
    /// <summary>
    /// An announcement which should be shown client side
    /// </summary>
    public class Announcement
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Client operating system
        /// </summary>
        public ClientOs Os { get; set; }

        /// <summary>
        /// Minimal app version for this announcement
        /// </summary>
        public int AppVersionMin { get; set; }

        /// <summary>
        /// Maximum app version for this announcement
        /// </summary>
        public int AppVersionMax { get; set; }

        /// <summary>
        /// Header or title
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Body or content
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// If an update is required the user cannot use the app until his current app version is greater than <see cref="AppVersionMax"/>
        /// </summary>
        public bool IsUpdateRequired { get; set; }
    }
}
