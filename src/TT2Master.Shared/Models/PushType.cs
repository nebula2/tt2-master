using System;
using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of Push Types
    /// </summary>
    [Obsolete("This is not needed anymore as offline builds got removed")]
    public enum PushType
    {
        /// <summary>
        /// Online
        /// </summary>
        [Description("Online")]
        Online,
        /// <summary>
        /// Offline (it is obsolete since 3.8)
        /// </summary>
        [Description("Offline")]
        Offline,
        /// <summary>
        /// Not set
        /// </summary>
        [Description("NotSet")]
        NotSet
    }
}
