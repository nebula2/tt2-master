namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a Tournament Member
    /// </summary>
    public class TournamentMember
    {
        /// <summary>
        /// Is this me?
        /// </summary>
        public bool IsMyself { get; set; }

        /// <summary>
        /// Current Rank
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// Player Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current Stage
        /// </summary>
        public string Stage { get; set; }

        /// <summary>
        /// Max Stage
        /// </summary>
        public string MaxStage { get; set; }

        /// <summary>
        /// Default ctor
        /// </summary>
        public TournamentMember()
        {
            IsMyself = false;
            Rank = "Rank";
            Name = "Name";
            Stage = "Stage";
            MaxStage = "MaxStage";
        }
    }
}
