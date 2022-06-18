using System.Security;

namespace TT2Master
{
    /// <summary>
    /// Interface for creating a screenshot
    /// </summary>
    [SecuritySafeCritical]
    public interface ICreateScreenshot
    {
        string Capture();
    }
}