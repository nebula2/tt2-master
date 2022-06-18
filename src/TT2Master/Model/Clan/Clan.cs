using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    public class Clan
    {
        /// <summary>
        /// Clan-ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Name of the Clan
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the clan
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Score of the clan
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Global Rank of clan
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Name of the leader
        /// </summary>
        public string LeaderName { get; set; }

        /// <summary>
        /// Countrycode
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// True if the clan membership is private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Password to enter the clan
        /// </summary>
        public string Passwort { get; set; }

        /// <summary>
        /// RequiredStage to enter
        /// </summary>
        public double StageRequired { get; set; }

        /// <summary>
        /// Days you can be offline until you get kicked automatically
        /// </summary>
        public double AutoKickDays { get; set; }

        /// <summary>
        /// List of Players that are in this clan
        /// </summary>
        public List<Player> ClanMember { get; set; } = new List<Player>();

        /// <summary>
        /// List of messages
        /// </summary>
        public List<ClanMessage> Messages { get; set; } = new List<ClanMessage>();

        /// <summary>
        /// Raid Exp
        /// </summary>
        public double ClanRaidExp { get; set; }

        /// <summary>
        /// Raid Tickets
        /// </summary>
        public double ClanRaidTickets { get; set; }

        public double AvgMaxStage => AverageMaxStage();

        public string ClanRaidExpString => $"{ClanRaidExp:N0}";

        public double ClanMessageCount => Messages == null ? 0 : Messages.Count;

        public Clan()
        {
            ClanMember = new List<Player>();
            Messages = new List<ClanMessage>();
        }

        public Clan(Clan clan)
        {
            ID = clan.ID;
            Name            = clan.Name             ;
            Description     = clan.Description      ;
            Score           = clan.Score            ;
            Rank            = clan.Rank             ;
            LeaderName      = clan.LeaderName       ;
            CountryCode     = clan.CountryCode      ;
            IsPrivate       = clan.IsPrivate        ;
            Passwort        = clan.Passwort         ;
            StageRequired   = clan.StageRequired    ;
            AutoKickDays    = clan.AutoKickDays     ;
            ClanMember      = clan.ClanMember       ;
            Messages        = clan.Messages         ;
            ClanRaidExp        = clan.ClanRaidExp ;
            ClanRaidTickets        = clan.ClanRaidTickets;
    }

        /// <summary>
        /// Returns the average max stage of all players
        /// </summary>
        /// <returns></returns>
        private double AverageMaxStage() => Math.Round(ClanMember.Select(x => x.StageMax).ToArray().Average(),0);

        public bool ReloadMessagesFromJson(JObject json)
        {
            try
            {
                string msgString = json.SelectToken("messages").ToString();
                var msgs = JArray.Parse(msgString);

                Messages = new List<ClanMessage>();

                foreach (JObject msg in msgs.Children())
                {
                    var message = new ClanMessage()
                    {
                        ClanMessageType = msg.GetValue("message_type").ToString(),
                        Message = msg.GetValue("message").ToString(),
                        PlayerIdFrom = msg.GetValue("player_from").ToString(),
                        MemberName = msg.GetValue("name").ToString(),
                        TimeStamp = JfTypeConverter.ForceDate(msg.GetValue("timestamp").ToString()),
                        IsMe = msg.GetValue("player_from").ToString() == App.Save.ThisPlayer.PlayerId ? true : false,
                        MessageID = JfTypeConverter.ForceInt(msg.GetValue("id").ToString()),
                    };

                    #region Format Message
                    switch (message.ClanMessageType)
                    {
                        case "Message":
                            break;
                        case "BuildShare":
                            break;
                        case "MakeItRain":
                            message.Message = "Clan crate dropped";
                            break;
                        case "LevelUp":
                            message.MemberName = "System";
                            message.Message = $"{message.ClanMessageType}: {message.Message}";
                            break;
                        default:
                            message.Message = $"{message.ClanMessageType}: {message.Message}";
                            break;
                    }
                    #endregion

                    Messages.Add(message);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
