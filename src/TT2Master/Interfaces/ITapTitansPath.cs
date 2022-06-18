using System.Security;

namespace TT2Master
{
    [SecuritySafeCritical]
    public interface ITapTitansPath
    {
        /// <summary>
        /// Provides full name for Tap Titans savefile
        /// </summary>
        string GetFileName();

        /// <summary>
        /// Provides full name for Tap Titans abyssal savefile
        /// </summary>
        string GetAbyssalFileName();

        string ProcessPathString(string filepath);
    }
}