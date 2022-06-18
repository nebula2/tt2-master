using System.Collections.Generic;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// App update request. This thing is used to automatically serve updates which can be handled server side
    /// </summary>
    public class AppUpdateRequest
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Client operating system which this item is designated to
        /// </summary>
        public ClientOs Os { get; set; }

        /// <summary>
        /// Minimal client side app version
        /// </summary>
        public string AppVersionMin { get; set; }

        /// <summary>
        /// Maximum client side app version
        /// </summary>
        public string AppVersionMax { get; set; }

        /// <summary>
        /// Instructions which the client needs to execute in order for this request to be completed
        /// </summary>
        public ICollection<UpdateInstruction> Instructions { get; set; }

        /// <summary>
        /// Changelog item 
        /// </summary>
        public ChangelogItem Change { get; set; }
    }
}
