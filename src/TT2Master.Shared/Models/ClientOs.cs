using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Client side operating system
    /// </summary>
    public enum ClientOs
    {
        /// <summary>
        /// Android client
        /// </summary>
        [Description("Android")]
        Android = 0,
        /// <summary>
        /// iOS client
        /// </summary>
        [Description("iOS")]
        iOS = 1,
        /// <summary>
        /// Web client
        /// </summary>
        [Description("Web")]
        Web = 2,
        /// <summary>
        /// UWP client
        /// </summary>
        [Description("Uwp")]
        Uwp = 3,
        /// <summary>
        /// All clients
        /// </summary>
        [Description("All")]
        All = 4
    }
}
