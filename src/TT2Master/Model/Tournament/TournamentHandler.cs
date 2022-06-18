using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TT2Master.Loggers;
using TT2Master.Shared.Helper;

namespace TT2Master.Model.Tournament
{
    /// <summary>
    /// Handles Tournament information
    /// </summary>
    public static class TournamentHandler
    {
        /// <summary>
        /// Current tournament
        /// </summary>
        public static TournamentModel TM { get; private set; } = new TournamentModel();

        /// <summary>
        /// Sets the tournament if the player is in one
        /// </summary>
        /// <param name="tm"></param>
        public static void SetTournamentModel(TournamentModel tm)
        {
            if (tm != null)
            {
                TM = tm;
            }

            if (IsPlayerInTournament(true))
            {
                ConsumeTournamentMembers();
            }
        }

        /// <summary>
        /// Returns true if the player is currently in a tournament
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInTournament(bool ignoreAbyss = false)
        {
            // if default save file is not active you are in abyssal mode
            if (!LocalSettingsORM.IsDefaultSavefileSelected && !ignoreAbyss)
            {
                return true;
            }

            if (!LocalSettingsORM.IsReadingDataFromSavefile)
            {
                return LocalSettingsORM.IsPlayerInTournament;
            }

            if (TM == null)
            {
                return false;
            }

            #region false
            if (string.IsNullOrEmpty(TM.CurrentTournament))
            {
                return false;
            }

            //If TournamentModel is not filled - you are not in a tourney
            if (TM.CurrentTournament.ToString() == "null")
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(TM.TournamentId);

            #endregion
        }

        /// <summary>
        /// Consumes cached tournament data
        /// </summary>
        /// <returns></returns>
        private static bool ConsumeTournamentMembers()
        {
            try
            {
                TM.Members = new List<Player>();

                var json = JObject.Parse(TM.CurrentTournament);

                if (json == null)
                {
                    return false;
                }

                string myName = json["cachedTournamentData"]["player_self"]["name"]["$content"].ToString();
                int myRank = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["rank"]["$content"].ToString());

                var me = new Player()
                {
                    IsMe = true,
                    ArtifactCount = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["artifacts"]["$content"].ToString()),
                    ClanCurrent = json["cachedTournamentData"]["player_self"]["clan_code"]["$content"].ToString(),
                    ClanName = json["cachedTournamentData"]["player_self"]["clan_name"]["$content"].ToString(),
                    CraftingShardsSpent = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["crafting_shards_spent"]["$content"].ToString()),
                    ClanCratesShared = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["crates_shared"]["$content"].ToString()),
                    EquipmentSetCount = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["equipment_set_count"]["$content"].ToString()),
                    StageMax = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["max_stage"]["$content"].ToString()),
                    PlayerName = myName,
                    PlayerId = json["cachedTournamentData"]["player_self"]["player_code"]["$content"].ToString(),
                    RaidTicketsCollected = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["raid_tickets_collected"]["$content"].ToString()),
                    RaidTotalXP = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["raid_xp"]["$content"].ToString()),
                    ClanRank = JfTypeConverter.ForceInt(myRank.ToString()),
                    CurrentStage = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["stage"]["$content"].ToString()),
                    TitanPoints = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["titan_points"]["$content"].ToString()),
                    RaidTotalCardLevel = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_card_level"]["$content"].ToString()),
                    TotalHelperScrolls = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_helper_scrolls"]["$content"].ToString()),
                    TotalHelperWeapons = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_helper_weapons"]["$content"].ToString()),
                    RaidAttackCount = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_num_raid_attacks"]["$content"].ToString()),
                    RaidUniqueSkillCount = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_num_unique_cards"]["$content"].ToString()),
                    TotalPetLevels = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_pet_levels"]["$content"].ToString()),
                    TotalSkillPoints = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_skill_points"]["$content"].ToString()),
                    TournamentCount = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["total_tournaments"]["$content"].ToString()),
                    WeeklyTicketCount = JfTypeConverter.ForceInt(json["cachedTournamentData"]["player_self"]["weekly_ticket_count"]["$content"].ToString()),
                };

                bool iGotAdded = false;

                string topPlayerString = json.SelectToken("cachedTournamentData").SelectToken("top_players").SelectToken("$content").ToString();

                var players = JArray.Parse(topPlayerString);

                foreach (JObject item in players.Children())
                {
                    try
                    {
                        #region Me
                        //add me if i am in top 10
                        if (item["player_code"]["$content"].ToString() == me.PlayerId)
                        {
                            TM.Members.Add(me);
                            iGotAdded = true;
                            continue;
                        }
                        #endregion

                        #region Not Me
                        //add the other idiots
                        var member = new Player()
                        {
                            IsMe = false,
                            ArtifactCount = JfTypeConverter.ForceInt(item["artifacts"]["$content"].ToString()),
                            ClanCurrent = item["clan_code"]["$content"].ToString(),
                            ClanName = item["clan_name"]["$content"].ToString(),
                            CraftingShardsSpent = JfTypeConverter.ForceInt(item["crafting_shards_spent"]["$content"].ToString()),
                            ClanCratesShared = JfTypeConverter.ForceInt(item["crates_shared"]["$content"].ToString()),
                            EquipmentSetCount = JfTypeConverter.ForceInt(item["equipment_set_count"]["$content"].ToString()),
                            StageMax = JfTypeConverter.ForceInt(item["max_stage"]["$content"].ToString()),
                            PlayerName = item["name"]["$content"].ToString(),
                            PlayerId = item["player_code"]["$content"].ToString(),
                            RaidTicketsCollected = JfTypeConverter.ForceInt(item["raid_tickets_collected"]["$content"].ToString()),
                            RaidTotalXP = JfTypeConverter.ForceInt(item["raid_xp"]["$content"].ToString()),
                            ClanRank = JfTypeConverter.ForceInt(item["rank"]["$content"].ToString()),
                            CurrentStage = JfTypeConverter.ForceInt(item["stage"]["$content"].ToString()),
                            TitanPoints = JfTypeConverter.ForceInt(item["titan_points"]["$content"].ToString()),
                            RaidTotalCardLevel = JfTypeConverter.ForceInt(item["total_card_level"]["$content"].ToString()),
                            TotalHelperScrolls = JfTypeConverter.ForceInt(item["total_helper_scrolls"]["$content"].ToString()),
                            TotalHelperWeapons = JfTypeConverter.ForceInt(item["total_helper_weapons"]["$content"].ToString()),
                            RaidAttackCount = JfTypeConverter.ForceInt(item["total_num_raid_attacks"]["$content"].ToString()),
                            RaidUniqueSkillCount = JfTypeConverter.ForceInt(item["total_num_unique_cards"]["$content"].ToString()),
                            TotalPetLevels = JfTypeConverter.ForceInt(item["total_pet_levels"]["$content"].ToString()),
                            TotalSkillPoints = JfTypeConverter.ForceInt(item["total_skill_points"]["$content"].ToString()),
                            TournamentCount = JfTypeConverter.ForceInt(item["total_tournaments"]["$content"].ToString()),
                            WeeklyTicketCount = JfTypeConverter.ForceInt(item["weekly_ticket_count"]["$content"].ToString()),
                        };

                        TM.Members.Add(member);
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
                    TM.Members.Add(me);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    //Logger.WriteToLogFile($"TournamentHandler.ConsumeTournamentMembers(): exception: {ex.Message}\n{ex.Data.ToString()}");
                }
                else
                {
                    //Logger.WriteToLogFile($"TournamentHandler.ConsumeTournamentMembers(): exception: exception is null");
                }
                return false;
            }
        }
    }
}