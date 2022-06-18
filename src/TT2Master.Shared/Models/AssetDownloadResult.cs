namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Result type of asset download
    /// </summary>
    public enum AssetDownloadResult
    {
        /// <summary>
        /// Download completed successfully
        /// </summary>
        SuccessfulAssetDownload = 0,
        /// <summary>
        /// Missing internet connection
        /// </summary>
        NoInternet = 1,
        /// <summary>
        /// Version does not exist yet on server
        /// </summary>
        VersionNotOnServer = 2,
        /// <summary>
        /// Could not save asset locally
        /// </summary>
        LocalFileError = 3,
        /// <summary>
        /// Server side error
        /// </summary>
        ServerError = 4,
        /// <summary>
        /// Something unexpected happened here
        /// </summary>
        TotalFuckUp = 5,
        /// <summary>
        /// Wrongly formatted parameter given to function
        /// </summary>
        WrongParameter = 6,
    }
}
