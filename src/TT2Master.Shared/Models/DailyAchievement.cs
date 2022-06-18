namespace TT2Master.Shared.Models
{
    /// <summary>
    /// An archievement which can be completed on a daily basis
    /// </summary>
    public class DailyAchievement
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string CurrentDailyAchievement { get; set; }
        /// <summary>
        /// Progress (current amount)
        /// </summary>
        public string CurrentDailyAchievementProgress { get; set; }
        /// <summary>
        /// Total possible amount
        /// </summary>
        public string CurrentDailyAchievementTotal { get; set; }
        /// <summary>
        /// Has it been collected?
        /// </summary>
        public bool DailyAchievementCollected { get; set; }
    }
}
