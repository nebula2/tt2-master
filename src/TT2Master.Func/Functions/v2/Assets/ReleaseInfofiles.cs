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
    public static class ReleaseInfofiles
    {
        /// <summary>
        /// Releases version from staging to production
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ReleaseInfofilesV2")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("ReleaseInfofiles function processed a request.");

            AssetReleaseRequest at;

            #region Parse body
            try
            {
                using var streamReader = new StreamReader(req.Body);
                string requestBody = await streamReader.ReadToEndAsync();
                at = JsonConvert.DeserializeObject<AssetReleaseRequest>(requestBody);
            }
            catch
            {
                return new BadRequestResult();
            }
            #endregion

            #region handle bad requests
            // if staging version cannot be parsed into version object this request is invalid
            if (!Version.TryParse(at.StagingVersion, out var stagingVersion))
            {
                return new BadRequestResult();
            }

            if (string.IsNullOrWhiteSpace(at.StagingContainer) || string.IsNullOrWhiteSpace(at.ProductionContainer))
            {
                return new BadRequestResult();
            } 
            #endregion


            // if production version was not specified we pick staging version
            if(at.ProductionVersion == null)
            {
                at.ProductionVersion = stagingVersion.ToString();
            }
            else
            {
                at.ProductionVersion = stagingVersion.ToString();
            }

            string conStr = Environment.GetEnvironmentVariable("AzureBlobConString", EnvironmentVariableTarget.Process);

            // if we have a requested version and it is existing on server, set it
            if(!await BlobStorageHelper.IsVersionExistingOnServerAsync(conStr, stagingVersion, at.StagingContainer))
            {
                return new BadRequestResult();
            }

            // everything is okay. copy from staging to production
            await BlobStorageHelper.CopyAssetsAsync(conStr, at);

            return new JsonContentResult(at);
        }


    }
}