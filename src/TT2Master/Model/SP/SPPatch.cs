using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// A class for SP patch. like what do i need to level next and so on
    /// </summary>
    public class SPPatch : Skill
    {
        #region Properties
        /// <summary>
        /// The amount to level up
        /// </summary>
        public int LevelUpAmount => LevelToAmount - CurrentLevel;

        /// <summary>
        /// Current milestone in Build
        /// </summary>
        public int CurrentMilestone { get; set; }

        /// <summary>
        /// The Level to aim for
        /// </summary>
        public int LevelToAmount { get; set; }

        /// <summary>
        /// What does it cost to purchase a level
        /// </summary>
        public int SPCost => Cost[CurrentLevel];

        /// <summary>
        /// Color that indicates wheter to upgrade or not
        /// </summary>
        public string ColorToShow => GetColor();

        /// <summary>
        /// The cost of one Upgrade
        /// </summary>
        public int UpgradeCost => Cost[CurrentLevel];
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor from <see cref="Skill"/> base class
        /// </summary>
        /// <param name="skill"></param>
        public SPPatch(Skill skill)
        {
            TalentID = skill.TalentID;
            Class = skill.Class;
            Branch = skill.Branch;
            Slot = skill.Slot;
            SPReq = skill.SPReq;
            TalentReq = skill.TalentReq;
            TierNum = skill.TierNum;
            Tier = skill.Tier;
            Name = skill.Name;
            Note = skill.Note;
            MaxLevel = skill.MaxLevel;
            S = skill.S;
            A = skill.A;
            B = skill.B;
            C = skill.C;
            Cost = skill.Cost;
            BonusTypeA = skill.BonusTypeA;
            BonusTypeB = skill.BonusTypeB;
            BonusTypeC = skill.BonusTypeC;
            BonusTypeD = skill.BonusTypeD;
            BonusAmountD = skill.BonusAmountD;
            CurrentLevel = skill.CurrentLevel;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the color to display this Skill
        /// </summary>
        /// <returns></returns>
        private string GetColor()
        {
            //too much points spent into this
            if (LevelUpAmount < 0)
            {
                return "Red";
            }

            //nothing to do
            if (LevelUpAmount == 0)
            {
                return "White";
            }

            //you can afford an upgrade
            if(UpgradeCost <= (SaveFile.SPReceived - SaveFile.SPSpent))
            {
                return "Green";
            }

            //indicates that this is something to upgrade
            return "Orange";
        } 
        #endregion
    }
}
