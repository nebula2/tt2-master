using System;
using SQLite;

namespace TT2Master
{
    /// <summary>
    /// Describes a single Clan Message that has been sent
    /// </summary>
    [Table("CLANMESSAGE")]
    public class ClanMessage
    {
        /// <summary>
        /// ID of the message
        /// </summary>
        [PrimaryKey]
        public int MessageID { get; set; }

        public string ClanMessageType { get; set; }

        /// <summary>
        /// ID of the clan this msg where sent to
        /// </summary>
        public string ClanCode { get; set; }

        /// <summary>
        /// Message that has been sent in text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// From whom was this sent
        /// </summary>
        public string PlayerIdFrom { get; set; }

        /// <summary>
        /// Name of the Member
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// When was this sent
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Has this been sent from me? True if yes
        /// </summary>
        public bool IsMe { get; set; }
        [Ignore]
        public string Color => GetColorByMessageType();

        private string GetColorByMessageType()
        {
            switch (ClanMessageType)
            {
                case "Join":
                    return "#0ea336";
                case "Leave":
                case "Kick":
                    return "#d0301f";
                case "MakeItRain":
                    return "#95a515";
                default:
                    return "#FFFFFF";
            }
        }

        [Ignore]
        public string FilterString => $"{MemberName}\n{Message}";

        public ClanMessage()
        {
        }
    }
}
