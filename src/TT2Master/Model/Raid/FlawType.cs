using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TT2Master.Model.Raid
{
    public enum FlawType
    {
        [Description("BelowAvgDamage")]
        BelowAvgDamage = 0,
        [Description("AboveMaxOverKill")]
        AboveMaxOverKill = 1,
        [Description("BelowMinAttacks")]
        BelowMinAttacks = 2
    }
}
