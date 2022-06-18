using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// Describes a skill to export
    /// </summary>
    public class ExportSkillLvl
    {
        /// <summary>
        /// Identifier of skill
        /// </summary>
        public string TalentID { get; set; }
        /// <summary>
        /// Current level of skill
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="skill"></param>
        public ExportSkillLvl(Skill skill)
        {
            TalentID = skill.TalentID;
            CurrentLevel = skill.CurrentLevel;
        }
    }
}