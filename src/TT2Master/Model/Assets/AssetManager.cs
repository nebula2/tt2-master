using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Shared;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master.Model.Assets
{
    /// <summary>
    /// Manages assets like info files. is able to download assets from azure blob storage if needed
    /// </summary>
    public static class AssetManager
    {
        #region private member

//#if DEBUG
//        private const string _postAssetVersionCheckUrl = @"http://192.168.178.20:7071/api/PostAssetVersionCheckV2";
//#else
        private const string _postAssetVersionCheckUrl = null; //TODO Consume this from somewhere
//#endif
        private const string _postLatestAssetVersionUrl = null; //TODO Consume this from somewhere

        private const string _assetTypesDir = "AssetLists";

        private static List<string> _validInfos = Enum.GetNames(typeof(InfoFileEnum)).Select(x => x.ToLower()).ToList<string>();
        #endregion

        #region Private methods
        /// <summary>
        /// Downloads assets for given type async
        /// </summary>
        /// <param name="version">version to get assets for</param>
        /// <param name="at">asset type to get</param>
        /// <returns>true if successful</returns>
        private static AssetDownloadResult DownloadAssets(AssetType at)
        {
            // return if offline
            if (!CrossConnectivity.Current.IsConnected)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("DownloadAssetsAsync ERROR: User is offline"));
                return AssetDownloadResult.NoInternet;
            }

            if(at.Assets == null)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("DownloadAssetsAsync ERROR: Assets are empty"));
                return AssetDownloadResult.TotalFuckUp;
            }

            string parentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), at.AzureContainer);

            try
            {
                if (!Directory.Exists(parentDir))
                {
                    Directory.CreateDirectory(parentDir);
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"DownloadAssetsAsync ERROR: failed to create destination directory for {at.Identifier}. {ex.Message}"));
                return AssetDownloadResult.TotalFuckUp;
            }

            foreach (var item in at.Assets)
            {
                string nameLocal = item.Segments.Last();

                string filename = Path.Combine(parentDir, nameLocal);

                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

                using var client = new WebClient();
                try
                {
                    if (File.Exists(filename))
                    {
                        OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"DownloadAssetsAsync: local file \"{filename}\" exists. going to delete it before download."));

                        File.Delete(filename);
                    }

                    client.DownloadFile(item, filename);
                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"DownloadAssetsAsync: Downloaded the file."));

                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"DownloadAssetsAsync ERROR: failed to download asset {item} for {at.Identifier}. {ex.Message}"));
                    return AssetDownloadResult.TotalFuckUp;
                }
            }

            return AssetDownloadResult.SuccessfulAssetDownload;
        }

        /// <summary>
        /// Reads the local list of required assets for the given AssetType
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        private static List<Uri> GetAssetsOffline(AssetType at)
        {
            string parentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _assetTypesDir);

            if (!Directory.Exists(parentDir))
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"GetAssetsOffline ERROR: parent directory {parentDir} does not exist"));
                return new List<Uri>();
            }

            string filename = Path.Combine(parentDir, $"{at.AzureContainer}.txt");

            if (!File.Exists(filename))
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"GetAssetsOffline ERROR: target file {filename} does not exist"));
                return new List<Uri>();
            }

            string content = File.ReadAllText(filename);

            if (string.IsNullOrWhiteSpace(content))
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"GetAssetsOffline ERROR: target file {filename} is empty"));
                return new List<Uri>();
            }

            var uris = new List<Uri>();

            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    uris.Add(new Uri(@"http://localstorage.com/" + lines[i]));
                }
            }

            return uris;
        }

        private static bool IsValidInfofileReference(string infofileName)
        {
            try
            {
                return _validInfos.Contains(infofileName.Replace(".csv", "").Replace(".", "").ToLower());
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"IsValidInfofileReference ERROR: {ex.Message}\n{ex.Data}"));
                return false;
            }
        }

        /// <summary>
        /// Saves asset names in at.Assets to a local text file
        /// </summary>
        /// <param name="at">AssetType container</param>
        /// <returns>true if successful</returns>
        private static bool SaveAssetListLocally(AssetType at)
        {
            if(at.Assets == null)
            {
                return false;
            }

            string parentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _assetTypesDir);

            try
            {
                if (!Directory.Exists(parentDir))
                {
                    Directory.CreateDirectory(parentDir);
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SaveAssetListLocally ERROR: failed to create destination directory {parentDir}. {ex.Message}"));
                return false;
            }

            string assetsListString = "";

            foreach (var item in at.Assets)
            {
                assetsListString += $"{item.Segments.Last()}\n";
            }

            try
            {
                string destinationFilename = Path.Combine(parentDir, $"{at.AzureContainer}.txt");

                if (File.Exists(destinationFilename))
                {
                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SaveAssetListLocally: asset file list exists. going to delete it"));

                    File.Delete(destinationFilename);
                }

                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SaveAssetListLocally: writing asset file list"));

                File.WriteAllText(destinationFilename, assetsListString);
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SaveAssetListLocally ERROR: failed to write assets list. {ex.Message}"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets assets list online async
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        private static async Task<AssetType> GetAssetsOnlineAsync(AssetType at)
        {
            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"GetAssetsOnlineAsync"));

            if (!CrossConnectivity.Current.IsConnected)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"User is offline"));

                at.CurrentVersion = null;
                at.AssetState = AssetDownloadResult.NoInternet;
                return at;
            }

            var result = new AssetType();

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(at), Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);
                if (string.IsNullOrEmpty(_postAssetVersionCheckUrl))
                {
                    throw new Exception("Post asset version check url is empty. Needs to be done (point to azure function)");
                }
                using var resonse = await client.PostAsync(_postAssetVersionCheckUrl, content);
                using var respContent = resonse.Content;
                string tr = await respContent.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<AssetType>(tr);
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"DownloadAssetsAsync ERROR: {ex.Message}"));
                return at;
            }

            if(result == null)
            {
                at.AssetState = AssetDownloadResult.ServerError;
                return at;
            }

            if (!Version.TryParse(result.CurrentVersion, out var storedVersion))
            {
                storedVersion = new Version("0.0.1");
            }

            if (!Version.TryParse(at.CurrentVersion, out var requiredVersion))
            {
                requiredVersion = new Version("0.0.1");
            }

            if (result.CurrentVersion == null) //Assets are null ?? 
            {
                at.AssetState = AssetDownloadResult.VersionNotOnServer;
                at.Assets = result.Assets;
            }
            else if (storedVersion < requiredVersion)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"DownloadAssetsAsync: Version does not exist on server. required {at.CurrentVersion}. available: {storedVersion}"));
                at.AssetState = AssetDownloadResult.VersionNotOnServer;
                at.Assets = result.Assets;
            }
            else
            {
                at.CurrentVersion = result.CurrentVersion;
                at.Assets = result.Assets;
            }

            return at;
        }

        private static async Task<bool> SecureAssetsAsync(AssetType at)
        {
            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SecureAssetsAsync: Doing {at.Identifier}"));

            at = await GetAssetsOnlineAsync(at);

            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SecureAssetsAsync: saving asset lists"));
            SaveAssetListLocally(at);
            
            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SecureAssetsAsync: downloading assets"));
            at.AssetState = DownloadAssets(at);
            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"SecureAssetsAsync Download result = {at.AssetState}"));

            return true;
        }

        /// <summary>
        /// Returns the latest available asset version
        /// </summary>
        /// <param name="at">type to check for</param>
        /// <returns></returns>
        public static async Task<Version> GetLatestAssetVersionAsync(AssetType at)
        {
            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"GetLatestAssetVersionAsync"));

            var result = new Version();

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(at), Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);

                if (string.IsNullOrEmpty(_postLatestAssetVersionUrl))
                {
                    throw new Exception("Post latest asset version url is empty. Needs to be done (point to azure function)");
                }

                using var resonse = await client.PostAsync(_postLatestAssetVersionUrl, content);
                using var respContent = resonse.Content;
                string tr = await respContent.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Version>(tr);
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"GetLatestAssetVersionAsync: Received version {result?.ToString() ?? "null"}"));
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"GetLatestAssetVersionAsync ERROR: {ex.Message}"));
                return null;
            }

            return result;
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Asset types
        /// </summary>
        public static List<AssetType> AssetTypes = new List<AssetType>()
        {
            new AssetType()
            {
                Identifier = "InfoFiles",
#if DEBUG
                AzureContainer = "infofiles-staging",
#else
                AzureContainer = Device.RuntimePlatform == Device.iOS ? "infofiles-ios-590" : "infofiles-android-590",
#endif
                AssetState = AssetDownloadResult.SuccessfulAssetDownload,
                IsAssetStateSave = true,
            },
            new AssetType()
            {
                Identifier = "Various",
#if DEBUG                
                AzureContainer = "div-staging",
#else
                AzureContainer = "div",
#endif
                AssetState = AssetDownloadResult.SuccessfulAssetDownload,
                IsAssetStateSave = true,
            },
        };

        /// <summary>
        /// Is it required that the default builds are getting rebuilt?
        /// </summary>
        public static bool IsDefaultBuildRebuildRequired { get; set; } = false;
        #endregion

        #region Public functions
        /// <summary>
        /// Checks asset state and downloads new ones if possible or needed
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckAndDownloadAssetsAsync(bool forceDownload = false)
        {
            try
            {
                #region Info files
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: Doing infofiles"));
                var infFileAt = AssetTypes.Where(x => x.Identifier == "InfoFiles").FirstOrDefault();
                infFileAt.StoredVersion = LocalSettingsORM.CurrentTTVersion;


                if (LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    infFileAt.CurrentVersion = App.Save.TapTitansVersion;
                }
                else
                {
                    // get current version
                    infFileAt.CurrentVersion = (await GetLatestAssetVersionAsync(infFileAt))?.ToString();
                }

                if (infFileAt.CurrentVersion != infFileAt.StoredVersion || forceDownload)
                {
                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"CheckAndDownloadAssetsAsync: files are not up to date TapTitansVersion: {App.Save.TapTitansVersion} StoredVersion: {infFileAt.StoredVersion} CurrentVersion: {infFileAt.CurrentVersion} forceDownload: {forceDownload}"));
                    await SecureAssetsAsync(infFileAt);

                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"CheckAndDownloadAssetsAsync: state: {infFileAt.AssetState}"));

                    // if download was successful, set new TT2Version
                    if (infFileAt.AssetState == AssetDownloadResult.SuccessfulAssetDownload)
                    {
                        LocalSettingsORM.CurrentTTVersion = infFileAt.CurrentVersion;
                        infFileAt.StoredVersion = infFileAt.CurrentVersion;
                        OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"CheckAndDownloadAssetsAsync: set new version to {LocalSettingsORM.CurrentTTVersion}"));
                    }
                    else
                    {
                        OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: did not save new version"));
                    }
                }
                else
                {
                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: infofiles are up to date"));
                }
                #endregion

                #region various
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: Doing various"));
                var variousAt = AssetTypes.Where(x => x.Identifier == "Various").FirstOrDefault();
                variousAt.StoredVersion = LocalSettingsORM.GetCurrentVariousAssetsVersion();

                // get current version if online
                if (!CrossConnectivity.Current.IsConnected)
                {
                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: user is offline"));
                    variousAt.CurrentVersion = null;
                }
                else
                {
                    OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: securing assets"));

                    // get current version
                    variousAt.CurrentVersion = (await GetLatestAssetVersionAsync(variousAt))?.ToString();

                    if (string.IsNullOrEmpty(variousAt.CurrentVersion))
                    {
                        OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: current version is empty. I will set it to stored version to prevent further issues."));
                        variousAt.CurrentVersion = variousAt.StoredVersion;
                    }

                    if (!Version.TryParse(variousAt.CurrentVersion, out var curVersion))
                    {
                        OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"CheckAndDownloadAssetsAsync: could not parse {variousAt.CurrentVersion ?? "null"} to a valid version"));
                    }
                    if (!Version.TryParse(variousAt.StoredVersion, out var stoVersion))
                    {
                        OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"CheckAndDownloadAssetsAsync: could not parse {variousAt.StoredVersion ?? "null"} to a valid version"));
                    }

                    if (curVersion > (stoVersion ?? new Version("1")) || forceDownload)
                    {
                        // download if newer version exists
                        await SecureAssetsAsync(variousAt);

                        OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: update available"));
                        // we need to rebuild the default builds
                        if (variousAt.AssetState == AssetDownloadResult.SuccessfulAssetDownload)
                        {
                            IsDefaultBuildRebuildRequired = true;
                            LocalSettingsORM.SetCurrentVariousAssetsVersion(variousAt.CurrentVersion);
                            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: set new version"));
                        }
                        else
                        {
                            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs("CheckAndDownloadAssetsAsync: did not set new version"));
                        }
                    }
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke("AssetManager", new CustErrorEventArgs(ex));
                return false;
            }
        }

        /// <summary>
        /// Checks if all needed asset files exist independent of version control.
        /// Call this if asset state is insecure to check for fallback availability
        /// </summary>
        /// <returns>True if further execution is save, else false</returns>
        public static async Task<bool> IsAssetStateSecure()
        {
            OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"IsAssetStateSecure start"));

            foreach (var item in AssetTypes)
            {
                OnLogMePlease?.Invoke("AssetManager", new InformationEventArgs($"IsAssetStateSecure: Checking {item.Identifier}"));

                // secure assets
                if(item.Assets == null || item.Assets.Count == 0)
                {
                    OnLogMePlease("AssetManager", new InformationEventArgs($"IsAssetStateSecure assets are null or empty: get assets offline for {item.Identifier}"));
                    item.Assets = GetAssetsOffline(item);

                    try
                    {
                        // if no local stuff is available, make one last try to get them online
                        if(item.Assets == null || item.Assets.Count == 0)
                        {
                            await SecureAssetsAsync(item);
                        }
                        else if(item.Assets.Count == 0)
                        {
                            await SecureAssetsAsync(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnLogMePlease("AssetManager", new InformationEventArgs($"IsAssetStateSecure ERROR: could not get asset list: {ex.Message}"));
                        return false;
                    }
                }

                string parentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), item.AzureContainer);

                foreach (var asset in item.Assets)
                {
                    if (!File.Exists(Path.Combine(parentDir, asset.Segments.Last())))
                    {

                        OnLogMePlease("AssetManager", new InformationEventArgs($"IsAssetStateSecure ERROR: {asset.Segments.Last()} does not exist. Trying to do a fresh download"));

                        // try to get that file online and check again
                        await SecureAssetsAsync(item);

                        if(!File.Exists(Path.Combine(parentDir, asset.Segments.Last())))
                        {
                            OnLogMePlease("AssetManager", new InformationEventArgs($"IsAssetStateSecure ERROR: file still does not exist: {asset.Segments.Last()}"));
                            return false;
                        }
                        else
                        {
                            OnLogMePlease("AssetManager", new InformationEventArgs($"IsAssetStateSecure ERROR: restoring file was successful"));
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the local filename
        /// </summary>
        /// <param name="info">info enum to get the path for</param>
        /// <returns>absolute path</returns>
        public static string GetInfoFilename(InfoFileEnum info)
        {
            string parentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AssetTypes.Where(x => x.Identifier == "InfoFiles").FirstOrDefault().AzureContainer);

            return Path.Combine(parentDir, $"{info.GetDescription()}.csv");
        }

        /// <summary>
        /// Returns the local filename for various assets
        /// </summary>
        /// <param name="info">file to get the path for</param>
        /// <returns>absolute path</returns>
        public static string GetVariousAssetFilename(string file)
        {
            string parentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AssetTypes.Where(x => x.Identifier == "Various").FirstOrDefault().AzureContainer);

            if (!file.EndsWith(".csv"))
            {
                file += ".csv";
            }

            return Path.Combine(parentDir, file);
        }
        #endregion

        #region E + D
        /// <summary>
        /// Delegate for <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for <see cref="ErrorCarrier"/>
        /// </summary>
        /// <param name="data"></param>
        public delegate void ErrorCarrier(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this class gets an error
        /// </summary>
        public static event ErrorCarrier OnError;
        #endregion
    }
}