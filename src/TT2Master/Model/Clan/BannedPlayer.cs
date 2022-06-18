using SQLite;

namespace TT2Master
{
    /// <summary>
    /// A banned player
    /// </summary>
    public class BannedPlayer
    {
        #region Props
        /// <summary>
        /// Identifier of the player
        /// </summary>
        [PrimaryKey]
        public string ID { get; set; }

        /// <summary>
        /// Last known name of Player
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Possible new name
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// Optional reason for ban
        /// </summary>
        public string Reason { get; set; }

        #endregion

        #region Ctor
        public BannedPlayer()
        {
            ID = "";
            Name = "";
            Reason = "";
        }

        public BannedPlayer(string id, string name)
        {
            ID = id;
            Name = name;
            Reason = "";
        }

        public BannedPlayer(string id, string name, string reason)
        {
            ID = id;
            Name = name;
            Reason = reason;
        } 
        #endregion
    }
}
