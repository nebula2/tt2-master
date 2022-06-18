using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TT2Master.Shared.Models;

namespace TT2Master
{
    public class AchievmentHandler
    {
        public List<DailyAchievement> Achievments = new List<DailyAchievement>();

        public void ReloadAchievments()
        {
            try
            {
                var amodel = (JObject)App.Save.SaveObject.SelectToken("AchievementModel");

                if (amodel == null)
                {
                    return;
                }

                var petLvl = new DailyAchievement()
                {
                    CurrentDailyAchievement = "Pet Levels",
                    CurrentDailyAchievementProgress = amodel["PetLevelsprogress"]["$content"].ToString(),
                    CurrentDailyAchievementTotal = "3",
                };
                petLvl.DailyAchievementCollected = petLvl.CurrentDailyAchievementProgress == petLvl.CurrentDailyAchievementTotal;
                Achievments.Add(petLvl);

                var equip = new DailyAchievement()
                {
                    CurrentDailyAchievement = "Collected Equipment",
                    CurrentDailyAchievementProgress = amodel["CollectedEquipmentprogress"]["$content"].ToString(),
                    CurrentDailyAchievementTotal = "4",
                };
                equip.DailyAchievementCollected = equip.CurrentDailyAchievementProgress == equip.CurrentDailyAchievementTotal;
                Achievments.Add(equip);


                var art = new DailyAchievement()
                {
                    CurrentDailyAchievement = "Unique Artifacts",
                    CurrentDailyAchievementProgress = amodel["UniqueArtifactsprogress"]["$content"].ToString(),
                    CurrentDailyAchievementTotal = "1",
                };
                art.DailyAchievementCollected = art.CurrentDailyAchievementProgress == art.CurrentDailyAchievementTotal;
                Achievments.Add(art);

                var dia = new DailyAchievement()
                {
                    CurrentDailyAchievement = "Click Fairies",
                    CurrentDailyAchievementProgress = amodel["ClickFairiesprogress"]["$content"].ToString(),
                    CurrentDailyAchievementTotal = "5",
                };
                dia.DailyAchievementCollected = dia.CurrentDailyAchievementProgress == dia.CurrentDailyAchievementTotal;
                Achievments.Add(dia);

                var prestige = new DailyAchievement()
                {
                    CurrentDailyAchievement = "Prestige",
                    CurrentDailyAchievementProgress = amodel["Prestigesprogress"]["$content"].ToString(),
                    CurrentDailyAchievementTotal = "1",
                };
                prestige.DailyAchievementCollected = prestige.CurrentDailyAchievementProgress == prestige.CurrentDailyAchievementTotal;
                Achievments.Add(prestige);

                var video = new DailyAchievement()
                {
                    CurrentDailyAchievement = "Video",
                    CurrentDailyAchievementProgress = amodel["WatchVideosprogress"]["$content"].ToString(),
                    CurrentDailyAchievementTotal = "1",
                };
                video.DailyAchievementCollected = video.CurrentDailyAchievementProgress == video.CurrentDailyAchievementTotal;
                Achievments.Add(video);

            }
            catch (Exception)
            {
                // TODO add log events and stuff
            }
        }
    }
}
