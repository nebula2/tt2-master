using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TT2Master.Shared;

namespace TT2MasterFunc.Util
{
    public class TT2ServerAPI
    {
        /// <summary>
        /// Ad id of user.
        /// </summary>
        private readonly string _adId;

        /// <summary>
        /// Authtoken of user.
        /// </summary>
        private readonly string _authToken;

        /// <summary>
        /// Ingameid of user.
        /// </summary>
        private readonly string _playerId;

        /// <summary>
        /// Version of tap titans.
        /// </summary>
        public static string AppVersion = "3.9.0";

        /// <summary>
        /// Name of devices os. Default is Android.
        /// </summary>
        public static string DeviceOS = "Android";

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="adId">Players ad id.</param>
        /// <param name="authToken">Players authtoken.</param>
        /// <param name="playerId">Players ingame id.</param>
        public TT2ServerAPI(string adId, string authToken, string playerId)
        {
            _adId = adId;
            _authToken = authToken;
            _playerId = playerId;

        }

        /// <summary>
        /// Initialises the server api. Needs to be called before usage.
        /// </summary>
        public void Initialize() => TTServerRequest.Init(_adId, _authToken);

        /// <summary>
        /// Get current time of gamehives server.
        /// </summary>
        /// <returns>Server time as json.</returns>
        public async Task<string> GetServerTimeAsync()
        {
            var request = new TTServerRequest("/server_time", HttpMethod.Get);

            string response = await request.SendRequestAsync();

            //Check if appversion matches the current version
            if (!AppVersionUpToDate(response))
            {
                response = await GetServerTimeAsync();
            }

            return response;
        }

        /// <summary>
        /// Gets metadata(Name,url and hash) of info files.
        /// </summary>
        /// <returns>Metadata of all infofiles.</returns>
        public async Task<string> GetInfoFilesMetadata()
        {
            var request = new TTServerRequest("/info_files");
            request.AddData("ad_id", _adId);
            request.AddData("player_id", _playerId);

            string response = await request.SendRequestAsync();

            //Check if appversion matches the current version
            if (!AppVersionUpToDate(response))
            {
                response = await GetInfoFilesMetadata();
            }

            return response;
        }

        /// <summary>
        /// Checks if the current version is okay.
        /// If not it will be set correctly
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static bool AppVersionUpToDate(string response)
        {
            try
            {
                var json = JObject.Parse(response);

                if (!json.ContainsKey("_error"))
                {
                    return true;
                }

                string ttversion = json["_error"]["message"].ToString();
                if (ttversion != AppVersion)
                {
                    AppVersion = ttversion;
                    return false;
                }
            }
            catch (Exception)
            {
                //Not handled
            }

            return true;
        }

        /// <summary>
        /// Gets App version of Tap Titans 2
        /// </summary>
        /// <returns>string of version number eg. "2.8.3"</returns>
        public async Task<string> GetTapTitansAppVersion()
        {
            var request = new TTServerRequest("/info_files");
            request.AddData("ad_id", _adId);
            request.AddData("player_id", _playerId);

            string response = await request.SendRequestAsync();

            //Check if appversion matches the current version
            if (!AppVersionUpToDate(response))
            {
                _ = await GetTapTitansAppVersion();
            }

            return AppVersion;
        }

        /// <summary>
        /// Gets the infofile as raw text(csv mostly).
        /// </summary>
        /// <param name="infoFile">Requested infofile.</param>
        /// <returns></returns>
        public async Task<string> GetInfoFile(InfoFileEnum infoFile)
        {
            string infoFilesMetadataString = await GetInfoFilesMetadata();
            var infoFilesMetadata = JObject.Parse(infoFilesMetadataString);
            string desiredInfoFileName = infoFile.GetDescription();

            //check if response indicates that the server is down
            if (infoFilesMetadata.ToString().Contains("maintenance downtime or capacity problems"))
            {
                return "Server down";
            }

            string infoFileUrl = infoFilesMetadata[desiredInfoFileName].Value<string>("url");

            var requestUri = new Uri(infoFileUrl + "?time=5.216266"); //lets pretend to be send from tap titans 2.

            var httpRequest = WebRequest.CreateHttp(requestUri);
            httpRequest.Method = HttpMethod.Get.ToString();
            httpRequest.UserAgent = "BestHTTP";

            using var httpResponse = await httpRequest.GetResponseAsync() as HttpWebResponse;
            using var responseStream = httpResponse.GetResponseStream();
            using var responseReader = new StreamReader(responseStream);
            string responseString = await responseReader.ReadToEndAsync();

            return responseString;
        }
    }
}
