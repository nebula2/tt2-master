namespace TT2MasterAdministrationApp.Shared
{
    public class AssetValidationResult
    {
        public bool IsSuccessful { get; set; }
        public int TestsFailedCount { get; set; }
        public int TestsSkippedCount { get; set; }
        public int TestSucceededCount { get; set; }
    }
}
