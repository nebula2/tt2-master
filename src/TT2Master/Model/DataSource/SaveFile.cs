using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using TT2Master.Resources;
using System.Linq;
using TT2Master.Model.Tournament;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Helpers;
using TT2Master.Model.Arti;
using TT2Master.Model.DataSource;
using TT2Master.Shared.Models;
using TT2Master.Shared.Helper;
using TT2Master.Model;

namespace TT2Master
{
    /// <summary>
    /// Class to interact with the Tap Titans Save file
    /// </summary>
    public class SaveFile
    {
        #region Properties
        public bool IsHavingSavefileContent { get; set; }

        public JObject SaveObject { get; set; }

        /// <summary>
        /// Dictionary of ProfileData
        /// </summary>
        public IDictionary<string, JToken> ProfileData { get; private set; }

        #region Passive Skills
        /// <summary>
        /// PassiveSkillModel
        /// </summary>
        public IDictionary<string, JToken> PassiveSkillModel { get; private set; }

        public int LessMonstersLevel { get; private set; }
        public int PetSplashSkipLevel { get; private set; }
        public int ClanShipSplashSkipLevel { get; private set; }
        public int SorcererSplashSkipLevel { get; private set; }
        public int RaidCardPowerLevel { get; private set; }
        public int SilentMarchLevel { get; private set; }
        #endregion

        /// <summary>
        /// Dictionary of ArtifactModel
        /// </summary>
        public IDictionary<string, JToken> ArtifactModel { get; private set; }

        /// <summary>
        /// Dictionary of EquipmentModel
        /// </summary>
        public IDictionary<string, JToken> EquipmentModel { get; private set; }
        
        public JArray EquipmentClipboardModel { get; private set; }

        /// <summary>
        /// Dictionary of RaidCardModel
        /// </summary>
        public IDictionary<string, JToken> RaidCardModel { get; private set; }

        /// <summary>
        /// Dictionary of PetModel
        /// </summary>
        public IDictionary<string, JToken> PetModel { get; private set; }

        /// <summary>
        /// Dictionary of AccountModel
        /// </summary>
        public IDictionary<string, JToken> AccountModel { get; private set; }

        /// <summary>
        /// Dictionary of SkilltreeModel
        /// </summary>
        public IDictionary<string, JToken> SkillTreeModel { get; private set; }

        /// <summary>
        /// Dictionary of ClanModel
        /// </summary>
        public IDictionary<string, JToken> ClanModel { get; private set; }

        /// <summary>
        /// Dictionary of TournamentModel
        /// </summary>
        public IDictionary<string, JToken> TournamentModel { get; private set; }

        /// <summary>
        /// Dictionary of TournamentData
        /// </summary>
        public static IDictionary<string, JToken> TournamentData { get; set; }

        /// <summary>
        /// Dictionary of StageLogicController
        /// </summary>
        private static IDictionary<string, JToken> StageLogicController { get; set; }

        /// <summary>
        /// Dictionary of OptionsController
        /// </summary>
        private static IDictionary<string, JToken> OptionsController { get; set; }

        /// <summary>
        /// True, if the savefile is loaded
        /// </summary>
        public bool Loaded { get; private set; }

        public Player ThisPlayer { get; set; }

        public Clan ThisClan { get; set; } = new Clan();

        public double CurrentRelics { get; set; }

        public static double RelicsReceived { get; set; }

        public static int SPReceived { get; set; }

        public static int SPSpent { get; set; }

        public static TournamentModel Tournament { get; private set; } = new TournamentModel();

        public static Stage MyStage = new Stage();

        public int CraftingPower { get; set; }
        public int ShardsSpent { get; set; }
        public int ShardsReceived { get; set; }

        /// <summary>
        /// Tap Titans version from savefile
        /// </summary>
        public string TapTitansVersion { get; set; } = null;

        /// <summary>
        /// Is rounding enabled ?
        /// </summary>
        public bool RoundedUpgrade { get; set; } = false;

        /// <summary>
        /// Starting stage from PrestigeModel
        /// </summary>
        public double StartingStage { get; set; }

        public int NumDailyDiamondFairiesCollected { get; set; }

        public const int NumDailyDiamondFairiesCollectedMax = 5;

        public int NumDailyPerkFairiesCollected { get; set; }
        public const int NumDailyPerkFairiesCollectedMax = 1;

        public int ServerEquipmentLeftToFarm { get; set; }
        public const int ServerEquipmentLeftToFarmMax = 20;

        private DBRepository _dbRepo;
        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        public SaveFile(DBRepository dbRepo)
        {
            Loaded = false;
            _dbRepo = dbRepo;
        }
        #endregion

        #region public Methods
        /// <summary>
        /// Initialize App from File
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Initialize(
              bool loadPlayer = true
            , bool loadAccountModel = true
            , bool loadArtifacts = true
            , bool loadSkills = true
            , bool loadClan = true
            , bool loadTournament = true
            , bool loadEquipment = true)
        {
            string finalString = "";
            try
            {
                finalString = LocalSettingsORM.IsReadingDataFromSavefile 
                    ? GetSaveFileContentString()
                    : GetExportFileContentString();
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Savefile", new CustErrorEventArgs(ex));
                Loaded = false;

                return Loaded;
            }

            if (string.IsNullOrWhiteSpace(finalString))
            {
                IsHavingSavefileContent = false;
                return true;
            }
            else
            {
                IsHavingSavefileContent = true;
            }

            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("processing files"));

            try
            {
                //Load json into objects
                if (LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    await LoadObjectsAsync(finalString, loadPlayer, loadAccountModel, loadArtifacts, loadSkills, loadClan, loadTournament, loadEquipment);
                }
                else
                {
                    await LoadObjectsFromExportFileAsync(finalString, loadPlayer, loadAccountModel, loadArtifacts, loadSkills, loadClan, loadTournament, loadEquipment);
                }
                Loaded = true;
            }
            catch (Exception e)
            {
                OnError?.Invoke("SaveFile", new CustErrorEventArgs(e));
                Loaded = false;
            }

            return Loaded;
        }
        #endregion

        #region Private Methods

        #region Savefile as data source
        private static string GetSaveFileContentString()
        {
            string json = "";

            try
            {
                string decryptedFile = "";

                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("Getting file"));

                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading file"));

                try
                {
                    string myAppDir = LocalSettingsORM.IsDefaultSavefileSelected 
                        ? LocalSettingsORM.TT2TempSavefilePath
                        : LocalSettingsORM.TT2TempAbyssSavefilePath;

                    OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"myAppDir: {myAppDir}"));

                    FileHelper.CopyFileSafely(SavefileHandler.GetSavefilePath(), myAppDir);

                    OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"file copied"));

                    decryptedFile = SaveFileReader.DecryptSaveFile(myAppDir);
                    OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"decrypted copied file"));
                }
                catch (Exception e)
                {
                    OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"Error processing files the normal way: {e.Message}"));
                    decryptedFile = SaveFileReader.DecryptSaveFile(SavefileHandler.GetSavefilePath());
                }

                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"removing content that is not needed"));

                //remove content not needed
                int start = decryptedFile.IndexOf("saveString");
                int endPos = decryptedFile.IndexOf("playerData");

                if (start < 0 || endPos < 0)
                {
                    OnError?.Invoke("SaveFile", new CustErrorEventArgs(new Exception("could not read complete file")));
                }

                json = decryptedFile.Substring(0, endPos - 2) + "}";
            }
            catch (Exception ex)
            {
                OnError?.Invoke("SaveFile", new CustErrorEventArgs(ex));
            }

            return json ?? "";
        }

        /// <summary>
        /// Loads a raw string in JSON format into Dictionaries
        /// </summary>
        /// <param name="rawjson"></param>
        private async Task LoadObjectsAsync(
              string rawjson
            , bool loadPlayer
            , bool loadAccountModel
            , bool loadArtifacts
            , bool loadSkills
            , bool loadClan
            , bool loadTourney
            , bool loadEquipment)
        {
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("thinking about savefiles in general"));

            var json = JObject.Parse(rawjson);

            SaveObject = JObject.Parse((string)json.GetValue("saveString"));

            if (loadPlayer)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading playerdata"));
                _ = await LoadPlayerDataAsync();
            }

            if (loadAccountModel)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading AccountModel"));
                LoadAccountModel();
            }

            if (loadArtifacts)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Artifacts"));
                LoadArtifacts();
            }

            if (loadSkills)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Skills"));
                LoadSkillTreeModel();
            }

            if (loadClan)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Clan data"));
                _ = LoadClanModelAsync();
            }

            if (loadTourney)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Tournament data"));
                LoadTourneyModel();
            }

            if (loadEquipment)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Equipment data"));
                LoadEquipmentModel();
            }

            LoadStageController();
            LoadPetModel();
            LoadRaidCardModel();
            LoadTapTitansVersion();
            LoadOptionsController();
        }

        /// <summary>
        /// Load TT2 game options
        /// </summary>
        private void LoadOptionsController()
        {
            try
            {
                OptionsController = (JObject)SaveObject.SelectToken("OptionsController");

                RoundedUpgrade = JfTypeConverter.ForceBool(OptionsController["roundedUpgrade"]["$content"].ToString());
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"LoadOptionsController ERROR: {ex.Message}"));
            }
        }

        /// <summary>
        /// Loads TapTitansVersion froms GameModel
        /// </summary>
        private void LoadTapTitansVersion()
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("Loading Tap Titans version"));
                
                var json = (JObject)SaveObject.SelectToken("GameModel");

                if(json == null)
                {
                    OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("could not get GameModel"));
                    return;
                }

                if (!json.ContainsKey("savedAppVersion"))
                {
                    OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("could not get savedAppVersion inside GameModel"));
                    return;
                }

                TapTitansVersion = json["savedAppVersion"]["$content"].ToString();
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("LoadTapTitansVersion: " + e.Message));
            }
        }

        /// <summary>
        /// Updates Player data async
        /// </summary>
        /// <returns></returns>
        private async Task<int> LoadPlayerDataAsync()
        {
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading player data"));
            ProfileData = (JObject)SaveObject.SelectToken("AccountModel.playerProfileData");

            ThisPlayer = new Player()
            {
                AuthToken = SaveObject["AccountModel"]["authToken"]["$content"]?.ToString(),
                ClanCratesShared = JfTypeConverter.ForceInt(ProfileData["clanCratesShared"]),
                ClanCurrent = ProfileData["clanCode"]?.ToString(),
                ClanRole = ProfileData["clanRole"]?.ToString(),
                CountryCode = ProfileData["countryCode"]?.ToString(),
                CurrentStage = JfTypeConverter.ForceDoubleUniversal(SaveObject["StageLogicController"]["currentStage"]["$content"]),
                IsMe = true,
                LastTimestamp = DateTime.Now.ToString("yyyy.MM.dd H:mm:ss"),
                PlayerId = ProfileData["playerID"]?.ToString(),
                PlayerIdHash = SaveObject["AccountModel"]["playerID"]["$content"]?.ToString(),
                PlayerName = ProfileData["playerName"]?.ToString(),
                StageMax = JfTypeConverter.ForceDoubleUniversal(ProfileData["stageCount"]),
                TournamentCount = JfTypeConverter.ForceInt(ProfileData["totalTournamentsCount"]),
                TournamentMaxRank = JfTypeConverter.ForceInt(ProfileData["highestTournamentRank"]),
                PrestigeCount = JfTypeConverter.ForceDoubleUniversal(SaveObject["AchievementModel"]["PrestigeCountprogress"]["$content"]?.ToString()),
                TotalSkillPoints = JfTypeConverter.ForceInt(SaveObject["PlayerModel"]["skillPointsReceivedServer"]["$content"]?.ToString()),
                UndisputedWins = JfTypeConverter.ForceInt(ProfileData["undisputedCount"]),
            };

            try
            {
                ThisPlayer.DustSpent = JfTypeConverter.ForceInt(SaveObject["PlayerModel"]["cardCurrencySpentServer"]["$content"]?.ToString());
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"SafeFile Error: could not load dust spent -> {e.Message}"));
                ThisPlayer.DustSpent = 0;
            }

            LoadShardsInformation();

            LoadHelperInformation();

            LoadPrestigeModel();

            LoadDailyInformation();

            LoadPassiveSkills();

            EnsureCorrectPlayerData();

            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("updating player data"));

            return await _dbRepo.UpdatePlayerAsync(ThisPlayer);
        }

        private void LoadPassiveSkills()
        {
            try
            {
                PassiveSkillModel = (JObject)SaveObject.SelectToken("PassiveSkillModel");

                LessMonstersLevel = JfTypeConverter.ForceInt(PassiveSkillModel["allPassiveSkillIdLevels"]["LessMonsters"]?.ToString());
                PetSplashSkipLevel = JfTypeConverter.ForceInt(PassiveSkillModel["allPassiveSkillIdLevels"]["PetSplashSkip"]?.ToString());
                ClanShipSplashSkipLevel = JfTypeConverter.ForceInt(PassiveSkillModel["allPassiveSkillIdLevels"]["ClanShipSplashSkip"]?.ToString());
                SorcererSplashSkipLevel = JfTypeConverter.ForceInt(PassiveSkillModel["allPassiveSkillIdLevels"]["SorcererSplashSkip"]?.ToString());
                RaidCardPowerLevel = JfTypeConverter.ForceInt(PassiveSkillModel["allPassiveSkillIdLevels"]["RaidCardPower"]?.ToString());
                SilentMarchLevel = JfTypeConverter.ForceInt(PassiveSkillModel["allPassiveSkillIdLevels"]["SilentMarch"]?.ToString());
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Passive Skills" + ex.Message));
            }
        }

        private void LoadDailyInformation()
        {
            try
            {
                NumDailyDiamondFairiesCollected = JfTypeConverter.ForceInt(SaveObject["FairyController"]["NumDailyDiamondFairiesCollected"]["$content"]?.ToString());
                NumDailyPerkFairiesCollected = JfTypeConverter.ForceInt(SaveObject["FairyController"]["NumDailyPerkFairiesCollected"]["$content"]?.ToString());
                ServerEquipmentLeftToFarm = JfTypeConverter.ForceInt(SaveObject["EquipmentModel"]["ServerEquipmentLeftToFarm"]["$content"]?.ToString());
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR: Could not load daily information: " + ex.Message + "\n"));
            }
        }

        /// <summary>
        /// Loads prestige model
        /// </summary>
        private void LoadPrestigeModel()
        {
            try
            {
                OnProgressMade?.Invoke("Savefile", new InformationEventArgs("reading prestige model"));
                var prestigeModel = (JObject)SaveObject.SelectToken("PrestigeModel");

                StartingStage = JfTypeConverter.ForceDoubleUniversal(prestigeModel["startingStage"]["$content"].ToString());
            }
            catch (Exception ex)
            {
                OnProgressMade?.Invoke("Savefile", new InformationEventArgs($"ERROR reading prestige model: {ex.Message}"));
            }
        }

        /// <summary>
        /// Updates Clan data async
        /// </summary>
        /// <returns></returns>
        private int LoadClanModelAsync()
        {
            #region Setting stuff up
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs(AppResources.ReadingClanData));

            //If player is currently not in a clan, return
            if (string.IsNullOrWhiteSpace(ThisPlayer.ClanCurrent))
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"ThisPlayer.ClanCurrent is null or white space"));
                ThisClan = new Clan();
                return 0;
            }

            ClanModel = (JObject)SaveObject.SelectToken("ClanModel");

            if (ClanModel == null)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"ClanModel is null"));
                ThisClan = new Clan();
                return 0;
            }
            #endregion

            #region Populate Clan
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"populating clan"));

            try
            {
                var clan = (JObject)ClanModel["clan"];

                if (clan == null)
                {
                    ThisClan = new Clan();
                    return 0;
                }
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs(e.Message + "\n" + ClanModel.ToString()));
            }
            try
            {
                ThisClan = new Clan()
                {
                    ID = ClanModel["clan"]["clanCode"]?.ToString(),
                    Name = ClanModel["clan"]["name"]?.ToString(),
                    Description = ClanModel["clan"]["description"]?.ToString(),
                    Score = JfTypeConverter.ForceDoubleUniversal(ClanModel["clan"]["clanRaidExp"]?.ToString()),
                    Rank = 0,
                    LeaderName = ClanModel["clan"]["leaderName"]?.ToString(),
                    CountryCode = ClanModel["clan"]["countryCode"]?.ToString(),
                    IsPrivate = ClanModel["clan"]["isPrivateClan"]?.ToString() == "true" ? true : false,
                    Passwort = ClanModel["clan"]["passCode"]?.ToString(),
                    StageRequired = JfTypeConverter.ForceInt(ClanModel["clan"]["stageRequirement"]?.ToString()),
                    AutoKickDays = JfTypeConverter.ForceDoubleUniversal(ClanModel["clan"]["autoKickDays"]?.ToString()),
                    ClanRaidExp = JfTypeConverter.ForceDoubleUniversal(ClanModel["clan"]["clanRaidExp"]?.ToString()),
                    ClanRaidTickets = JfTypeConverter.ForceDoubleUniversal(ClanModel["clan"]["raidTickets"]?.ToString()),
                };
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs(e.Message + "\n" + ClanModel.ToString()));
                ThisClan = new Clan();
            }

            #endregion

            #region Populate Member
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"populating clan member"));
                int memberNum = 1;

                foreach (var player in ClanModel["clanMembers"].Children())
                {
                    var content = player.Children().FirstOrDefault();

                    if (!content.HasValues)
                    {
                        continue;
                    }

                    string myId = ThisPlayer == null ? "?" : ThisPlayer.PlayerId;

                    var member = new Player()
                    {
                        IsMe = ThisPlayer.PlayerId == content["playerID"]?.ToString(),
                        PlayerId = content["playerID"]?.ToString(),
                        PlayerName = content["playerName"]?.ToString(),
                        CurrentStage = JfTypeConverter.ForceDoubleUniversal(content["currentStage"]?.ToString()),
                        StageMax = JfTypeConverter.ForceDoubleUniversal(content["stageCount"]?.ToString()),
                        WeeklyTicketCount = JfTypeConverter.ForceInt(content["weeklyTicketCount"]?.ToString()),
                        RaidTicketsCollected = JfTypeConverter.ForceInt(content["raidTicketsCollected"]?.ToString()),
                        ClanRank = JfTypeConverter.ForceInt(content["rank"]?.ToString()),
                        ArtifactCount = JfTypeConverter.ForceInt(content["artifactCount"]?.ToString()),
                        ClanCurrent = content["clanCode"]?.ToString(),
                        ClanName = content["clanName"]?.ToString(),
                        ClanRole = content["clanRole"]?.ToString(),
                        LastTimestamp = JfTypeConverter.ForceDate(content["lastUsed"]?.ToString()).ToString("yyyy.MM.dd H:mm:ss"),
                        TournamentCount = JfTypeConverter.ForceInt(content["totalTournamentsCount"]?.ToString()),
                        TournamentMaxRank = JfTypeConverter.ForceInt(content["highestTournamentRank"]?.ToString()),
                        CountryCode = content["countryCode"]?.ToString(),
                        ClanCratesShared = JfTypeConverter.ForceInt(content["clanCratesShared"]?.ToString()),
                        TitanPoints = JfTypeConverter.ForceInt(content["titanPoints"]?.ToString()),
                        RaidAttackCount = JfTypeConverter.ForceInt(content["raidAttackCount"]?.ToString()),
                        RaidTotalXP = JfTypeConverter.ForceDoubleUniversal(content["raidTotalXP"]?.ToString()),
                        PrestigeCount = ThisPlayer.PlayerId == content["playerID"]?.ToString() ? ThisPlayer.PrestigeCount : 0,
                        EquipmentSetCount = JfTypeConverter.ForceInt(content["equipmentSetCount"]?.ToString()),
                        CraftingShardsSpent = JfTypeConverter.ForceInt(content["craftingShardsSpent"]?.ToString()),
                        TotalPetLevels = JfTypeConverter.ForceInt(content["totalPetLevels"]?.ToString()),
                        TotalSkillPoints = JfTypeConverter.ForceInt(content["totalSkillPoints"]?.ToString()),
                        TotalHelperWeapons = JfTypeConverter.ForceInt(content["totalHelperWeapons"]?.ToString()),
                        TotalHelperScrolls = JfTypeConverter.ForceInt(content["totalHelperScrolls"]?.ToString()),
                        RaidTotalCardLevel = JfTypeConverter.ForceInt(content["raidTotalCardLevel"]?.ToString()),
                        RaidUniqueSkillCount = JfTypeConverter.ForceInt(content["raidUniqueSkillCount"]?.ToString()),
                        RaidBaseDamage = JfTypeConverter.ForceInt(content["raidBaseDamage"]?.ToString()),
                        RaidPlayerLevel = JfTypeConverter.ForceInt(content["raidPlayerLevel"]?.ToString()),
                        DustSpent = ThisPlayer.PlayerId == content["playerID"].ToString() ? ThisPlayer.DustSpent : 0,
                        UndisputedWins = JfTypeConverter.ForceInt(content["undisputedCount"]?.ToString()),
                    };

                    ThisClan.ClanMember.Add(member);

                    if (member.IsMe) EnsureCorrectPlayerData();

                    memberNum++;
                }

                try
                {
                    //adjust clanrank
                    var tmpLst = ThisClan.ClanMember.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId).ToList();

                    for (int i = 0; i < tmpLst.Count; i++)
                    {
                        tmpLst[i].ClanRank = i + 1;
                    }

                    ThisClan.ClanMember = tmpLst;
                }
                catch (Exception e)
                {
                    OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"ClanMemberPopulation Error: Could not sort members: {e.Message}"));
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ClanMemberPopulation Error " + ex.Message));
                return 0;
            }
            #endregion

            #region Populate Messages
            try
            {
                var msgObj = ClanModel["clanMessages"];
                string msgString = msgObj.SelectToken("$content").ToString();
                var msgs = JArray.Parse(msgString);

                foreach (JObject msg in msgs.Children())
                {
                    var message = new ClanMessage()
                    {
                        MessageID = JfTypeConverter.ForceInt(msg.GetValue("id").ToString()),
                        ClanMessageType = msg.GetValue("clanMessageType").ToString(),
                        Message = msg.GetValue("message").ToString(),
                        PlayerIdFrom = msg.GetValue("memberPlayerId").ToString(),
                        MemberName = msg.GetValue("memberName").ToString(),
                        TimeStamp = JfTypeConverter.ForceDate(msg.GetValue("timeStamp").ToString()),
                        IsMe = msg.GetValue("isMe").ToString() == "true" ? true : false,
                        ClanCode = ClanModel["clan"]["clanCode"].ToString(),
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

                    ThisClan.Messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("SaveFilePopulateMsgs " + ex.Message));
                return 0;
            }

            #endregion
            return 0;
        }

        private void EnsureCorrectPlayerData()
        {
            var pfc = new Player { PlayerId = "FusselbirnenKasper" };

            try
            {
                pfc = ThisClan?.ClanMember?.Where(x => x.IsMe).FirstOrDefault();
            }
            catch (Exception)
            {
                return;
            }

            if (pfc == null || ThisPlayer == null)
            {
                return;
            }

            ThisPlayer.ArtifactCount        = Math.Max(ThisPlayer.ArtifactCount, pfc.ArtifactCount);
            ThisPlayer.CurrentStage         = Math.Max(ThisPlayer.CurrentStage, pfc.CurrentStage);
            ThisPlayer.StageMax             = Math.Max(ThisPlayer.StageMax, pfc.StageMax);
            ThisPlayer.WeeklyTicketCount    = Math.Max(ThisPlayer.WeeklyTicketCount, pfc.WeeklyTicketCount);
            ThisPlayer.RaidTicketsCollected = Math.Max(ThisPlayer.RaidTicketsCollected, pfc.RaidTicketsCollected);
            ThisPlayer.ClanRank             = Math.Max(ThisPlayer.ClanRank, pfc.ClanRank);
            ThisPlayer.TournamentCount      = Math.Max(ThisPlayer.TournamentCount, pfc.TournamentCount);
            ThisPlayer.TournamentMaxRank    = Math.Max(ThisPlayer.TournamentMaxRank, pfc.TournamentMaxRank);
            ThisPlayer.ClanCratesShared     = Math.Max(ThisPlayer.ClanCratesShared, pfc.ClanCratesShared);
            ThisPlayer.TitanPoints          = Math.Max(ThisPlayer.TitanPoints, pfc.TitanPoints);
            ThisPlayer.RaidAttackCount      = Math.Max(ThisPlayer.RaidAttackCount, pfc.RaidAttackCount);
            ThisPlayer.RaidTotalXP          = Math.Max(ThisPlayer.RaidTotalXP, pfc.RaidTotalXP);
            ThisPlayer.TitanPoints          = Math.Max(ThisPlayer.TitanPoints, pfc.TitanPoints);
            ThisPlayer.EquipmentSetCount    = Math.Max(ThisPlayer.EquipmentSetCount, pfc.EquipmentSetCount);
            ThisPlayer.CraftingShardsSpent  = Math.Max(ThisPlayer.CraftingShardsSpent, pfc.CraftingShardsSpent);
            ThisPlayer.TotalPetLevels       = Math.Max(ThisPlayer.TotalPetLevels, pfc.TotalPetLevels);
            ThisPlayer.TotalSkillPoints     = Math.Max(ThisPlayer.TotalSkillPoints, pfc.TotalSkillPoints);
            ThisPlayer.TotalHelperWeapons   = Math.Max(ThisPlayer.TotalHelperWeapons, pfc.TotalHelperWeapons);
            ThisPlayer.TotalHelperScrolls   = Math.Max(ThisPlayer.TotalHelperScrolls, pfc.TotalHelperScrolls);
            ThisPlayer.RaidTotalCardLevel   = Math.Max(ThisPlayer.RaidTotalCardLevel, pfc.RaidTotalCardLevel);
            ThisPlayer.RaidUniqueSkillCount = Math.Max(ThisPlayer.RaidUniqueSkillCount, pfc.RaidUniqueSkillCount);
            ThisPlayer.RaidBaseDamage       = Math.Max(ThisPlayer.RaidBaseDamage, pfc.RaidBaseDamage);
            ThisPlayer.RaidPlayerLevel      = Math.Max(ThisPlayer.RaidPlayerLevel, pfc.RaidPlayerLevel);
            ThisPlayer.DustSpent            = Math.Max(ThisPlayer.DustSpent, pfc.DustSpent);
            ThisPlayer.UndisputedWins       = Math.Max(ThisPlayer.UndisputedWins, pfc.UndisputedWins);
        }

        private void LoadPetModel()
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading pet model"));
                PetModel = (JObject)SaveObject.SelectToken("PetModel");
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs(e.Message));
            }
        }

        private void LoadRaidCardModel()
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading RaidCardModel"));
                RaidCardModel = (JObject)SaveObject.SelectToken("RaidCardModel");
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs(e.Message));
            }
        }

        /// <summary>
        /// Loads Account Model
        /// </summary>
        private void LoadAccountModel()
        {
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading account data"));
            AccountModel = (JObject)SaveObject.SelectToken("AccountModel");
        }

        /// <summary>
        /// Loads Artifact Data
        /// </summary>
        /// <param name="rawjson"></param>
        private void LoadArtifacts()
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading artifact data"));
                ArtifactModel = (JObject)SaveObject.SelectToken("ArtifactModel.allArtifactInfo");

                CurrentRelics = JfTypeConverter.ForceDoubleUniversal(SaveObject["PlayerModel"]["relicsServer"]["$content"]?.ToString());
                RelicsReceived = JfTypeConverter.ForceDoubleUniversal(SaveObject["PlayerModel"]["relicsReceivedServer"]["$content"]?.ToString());
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"Savefile.LoadArtifacts: CurrentRelics: {CurrentRelics}"));
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Artifacts" + ex.Message));
            }
        }

        /// <summary>
        /// Loads Information about Shards
        /// </summary>
        private void LoadShardsInformation()
        {
            try
            {
                ShardsSpent = Math.Max(JfTypeConverter.ForceInt(SaveObject["PlayerModel"]["shardsSpentServer"]["$content"].ToString()), 1);
            }
            catch (Exception)
            {
                ShardsSpent = 1;
            }

            try
            {
                ShardsReceived = Math.Max(JfTypeConverter.ForceInt(SaveObject["PlayerModel"]["shardsReceivedServer"]["$content"].ToString()), 1);
            }
            catch (Exception)
            {
                ShardsReceived = 1;
            }

            try
            {
                CraftingPower = FormulaHelper.GetCraftingPower(ShardsSpent);
            }
            catch (Exception)
            {
                CraftingPower = 1;
            }
        }

        private void LoadHelperInformation()
        {
            try
            {
                var helperModel = (JObject)SaveObject.GetValue("HelperModel");

                HelpersFactory.OnProblemHaving += HelpersFactory_OnProblemHaving;
                HelpersFactory.LoadHelperMasteries(helperModel);
                HelpersFactory.OnProblemHaving -= HelpersFactory_OnProblemHaving;
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"SaveFile Error: Could not load Helper information: {e.Message}"));
            }
        }

        /// <summary>
        /// Sets EquipmentModel
        /// </summary>
        private void LoadEquipmentModel()
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading equipment data"));
                EquipmentModel = (JObject)SaveObject.SelectToken("EquipmentModel");
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Equipment Model" + ex.Message));
            }
        }

        /// <summary>
        /// Loads SkillTreeModel
        /// </summary>
        private void LoadSkillTreeModel()
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading Skill tree Model"));
                SkillTreeModel = (JObject)SaveObject.SelectToken("SkillTreeModel.['idToLevelDict2.0']"); //Use Escaping [''] to get this crap

                SPReceived = JfTypeConverter.ForceInt(SaveObject["PlayerModel"]["skillPointsReceivedServer"]["$content"].ToString());
                SPSpent = JfTypeConverter.ForceInt(SaveObject["PlayerModel"]["skillPointsSpentServer"]["$content"].ToString());
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Skill Tree Model" + ex.Message));
            }
        }

        private void LoadTourneyModel()
        {
            try
            {
                TournamentModel = (JObject)App.Save.SaveObject.SelectToken("TournamentModel");

                if (TournamentModel != null)
                {
                    Tournament = new TournamentModel()
                    {
                        CurrentTournament = TournamentModel["currentTournament"].ToString(),
                    };

                    if (!string.IsNullOrWhiteSpace(Tournament.CurrentTournament))
                    {
                        Tournament.TournamentId = TournamentModel["currentTournament"]["tournamentID"].ToString();
                        Tournament.StartTime = JfTypeConverter.ForceDate(TournamentModel["currentTournament"]["startTime"].ToString());
                        Tournament.EndTime = JfTypeConverter.ForceDate(TournamentModel["currentTournament"]["endTime"].ToString());
                        Tournament.BonusType = (TournamentModel["currentTournament"]["bonusType"].ToString().SplitCamelCase());
                        Tournament.BonusAmount = JfTypeConverter.ForceDoubleUniversal(TournamentModel["currentTournament"]["bonusAmount"].ToString());
                        Tournament.PrizeType = (TournamentModel["currentTournament"]["prizeType"].ToString());
                        Tournament.IsRetired = JfTypeConverter.ForceBool(TournamentModel["currentTournament"]["isRetired"].ToString());
                        Tournament.PrizeId = JfTypeConverter.ForceInt(TournamentModel["currentTournament"]["prizeID"].ToString());
                    }
                    else
                    {
                        Tournament.TournamentId = "";
                        Tournament.StartTime = JfTypeConverter.ForceDate(TournamentModel["nextTournament"]["startTime"].ToString());
                        Tournament.EndTime = JfTypeConverter.ForceDate(TournamentModel["nextTournament"]["endTime"].ToString());
                        Tournament.BonusType = (TournamentModel["nextTournament"]["bonusType"].ToString().SplitCamelCase());
                        Tournament.BonusAmount = JfTypeConverter.ForceDoubleUniversal(TournamentModel["nextTournament"]["bonusAmount"].ToString());
                        Tournament.PrizeType = (TournamentModel["nextTournament"]["prizeType"].ToString());
                        Tournament.IsRetired = JfTypeConverter.ForceBool(TournamentModel["nextTournament"]["isRetired"].ToString());
                        Tournament.PrizeId = JfTypeConverter.ForceInt(TournamentModel["nextTournament"]["prizeID"].ToString());
                    }

                    if (Tournament.PrizeType == null)
                    {
                        Tournament.PrizeText = "?";
                    }
                    if (Tournament.PrizeType == "Type1")
                    {
                        Tournament.PrizeText = "SP and Perk";
                    }
                    if (Tournament.PrizeType == "Type2")
                    {
                        Tournament.PrizeText = "Shards and Pet";
                    }
                    if (Tournament.PrizeType == "Type3")
                    {
                        Tournament.PrizeText = "Hero Weapon";
                    }

                    TournamentHandler.SetTournamentModel(Tournament);
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"LoadTourneyModel.Error: {ex.Message}"));
                Tournament = new TournamentModel();
            }
        }
        
        private void LoadStageController()
        {
            try
            {
                StageLogicController = (JObject)SaveObject.SelectToken("StageLogicController");

                if (StageLogicController != null)
                {
                    MyStage.CurrentStage = JfTypeConverter.ForceInt(StageLogicController["currentStage"]["$content"].ToString());
                    MyStage.AutoAdvance = StageLogicController["autoAdvance"]["$content"].ToString() == "true" ? true : false;
                    MyStage.EnemyKillCount = JfTypeConverter.ForceInt(StageLogicController["enemyKillCount"]["$content"].ToString());
                    MyStage.BossDefeated = StageLogicController["bossDefeated"]["$content"].ToString() == "true" ? true : false;
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"LoadStageController Error: {ex.Message}"));
                MyStage = new Stage();
            }
        }
        #endregion

        #region Clipboard Export as data source
        private static string GetExportFileContentString()
        {
            string json = "";
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("GetExportFileContentString: Getting file"));

                json = Xamarin.Forms.DependencyService.Get<IDirectory>().GetTapTitansExportFile();

            }
            catch (Exception ex)
            {
                Xamarin.Forms.DependencyService.Get<IDirectory>().DeleteTapTitansExportFile();
                OnError?.Invoke("SaveFile", new CustErrorEventArgs(ex));
            }

            var validCheck = ClipboardSfImporter.IsValidInput(json);
            if (!validCheck.success)
            {
                Xamarin.Forms.DependencyService.Get<IDirectory>().DeleteTapTitansExportFile();
                return "";
            }

            return json ?? "";
        }

        /// <summary>
        /// Loads a raw string in JSON format into Dictionaries
        /// </summary>
        /// <param name="rawjson"></param>
        private async Task LoadObjectsFromExportFileAsync(
              string rawjson
            , bool loadPlayer
            , bool loadAccountModel
            , bool loadArtifacts
            , bool loadSkills
            , bool loadClan
            , bool loadTourney
            , bool loadEquipment)
        {
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("thinking about savefiles in general"));

            var json = JObject.Parse(rawjson);

            //SaveObject = JObject.Parse((string)json.GetValue("saveString"));

            if (loadPlayer)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading playerdata"));
                _ = await LoadPlayerDataFromExportAsync(json);
            }

            //if (loadAccountModel)
            //{
            //    OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading AccountModel"));
            //    LoadAccountModel();
            //}

            if (loadArtifacts)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Artifacts"));
                LoadArtifactsFromExport(json);
            }

            if (loadSkills)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Skills"));
                LoadSkillTreeModelFromExport(json);
            }

            if (loadClan)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Clan data"));
                _ = LoadClanModelAsyncFromExport();
            }

            //if (loadTourney)
            //{
            //    OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Tournament data"));
            //    LoadTourneyModel();
            //}

            // load set information
            if (loadEquipment)
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"loading Equipment data"));
                LoadEquipmentModelFromExport(json);
            }

            //LoadStageController();

            LoadPetModelFromExport(json);
            
            LoadRaidCardModelFromExport(json);

            // TODO add an option for that later
            //LoadOptionsController();
            RoundedUpgrade = false;
        }

        private async Task<int> LoadPlayerDataFromExportAsync(JObject json)
        {
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading player data"));

            ProfileData = (JObject)json.SelectToken("playerStats");

            ThisPlayer = new Player
            {
                AuthToken = "",
                ClanCratesShared = 0,
                ClanCurrent = "__DUMMYCLANID__",
                ClanRole = "",
                CountryCode = "",
                CurrentStage = 0,
                IsMe = true,
                LastTimestamp = DateTime.Now.ToString("yyyy.MM.dd H:mm:ss"),
                PlayerId = "__DUMMYPLAYERID__",
                PlayerIdHash = "",
                PlayerName = "__DUMMYPLAYERNAME__",
                StageMax = 1,
                TournamentCount = 0,
                TournamentMaxRank = 0,
                PrestigeCount = 0,
                TotalSkillPoints = 0,
                UndisputedWins = 0,
            };

            try
            {
                ThisPlayer.StageMax = JfTypeConverter.ForceDoubleUniversal(ProfileData["Max Prestige Stage"]);
                ThisPlayer.ArtifactCount = JfTypeConverter.ForceInt(ProfileData["Artifacts Collected"]);
                ThisPlayer.TotalPetLevels = JfTypeConverter.ForceInt(ProfileData["Total Pet Levels"]);
                ThisPlayer.TotalSkillPoints = JfTypeConverter.ForceInt(ProfileData["Skill Points Owned"]);
                ThisPlayer.TotalHelperWeapons = JfTypeConverter.ForceInt(ProfileData["Hero Weapon Upgrades"]);
                ThisPlayer.TotalHelperScrolls = JfTypeConverter.ForceInt(ProfileData["Hero Scroll Upgrades"]);
                ThisPlayer.TournamentCount = JfTypeConverter.ForceInt(ProfileData["Tournaments Joined"]);
                ThisPlayer.UndisputedWins = JfTypeConverter.ForceInt(ProfileData["Undisputed Wins"]);
                ThisPlayer.TitanPoints = JfTypeConverter.ForceInt(ProfileData["Tournament Points"]);

                //ThisPlayer.CraftingShardsSpent = JfTypeConverter.ForceInt(ProfileData["Crafting Power"]); // TODO calc back from CP to amount spent to get this equal or new prop?
                CraftingPower = JfTypeConverter.ForceInt(ProfileData["Crafting Power"]);

                IDictionary<string, JToken> raidData = (JObject)json.SelectToken("raidStats");

                // raid data
                ThisPlayer.RaidPlayerLevel = JfTypeConverter.ForceInt(raidData["Raid Level"]);
                ThisPlayer.RaidBaseDamage = JfTypeConverter.ForceInt(raidData["Raid Damage"]);
                ThisPlayer.RaidTotalXP = JfTypeConverter.ForceInt(raidData["Total Raid Experience"]);
                ThisPlayer.RaidAttackCount = JfTypeConverter.ForceInt(raidData["Total Raid Attacks"]);
                ThisPlayer.RaidTotalCardLevel = JfTypeConverter.ForceInt(raidData["Total Raid Card Levels"]);
                ThisPlayer.RaidUniqueSkillCount = JfTypeConverter.ForceInt(raidData["Raid Cards Owned"]);
                ThisPlayer.RaidTicketsCollected = JfTypeConverter.ForceInt(raidData["Lifetime Raid Tickets"]);
                ThisPlayer.WeeklyTicketCount = 0;

                // equip set count
                ThisPlayer.EquipmentSetCount = ((JArray)json.SelectToken("equipmentSets")).Count;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"SafeFile Error: could not load ThisPlayer -> {ex.Message}"));
            }


            ThisPlayer.DustSpent = 0;

            //LoadShardsInformation();

            //LoadHelperInformation();

            //LoadPrestigeModel();

            //LoadDailyInformation();

            LoadPassiveSkillsFromExport(json);

            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("updating player data"));

            return await _dbRepo.UpdatePlayerAsync(ThisPlayer);
        }

        private void LoadPassiveSkillsFromExport(JObject json)
        {
            try
            {
                PassiveSkillModel = (JObject)json.SelectToken("passiveSkills");

                LessMonstersLevel = JfTypeConverter.ForceInt(PassiveSkillModel["Intimidating Presence"]?.ToString());
                PetSplashSkipLevel = JfTypeConverter.ForceInt(PassiveSkillModel["Power Surge"]?.ToString());
                ClanShipSplashSkipLevel = JfTypeConverter.ForceInt(PassiveSkillModel["Anti-Titan Cannon"]?.ToString());
                SorcererSplashSkipLevel = JfTypeConverter.ForceInt(PassiveSkillModel["Mystical Impact"]?.ToString());
                RaidCardPowerLevel = JfTypeConverter.ForceInt(PassiveSkillModel["Arcane Bargain"]?.ToString());
                SilentMarchLevel = JfTypeConverter.ForceInt(PassiveSkillModel["Silent March"]?.ToString());
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Passive Skills" + ex.Message));
            }
        }


        /// <summary>
        /// Loads Artifact Data
        /// </summary>
        /// <param name="json"></param>
        private void LoadArtifactsFromExport(JObject json)
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading artifact data"));
                ArtifactModel = (JObject)json.SelectToken("artifacts");

                RelicsReceived = JfTypeConverter.ForceDoubleUniversal(json["playerStats"]["Lifetime Relics"]?.ToString());
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"Savefile.LoadArtifacts: CurrentRelics: {CurrentRelics}"));
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Artifacts" + ex.Message));
            }
        }

        public void CalculateCurrentRelics()
        {
            if (LocalSettingsORM.IsReadingDataFromSavefile)
            {
                return;
            }

            try
            {
                double spentAmount = ArtifactHandler.GetLifeTimeSpentOnAll() + ArtifactCostHandler.CostSum(ArtifactHandler.Artifacts.Where(x => x.Level > 0).Count(), ArtifactHandler.Artifacts.Where(x => x.EnchantmentLevel > 0).Count());
                CurrentRelics = RelicsReceived - spentAmount;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not calculate CurrentRelics" + ex.Message));
            }
        }

        /// <summary>
        /// Loads SkillTreeModel
        /// </summary>
        private void LoadSkillTreeModelFromExport(JObject json)
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading Skill tree Model"));
                SkillTreeModel = (JObject)json.SelectToken("skillTree");
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Skill Tree Model" + ex.Message));
            }
        }

        /// <summary>
        /// Updates Clan data async
        /// </summary>
        /// <returns></returns>
        private int LoadClanModelAsyncFromExport()
        {
            #region Setting stuff up
            OnProgressMade?.Invoke("SaveFile", new InformationEventArgs(AppResources.ReadingClanData));

            try
            {
                ThisClan = new Clan()
                {
                    ID = ThisPlayer.ClanCurrent,
                    Name = "__DUMMYCLANNAME__",
                    Description = "Dummy Clan",
                    Score = 0.0,
                    Rank = 0,
                    LeaderName = ThisPlayer.PlayerName,
                    CountryCode = ThisPlayer.CountryCode,
                    IsPrivate = true,
                    Passwort = "xyz_abc",
                    StageRequired = 0,
                    AutoKickDays = 100,
                    ClanRaidExp = 0,
                    ClanRaidTickets = 0,
                };
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs(e.Message + "\n" + ClanModel.ToString()));
                ThisClan = new Clan();
            }
            #endregion

            #region Populate Member
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs($"populating clan member"));                    

                ThisClan.ClanMember.Add(ThisPlayer);
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ClanMemberPopulation Error " + ex.Message));
                return 0;
            }
            #endregion

            return 0;
        }

        private void LoadPetModelFromExport(JObject json)
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading pet model"));
                PetModel = (JObject)json.SelectToken("petLevels");
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not read pet model" + ex.Message));
            }
        }

        private void LoadRaidCardModelFromExport(JObject json)
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading raid card model"));
                RaidCardModel = (JObject)json.SelectToken("raidCards");
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not read raid card model" + ex.Message));
            }
        }

        /// <summary>
        /// Sets EquipmentModel
        /// </summary>
        private void LoadEquipmentModelFromExport(JObject json)
        {
            try
            {
                OnProgressMade?.Invoke("SaveFile", new InformationEventArgs("reading equipment data"));
                EquipmentClipboardModel = (JArray)json.SelectToken("equipmentSets");
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs("ERROR Could not load Equipment Model" + ex.Message));
            }
        }
        #endregion

        #endregion

        #region Events Delegates
        private void HelpersFactory_OnProblemHaving(object sender, CustErrorEventArgs e) => OnLogMePlease?.Invoke("SaveFile", new InformationEventArgs($"{sender.ToString()} Error: {e.MyException.Message}"));

        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when <see cref="Initialize"/> made progress
        /// </summary>
        public static event ProgressCarrier OnProgressMade;

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for <see cref="ErrorCarrier"/>
        /// </summary>
        /// <param name="data"></param>
        public delegate void ErrorCarrier(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this class gets an error
        /// </summary>
        public static event ErrorCarrier OnError; 
#endregion
    }
}