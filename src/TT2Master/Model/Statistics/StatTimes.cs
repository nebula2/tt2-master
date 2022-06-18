using System.Collections.Generic;

namespace TT2Master
{
    /// <summary>
    /// Times to pick for statistics
    /// </summary>
    public static class StatTimes
    {
        /// <summary>
        /// Available times
        /// </summary>
        public static List<StatTime> Times { get; set; } = new List<StatTime>()
        {
            new StatTime()
            {
                ID = 0,
                Days = 7,
                Description = "7",
            },
            new StatTime()
            {
                ID = 1,
                Days = 30,
                Description = "30",
            },
            new StatTime()
            {
                ID = 2,
                Days = 60,
                Description = "60",
            },
            new StatTime()
            {
                ID = 3,
                Days = 100,
                Description = "100"
            },
        };
    }
}