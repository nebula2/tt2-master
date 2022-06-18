using System;
using System.Collections.Generic;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Type of asset (like infofiles or pictures)
    /// </summary>
    public class AssetType
    {
        /// <summary>
        /// Identifier for this object. Must be distinct when used
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Is asset state save?
        /// </summary>
        public bool IsAssetStateSave { get; set; }

        /// <summary>
        /// The state of this asset type. 
        /// </summary>
        public AssetDownloadResult AssetState { get; set; }

        /// <summary>
        /// Name of container in Azure cloud storage
        /// </summary>
        public string AzureContainer { get; set; }

        /// <summary>
        /// Locally stored version
        /// </summary>
        public string StoredVersion { get; set; }

        /// <summary>
        /// Current version available on server
        /// </summary>
        public string CurrentVersion { get; set; }

        /// <summary>
        /// List of assets for this asset type
        /// </summary>
        public List<Uri> Assets { get; set; }
    }
}
