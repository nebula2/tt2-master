namespace TT2Master.Shared.Models
{
    /// <summary>
    /// The Update type which needs to be performed
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// Download all info files which are served by Tap Titans
        /// </summary>
        AssetInfoDownload = 0,
        /// <summary>
        /// Download all info files which are served by TT2Master
        /// </summary>
        AssetDivDownload = 1,
        /// <summary>
        /// Download all translation related files
        /// </summary>
        TranslationDownload = 2,
        /// <summary>
        /// Download all images
        /// </summary>
        ImagesDownload = 3,
        /// <summary>
        /// Check the integrety of the client. Meaning a full check for the completeness of all files
        /// </summary>
        IntegrityCheck = 4
    }
}
