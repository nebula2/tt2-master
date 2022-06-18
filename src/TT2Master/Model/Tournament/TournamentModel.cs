using System;
using System.Collections.Generic;

namespace TT2Master
{
    /// <summary>
    /// Class that describes a Tournament
    /// </summary>
    public class TournamentModel
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string TournamentId { get; set; }

        /// <summary>
        /// Current tournament
        /// </summary>
        public string CurrentTournament { get; set; }

        /// <summary>
        /// Starting time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Type of bonus you get
        /// </summary>
        public string BonusType { get; set; }

        /// <summary>
        /// Amount of bonus you get
        /// </summary>
        public double BonusAmount { get; set; }

        /// <summary>
        /// The type of prize you can achieve
        /// </summary>
        public string PrizeType { get; set; }

        /// <summary>
        /// Prize identifier
        /// </summary>
        public int PrizeId { get; set; }

        /// <summary>
        /// Has the player retired from this tournament?
        /// </summary>
        public bool IsRetired { get; set; }

        /// <summary>
        /// Text for prize
        /// </summary>
        public string PrizeText { get; set; }

        /// <summary>
        /// Member of this tournament
        /// </summary>
        public List<Player> Members { get; set; } = new List<Player>();

        /// <summary>
        /// Ctor
        /// </summary>
        public TournamentModel()
        {
            CurrentTournament = "";
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            BonusType = "";
            BonusAmount = 0;
            PrizeText = "";
            PrizeType = "";
        }
    }
}