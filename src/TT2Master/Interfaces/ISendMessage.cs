using System.Security;

namespace TT2Master
{
    /// <summary>
    /// Interface for creating a Toast Message
    /// </summary>
    [SecuritySafeCritical]
    public interface ISendMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
