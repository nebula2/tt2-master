using System;
using System.Linq;
using TT2Master.Loggers;
using TT2Master.Model.Helpers;
using TT2Master.Resources;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master.Model.SP
{
    /// <summary>
    /// Handles skip calculation for skills
    /// </summary>
    public static class SPSkipHandler
    {
        /// <summary>
        /// Eternal Darkness
        /// </summary>
        private static Skill EDSkill { get; set; }

        /// <summary>
        /// Lightning Burst
        /// </summary>
        private static Skill LBSkill { get; set; }

        /// <summary>
        /// Angelic Radiance
        /// </summary>
        private static Skill ARSkill { get; set; }

        /// <summary>
        /// Calcualtes Eternal Darkness efficiency
        /// </summary>
        /// <param name="reload">Reload all dependencies or take cached value?</param>
        /// <returns></returns>
        public static Skill CalculateCloneSkillBoostEfficiency(SaveFile save, bool reload = false)
        {
            if (EDSkill != null && !reload)
            {
                return EDSkill;
            }

            EquipmentHandler.FillEquipment(save);
            EquipmentHandler.LoadSetInformation(save);

            string AGSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "HighTecLightning").Count() > 0 ? AppResources.YesText : AppResources.NoText;
            bool APSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Platinum").Count() > 0;

            FormulaHelper.LoadPassiveSkillCosts(EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Mech").Count() > 0);
            int _arcaneBargainLevel = FormulaHelper.GetArcaneBargainLevel(save.ThisPlayer.DustSpent);
            int _intimidatingPresenceLevel = FormulaHelper.GetIntimidatingPresenceLevel(save.ThisPlayer.TotalSkillPoints);
            int _powerSurgeLevel = FormulaHelper.GetPowerSurgeLevel(save.ThisPlayer.TotalPetLevels);
            int _mysticalImpactLevel = FormulaHelper.GetMysticalImpactLevel(save.ThisPlayer.TitanPoints);
            int _antiTitanCannonLevel = FormulaHelper.GetAntiTitanCannonLevel(HelpersFactory.GetTotalMasteriesLevel());

            int PassiveSkillReduction = _arcaneBargainLevel + _intimidatingPresenceLevel;

            // AP Set bonus
            double APSetBonus = 1;
            if (APSetCompleted)
            {
                var apSet = EquipmentHandler.EquipmentSets.Where(x => x.Set == "Platinum").FirstOrDefault();
                int cp = save.CraftingPower;

                APSetBonus = FormulaHelper.GetEquipmentSetBonus(apSet, cp, 2);
            }

            int BaseSkip = AGSetCompleted == AppResources.YesText ? 11 : 3;
            BaseSkip = 0; //Das ist splashing. Hier geht es um skipping

            var SplashSnap = (SplashSnapEnum)JfTypeConverter.ForceInt(LocalSettingsORM.GetSplashSnapSetting());

            int NormalTitanCount = FormulaHelper.GetTitanCount((int)save.ThisPlayer.StageMax);
            int StagesToSplash = (NormalTitanCount - PassiveSkillReduction) / (SplashSnap == SplashSnapEnum.SingleSnap ? 2 : 1);
            if (SplashSnap == SplashSnapEnum.DoubleSnap)
            {
                StagesToSplash /= 2;
                StagesToSplash /= 2;
            }

            //Load Skills
            SkillInfoHandler.LoadSkills();
            SkillInfoHandler.FillSkills(save);

            EDSkill = SkillInfoHandler.Skills.Where(x => x.TalentID == "CloneSkillBoost").FirstOrDefault();

            //Set the current Level to the value which is required for splashing
            EDSkill.CurrentLevel = GetSplashSkillLevel(EDSkill, _mysticalImpactLevel + _arcaneBargainLevel, BaseSkip, APSetBonus, StagesToSplash);

            // set special efficiency
            return EDSkill;
        }

        /// <summary>
        /// Calculates Lightning Burst efficiency
        /// </summary>
        /// <param name="reload">reload all dependencies or take cached value?</param>
        /// <returns></returns>
        public static Skill CalculatePetQTEEfficiency(SaveFile save, bool reload = false)
        {
            if (LBSkill != null && !reload)
            {
                return LBSkill;
            }

            EquipmentHandler.FillEquipment(save);
            EquipmentHandler.LoadSetInformation(save);

            string AGSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "HighTecLightning").Count() > 0 ? AppResources.YesText : AppResources.NoText;
            bool APSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Platinum").Count() > 0;

            FormulaHelper.LoadPassiveSkillCosts(EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Mech").Count() > 0);
            int _arcaneBargainLevel = FormulaHelper.GetArcaneBargainLevel(save.ThisPlayer.DustSpent);
            int _intimidatingPresenceLevel = FormulaHelper.GetIntimidatingPresenceLevel(save.ThisPlayer.TotalSkillPoints);
            int _powerSurgeLevel = FormulaHelper.GetPowerSurgeLevel(save.ThisPlayer.TotalPetLevels);
            int _mysticalImpactLevel = FormulaHelper.GetMysticalImpactLevel(save.ThisPlayer.TitanPoints);
            int _antiTitanCannonLevel = FormulaHelper.GetAntiTitanCannonLevel(HelpersFactory.GetTotalMasteriesLevel());

            int PassiveSkillReduction = _arcaneBargainLevel + _intimidatingPresenceLevel;

            // AP Set bonus
            double APSetBonus = 1;
            if (APSetCompleted)
            {
                var apSet = EquipmentHandler.EquipmentSets.Where(x => x.Set == "Platinum").FirstOrDefault();
                int cp = save.CraftingPower;

                APSetBonus = FormulaHelper.GetEquipmentSetBonus(apSet, cp, 2);
            }

            int BaseSkip = AGSetCompleted == AppResources.YesText ? 11 : 3;
            BaseSkip = 0; //Das ist splashing. Hier geht es um skipping

            int NormalTitanCount = FormulaHelper.GetTitanCount((int)save.ThisPlayer.StageMax);
            var SplashSnap = (SplashSnapEnum)JfTypeConverter.ForceInt(LocalSettingsORM.GetSplashSnapSetting());

            int StagesToSplash = (NormalTitanCount - PassiveSkillReduction) / (SplashSnap == SplashSnapEnum.SingleSnap ? 2 : 1);
            if (SplashSnap == SplashSnapEnum.DoubleSnap)
            {
                StagesToSplash /= 2;
                StagesToSplash /= 2;
            }

            //Load Skills
            SkillInfoHandler.LoadSkills();
            SkillInfoHandler.FillSkills(save);

            LBSkill = SkillInfoHandler.Skills.Where(x => x.TalentID == "PetQTE").FirstOrDefault();

            LBSkill.CurrentLevel = GetSplashSkillLevel(LBSkill, _powerSurgeLevel + _arcaneBargainLevel, BaseSkip, APSetBonus, StagesToSplash);

            return LBSkill;
        }

        [Obsolete("Logic changed with 3.2")]
        public static Skill CalculateBurstSkillBoostEfficiency(SaveFile save, bool reload = false)
        {
            if (ARSkill != null && !reload)
            {
                return ARSkill;
            }

            EquipmentHandler.FillEquipment(save);
            EquipmentHandler.LoadSetInformation(save);

            string AGSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "HighTecLightning").Count() > 0 ? AppResources.YesText : AppResources.NoText;
            bool APSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Platinum").Count() > 0;

            FormulaHelper.LoadPassiveSkillCosts(EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Mech").Count() > 0);
            int _arcaneBargainLevel = FormulaHelper.GetArcaneBargainLevel(save.ThisPlayer.DustSpent);
            int _intimidatingPresenceLevel = FormulaHelper.GetIntimidatingPresenceLevel(save.ThisPlayer.TotalSkillPoints);
            int _powerSurgeLevel = FormulaHelper.GetPowerSurgeLevel(save.ThisPlayer.TotalPetLevels);
            int _mysticalImpactLevel = FormulaHelper.GetMysticalImpactLevel(save.ThisPlayer.TitanPoints);
            int _antiTitanCannonLevel = FormulaHelper.GetAntiTitanCannonLevel(HelpersFactory.GetTotalMasteriesLevel());

            int PassiveSkillReduction = _arcaneBargainLevel + _intimidatingPresenceLevel;

            // AP Set bonus
            double APSetBonus = 1;
            if (APSetCompleted)
            {
                var apSet = EquipmentHandler.EquipmentSets.Where(x => x.Set == "Platinum").FirstOrDefault();
                int cp = save.CraftingPower;

                APSetBonus = FormulaHelper.GetEquipmentSetBonus(apSet, cp, 2);
            }

            int BaseSkip = AGSetCompleted == AppResources.YesText ? 11 : 3;
            BaseSkip = 0; //Das ist splashing. Hier geht es um skipping

            int NormalTitanCount = FormulaHelper.GetTitanCount((int)save.ThisPlayer.StageMax);
            var SplashSnap = (SplashSnapEnum)JfTypeConverter.ForceInt(LocalSettingsORM.GetSplashSnapSetting());
            int StagesToSplash = (NormalTitanCount - PassiveSkillReduction) / (SplashSnap == SplashSnapEnum.SingleSnap ? 2 : 1);
            if (SplashSnap == SplashSnapEnum.DoubleSnap)
            {
                StagesToSplash /= 2;
                StagesToSplash /= 2;
            }

            //Load Skills
            SkillInfoHandler.LoadSkills();
            SkillInfoHandler.FillSkills(save);

            ARSkill = SkillInfoHandler.Skills.Where(x => x.TalentID == "BurstSkillBoost").FirstOrDefault();

            ARSkill.CurrentLevel = GetSplashSkillLevel(ARSkill, _mysticalImpactLevel + _arcaneBargainLevel, BaseSkip, APSetBonus, StagesToSplash);
            return ARSkill;
        }

        /// <summary>
        /// Calculates the level you need to aim for
        /// </summary>
        /// <param name="skill">Skill to calculate level for</param>
        /// <param name="passiveSkillBoost">Boost of corresponding passive skill</param>
        /// <param name="baseSkip">Base skip amount</param>
        /// <param name="apSetBonus">Bonus from AP Set</param>
        /// <param name="stagesToSplash">Stages you need to splash through</param>
        /// <returns></returns>
        private static int GetSplashSkillLevel(Skill skill, int passiveSkillBoost, int baseSkip, double apSetBonus, int stagesToSplash)
        {
            // Splash Effect is located in B

            try
            {
                int skillLevel = 0;
                int possibleSkip = 0;
                for (int i = 0; i < skill.B.Count; i++)
                {
                    possibleSkip = (int)((baseSkip + passiveSkillBoost + skill.B[i]) * apSetBonus);

                    if (i == skill.B.Count - 1)
                    {
                        skillLevel = i;
                        skill.SetSplashEffect(possibleSkip);
                        break;
                    }

                    if (possibleSkip >= stagesToSplash)
                    {
                        skillLevel = i;
                        skill.SetSplashEffect(possibleSkip);
                        break;
                    }
                }

                skill.SetSplashEffect(possibleSkip);
                return skillLevel;

            }
            catch (Exception)
            {
                //Logger.WriteToLogFile($"Could not Get Splash Skill level: {e.Message}");
                return skill.MaxLevel;
            }
        }
    }
}