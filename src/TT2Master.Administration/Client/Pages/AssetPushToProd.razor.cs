using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Shared;

namespace TT2Master.AdministrationApp.Client.Pages
{
    public partial class AssetPushToProd
    {
        [Parameter]
        public string ContainerReference { get; set; }
        [Parameter]
        public string AssetVersion { get; set; }
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        private AssetContainer Container = new AssetContainer();
        private List<AssetContainer> prodContainers;
        private HubConnection hubConnection;
        private List<JfProgressEventArgs> messages = new List<JfProgressEventArgs>();
        private bool? pushResult = null;
        private bool isWorking;

        protected override async Task OnInitializedAsync()
        {
            // set up container stuff
            Container.ContainerReference = ContainerReference;
            Container.LatestVersion = AssetVersion;

            var containers = await Http.GetFromJsonAsync<AssetContainer[]>("Asset");
            var thisContainer = containers.Where(x => x.ContainerReference == ContainerReference).FirstOrDefault();
            prodContainers = containers.Where(x => thisContainer.ProductionContainerReferences?.Contains(x.ContainerReference) ?? false).ToList();
            Container.ProductionContainerReferences = prodContainers?.Select(x => x.ContainerReference).ToList();

            // set up connection hub
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/assethub"))
                .Build();

            hubConnection.On<JfProgressEventArgs>("PushAssetsToProductionProgress", (msg) =>
            {
                messages.Insert(0, msg);
                StateHasChanged();
            });

            hubConnection.On<bool>("PushAssetsToProductionComplete", (msg) =>
            {
                pushResult = msg;
                isWorking = false;
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        async Task StartPush(object prodContainer)
        {
            messages = new List<JfProgressEventArgs>();
            pushResult = null;
            isWorking = true;
            await hubConnection.SendAsync("PushAssetsToProduction", Container, prodContainer.ToString());
        }

        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }
    }
}
