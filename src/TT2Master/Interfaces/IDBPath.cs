using System.Security;

namespace TT2Master
{
    [SecuritySafeCritical]
    public interface IDBPath
    {
        string DBPath(string dbname);
    }
}