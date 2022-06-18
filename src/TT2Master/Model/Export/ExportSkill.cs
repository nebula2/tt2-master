using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// Descrtibes exportable skill
    /// </summary>
    public class ExportSkill
    {
        /// <summary>
        /// ID of skill
        /// </summary>
        public string TalentID { get; set; }
        /// <summary>
        /// Class
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// Branch
        /// </summary>
        public string Branch { get; set; }
        /// <summary>
        /// Slot location
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// SP you need to unlock this
        /// </summary>
        public int SPReq { get; set; }
        /// <summary>
        /// skill you need to have unlocked and leveled before you can acces this
        /// </summary>
        public string TalentReq { get; set; }
        /// <summary>
        /// Tier as int
        /// </summary>
        public int TierNum { get; set; }
        /// <summary>
        /// Tier
        /// </summary>
        public string Tier { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Note
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Max level
        /// </summary>
        public int MaxLevel { get; set; }
        /// <summary>
        /// Primary bonus type
        /// </summary>
        public string BonusTypeA { get; set; }
        /// <summary>
        /// Secondary bonus type
        /// </summary>
        public string BonusTypeB { get; set; }
        /// <summary>
        /// Tertiary bonus type
        /// </summary>
        public string BonusTypeC { get; set; }
        /// <summary>
        /// Currrent level
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="skill"></param>
        public ExportSkill(Skill skill)
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
            BonusTypeA = skill.BonusTypeA;
            BonusTypeB = skill.BonusTypeB;
            BonusTypeC = skill.BonusTypeC;
            CurrentLevel = skill.CurrentLevel;
        }
    }
}