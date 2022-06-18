using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TT2Master.Shared;
using TT2MasterFunc.Util;

namespace TT2MasterFunc.Functions.v2.Assets
{
    /// <summary>
    /// Updates info files if needed
    /// </summary>
    public static class UpdateInfoFiles
    {
        private static string _connectionString;
        private const string _container = "infofiles";
        private const string _containerForStaging = "infofiles-staging";
        private static ILogger _logger;

        /// <summary>
        /// Timer triggered automatic update of Tap Titans 2 info files
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("UpdateInfoFilesV2")]
        public static async Task Run([TimerTrigger("0 0 */2 * * *")]TimerInfo myTimer, ILogger log)
        {
            // Set initial stuff
            _connectionString = Environment.GetEnvironmentVariable("AzureBlobConString", EnvironmentVariableTarget.Process);
            _logger = log;

            // get current version
            string versionToCheck = await GetTapTitansVersionAsync();

            // check if returned version is valid
            if (!IsVersion(versionToCheck))
            {
                throw new InfoUpdateFailureExeption($"{versionToCheck ?? "<null>"} is not a valid version!");
            }

            // upload info files if needed
            if (!await BlobStorageHelper.IsVersionExistingOnServerAsync(_connectionString, Version.Parse(versionToCheck), _container))
            {
                _logger.LogInformation($"Getting data for version: {versionToCheck}");
                await CreateStagingAssets(versionToCheck);
            }
            else
            {
                _logger.LogInformation($"Info files are up to date");
            }

            _logger.LogInformation($"Function executed at: {DateTime.Now}.");
        }

        /// <summary>
        /// Returns a filled ServerAPI object. you need to init this afterwards
        /// </summary>
        /// <returns></returns>
        private static TT2ServerAPI GetFilledServerApi()
        {
            string adId = Environment.GetEnvironmentVariable("AdId", EnvironmentVariableTarget.Process); ;
            string authToken = Environment.GetEnvironmentVariable("AuthToken", EnvironmentVariableTarget.Process);
            string playerId = Environment.GetEnvironmentVariable("PlayerId", EnvironmentVariableTarget.Process);

            return new TT2ServerAPI(adId, authToken, playerId);
        }

        /// <summary>
        /// Returns true if given string can be parsed to a System.Version object
        /// </summary>
        /// <param name="s">string to parse</param>
        /// <returns>true if parseable</returns>
        private static bool IsVersion(string s) => Version.TryParse(s, out _);

        /// <summary>
        /// Gets the latest version of Tap Titans 2
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetTapTitansVersionAsync()
        {
            var ttApi = GetFilledServerApi();
            ttApi.Initialize();

            return await ttApi.GetTapTitansAppVersion();
        }

        /// <summary>
        /// Gets all info files from GameHive servers and stores them in staging blob storage
        /// </summary>
        /// <param name="version">version to get infofiles for</param>
        /// <returns>true if successful else false</returns>
        private static async Task CreateStagingAssets(string version)
        {
            var ttApi = GetFilledServerApi();
            ttApi.Initialize();

            foreach (var item in (InfoFileEnum[])Enum.GetValues(typeof(InfoFileEnum)))
            {
                try
                {
                    //Get content
                    string content = await ttApi.GetInfoFile(item);

                    //write content
                    string filename = $"{version}\\{item.GetDescription()}.csv";

                    await BlobStorageHelper.CreateBlobAsync(_connectionString, filename, content, _containerForStaging);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error {DateTime.Now}: {ex.Message}");
                    throw new InfoUpdateFailureExeption($" Error {DateTime.Now}: {ex.Message}");
                }
            }
        }
    }
}