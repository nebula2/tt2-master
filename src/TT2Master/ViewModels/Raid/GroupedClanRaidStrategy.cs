using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TT2Master.Model.Raid;

namespace TT2Master.ViewModels.Raid
{
    public class GroupedClanRaidStrategy : ObservableCollection<RaidStrategy>
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
    }
}
