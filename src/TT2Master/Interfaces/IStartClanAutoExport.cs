using System.Security;

namespace TT2Master
{
    [SecuritySafeCritical]
    public interface IStartClanAutoExport
    {
        bool StartService(bool forceCloseBefore = false);
    }
}