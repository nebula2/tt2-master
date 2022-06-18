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
    public static class PostAssetVersionCheck
    {
        /// <summary>
        /// Checks for the latest available version of given asset type
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("PostAssetVersionCheckV2")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("PostAssetVersionCheck function processed a request.");

            AssetType at;
            Version currentVersion;

            #region Parse body
            try
            {
                using (var streamReader = new StreamReader(req.Body))
                {
                    string requestBody = await streamReader.ReadToEndAsync();
                    at = JsonConvert.DeserializeObject<AssetType>(requestBody);
                }
            }
            catch
            {
                return new BadRequestResult();
            }
            #endregion

            if (!Version.TryParse(at.StoredVersion, out var storedVersion))
            {
                storedVersion = new Version("0.0.1");
            }

            if (!Version.TryParse(at.CurrentVersion, out var requiredVersion))
            {
                requiredVersion = null;
            }

            string conStr = Environment.GetEnvironmentVariable("AzureBlobConString", EnvironmentVariableTarget.Process);

            // if we have a requested version and it is existing on server, set it
            currentVersion = requiredVersion != null && await BlobStorageHelper.IsVersionExistingOnServerAsync(conStr, requiredVersion, at.AzureContainer)
                ? requiredVersion
                : await BlobStorageHelper.GetLatestVersionExistingOnServerAsync(conStr, at.AzureContainer);

            at.CurrentVersion = currentVersion.ToString();

            // fill list of files if needed
            if ((requiredVersion == null && storedVersion < currentVersion) || requiredVersion != null)
            {
                at.Assets = await BlobStorageHelper.SaveAssetsAsync(conStr, at);
            }

            return new JsonContentResult(at);
        }


    }
}