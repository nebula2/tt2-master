using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TT2MasterAdministrationApp.Shared;

namespace TT2Master.AdministrationApp.Client.Pages
{
    public partial class UploadDivVersion
    {
        [Parameter]
        public string ContainerReference { get; set; }
        public string AssetVersion { get; set; } = "";
        public List<AssetUploadItem> UploadItems { get; set; } = new List<AssetUploadItem>();

        private InfofileAssetDownloadResult _downloadResult = null;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            UploadItems = new List<AssetUploadItem>();

            foreach (var file in e.GetMultipleFiles())
            {
                using var mStream = file.OpenReadStream();
                using var ms = new MemoryStream();
                await mStream.CopyToAsync(ms);
                UploadItems.Add(new AssetUploadItem
                {
                    FileName = file.Name,
                    FileContent = ms.ToArray(),
                });
            }
        }

        async Task UploadAssets()
        {
            if (string.IsNullOrEmpty(AssetVersion) || UploadItems.Count == 0)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Asset Version not given or no files for upload specified");
                return;
            }

            var pattern = @"^(\d+\.)(\d+\.)(\d+)$";
            if (!Regex.IsMatch(AssetVersion, pattern))
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Asset Version does not match required Pattern of ({pattern})");
                return;
            }

            if (!Version.TryParse(AssetVersion, out var _))
            {
                await JSRuntime.InvokeVoidAsync("alert", "Asset Version could not be parsed to a version object");
                return;
            }

            _downloadResult = null;

            foreach (var item in UploadItems)
            {
                item.ContainerReference = ContainerReference;
                item.Version = AssetVersion;
            }

            var result = await http.PostAsJsonAsync("Asset/PostAssetUpload", UploadItems);


            try
            {
                if (result.IsSuccessStatusCode)
                {
                    _downloadResult = await result.Content.ReadFromJsonAsync<InfofileAssetDownloadResult>();
                }
                else
                {
                    _downloadResult = new InfofileAssetDownloadResult
                    {
                        IsSuccessful = false,
                        Message = $"Server did not return a success status code but {result.StatusCode}",
                    };
                }
            }
            catch (Exception ex)
            {
                _downloadResult = new InfofileAssetDownloadResult
                {
                    IsSuccessful = false,
                    Message = $"Could not read response from server ({ex.Message})",
                };
            }
        }
    }
}
