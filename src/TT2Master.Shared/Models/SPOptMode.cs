namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Enum of possible modes for SP optimization
    /// </summary>
    public enum SPOptMode
    {
        /// <summary>
        /// Continuous mode (build upon current stats)
        /// </summary>
        Continuous = 0,
        /// <summary>
        /// Creation mode (build fresh from start)
        /// </summary>
        Creation = 1,
    }
}
