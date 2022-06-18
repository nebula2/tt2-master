using System;

namespace TT2Master.Helpers
{
    /// <summary>
    /// Exception event data
    /// </summary>
    public class CustErrorEventArgs
    {
        /// <summary>
        /// Exception data
        /// </summary>
        public Exception MyException { get; set; }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="e"></param>
        public CustErrorEventArgs(Exception e)
        {
            MyException = e;
        }
    }
}