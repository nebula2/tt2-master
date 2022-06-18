using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Loggers;
using Xamarin.Forms;

namespace TT2Master.TT2WebMaster
{
    public class WebMasterService
    {
        private const int _snapshotSingleTransferLimit = 50;
        private readonly string _baseUrl = "https://192.168.178.69:44319";
        private readonly Lazy<HttpClient> _apiClient;
        private readonly OidcClient _client;
        private readonly DBRepository _dbRepository;

        public WebMasterService(DBRepository dbRepo)
        {
            _dbRepository = dbRepo;

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                (message, cert, chain, errors) => { return true; }
            };

            _apiClient = new Lazy<HttpClient>(() => new HttpClient(httpClientHandler));

            var browser = DependencyService.Get<IBrowser>();
            var options = new OidcClientOptions
            {
                Authority = _baseUrl,
                ClientId = "TT2WebMaster_Xamarin",
                Scope = "email openid profile role phone address TT2WebMaster",
                ClientSecret = "SupaB@nAna31!",
                RedirectUri = "xamarinformsclients://callback",
                PostLogoutRedirectUri = "xamarinformsclients://callback",
                Browser = browser,
                // TODO ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };
            options.BackchannelHandler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) => true };
            options.Policy.Discovery.RequireHttps = true;
            _client = new OidcClient(options);
        }

        public async Task<bool> ConnectAsync()
        {
            if (LocalSettingsORM.AccessTokenExpiration > DateTime.UtcNow &&
                !string.IsNullOrEmpty(LocalSettingsORM.AccessToken))
            {
                return true;
            }
            
            try
            {
                var result = await _client.LoginAsync(new LoginRequest());

                if (result.IsError)
                    throw new Exception(result.Error);

                LocalSettingsORM.AccessToken = result.AccessToken;
                LocalSettingsORM.AccessTokenExpiration = result.AccessTokenExpiration.DateTime;
                LocalSettingsORM.IdentityToken = result.IdentityToken;
                LocalSettingsORM.IdentityUsername = result.User?.Identity?.Name;

                return true;
            }
            catch (Exception ex)
            {
                DependencyService.Get<ISendMessage>().LongAlert(ex.Message);
                return false;
            }
        }

        public async Task<bool> DisconnectAsync()
        {
            try
            {
                var result = await _client.LogoutAsync(new LogoutRequest() { IdTokenHint = LocalSettingsORM.IdentityToken });

                if (result.IsError)
                    throw new Exception(result.Error);

                LocalSettingsORM.AccessToken = null;
                LocalSettingsORM.AccessTokenExpiration = DateTime.UtcNow;
                LocalSettingsORM.IdentityToken = null;

                return true;
            }
            catch (Exception ex)
            {
                DependencyService.Get<ISendMessage>().LongAlert(ex.Message);
                return false;
            }
        }

        public async Task<int> UpsertPlayerSnapshotsAsync()
        {
            if (!await ConnectAsync())
            {
                return 0;
            }

            var snapshotsToTransfer = await _dbRepository.GetSnapshotsToTransferAsync();

            if (snapshotsToTransfer == null)
            {
                Logger.WriteToLogFile($"UpsertPlayerSnapshotsAsync ERROR: Could not get snapshots to transfer -> {_dbRepository.StatusMessage}");
                return 0;
            }

            if (snapshotsToTransfer.Count == 0)
            {
                Logger.WriteToLogFile($"UpsertPlayerSnapshotsAsync: There is nothing to transfer");
                return 0;
            }

            var dtos = new List<PlayerSnapshotDto>();
            foreach (var snapshot in snapshotsToTransfer.Take(_snapshotSingleTransferLimit))
            {
                var parent = await _dbRepository.GetSnapshotByID(snapshot.SnapshotId);

                var dto = PlayerSnapshotConverter.GetPlayerSnapshotDto(snapshot, parent.Timestamp);
                dtos.Add(dto);
            }

            _apiClient.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LocalSettingsORM.AccessToken ?? "");

            var json = JsonConvert.SerializeObject(dtos);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _apiClient.Value.PostAsync($"{_baseUrl}/api/app/player-snapshot-sync/upsert-player-snapshots", data);
            string result = await response.Content.ReadAsStringAsync();
            var dtoResults = JsonConvert.DeserializeObject<List<PlayerSnapshotDto>>(result);

            foreach (var item in dtoResults)
            {
                if (await _dbRepository.UpdateMemberSnapshotItemExternalIdAsync(item.LocalAppId, item.Id) == 0)
                {
                    Logger.WriteToLogFile($"UpsertPlayerSnapshotsAsync Error: could not update local item {item.LocalAppId} with external id {item.Id}");
                }
            }

            return dtoResults.Count;
        }
    }
}
