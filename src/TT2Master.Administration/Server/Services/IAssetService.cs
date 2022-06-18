using System.Collections.Generic;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Shared;

namespace TT2MasterAdministrationApp.Server.Services
{
    public interface IAssetService
    {
        Task<InfofileAssetDownloadResult> DownloadAssetsFromGameHiveAsync(AssetContainer container);
        Task<IEnumerable<string>> GetVersionsStoredInContainerAsync(string containerReference);
        Task<IEnumerable<AssetContainer>> GetAssetContainersWithLatestVersionAsync();
        Task<AssetValidationResult> ValidateAssetVersion(AssetContainer newContainer
            //, string[] oldContainers
            );

        Task<bool> PushAssetsToProduction(AssetContainer source, string targetContainerReference);

        /// <summary>
        /// Downloads all assets in given container/version
        /// </summary>
        /// <param name="containerReference"></param>
        /// <param name="version"></param>
        /// <returns>directory path where the assets are stored</returns>
        Task<string> DownloadAssetsFromContainerToTempDirAsync(string containerReference, string version);

        Task<InfofileAssetDownloadResult> UploadAssets(List<AssetUploadItem> items);

        delegate Task ProgressCarrier(object sender, JfProgressEventArgs e);

        public event ProgressCarrier OnProgressMade;
    }
}
