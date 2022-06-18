using System;
using System.Collections.Generic;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Class that describes a skill
    /// </summary>
    public class Skill
    {
        #region Properties
        /// <summary>
        /// Identifier
        /// </summary>
        public string TalentID { get; set; }

        /// <summary>
        /// Knight, warlord, sorcerer, rogue
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// To which branch does this belong
        /// BranchRed = Knight
        /// BranchYellow = Warlord
        /// BranchBlue = Sorcerer
        /// BranchGreen = Rogue
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// On which slot does this sit?
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// Amount of Spell Points you have to put into <see cref="Branch"/> to be able to purchase this
        /// </summary>
        public int SPReq { get; set; }

        /// <summary>
        /// The <see cref="TalentID"/> you need in order to get this
        /// </summary>
        public string TalentReq { get; set; }

        /// <summary>
        /// Tier as numeric
        /// </summary>
        public int TierNum { get; set; }

        /// <summary>
        /// The Tier in greek letters
        /// I, II, III, IV
        /// </summary>
        public string Tier { get; set; }

        /// <summary>
        /// Name of this skill
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A brief description for this skill
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The max level for this skill
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Don't know what this is for
        /// </summary>
        public List<int> S { get; set; }

        /// <summary>
        /// The cost for each level sorted by level asc
        /// </summary>
        public List<int> Cost { get; set; }

        /// <summary>
        /// The primary Bonus type
        /// </summary>
        public string BonusTypeA { get; set; }

        /// <summary>
        /// The efficiency for <see cref="BonusTypeA"/> on each level
        /// </summary>
        public List<double> A { get; set; }

        /// <summary>
        /// The secondary Bonus Type
        /// </summary>
        public string BonusTypeB { get; set; }

        /// <summary>
        /// The efficiency for <see cref="BonusTypeB"/>
        /// </summary>
        public List<double> B { get; set; }

        /// <summary>
        /// The third Bonus type
        /// </summary>
        public string BonusTypeC { get; set; }

        /// <summary>
        /// The amount of <see cref="BonusTypeC"/>
        /// </summary>
        public List<double> C { get; set; }

        /// <summary>
        /// The fourth Bonus type
        /// </summary>
        public string BonusTypeD { get; set; } // came with 5.3.0 (moved from BonusTypeC)

        /// <summary>
        /// The amount of <see cref="BonusTypeC"/>
        /// </summary>
        public double BonusAmountD { get; set; } // came with 5.3.0 (moved from BonusAmountD)

        /// <summary>
        /// Current Level the Player has on this
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Path to image
        /// </summary>
        public string ImagePath => $"{TalentID}";

        /// <summary>
        /// Splashing effect at current level
        /// </summary>
        public double SplashEffectAtLevel { get; set; }

        /// <summary>
        /// The amount of level increased by a set
        /// Cost and upgrade calculation will be reduced by the amount of this it is higher than 0
        /// </summary>
        public int SetLevelIncreaseAmount { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public Skill()
        {
            S = new List<int>();
            Cost = new List<int>();
            A = new List<double>();
            B = new List<double>();
            C = new List<double>();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Resets splash effect
        /// </summary>
        public void ResetSplashEffect()
        {
            SplashEffectAtLevel = CurrentLevel <= B.Count ? B[CurrentLevel] : B[B.Count];
        }

        /// <summary>
        /// Sets splash effect
        /// </summary>
        /// <param name="effect"></param>
        public void SetSplashEffect(int effect)
        {
            SplashEffectAtLevel = effect;
        }

        /// <summary>
        /// Gets amount of SP which has been spent on this skill
        /// </summary>
        /// <returns></returns>
        public int GetSpSpentAmount()
        {
            int sp = 0;
            try
            {
                if (CurrentLevel == 0)
                {
                    return 0;
                }

                int index = CurrentLevel <= 1 ? CurrentLevel : CurrentLevel - SetLevelIncreaseAmount;

                for (int i = 0; i < index; i++)
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

        /// <summary>
        /// Gets costs for an upgrade to the next level
        /// </summary>
        /// <returns></returns>
        public int GetUpgradeCost()
        {
            try
            {
                int index = CurrentLevel - SetLevelIncreaseAmount;
                return Cost[Math.Max(index, 0)];
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the primary effect this skill has at the given level
        /// </summary>
        /// <param name="level">skill-level</param>
        /// <returns></returns>
        public double GetPrimaryEffectForLevel(int level)
        {
            try
            {
                return level > A.Count ? A[A.Count - 1] : level < 0 ? A[0] : A[level];
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the secondary effect this skill has at the given level
        /// </summary>
        /// <param name="level">skill-level</param>
        /// <returns></returns>
        public double GetSecondaryEffectForLevel(int level)
        {
            try
            {
                return level > B.Count ? B[B.Count - 1] : level < 0 ? B[0] : B[level];
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion
    }
}
