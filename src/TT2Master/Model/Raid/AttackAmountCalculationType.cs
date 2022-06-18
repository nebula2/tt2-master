using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Raid
{
    public enum AttackAmountCalculationType
    {
        AbsoluteInAttacks = 0,
        AbsoluteInWavesIncludingLastWave = 1,
        AbsoluteInWavesExcludingLastWave = 2,
        RelativeFromAllAttacksSum = 3,
    }
}
