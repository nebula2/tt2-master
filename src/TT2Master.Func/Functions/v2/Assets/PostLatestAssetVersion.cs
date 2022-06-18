using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TT2Master.Shared.Models;
using TT2MasterFunc.Util;

namespace TT2MasterFunc.Functions.v2.Assets
{
    public static class PostLatestAssetVersion
    {
        [FunctionName("PostLatestAssetVersionV2")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("PostAssetVersionCheck function processed a request.");

            AssetType at;

            #region Parse body
            try
            {
                using var streamReader = new StreamReader(req.Body);
                string requestBody = await streamReader.ReadToEndAsync();
                at = JsonConvert.DeserializeObject<AssetType>(requestBody);
            }
            catch
            {
                return new BadRequestResult();
            }
            #endregion

            string conStr = Environment.GetEnvironmentVariable("AzureBlobConString", EnvironmentVariableTarget.Process);

            var result = await BlobStorageHelper.GetLatestVersionExistingOnServerAsync(conStr, at.AzureContainer);

            return new JsonContentResult(result.ToString());
        }
    }
}