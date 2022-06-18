using System.Security;

namespace TT2Master
{
    /// <summary>
    /// Boss-Service-Access
    /// </summary>
    [SecuritySafeCritical]
    public interface IAutomationService
    {
        /// <summary>
        /// Start the Service
        /// </summary>
        /// <returns>True if sucessful, else false</returns>
        bool StartService();

        /// <summary>
        /// Stops the service
        /// </summary>
        /// <returns></returns>
        bool StopService();

        /// <summary>
        /// Invokes an update of the content
        /// </summary>
        /// <returns></returns>
        bool UpdateContent();
    }
}