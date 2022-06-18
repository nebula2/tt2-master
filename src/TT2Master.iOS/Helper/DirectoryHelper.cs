using System;
using System.IO;
using TT2Master.iOS;
using Xamarin.Forms;
using TT2Master;

[assembly: Dependency(typeof(DirectoryHelper))]

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
namespace TT2Master.iOS
{
    public class DirectoryHelper : IDirectory
    {
        private readonly string _documentBasePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private string GetExportFilepath() => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "exportfile.json");


        public string GetDocumentBasePath() => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public string GetDownloadPath() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public string GetDocumentPath() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public string CreateDirectory(string directoryName)
        {
            var directoryPath = Path.Combine(_documentBasePath, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }

        public void RemoveDirectory()
        {
            DirectoryInfo directory = new DirectoryInfo(_documentBasePath);
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public string RenameDirectory(string oldDirectoryName, string newDirectoryName)
        {
            var olddirectoryPath = Path.Combine(_documentBasePath, oldDirectoryName);
            var newdirectoryPath = Path.Combine(_documentBasePath, newDirectoryName);
            if (Directory.Exists(olddirectoryPath))
            {
                Directory.Move(olddirectoryPath, newdirectoryPath);
            }
            return newdirectoryPath;
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