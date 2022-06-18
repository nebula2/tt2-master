using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master
{
    /// <summary>
    /// Possible Areas for a <see cref="DashboardItem"/>
    /// </summary>
    public enum DashboardArea
    {
        /// <summary>
        /// Informational area. Will only be shown if needed
        /// Could be Information about updates available
        /// </summary>
        Information = 0,

        /// <summary>
        /// Daily stuff like clan quests or equip drops
        /// </summary>
        ShortTerm = 1,

        /// <summary>
        /// Optimizer specific stuff. 
        /// </summary>
        LongTerm = 2,

        /// <summary>
        /// Area not set
        /// </summary>
        NotSet = 99
    }
}