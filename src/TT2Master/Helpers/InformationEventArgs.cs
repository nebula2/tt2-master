using System;

namespace TT2Master.Helpers
{
    /// <summary>
    /// Informational Event
    /// </summary>
    public class InformationEventArgs : EventArgs
    {
        /// <summary>
        /// Information string
        /// </summary>
        public string Information { get; set; } = "";

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="info"></param>
        public InformationEventArgs(string info)
        {
            Information = info;
        }
    }
}