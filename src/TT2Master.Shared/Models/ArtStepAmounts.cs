using System.Collections.Generic;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Object to hold possible values for <see cref="ArtStepAmount"/>
    /// </summary>
    public static class ArtStepAmounts
    {
        /// <summary>
        /// List of possible <see cref="ArtStepAmount"/>
        /// </summary>
        public static List<ArtStepAmount> StepAmounts = new List<ArtStepAmount>()
        {
            new ArtStepAmount()
            {
                ID = 0,
                Value = 1,
                Description = "1",
                IsInPercent = false,
            },
            new ArtStepAmount()
            {
                ID = 1,
                Value = 10,
                Description = "10",
                IsInPercent = false,
            },
            new ArtStepAmount()
            {
                ID = 2,
                Value = 100,
                Description = "100",
                IsInPercent = false,
            },
            new ArtStepAmount()
            {
                ID = 3,
                Value = 1000,
                Description = "1000",
                IsInPercent = false,
            },
            new ArtStepAmount()
            {
                ID = 9,
                Value = 1,
                Description = "1 %",
                IsInPercent = true,
            },
            new ArtStepAmount()
            {
                ID = 6,
                Value = 5,
                Description = "5 %",
                IsInPercent = true,
            },
            new ArtStepAmount()
            {
                ID = 7,
                Value = 25,
                Description = "25 %",
                IsInPercent = true,
            },
            new ArtStepAmount()
            {
                ID = 8,
                Value = 100,
                Description = "100 %",
                IsInPercent = true,
            },
        };
    }
}
