using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Server.Services;
using TT2MasterAdministrationApp.Shared;

namespace TT2MasterAdministrationApp.Server.Hubs
{
    public class AssetHub : Hub
    {
        private readonly IServiceProvider _sp;
        public AssetHub(IServiceProvider sp) => _sp = sp;

        public async Task DownloadAssets(AssetContainer container)
        {
            using var scope = _sp.CreateScope();
            var assetService = scope.ServiceProvider.GetRequiredService<IAssetService>();
            assetService.OnProgressMade += AssetService_OnProgressMade;
            var result = await assetService.DownloadAssetsFromGameHiveAsync(container);
            assetService.OnProgressMade -= AssetService_OnProgressMade;

            await Clients.All.SendAsync("AssetDownloadComplete", result);
        }
        private async Task AssetService_OnProgressMade(object sender, JfProgressEventArgs e) 
            => await Clients.All.SendAsync("AssetDownloadProgress", e);

        public async Task ValidateAssets(AssetContainer container)
        {
            using var scope = _sp.CreateScope();
            var assetService = scope.ServiceProvider.GetRequiredService<IAssetService>();
            assetService.OnProgressMade += AssetService_OnValidationProgressMade;
            var result = await assetService.ValidateAssetVersion(container);
            assetService.OnProgressMade -= AssetService_OnValidationProgressMade;

            await Clients.All.SendAsync("AssetValidationComplete", result);
        }
        private async Task AssetService_OnValidationProgressMade(object sender, JfProgressEventArgs e) 
            => await Clients.All.SendAsync("AssetValidationProgress", e);

        public async Task PushAssetsToProduction(AssetContainer source, string targetContainerReference)
        {
            using var scope = _sp.CreateScope();
            var assetService = scope.ServiceProvider.GetRequiredService<IAssetService>();
            assetService.OnProgressMade += AssetService_OnAssetPushProgressMade;
            var result = await assetService.PushAssetsToProduction(source, targetContainerReference);
            assetService.OnProgressMade -= AssetService_OnAssetPushProgressMade;

            await Clients.All.SendAsync("PushAssetsToProductionComplete", result);
        }
        private async Task AssetService_OnAssetPushProgressMade(object sender, JfProgressEventArgs e) 
            => await Clients.All.SendAsync("PushAssetsToProductionProgress", e);
    }
}
