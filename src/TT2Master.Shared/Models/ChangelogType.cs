using System.ComponentModel;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// The type of a changelog which determines the character of a <see cref="ChangelogItem"/>
    /// </summary>
    public enum ChangelogType
    {
        /// <summary>
        /// Version update (if the app version did increment
        /// </summary>
        [Description("VersionUpdate")]
        VersionUpdate = 0,
        /// <summary>
        /// Informational update. Use this for updates which are handled server side only
        /// </summary>
        [Description("InformationalUpdate")]
        InformationalUpdate = 1,
    }
}
