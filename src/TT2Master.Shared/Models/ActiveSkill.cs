using System;
using System.Collections.Generic;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes an active skill
    /// </summary>
    public class ActiveSkill
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string ActiveSkillID { get; set; }

        /// <summary>
        /// Primary bonus type
        /// </summary>
        public string BonusTypeA { get; set; }

        /// <summary>
        /// Primary bonus amounts
        /// </summary>
        public List<double> Amount { get; set; }

        /// <summary>
        /// Secondary bonus type
        /// </summary>
        public string BonusTypeB { get; set; }

        /// <summary>
        /// Secondary bonus amounts
        /// </summary>
        public List<double> SecondaryAmount { get; set; }

        /// <summary>
        /// Costs for each level
        /// </summary>
        public List<double> Cost { get; set; }

        /// <summary>
        /// Skill duration
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Skill cooldown
        /// </summary>
        public int Cooldown { get; set; }

        /// <summary>
        /// Stage on which the player unlocks this skill
        /// </summary>
        public int UnlockLevel { get; set; }

        /// <summary>
        /// Cost in diamonds for this skill
        /// </summary>
        public int DiamondCost { get; set; }

        /// <summary>
        /// Mana costs for this skill
        /// </summary>
        public List<double> ManaCost { get; set; }

        /// <summary>
        /// Does this run while the player is inactive?
        /// </summary>
        public bool RunWhileInactive { get; set; }

        /// <summary>
        /// Type of skill
        /// </summary>
        public string SkillType { get; set; }

        /// <summary>
        /// Splash effect at level
        /// </summary>
        public int SplashEffectAtLevel { get; set; }

        /// <summary>
        /// Current Level
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public ActiveSkill()
        {
            Amount = new List<double>();
            SecondaryAmount = new List<double>();
            Cost = new List<double>();
            ManaCost = new List<double>();
        }

        /// <summary>
        /// Sets splash effect at the given value
        /// </summary>
        /// <param name="effect">effect to set</param>
        public void SetSplashEffect(int effect) => SplashEffectAtLevel = effect;

        /// <summary>
        /// Gets the spent cost to achieve level
        /// </summary>
        /// <returns></returns>
        public double GetCostSpentAmount()
        {
            double sp = 0;
            try
            {
                if (CurrentLevel == 0)
                {
                    return 0;
                }

                for (int i = 0; i < CurrentLevel; i++)
                {
                    sp += Cost[i];
                }

                return sp;
            }
            catch (Exception)
            {
                sp = 0;
                for (int i = 0; i < Cost.Count - 1; i++)
                {
                    sp += Cost[i];
                }

                return sp;
            }
        }
    }
}
