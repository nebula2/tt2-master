using System.IO;
using TT2Master.Droid;
using TT2Master.Droid.Loggers;
using Xamarin.Forms;

[assembly: Dependency(typeof(TapTitansPathHelper))]
namespace TT2Master.Droid
{
    public class TapTitansPathHelper : ITapTitansPath
    {
        public string GetFileName() => Path.Combine(@"/storage/emulated/0/Android/data/com.gamehivecorp.taptitans2/files", "ISavableGlobal.adat");
        public string GetAbyssalFileName() => Path.Combine(@"/storage/emulated/0/Android/data/com.gamehivecorp.taptitans2/files", "ISavableGlobalChallenge.adat");

        /// <summary>
        /// Process string to get path
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public string ProcessPathString(string filepath)
        {
            AutoServiceLogger.WriteToLogFile($"TapTitansPathHelper.ProcessPathString: str = {filepath}");

            //If string is UrlEncoded - decode it
            string normalized;
            try
            {
                normalized = Java.Net.URLDecoder.Decode(filepath, "UTF-8");
                AutoServiceLogger.WriteToLogFile($"TapTitansPathHelper.ProcessPathString: normalized = {normalized}");
            }
            catch (System.Exception)
            {
                normalized = filepath;

                AutoServiceLogger.WriteToLogFile($"TapTitansPathHelper.ProcessPathString: Exception occured. Normalized is now {normalized}");
            }

            //convert string to Uri
            var uri = Android.Net.Uri.Parse(normalized);

            AutoServiceLogger.WriteToLogFile($"TapTitansPathHelper.ProcessPathString: created Uri from path string. Try to get Path");

            string result = UriToPath.GetActualPathFromFile(uri);

            //When string is no Uri - return normalized parameter
            if (result == null)
            {
                result = normalized;

                AutoServiceLogger.WriteToLogFile($"TapTitansPathHelper.ProcessPathString: Uri creation returned null. returning normalized string");
            }

            return result;
        }
    }
}