using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.DataSource
{
    public enum ClipboardSfImporterError
    {
        None = 0,
        ClipboardEmpty = 1,
        MalformattedClipboardData = 2,
        SameDataAsBefore = 3,
        InternalError = 4,
    }
}
