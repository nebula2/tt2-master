using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Dashboard
{
    public class AvailableShortcut
    {
        public int ShortcutId { get; set; }

        public string LocalizationKey { get; set; }

        public string Name { get; set; }

        public Type ShortcutType { get; set; }
    }
}
