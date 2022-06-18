using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.DataSource
{
    public class ClipboardSfImporterResult
    {
        public bool IsSuccessful { get; set; }

        public string AdditionalInformation { get; set; }

        public ClipboardSfImporterError ImportError { get; set; }

        public ClipboardSfImporterResult(bool success, ClipboardSfImporterError error, string additionalInfo = null)
        {
            IsSuccessful = success;
            ImportError = error;
            AdditionalInformation = additionalInfo;
        }

        public ClipboardSfImporterResult(bool success) : this(success, ClipboardSfImporterError.None) { }

        public ClipboardSfImporterResult() { }
    }
}
