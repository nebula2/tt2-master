using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Raid
{
    public class ClanRaidAttackFlaw
    {
        public FlawType Flaw { get; set; }

        public string PlayerId { get; set; }

        public string PlayerName { get; set; }

        public double Value { get; set; }

        public string FlawText { get; set; }
    }
}
