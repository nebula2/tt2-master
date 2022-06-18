using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TT2Master.Model.Raid;

namespace TT2Master.ViewModels.Raid
{
    public class GroupedClanRaidAttackFlaw : ObservableCollection<ClanRaidAttackFlaw>
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
    }
}
