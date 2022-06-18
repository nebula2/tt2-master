using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Loggers;
using TT2Master.Shared.Models;

namespace TT2Master.Model.SP
{
    /// <summary>
    /// Factory class for SP optimizer configuration
    /// </summary>
    public static class SPOptConfigFactory
    {
        /// <summary>
        /// Loads the desired configuration async
        /// </summary>
        /// <param name="idToLoad">id you want to load</param>
        /// <param name="forceSkillReload">force a skill reload?</param>
        /// <returns></returns>
        public static async Task<SPOptConfiguration> LoadConfigAsync(DBRepository dbRepo, SaveFile save, string idToLoad = null, bool forceSkillReload = false)
        {
            var config = new SPOptConfiguration();

            try
            {
                // load config itself or create a new one if parameter is null
                config = !string.IsNullOrWhiteSpace(idToLoad)
                    ? await dbRepo.GetSPOptConfigurationSavedByID(idToLoad)
                        ? await dbRepo.GetSPOptConfigurationByID(idToLoad)
                        : new SPOptConfiguration()
                    : new SPOptConfiguration();

                // load skill information
                config.SkillSettings = await LoadSkillConfiguration(dbRepo, config, save, forceSkillReload);

                // reset weights
                ResetGoldWeight(config, save);
                ResetDamageWeight(config, save);

                return config;
            }
            catch (Exception)
            {
                //Logger.WriteToLogFile($"Error loading sp config: {e.Message}");
                return config;
            }
        }

        /// <summary>
        /// Loads skill settings for the given configuration
        /// </summary>
        /// <param name="config">configuration to be filled</param>
        /// <param name="freshReload">reload skill handler?</param>
        /// <returns></returns>
        private static async Task<ObservableCollection<SPOptSkillSetting>> LoadSkillConfiguration(DBRepository dbRepo, SPOptConfiguration config, SaveFile save, bool freshReload = false)
        {
            var resultList = new ObservableCollection<SPOptSkillSetting>();

            try
            {
                // get stored settings
                var skillsInConfig = string.IsNullOrWhiteSpace(config.Name)
                    ? new List<SPOptSkillSetting>()
                    : await dbRepo.GetAllSPOptSkillSettingByParentIdAsync(config.Name);

                // stored null check
                if (skillsInConfig == null)
                {
                    skillsInConfig = new List<SPOptSkillSetting>();
                }

                // get full list
                SkillInfoHandler.PopulateOptSkills(save, freshReload);

                // merge them to the configuration property
                foreach (var item in SkillInfoHandler.OptSkills)
                {
                    //check if item is in settings
                    var tmp = skillsInConfig.Where(x => x.SkillId == item.TalentID).FirstOrDefault();

                    if (tmp == null)
                    {
                        tmp = new SPOptSkillSetting()
                        {
                            SkillId = item.TalentID,
                            CustomDmgReduction = 0,
                            CustomGoldReduction = 0,
                            IsTolerated = true,
                            IsSpecialCalculationEnabled = true,
                        };
                    }

                    tmp.MySPOptSkill = item;
                    tmp.IsHavingSpecialCalculation = item.IsHavingSpecialCalculation;
                    tmp.IsDmgRelevant = item.DefaultReduction?.IsDmgRelevant ?? false;
                    tmp.IsGoldRelevant = item.DefaultReduction?.IsGoldRelevant ?? false;

                    resultList.Add(tmp);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return new ObservableCollection<SPOptSkillSetting>();
            }
        }

        /// <summary>
        /// Resets the gold weights for the skills in given configuration
        /// </summary>
        /// <param name="config">configuration you want the gold weights to be resetted for</param>
        public static void ResetGoldWeight(SPOptConfiguration config, SaveFile save)
        {
            // load Skills if needed
            SkillInfoHandler.PopulateOptSkills(save);

            // get dmg source
            var goldSource = (GoldType)config.GoldSourceInt;

            // set new default weights
            foreach (var item in config.SkillSettings)
            {
                var skill = SkillInfoHandler.OptSkills.Where(x => x.TalentID == item.SkillId).FirstOrDefault();

                if (skill == null)
                {
                    item.DefaultGoldReduction = 0;
                    continue;
                }

                switch (goldSource)
                {
                    case GoldType.pHoM:
                        item.DefaultGoldReduction = skill.DefaultReduction.PHoMReduction;
                        break;
                    case GoldType.BossGold:
                        item.DefaultGoldReduction = skill.DefaultReduction.BossGoldReduction;
                        break;
                    case GoldType.ChestersonGold:
                        item.DefaultGoldReduction = skill.DefaultReduction.ChestersonReduction;
                        break;
                    case GoldType.NormalMobGold:
                        item.DefaultGoldReduction = skill.DefaultReduction.AllGoldReduction;
                        break;
                    case GoldType.FairyGold:
                        item.DefaultGoldReduction = skill.DefaultReduction.FairyReduction;
                        break;
                    default:
                        item.DefaultGoldReduction = 0;
                        break;
                }
            }
        }

        /// <summary>
        /// Resets the damage weights for the skills in given configuration
        /// </summary>
        /// <param name="config">configuration you want the damage weights to be resetted for</param>
        public static void ResetDamageWeight(SPOptConfiguration config, SaveFile save)
        {
            // load Skills if needed
            SkillInfoHandler.PopulateOptSkills(save);

            // get dmg source
            var dmgSource = (SPOptDamageSource)config.DamageSourceInt;

            // set new default weights
            foreach (var item in config.SkillSettings)
            {
                var skill = SkillInfoHandler.OptSkills.Where(x => x.TalentID == item.SkillId).FirstOrDefault();

                if (skill == null)
                {
                    item.DefaultDmgReduction = 0;
                    continue;
                }

                switch (dmgSource)
                {
                    case SPOptDamageSource.ClanShip:
                        item.DefaultDmgReduction = skill.DefaultReduction.ShipReduction;
                        break;
                    case SPOptDamageSource.ShadowClone:
                        item.DefaultDmgReduction = skill.DefaultReduction.ShadowCloneReduction;
                        break;
                    case SPOptDamageSource.HeavenlyStrike:
                        item.DefaultDmgReduction = skill.DefaultReduction.HeavenlyStrikeReduction;
                        break;
                    case SPOptDamageSource.Pet:
                        item.DefaultDmgReduction = skill.DefaultReduction.PetReduction;
                        break;
                    default:
                        item.DefaultDmgReduction = 0;
                        break;
                }
            }
        }
    }
}