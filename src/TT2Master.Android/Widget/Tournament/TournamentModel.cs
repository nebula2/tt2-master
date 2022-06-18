using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Droid
{
    public class TournamentModel
    {
        public string CurrentTournament { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string BonusType { get; set; }

        public string BonusAmount { get; set; }

        public string PrizeType { get; set; }

        public string PrizeText { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public TournamentModel()
        {
            CurrentTournament = "";
            StartTime = "";
            EndTime = "";
            BonusType = "";
            BonusAmount = "";
            PrizeText = "";
            PrizeType = "";
        }
    }
}
