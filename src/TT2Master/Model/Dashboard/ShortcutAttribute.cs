using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Dashboard
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShortcutAttribute : Attribute
    {
        public int ShortcutId { get; set; }
        public string LocalizationKey { get; set; }

        public bool IsAvailableWithNewExport { get; set; }

        public ShortcutAttribute(int id, string key, bool availableWithNewExport)
        {
            ShortcutId = id;
            LocalizationKey = key;
            IsAvailableWithNewExport = availableWithNewExport;
        }
    }
}
