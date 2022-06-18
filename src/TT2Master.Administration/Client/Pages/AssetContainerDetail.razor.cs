using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Shared;

namespace TT2Master.AdministrationApp.Client.Pages
{
    public partial class AssetContainerDetail
    {
        [Parameter]
        public string ContainerReference { get; set; }

        private List<Version> versions = new List<Version>();

        private AssetContainer thisContainer;
        private List<AssetContainer> prodContainers;
        private IJSObjectReference _jsModule;

        protected override async Task OnInitializedAsync()
        {
            var vs = await Http.GetFromJsonAsync<string[]>("Asset/" + ContainerReference);

            foreach (var item in vs)
            {
                versions.Add(new Version(item));
            }

            versions = versions.OrderByDescending(x => x).ToList();

            var containers = await Http.GetFromJsonAsync<AssetContainer[]>("Asset");
            thisContainer = containers.Where(x => x.ContainerReference == ContainerReference).FirstOrDefault();
            prodContainers = thisContainer == null ? null : containers.Where(x => thisContainer.ProductionContainerReferences?.Contains(x.ContainerReference) ?? false).ToList();

            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/clientShared.js");
        }

        private void ValidateAssetVersion(string version)
        {
            NavigationManager.NavigateTo($"/assets/infofilevalidation/{ContainerReference}/{version}");
        }

        private void PushToProduction(string version)
        {
            NavigationManager.NavigateTo($"/assets/assetpushtoprod/{ContainerReference}/{version}");
        }

        private async Task DownloadAssetsAsync(string version)
        {
            await _jsModule.InvokeVoidAsync("downloadAssetVersionAsZip", ContainerReference, version);
        }
    }
}
