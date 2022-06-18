using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master
{
    public class DashboardItem
    {
        public string ID { get; set; }

        public DashboardArea Area { get; set; }

        public string Destination { get; set; }

        public string Header { get; set; }

        public string Content { get; set; }

        public string Icon { get; set; }
    }
}