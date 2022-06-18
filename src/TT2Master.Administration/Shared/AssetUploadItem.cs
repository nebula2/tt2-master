namespace TT2MasterAdministrationApp.Shared
{
    public class AssetUploadItem
    {
        public string ContainerReference { get; set; }
        public string Version { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
