namespace TT2MasterAdministrationApp.Shared
{
    public class InfofileAssetDownloadResult
    {
        public bool IsSuccessful { get; set; }
        public bool IsVersionAlreadyExisting { get; set; }
        public int DownloadedInfofilesCount { get; set; }
        public string Message { get; set; }
    }
}
