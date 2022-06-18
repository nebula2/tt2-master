namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a raid card
    /// </summary>
    public class RaidCard
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Amount of cards available
        /// </summary>
        public int CardAmount { get; set; }

        /// <summary>
        /// Is activated by GH
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Card type
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// Card tier
        /// </summary>
        public int Tier { get; set; }

        /// <summary>
        /// Is best against
        /// </summary>
        public string BestAgainst { get; set; }

        /// <summary>
        /// Max stacks
        /// </summary>
        public int MaxStacks { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Activation chance
        /// </summary>
        public double Chance { get; set; }

        /// <summary>
        /// Spatial length
        /// </summary>
        public int SpatialLength { get; set; }

        /// <summary>
        /// Base cooldown
        /// </summary>
        public int BaseCooldown { get; set; }

        /// <summary>
        /// Max card level
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public string Color { get; set; }
    }
}
