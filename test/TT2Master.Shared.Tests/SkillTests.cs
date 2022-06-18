using System.Collections.Generic;
using TT2Master.Shared.Models;
using Xunit;

namespace TT2Master.Shared.Tests
{
    public class SkillTests
    {
        private static Skill GetMockSkill()
        {
            return new Skill()
            {
                A = new List<double> { 1.25E+00, 2.18E+00, 4.13E+00, 6.16E+00, 1.61E+01, 5.32E+01, 1.76E+02, 1.09E+03, 1.02E+04, 1.28E+05, 4.08E+06, 2.64E+08, 6.50E+10, 6.16E+13, 5.07E+17, 6.21E+22, 3.00E+29, 2.07E+38 },
                B = new List<double> { 0, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7, 7, 7, 7, 7 },
                C = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                BonusTypeA = "GuidedBladeDeadlyStrikeBoost",
                BonusTypeB = "GuidedBladeBoostMaxCount",
                BonusTypeC = "None",
                BonusTypeD = "GuidedBladeApplyChance",
                BonusAmountD = 0.01,
                Branch = "BranchGreen",
                Cost = new List<int> { 3, 4, 5, 7, 9, 12, 16, 21, 27, 35, 46, 60, 78, 101, 131, 170, 221, 287 },
                CurrentLevel = 0,
                MaxLevel = 15,
                Name = "Guided Blade",
                Note = "Chance to gain stackable DS buff after each DS.",
                S = new List<int> { 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800 },
                Slot = 9,
                SplashEffectAtLevel = 0,
                SPReq = 50,
                TalentID = "GuidedBlade",
                Class = "Rogue",
                TalentReq = "Cloaking",
                TierNum = 4,
                Tier = "IV",
                SetLevelIncreaseAmount = 0,
            };
        }

        /// <summary>
        /// Skill should return correct spent amount for a default skill
        /// </summary>
        [Fact]
        public void Skill_ShouldReturnCorrectSpentAmount()
        {
            var skill = GetMockSkill();
            Assert.Equal(0, skill.GetSpSpentAmount());

            skill.CurrentLevel = 1;
            Assert.Equal(3, skill.GetSpSpentAmount());
        }

        /// <summary>
        /// Skill should return correct spent amount for a skill with a special set increasement
        /// </summary>
        [Fact]
        public void Skill_ShouldReturnCorrectSpentAmountForSpecialSetIncreasement()
        {
            var skill = GetMockSkill();
            skill.SetLevelIncreaseAmount = 1;

            // this can not happen because if you have increasement you jump straight from 0 to 2 but should be handled.
            skill.CurrentLevel = 1;
            Assert.Equal(3, skill.GetSpSpentAmount());

            skill.CurrentLevel = 2;
            Assert.Equal(3, skill.GetSpSpentAmount());
        }

        /// <summary>
        /// Skill should return correct upgrade cost for a default skill
        /// </summary>
        [Fact]
        public void Skill_ShouldReturnCorrectUpgradeCost()
        {
            var skill = GetMockSkill();

            Assert.Equal(3, skill.GetUpgradeCost());
        }

        /// <summary>
        /// Skill should return correct upgrade cost for a skill with a special set increasement
        /// </summary>
        [Fact]
        public void Skill_ShouldReturnCorrectUpgradeCostForSpecialSetIncreasement()
        {
            var skill = GetMockSkill();
            skill.SetLevelIncreaseAmount = 1;

            // this can not happen because if you have increasement you jump straight from 0 to 2 but should be handled.
            skill.CurrentLevel = 1;
            Assert.Equal(3, skill.GetUpgradeCost());

            skill.CurrentLevel = 2;
            Assert.Equal(4, skill.GetUpgradeCost());
        }
    }
}
