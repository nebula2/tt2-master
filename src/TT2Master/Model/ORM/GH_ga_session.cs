using SQLite;
using System;

namespace TT2Master
{
    /// <summary>
    /// Session table in GH database
    /// </summary>
    [Table("ga_session")]
    public class GH_ga_session
    {
        /// <summary>
        /// Session identifier
        /// </summary>
        [PrimaryKey, Column("session_Id")]
        public string Session_Id { get; set; }

        /// <summary>
        /// Session timestamp
        /// </summary>
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Event
        /// </summary>
        [Column("event")]
        public string Event { get; set; }
    }
}