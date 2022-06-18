using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TT2Master.Droid
{
    public class Stage
    {
        public int CurrentStage { get; set; }
        public bool AutoAdvance { get; set; }
        public int EnemyKillCount { get; set; }
        public bool BossDefeated { get; set; }
        public int MegaBombEffectEndStage { get; set; }

        public Stage()
        {
            CurrentStage = 0;
            AutoAdvance = false;
            EnemyKillCount = 0;
            BossDefeated = false;
            MegaBombEffectEndStage = 0;
        }
    }
}