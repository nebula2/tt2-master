using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Shared;

namespace TT2Master.AdministrationApp.Client.Pages
{
    public partial class InfofileValidation
    {
        [Parameter]
        public string ContainerReference { get; set; }
        [Parameter]
        public string AssetVersion { get; set; }

        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        private AssetContainer Container = new AssetContainer();
        private HubConnection hubConnection;
        private List<JfProgressEventArgs> messages = new List<JfProgressEventArgs>();
        private AssetValidationResult validationResult = null;
        private bool isWorking;

        protected override void OnInitialized()
        {
            Container.ContainerReference = ContainerReference;
            Container.LatestVersion = AssetVersion;

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/assethub"))
                .Build();

            hubConnection.On<JfProgressEventArgs>("AssetValidationProgress", (msg) =>
            {
                messages.Insert(0, msg);
                StateHasChanged();
            });

            hubConnection.On<AssetValidationResult>("AssetValidationComplete", (msg) =>
            {
                validationResult = msg;
                isWorking = false;
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        async Task StartCheck()
        {
            messages = new List<JfProgressEventArgs>();
            validationResult = null;
            isWorking = true;
            await hubConnection.SendAsync("ValidateAssets", Container);
        }

        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }
    }
}
