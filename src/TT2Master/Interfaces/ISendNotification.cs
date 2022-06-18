using System.Security;

namespace TT2Master
{
    [SecuritySafeCritical]
    public interface ISendNotification
    {
        void ShowNotification(string header, string content);
    }
}