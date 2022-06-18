using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master.Droid
{
    /// <summary>
    /// A class for receiving Tournament Info
    /// </summary>
    public static class TournamentInfo
    {
        #region private
        private static SaveFile Save { get; set; }

        private static IDictionary<string, JToken> TournamentData { get; set; }

        /// <summary>
        /// Dictionary of AchievementModel
        /// </summary>
        private static IDictionary<string, JToken> AchievementModel { get; set; }

        /// <summary>
        /// Dictionary of StageLogicController
        /// </summary>
        private static IDictionary<string, JToken> StageLogicController { get; set; }
        #endregion

        /// <summary>
        /// Tournament Model from Save file
        /// </summary>
        public static TournamentModel Model { get; private set; }

        /// <summary>
        /// List of Tournament Member
        /// </summary>
        public static List<TournamentMember> Members { get; private set; }

        public static Stage MyStage { get; private set; }

        [Obsolete("Gone with 3.0")]
        public static DailyAchievement MyAchievement { get; private set; }

        public static List<DailyAchievement> MyAchievments { get; private set; }

        public static List<int> ExtraEquipmentDrops { get; private set; }

        /// <summary>
        /// Initializes Tournament Info
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> Init()
        {
            WidgetLogger.WriteToLogFile($"TournamentInfo.Init()");

            // create dbRepo
            var h = new DBPathHelper();
            var dBRepo = new DBRepository(h.DBPath("tt2master.db3"));

            Save = new SaveFile(dBRepo);
            Model = new TournamentModel();
            Members = new List<TournamentMember>();
            MyStage = new Stage();
            MyAchievments = new List<DailyAchievement>();
            ExtraEquipmentDrops = new List<int>();

            WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): Reading Savefile");
            //read save file
            bool result = await InitForWidget().ConfigureAwait(false);

            WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): Reading Savefile -> {result}");

            #region Fill TournamentModel

            WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): Populating TournamentData from SaveObject");

            TournamentData = (JObject)Save.SaveObject.SelectToken("TournamentModel");

            if (TournamentData != null)
            {
                try
                {
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): Setting Model from TournamentModel");

                    Model.CurrentTournament = TournamentData["currentTournament"].ToString();
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): CurrentTournament: {Model.CurrentTournament}");

                    Model.StartTime = (TournamentData["nextTournament"]["startTime"].ToString());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): StartTime: {Model.StartTime}");

                    Model.EndTime = (TournamentData["nextTournament"]["endTime"].ToString());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): EndTime: {Model.EndTime}");

                    Model.BonusType = (TournamentData["nextTournament"]["bonusType"].ToString().SplitCamelCase());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): BonusType: {Model.BonusType}");

                    Model.BonusAmount = (TournamentData["nextTournament"]["bonusAmount"].ToString());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): bonusAmount: {Model.BonusAmount}");

                    Model.PrizeType = (TournamentData["nextTournament"]["prizeType"].ToString());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): prizeType: {Model.PrizeType}");

                    if (Model.PrizeType == null)
                    {
                        Model.PrizeText = "?";
                    }
                    if (Model.PrizeType == "Type1")
                    {
                        Model.PrizeText = "SP and Perk";
                    }
                    if (Model.PrizeType == "Type2")
                    {
                        Model.PrizeText = "Shards and Pet";

                    }
                    if (Model.PrizeType == "Type3")
                    {
                        Model.PrizeText = "Hero Weapon";
                    }
                }
                catch (Exception e)
                {
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): TournamentData Error. {e.Message}");
                }

            }
            else
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): TournamentData is null");
            }

            #endregion

            #region Fill StageLogicController
            StageLogicController = (JObject)Save.SaveObject.SelectToken("StageLogicController");

            if (StageLogicController != null)
            {
                try
                {
                    MyStage.CurrentStage = TypeConverter.ForceInt(StageLogicController["currentStage"]["$content"].ToString());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): CurrentStage: {MyStage.CurrentStage}");

                    MyStage.AutoAdvance = StageLogicController["autoAdvance"]["$content"].ToString() == "true";
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): AutoAdvance: {MyStage.AutoAdvance}");

                    MyStage.EnemyKillCount = TypeConverter.ForceInt(StageLogicController["enemyKillCount"]["$content"].ToString());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): EnemyKillCount: {MyStage.EnemyKillCount}");

                    MyStage.BossDefeated = StageLogicController["bossDefeated"]["$content"].ToString() == "true";
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): BossDefeated: {MyStage.BossDefeated}");

                    MyStage.MegaBombEffectEndStage = TypeConverter.ForceInt(StageLogicController["megaBombEffectEndStage"]["$content"].ToString());
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): MegaBombEffectEndStage: {MyStage.MegaBombEffectEndStage}");
                }
                catch (Exception e)
                {
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): StageLogicController Error: {e.Message}");

                }

            }
            else
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): StageLogicController is null");
            }
            #endregion

            FillAchievementModel();

            #region Fill extra equipment drops
            string dropsStr = Save.SaveObject["EquipmentModel"]["extraEquipmentDrops"].ToString();

            if (dropsStr == null)
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): extraEquipmentDrops is null");

                ExtraEquipmentDrops = new List<int>();
            }
            else
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): extraEquipmentDrops is not null");

                var dropsObj = JObject.Parse(dropsStr);
                var dropsToken = dropsObj.GetValue("$content");
                int[] dropsArray = dropsToken.Values<int>().ToArray();

                ExtraEquipmentDrops = dropsArray.ToList();

                WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): added {ExtraEquipmentDrops.Count} items to extraEquipmentDrops");
            }

            #endregion

            WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): calling InitServerApiAsync");

            #region ServerApi
            if (!await InitServerApiAsync())
            {
                result = false;

                WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): failed with InitServerApiAsync");
            }
            #endregion

            return result;
        }

        private static void FillAchievementModel()
        {
            #region Fill AchievementModel
            AchievementModel = (JObject)Save.SaveObject.SelectToken("AchievementModel");

            if (AchievementModel != null)
            {
                try
                {
                    var petLvl = new DailyAchievement()
                    {
                        CurrentDailyAchievement = "Pet Levels",
                        CurrentDailyAchievementProgress = AchievementModel["PetLevelsprogress"]["$content"].ToString(),
                        CurrentDailyAchievementTotal = "3",
                    };

                    petLvl.DailyAchievementCollected = petLvl.CurrentDailyAchievementProgress == petLvl.CurrentDailyAchievementTotal;
                    MyAchievments.Add(petLvl);

                    var equip = new DailyAchievement()
                    {
                        CurrentDailyAchievement = "Collected Equipment",
                        CurrentDailyAchievementProgress = AchievementModel["CollectedEquipmentprogress"]["$content"].ToString(),
                        CurrentDailyAchievementTotal = "4",
                    };
                    equip.DailyAchievementCollected = equip.CurrentDailyAchievementProgress == equip.CurrentDailyAchievementTotal;
                    MyAchievments.Add(equip);


                    var art = new DailyAchievement()
                    {
                        CurrentDailyAchievement = "Unique Artifacts",
                        CurrentDailyAchievementProgress = AchievementModel["UniqueArtifactsprogress"]["$content"].ToString(),
                        CurrentDailyAchievementTotal = "1",
                    };
                    art.DailyAchievementCollected = art.CurrentDailyAchievementProgress == art.CurrentDailyAchievementTotal;
                    MyAchievments.Add(art);

                    var dia = new DailyAchievement()
                    {
                        CurrentDailyAchievement = "Click Fairies",
                        CurrentDailyAchievementProgress = AchievementModel["ClickFairiesprogress"]["$content"].ToString(),
                        CurrentDailyAchievementTotal = "5",
                    };
                    dia.DailyAchievementCollected = dia.CurrentDailyAchievementProgress == dia.CurrentDailyAchievementTotal;
                    MyAchievments.Add(dia);

                    var prestige = new DailyAchievement()
                    {
                        CurrentDailyAchievement = "Prestige",
                        CurrentDailyAchievementProgress = AchievementModel["Prestigesprogress"]["$content"].ToString(),
                        CurrentDailyAchievementTotal = "1",
                    };
                    prestige.DailyAchievementCollected = prestige.CurrentDailyAchievementProgress == prestige.CurrentDailyAchievementTotal;
                    MyAchievments.Add(prestige);

                    var video = new DailyAchievement()
                    {
                        CurrentDailyAchievement = "Video",
                        CurrentDailyAchievementProgress = AchievementModel["WatchVideosprogress"]["$content"].ToString(),
                        CurrentDailyAchievementTotal = "1",
                    };
                    video.DailyAchievementCollected = video.CurrentDailyAchievementProgress == video.CurrentDailyAchievementTotal;
                    MyAchievments.Add(video);

                    #region Pre 3.0
                    //MyAchievement.CurrentDailyAchievement = AchievementModel["currentDailyAchievement"]["$content"].ToString().SplitCamelCase();
                    //MyAchievement.CurrentDailyAchievement = "?";
                    //WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): CurrentDailyAchievement: {MyAchievement.CurrentDailyAchievement}");

                    ////MyAchievement.CurrentDailyAchievementProgress = AchievementModel["currentDailyAchievementProgress"]["$content"].ToString();
                    //MyAchievement.CurrentDailyAchievementProgress = "?";
                    //WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): currentDailyAchievementProgress: {MyAchievement.CurrentDailyAchievementProgress}");

                    //MyAchievement.CurrentDailyAchievementTotal = AchievementModel["currentDailyAchievementTotal"]["$content"].ToString();
                    //MyAchievement.CurrentDailyAchievementTotal = "?";
                    //WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): currentDailyAchievementTotal: {MyAchievement.CurrentDailyAchievementTotal}");

                    ////MyAchievement.DailyAchievementCollected = AchievementModel["dailyAchievementCollected"]["$content"].ToString() == "true" ? true : false;
                    //MyAchievement.DailyAchievementCollected = false;
                    //WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): dailyAchievementCollected: {MyAchievement.DailyAchievementCollected}"); 
                    #endregion
                }
                catch (Exception e)
                {
                    WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): AchievementModel Error: {e.Message}");
                }
            }
            else
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.Init(): AchievementModel is null");
            }
            #endregion
        }

        /// <summary>
        /// True, if a tournament is currently running
        /// </summary>
        /// <returns></returns>
        public static bool TournamentRunning()
        {
            WidgetLogger.WriteToLogFile($"TournamentInfo.TournamentRunning()");

            #region false
            if (string.IsNullOrEmpty(Model.CurrentTournament))
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.TournamentRunning(): Model.CurrentTournament is null");
                return false;
            }

            //If TournamentModel is not filled - you are not in a tourney
            if (Model.CurrentTournament.ToString() == "null")
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.TournamentRunning(): Model.CurrentTournament.ToString() == null");
                return false;
            }

            //If TournamentModel is filled and can pe parsed to a date in future, no tournament is running

            var date = JfTypeConverter.ForceDate(Model.CurrentTournament);

            if (date.Date > DateTime.Now.Date)
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.TournamentRunning(): date: {date.Date} > {DateTime.Now.Date}");
                return false;
            }
            #endregion

            return true;
        }

        /// <summary>
        /// Initializes the ServerApi Async
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> InitServerApiAsync()
        {
            try
            {
                WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync()");

                #region Get Player from Database. There are the informations needed to init serverApi

                WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync(): Creating DbRepo-Object to get the player from");
                //Init dbRepo
                var h = new DBPathHelper();
                var dBRepo = new DBRepository(h.DBPath("tt2master.db3"));

                //get info nessecary for server initialization
                var p = await dBRepo.GetMyPlayer();

                if (p == null)
                {
                    WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync(): Player could not be loaded from database");
                    Console.WriteLine("InitServerApiAsync: Player could not be loaded from database");
                    return false;
                }

                if (p.AdId == null || p.AuthToken == null || p.PlayerIdHash == null)
                {
                    WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync(): Player data missing to initialize ServerApi");
                    return false;
                }
                #endregion

                //WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync(): Initializing ServerApi with info from player");

                //WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync(): Initialization complete");

                return true;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    Console.WriteLine(ex.Message);
                    WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync(): Exception -> {ex.Message}\n{ex.Data}");
                }
                else
                {
                    WidgetLogger.WriteToLogFile($"TournamentInfo.InitServerApiAsync(): Exception: got null exception");
                }

                return false;
            }
        }

        /// <summary>
        /// Savefile stuff
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> InitForWidget()
        {
            try
            {
                WidgetLogger.WriteToLogFile("Getting file");

                //get full name of file to decrypt - then decrypt it

                var ttp = new TapTitansPathHelper();

                string f = ttp.GetFileName();

                WidgetLogger.WriteToLogFile("reading file");

                string decryptedFile = await SaveFileReader.DecryptSaveFileAsync(f);

                //remove content not needed
                int start = decryptedFile.IndexOf("saveString");
                int endPos = decryptedFile.IndexOf("playerData");
                string s = decryptedFile[..(endPos - 2)] + "}";

                WidgetLogger.WriteToLogFile("processing files");

                //Load json into objects
                LoadObjectsForTournament(s);

                return true;
            }
            catch (Exception ex)
            {
                if (ex == null)
                {
                    WidgetLogger.WriteToLogFile("InitForWidget: Exception null");
                }
                else
                {
                    WidgetLogger.WriteToLogFile($"InitForWidget: Exception {ex}, {ex.Data}");
                }

                return false;
            }
        }

        private static void LoadObjectsForTournament(string rawjson)
        {
            WidgetLogger.WriteToLogFile("LoadObjectsForTournament()");
            var json = JObject.Parse(rawjson);
            Save.SaveObject = JObject.Parse((string)json.GetValue("saveString"));
        }

        /// <summary>
        /// Re-initializes this objects member
        /// </summary>
        public static void HardResetModel()
        {
            Model = new TournamentModel();
            Members = new List<TournamentMember>();
        }

        public static void HardResetEquip()
        {
            ExtraEquipmentDrops = new List<int>();
            MyStage = new Stage();
        }

        public static void HardResetArchievement() => MyAchievments = new List<DailyAchievement>();
    }
}