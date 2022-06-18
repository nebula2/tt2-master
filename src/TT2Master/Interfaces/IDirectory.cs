using System.Security;

namespace TT2Master
{
    /// <summary>
    /// Directory-Access
    /// </summary>
    [SecuritySafeCritical]
    public interface IDirectory
    {
        string CreateDirectory(string directoryName);

        void RemoveDirectory();

        string RenameDirectory(string oldDirectoryName, string newDirectoryName);

        string GetDownloadPathName(string filename);

        string GetDownloadPath();

        string GetDocumentName(string filename);

        string GetDocumentPath();

        string GetWidgetLoggerPath();

        string CopySaveFileAndGetPath();

        bool SaveTapTitansExportFile(string content);

        string GetTapTitansExportFile();

        string GetTapTitansExportFilePath();

        bool DeleteTapTitansExportFile();
    }
}