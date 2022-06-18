using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TT2Master.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Deletes a file
        /// </summary>
        public static bool DeleteFile(string dir, string path)
        {
            try
            {
                //create path if non existant
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                //delete if existant
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Writes text to file in download directory
        /// </summary>
        /// <param name="text"></param>
        public static string WriteFileToDownloads(string text, string filename)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(filename))
            {
                return "";
            }

            // get path and dir
            string path = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPathName(filename);
            string dir = Xamarin.Forms.DependencyService.Get<IDirectory>().GetDownloadPath();

            // Check path and create if non existant
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // write
            using (var sw = new StreamWriter(path, false, Encoding.Default))
            {
                sw.Write(text);
                sw.Close();
            }

            return path;
        }

        /// <summary>
        /// Writes text fo file in cache directory
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<string> WriteAndShareFileAsync(string text, string filename)
        {
            var filePath = Path.Combine(FileSystem.CacheDirectory, filename);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.WriteAllText(filePath, text);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = filename,
                File = new ShareFile(filePath)
            });

            return filePath;
        }

        public static async Task<string> WriteAndShareFileAsCsvAsync<T>(IEnumerable<T> collection, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) return "";

            filename = filename.EndsWith(".csv") ? filename : filename + ".csv";

            var filePath = Path.Combine(FileSystem.CacheDirectory, filename);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(collection);
            }

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = filename,
                File = new ShareFile(filePath)
            });

            return filePath;
        }

        /// <summary>
        /// Writes text fo file in cache directory
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<bool> ShareFileAsync(string message, string sourceFile)
        {
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = message,
                File = new ShareFile(sourceFile),
            });

            return true;
        }


        /// <summary>
        /// Copies a file safely. for more info see here: https://github.com/xamarin/xamarin-android/issues/3426
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyFileSafely(string source, string destination)
        {
            // if the source file does not exist then the following operation will fail anyway
            if (!File.Exists(source))
            {
                throw new Exception($"Could not find source file: {source}");
            }

            // just to be really really sure - delete existing file!
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            // read all bytes from source file
            byte[] fileBytes = File.ReadAllBytes(source);

            // write all bytes to destination file
            File.WriteAllBytes(destination, fileBytes);
        }
    }
}