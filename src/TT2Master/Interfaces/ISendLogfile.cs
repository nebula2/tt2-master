using System.Security;

namespace TT2Master
{
    [SecuritySafeCritical]
    public interface ISendLogfile
    {
        bool SendLogfile();
    }
}