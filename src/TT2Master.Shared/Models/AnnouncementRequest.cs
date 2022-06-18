namespace TT2Master.Shared.Models
{
    /// <summary>
    /// An announcement request which the client sends to the server
    /// </summary>
    public class AnnouncementRequest
    {
        /// <summary>
        /// Client operating system
        /// </summary>
        public ClientOs Os { get; set; }

        /// <summary>
        /// Current client app version
        /// </summary>
        public int CurrentAppVersion { get; set; }

        /// <summary>
        /// Latest client id
        /// </summary>
        public int LatestClientId { get; set; }
    }
}
