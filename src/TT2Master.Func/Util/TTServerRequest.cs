using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TT2MasterFunc.Util
{
    class TTServerRequest
    {
        /// <summary>
        /// Url to gamehives tap titans 2 server.
        /// </summary>
        private const string _serverUrl = "https://tt2.gamehivegames.com";

        /// <summary>
        /// HMACMD5 Hasher for generating the content signature.
        /// </summary>
        private static HMACMD5 _signatureHasher;

        /// <summary>
        /// Authtoken for server requests.
        /// </summary>
        private static string _authToken;

        /// <summary>
        /// Stopwatch for dummy generation.
        /// </summary>
        private static Stopwatch _stopwatch;

        /// <summary>
        /// Full Url of this request.
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// Relative Url of this request.
        /// </summary>
        private readonly string _endpointPath;

        /// <summary>
        /// HttpMethod of this request.
        /// </summary>
        private readonly HttpMethod _requestMethod;

        /// <summary>
        /// Content data of this request.
        /// </summary>
        private readonly Dictionary<string, object> _data;

        /// <summary>
        /// Should the request content be minified.
        /// </summary>
        public bool MinifyJSONContent = true;


        /// <summary>
        /// Creates new post request to given endpoint.
        /// </summary>
        /// <param name="endpointPath">Endpoint of the request</param>
        public TTServerRequest(string endpointPath)
        {
            if (_signatureHasher == null)
            {
                throw new InvalidOperationException("Server API needs to be initialized before creating requests.");
            }

            _endpointPath = endpointPath;
            _url = $"{_serverUrl}{_endpointPath}";
            _data = new Dictionary<string, object>();
            _requestMethod = HttpMethod.Post;
        }

        /// <summary>
        /// Creates new request with given method to given endpoint.
        /// </summary>
        /// <param name="enpointPath">Endpoint of the request</param>
        /// <param name="httpMethod">Request method of the request</param>
        public TTServerRequest(string enpointPath, HttpMethod httpMethod) : this(enpointPath)
        {
            _requestMethod = httpMethod;
        }

        /// <summary>
        /// Initializes server requests. Needs to be callen before object instantiation.
        /// </summary>
        /// <param name="adId">Ad Id of user</param>
        /// <param name="authToken">Authtoken of user</param>
        public static void Init(string adId, string authToken = "")
        {
            char[] charArray = adId.ToCharArray();
            string s = string.Empty;
            if (charArray.Length >= 3)
            {
                s = string.Empty + charArray[1] + charArray[0] + charArray[3];
            }

            _signatureHasher = new HMACMD5(Encoding.UTF8.GetBytes(s));
            TTServerRequest._authToken = authToken;
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Adds data to request content.
        /// </summary>
        /// <param name="key">Key for field</param>
        /// <param name="value">Field value</param>
        public void AddData(string key, object value) => _data[key] = value;

        /// <summary>
        /// Gets Url parameters for this request.
        /// </summary>
        /// <returns>Url parameters as string</returns>
        private string GetUrlParams()
        {
            var now = DateTime.Now;
            string signature = GetSignature();
            string urlParams = $"?d={TT2ServerAPI.DeviceOS.ToLower()}&v={TT2ServerAPI.AppVersion}&time={now.Ticks}&dummy={GetDummy()}&s={signature}";
            return urlParams;
        }

        /// <summary>
        /// Generates elapsed seconds for dummy url parameter.
        /// </summary>
        /// <returns></returns>
        private static string GetDummy()
        {
            double elapsedMiliseconds = _stopwatch.ElapsedMilliseconds + 200540.0;
            double elapesedSeconds = elapsedMiliseconds / 1000.0;
            string elapesedSecondsString = elapesedSeconds.ToString("F3").Replace(',', '.');

            return elapesedSecondsString;
        }

        /// <summary>
        /// Generates the signature for the content data.
        /// </summary>
        /// <returns>Signature bytes as string</returns>
        private string GetSignature()
        {
            byte[] rawData = Encoding.UTF8.GetBytes(GetDataAsJSONString());

            byte[] hash = _signatureHasher.ComputeHash(rawData);
            var stringBuilder = new StringBuilder();
            foreach (byte num in hash)
            {
                stringBuilder.Append(num.ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets content data as non formated json.
        /// </summary>
        /// <returns></returns>
        private string GetDataAsJSONString()
        {
            var jsonObject = JObject.FromObject(_data);
            string jsonString = jsonObject.ToString(Newtonsoft.Json.Formatting.None);
            if (!MinifyJSONContent)
            {
                jsonString = jsonObject.ToString();
            }

            return jsonString;
        }

        /// <summary>
        /// Sends the request to server and returns response as string async.
        /// </summary>
        /// <returns>Server response as string</returns>
        public async Task<string> SendRequestAsync()
        {
            var requestUri = new Uri(_url + GetUrlParams());
            string contentJson = GetDataAsJSONString();

            var httpRequest = WebRequest.CreateHttp(requestUri);
            httpRequest.Method = _requestMethod.ToString();
            httpRequest.Headers.Add("X-HTTP-Method-Override", _requestMethod.ToString());
            httpRequest.Accept = "application/vnd.gamehive.btb4-v1.0+json";
            httpRequest.Headers.Add(HttpRequestHeader.Authorization, "token " + _authToken);

            if (_requestMethod == HttpMethod.Post)
            {
                httpRequest.ContentType = "application/json; charset=UTF-8";
            }

            httpRequest.Host = "tt2.gamehivegames.com";
            httpRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, identity");
            httpRequest.KeepAlive = true;
            httpRequest.Connection = "TE";
            httpRequest.Headers.Add(HttpRequestHeader.Te, "identity");
            httpRequest.UserAgent = "BestHTTP";
            httpRequest.Expect = "";

            if (_requestMethod == HttpMethod.Post)
            {
                httpRequest.ContentLength = contentJson.Length;
                byte[] contentBytes = Encoding.UTF8.GetBytes(contentJson);
                using var requeststream = await httpRequest.GetRequestStreamAsync();
                await requeststream.WriteAsync(contentBytes, 0, contentBytes.Length);
                requeststream.Close();
            }

            using var httpResponse = await httpRequest.GetResponseAsync() as HttpWebResponse;
            using var responseStream = httpResponse.GetResponseStream();
            using var responseReader = new StreamReader(responseStream);
            string responseString = await responseReader.ReadToEndAsync();

            return responseString;
        }
    }
}
