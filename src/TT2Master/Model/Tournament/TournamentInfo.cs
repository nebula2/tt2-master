using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using TT2Master.Loggers;
using TT2Master.Shared.Models;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// A class for receiving Tournament Info
    /// </summary>
    public static class TournamentInfo
    {
        /// <summary>
        /// List of Tournament Member
        /// </summary>
        public static List<TournamentMember> Members { get; private set; } = new List<TournamentMember>();

        /// <summary>
        /// Returns a List of Tournament Members
        /// </summary>
        /// <returns></returns>
        public static bool ConsumeTournamentMembers()
        {
            try
            {
                //Logger.WriteToLogFile($"TournamentInfo.ConsumeTournamentMembers()");

                Members = new List<TournamentMember>();

                var json = (JObject)App.Save.TournamentModel;

                if (json == null)
                {
                    //Logger.WriteToLogFile($"TournamentInfo.ConsumeTournamentMembers(): json conversion kinda failed");
                    return false;
                }

                string myName = json["player_self"]["name"].ToString();
                int myRank = JfTypeConverter.ForceInt(json["player_self"]["rank"].ToString());

                //Logger.WriteToLogFile($"TournamentInfo.ConsumeTournamentMembers(): myName: {myName}, myRank: {myRank}. Creating me in Tournament");

                var me = new TournamentMember()
                {
                    IsMyself = true,
                    Name = myName,
                    MaxStage = json["player_self"]["max_stage"].ToString(),
                    Stage = json["player_self"]["stage"].ToString(),
                    Rank = $"{myRank.ToString()} "
                };

                bool iGotAdded = false;

                string topPlayerString = json.SelectToken("top_players").ToString();

                var players = JArray.Parse(topPlayerString);

                //Logger.WriteToLogFile($"TournamentInfo.ConsumeTournamentMembers(): Looping through top_players");

                foreach (JObject item in players.Children())
                {
                    try
                    {
                        #region Me
                        //add me if i am in top 10
                        if (item.GetValue("name").ToString() == myName)
                        {
                            Members.Add(me);
                            iGotAdded = true;
                            continue;
                        }
                        #endregion

                        #region Not Me
                        //add the other idiots
                        var member = new TournamentMember()
                        {
                            Name = item.GetValue("name").ToString(),
                            Rank = $"{item.GetValue("rank").ToString()} ",
                            Stage = item.GetValue("stage").ToString(),
                            MaxStage = item.GetValue("max_stage").ToString()
                        };

                        Members.Add(member);
                        #endregion
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                //add me and the player around me if I am beneath the top 10
                if (!iGotAdded)
                {
                    Members.Add(me);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    //Logger.WriteToLogFile($"TournamentInfo.ConsumeTournamentMembers(): exception: {ex.Message}\n{ex.Data.ToString()}");
                }
                else
                {
                    //Logger.WriteToLogFile($"TournamentInfo.ConsumeTournamentMembers(): exception: exception is null");
                }
                return false;
            }
        }
    }
}