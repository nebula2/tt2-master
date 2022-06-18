using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TT2Master.Helpers;
using TT2Master.Model.Assets;
using TT2Master.Resources;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;

namespace TT2Master.Model.PassiveSkills
{
    public static class PassiveSkillHandler
    {
        private static PassiveSkill GetPassiveSkillFromRow(bool mechaSetComplete, PassiveSkill skill)
        {
            if (!mechaSetComplete)
            {
                return skill;
            }

            foreach (var item in skill.CostValueDict)
            {
                item.Cost = Math.Floor(item.Cost * Reductions.MechaSetReduction);
            }

            return skill;
        }

        #region Properties
        public static PassiveSkill IntimidatingPresence { get; private set; } = new PassiveSkill();
        public static PassiveSkill PowerSurge { get; private set; } = new PassiveSkill();
        public static PassiveSkill AntiTitanCannon { get; private set; } = new PassiveSkill();
        public static PassiveSkill SorcererSplashSkip { get; private set; } = new PassiveSkill();
        public static PassiveSkill ArcaneBargain { get; private set; } = new PassiveSkill();
        public static PassiveSkill SilentMarch { get; private set; } = new PassiveSkill();
        #endregion

        #region Public methods
        /// <summary>
        /// Fills properties from info file. call this before using properties!
        /// </summary>
        /// <returns></returns>
        public static bool LoadPassiveSkills(bool mechaSetComplete)
        {
            try
            {
                OnLogMePlease?.Invoke("PassiveSkillHandler", new InformationEventArgs("LoadPassiveSkills start"));

                var passives = AssetReader.GetInfoFile<PassiveSkill, PassiveSkillMap>(InfoFileEnum.PassiveSkillInfo);

                IntimidatingPresence = GetPassiveSkillFromRow(mechaSetComplete, passives.Where(x => x.PassiveSkillId == "LessMonsters").FirstOrDefault());
                PowerSurge = GetPassiveSkillFromRow(mechaSetComplete, passives.Where(x => x.PassiveSkillId == "PetSplashSkip").FirstOrDefault());
                AntiTitanCannon = GetPassiveSkillFromRow(mechaSetComplete, passives.Where(x => x.PassiveSkillId == "ClanShipSplashSkip").FirstOrDefault());
                SorcererSplashSkip = GetPassiveSkillFromRow(mechaSetComplete, passives.Where(x => x.PassiveSkillId == "SorcererSplashSkip").FirstOrDefault());
                ArcaneBargain = GetPassiveSkillFromRow(mechaSetComplete, passives.Where(x => x.PassiveSkillId == "RaidCardPower").FirstOrDefault());
                SilentMarch = GetPassiveSkillFromRow(mechaSetComplete, passives.Where(x => x.PassiveSkillId == "SilentMarch").FirstOrDefault());

                OnLogMePlease?.Invoke("PassiveSkillHandler.LoadPassiveSkills", new InformationEventArgs("Passive skills loaded"));
                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("PassiveSkillHandler.LoadPassiveSkills", new CustErrorEventArgs(ex));
                return false;
            }
        }
        #endregion

        #region events and delegates
        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public static event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}
