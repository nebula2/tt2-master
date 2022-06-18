using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Assets;
using TT2Master.Model.Clan;
using TT2Master.Model.PassiveSkills;
using TT2Master.Resources;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master
{
    public static class FormulaHelper
    {
        #region Stage Logic
        public static double StartingStage()
        {
            try
            {
                // get normal advanced start from clan exp
                ClanTraitHandler.OnProblemHaving += ClanTraitHandler_OnProblemHaving;
                var trait = ClanTraitHandler.GetClanTrait();
                ClanTraitHandler.OnProblemHaving -= ClanTraitHandler_OnProblemHaving;

                double startFromClanPercentage = trait != null ? trait.AdvancedStart : 0;
                double cleanAdvStage = startFromClanPercentage > 0 ? startFromClanPercentage * App.Save.ThisPlayer?.StageMax ?? 0 : App.Save.ThisPlayer?.StageMax ?? 0;

                // get if we need to add bonus from set
                EquipmentHandler.LoadSetInformation(App.Save);
                var set = EquipmentHandler.EquipmentSets.Where(x => x.Set == "Musketeer").FirstOrDefault();

                double setMultiplier = set != null
                    ? set.Completed ? set.Amount1 * (1 - startFromClanPercentage) : 1
                    : 1;

                return cleanAdvStage + cleanAdvStage * setMultiplier;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double GetClanAdvancedStart()
        {
            try
            {
                // get normal advanced start from clan exp
                ClanTraitHandler.OnProblemHaving += ClanTraitHandler_OnProblemHaving;
                bool success = ClanTraitHandler.LoadItemsFromInfoFile();
                var trait = ClanTraitHandler.ClanTraits.Where(x => x.ClanExp < App.Save.ThisClan?.ClanRaidExp).OrderByDescending(x => x.ClanLevel).FirstOrDefault();
                ClanTraitHandler.OnProblemHaving -= ClanTraitHandler_OnProblemHaving;

                return trait != null ? trait.AdvancedStart : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the amount of titans for the given stage
        /// </summary>
        /// <param name="stage">Stage</param>
        /// <returns>TitanCount</returns>
        public static int GetTitanCount(int stage) => (int)(2 * Math.Floor((double)(stage / 500 - 1)) + 10);
        #endregion

        #region Equipment
        /// <summary>
        /// Calculates CraftingPower for spent shards
        /// </summary>
        /// <param name="shardsSpent">how many shards have you spent?</param>
        /// <returns>Crafting Power</returns>
        public static int GetCraftingPower(int shardsSpent) => 1 + JfTypeConverter.ForceInt(((shardsSpent) / 500).ToString("N0"));

        /// <summary>
        /// Calculates the equipmentBonus for a set under consideration of craftingPower
        /// </summary>
        /// <param name="equip">the <see cref="EquipmentSet"/> to calculate the value for</param>
        /// <param name="craftingPower">Crafting Power. Can be calculated with <see cref="GetCraftingPower(int)"/></param>
        /// <param name="bonusId">Bonus ID (1 to 3)</param>
        /// <returns></returns>
        public static double GetEquipmentSetBonus(EquipmentSet set, int craftingPower, int bonusId)
        {
            // crappy input prevention
            if(bonusId < 1 || bonusId > 3)
            {
                bonusId = 1;
            }

            switch (bonusId)
            {
                case 1:
                    return set.Amount1 * Math.Pow(set.Inc1, Math.Pow(craftingPower - 1, set.Expo1));
                case 2:
                    return set.Amount2 * Math.Pow(set.Inc2, Math.Pow(craftingPower - 1, set.Expo2));
                case 3:
                    return set.Amount3 * Math.Pow(set.Inc3, Math.Pow(craftingPower - 1, set.Expo3));
                default:
                    return 0;
            }
        }
        #endregion

        #region Passive Skills
        /// <summary>
        /// Loads the Dictionaries for the passive skill costs under concideration of the mech set
        /// </summary>
        /// <param name="mechSetCompleted">Is the mech set completed?</param>
        /// <returns>True if successful</returns>
        public static bool LoadPassiveSkillCosts(bool mechSetCompleted)
        {
            try
            {
                PassiveSkillHandler.OnLogMePlease += PassiveSkillHandler_OnLogMePlease;
                PassiveSkillHandler.OnProblemHaving += PassiveSkillHandler_OnProblemHaving;
                bool success = PassiveSkillHandler.LoadPassiveSkills(mechSetCompleted);

                PassiveSkillHandler.OnLogMePlease += PassiveSkillHandler_OnLogMePlease;
                PassiveSkillHandler.OnProblemHaving += PassiveSkillHandler_OnProblemHaving;

                return success;

            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("FormulaHelper.LoadPassiveSkillCosts", new CustErrorEventArgs(ex));
                return false;
            }
        }

        public static int GetMysticalImpactLevel(double spent)
        {
            try
            {
                return App.Save.SorcererSplashSkipLevel;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public static int GetIntimidatingPresenceLevel(int spent)
        {

            try
            {
                return App.Save.LessMonstersLevel;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns level for the passive skill Arcane Bargain
        /// </summary>
        /// <param name="dustSpent">how much dust has the player spent?</param>
        /// <returns></returns>
        public static int GetArcaneBargainLevel(int spent)
        {
            try
            {
                return App.Save.RaidCardPowerLevel;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public static int GetAntiTitanCannonLevel(int spent)
        {
            try
            {
                return App.Save.ClanShipSplashSkipLevel;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public static int GetPowerSurgeLevel(int spent)
        {
            try
            {
                return App.Save.PetSplashSkipLevel;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public static int GetSilentMarchLevel(int spent)
        {
            try
            {
                return App.Save.SilentMarchLevel;
            }
            catch (Exception)
            {
                return 1;
            }
        }
        #endregion

        #region E + D

        private static void PassiveSkillHandler_OnProblemHaving(object sender, Helpers.CustErrorEventArgs e) => OnProblemHaving?.Invoke(sender, e);

        private static void PassiveSkillHandler_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => OnLogMePlease?.Invoke(sender, e);

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

        private static void ClanTraitHandler_OnProblemHaving(object sender, Helpers.CustErrorEventArgs e) => OnProblemHaving?.Invoke(sender, e);
        #endregion
    }
}