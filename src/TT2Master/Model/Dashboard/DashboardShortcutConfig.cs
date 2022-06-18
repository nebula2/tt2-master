using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Dashboard
{
    public class DashboardShortcutConfig
    {
        public int SortId { get; set; }

        public int ShortcutId { get; set; }

        public string Name { get; set; }

        public DashboardShortcutConfig()
        {

        }

        public DashboardShortcutConfig(int sortId, int shortcutId, string name)
        {
            SortId = sortId;
            ShortcutId = shortcutId;
            Name = name;
        }
    }
}
