namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes an ingame stage
    /// </summary>
    public class Stage
    {
        /// <summary>
        /// Stage number
        /// </summary>
        public int CurrentStage { get; set; }
        /// <summary>
        /// Is auto advance enabled ? (Silent March)
        /// </summary>
        public bool AutoAdvance { get; set; }

        /// <summary>
        /// How many enemies has been killed this stage?
        /// </summary>
        public int EnemyKillCount { get; set; }

        /// <summary>
        /// Is the boss defeated?
        /// </summary>
        public bool BossDefeated { get; set; }

        /// <summary>
        /// When does Snap end?
        /// </summary>
        public int MegaBombEffectEndStage { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
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
