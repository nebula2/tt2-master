using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Shared;

namespace TT2Master.AdministrationApp.Client.Pages
{
    public partial class AssetGameHiveDownload
    {
        [Parameter]
        public string ContainerReference { get; set; }
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        private AssetContainer Container = new AssetContainer();
        private HubConnection hubConnection;
        private List<JfProgressEventArgs> messages = new List<JfProgressEventArgs>();
        private InfofileAssetDownloadResult downloadResult = null;
        private bool isWorking;

        protected override void OnInitialized()
        {
            Container.ContainerReference = ContainerReference;

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/assethub"))
                .Build();

            hubConnection.On<JfProgressEventArgs>("AssetDownloadProgress", (msg) =>
            {
                messages.Insert(0, msg);
                StateHasChanged();
            });

            hubConnection.On<InfofileAssetDownloadResult>("AssetDownloadComplete", (msg) =>
            {
                downloadResult = msg;
                isWorking = false;
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        async Task DownloadAssets()
        {
            messages = new List<JfProgressEventArgs>();
            downloadResult = null;
            isWorking = true;
            await hubConnection.SendAsync("DownloadAssets", Container);
        }

        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }
    }
}
