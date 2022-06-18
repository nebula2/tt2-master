using System;
using System.IO;
using Xamarin.Forms;
using TT2Master.Droid;
using Android.OS;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(DirectoryHelper))]
namespace TT2Master.Droid
{
    public class DirectoryHelper : IDirectory
    {
        private string GetExportFilepath() => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "exportfile.json");

        public string GetDocumentBasePath() => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
            : System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public string GetDownloadPath() => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            //? Path.Combine(Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).AbsolutePath, "TT2Master")
#pragma warning disable CS0618 // Type or member is obsolete
            ? Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, "TT2Master")
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
            : Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, "TT2Master");
#pragma warning restore CS0618 // Type or member is obsolete

        public string GetDocumentPath() => (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            //? Path.Combine(Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments).AbsolutePath, "TT2Master")
#pragma warning disable CS0618 // Type or member is obsolete
            ? Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDocuments, "TT2Master")
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
            : Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDocuments, "TT2Master");
#pragma warning restore CS0618 // Type or member is obsolete

        public string CreateDirectory(string directoryName)
        {
            var directoryPath = Path.Combine(GetDocumentBasePath(), directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }

        public void RemoveDirectory()
        {
            var directory = new DirectoryInfo(GetDocumentBasePath());
            foreach (var dir in directory.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public string RenameDirectory(string oldDirectoryName, string newDirectoryName)
        {
            string oldDirectoryPath = Path.Combine(GetDocumentBasePath(), oldDirectoryName);
            string newDirectoryPath = Path.Combine(GetDocumentBasePath(), newDirectoryName);
            if (!Directory.Exists(oldDirectoryPath))
            {
                Directory.Move(oldDirectoryPath, newDirectoryPath);
            }
            return newDirectoryPath;
        }

        public string GetDownloadPathName(string filename) => Path.Combine(GetDownloadPath(), filename);

        public string GetWidgetLoggerPath() => Path.Combine(GetDocumentPath(), "tt2widgetlogger.txt");

        public string GetDocumentName(string filename) => Path.Combine(GetDocumentPath(), filename);

        public string CopySaveFileAndGetPath()
        {
            var dbPathHelper = new DBPathHelper();
            string savefilePath = dbPathHelper.DBPath("mySave.adat");

            // save file
            if (System.IO.File.Exists(savefilePath))
            {
                string destination = GetDocumentName("mySave.adat");
                File.Copy(savefilePath, destination, true);

                return destination;
            }

            return null;
        }

        public bool SaveTapTitansExportFile(string content)
        {
            try
            {
                DeleteTapTitansExportFile();
                
                File.WriteAllText(GetExportFilepath(), content);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetTapTitansExportFile()
        {
            string filePath = GetExportFilepath();

            return !File.Exists(filePath) ? null : File.ReadAllText(filePath);
        }

        public string GetTapTitansExportFilePath() => GetExportFilepath();

        public bool DeleteTapTitansExportFile()
        {
            string filePath = GetExportFilepath();

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
