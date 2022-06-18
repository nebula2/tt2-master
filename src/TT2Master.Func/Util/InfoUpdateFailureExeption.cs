using System;
using System.Collections.Generic;
using System.Text;

namespace TT2MasterFunc.Util
{
    /// <summary>
    /// Represents errors that occur during info file update execution
    /// </summary>
    public class InfoUpdateFailureExeption : Exception
    {
        public InfoUpdateFailureExeption(string message) : base(message)
        {
        }

        public InfoUpdateFailureExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InfoUpdateFailureExeption()
        {
        }
    }
}
