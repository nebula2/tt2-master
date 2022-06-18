using System.Collections.Generic;
using TT2MasterAdministrationApp.Shared;

namespace TT2MasterAdministrationApp.Server.Models
{
    public class AppSetting
    {
        public string AzureBlobConString { get; set; }
        public string AzureWebJobsStorage { get; set; }
        public string TableStorageAccountName { get; set; }
        public string TableStorageAccountKey { get; set; }
        public string AnnouncementTableName { get; set; }
        public string AdId { get; set; }
        public string AuthToken { get; set; }
        public string PlayerId { get; set; }
        public List<AssetContainer> Containers { get; set; }

        public string PostAssetVersionCheckUrl { get; set; }
        public string PostLatestAssetVersionUrl { get; set; }
    }
}
