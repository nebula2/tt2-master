using System;
using System.Collections.Generic;
using System.Linq;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a passive skill from "PassiveSkillInfo.csv
    /// </summary>
    public class PassiveSkill
    {
        #region Properties
        /// <summary>
        /// Identifier
        /// </summary>
        public string PassiveSkillId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Upgrade type
        /// </summary>
        public string UpgradeType { get; set; }

        /// <summary>
        /// Bonus type
        /// </summary>
        public string BonusType { get; set; }

        /// <summary>
        /// Sprite index
        /// </summary>
        public int TextSpriteIndex { get; set; }

        /// <summary>
        /// Max level
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Cost value dictionary
        /// </summary>
        public List<LevelCostValue> CostValueDict { get; set; } = new List<LevelCostValue>();

        #endregion

        #region public methods
        /// <summary>
        /// Returns the value from cost
        /// </summary>
        /// <param name="cost">amount of units spent</param>
        /// <returns></returns>
        public double GetValueFromCost(double cost)
        {
            try
            {
                return Math.Max(CostValueDict.Where(x => cost <= x.Cost).OrderBy(n => n.Level).FirstOrDefault().Value, 1);
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns the value from level
        /// </summary>
        /// <param name="level">the current level of the skill</param>
        /// <returns></returns>
        public double GetValueFromLevel(int level)
        {
            try
            {
                return Math.Max(CostValueDict.Where(x => level == x.Level).FirstOrDefault().Value, 1);
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns the level from cost
        /// </summary>
        /// <param name="cost">amount of units spent</param>
        /// <returns></returns>
        public int GetLevelFromCost(double cost)
        {
            try
            {
                return Math.Max(CostValueDict.Where(x => cost <= x.Cost).OrderBy(n => n.Level).FirstOrDefault().Level, 1);
            }
            catch (Exception)
            {
                return 1;
            }
        }
        #endregion
    }
}
