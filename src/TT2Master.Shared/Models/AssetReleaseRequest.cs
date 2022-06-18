namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Asset release request to move staging assets to production
    /// </summary>
    public class AssetReleaseRequest
    {
        /// <summary>
        /// The container name of the staging environment
        /// </summary>
        public string StagingContainer { get; set; }
        /// <summary>
        /// Version number of staging
        /// </summary>
        public string StagingVersion { get; set; }

        /// <summary>
        /// Production container name
        /// </summary>
        public string ProductionContainer { get; set; }

        /// <summary>
        /// Production version number. leave this empty to keep staging version name
        /// </summary>
        public string ProductionVersion { get; set; }
    }
}
