using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TT2Master
{
    /// <summary>
    /// An item of a Milestone for an SP-Build
    /// </summary>
    [Table("SPBUILDMILESTONEITEM")]
    public class SPBuildMilestoneItem
    {
        #region Properties
        /// <summary>
        /// Identifier for sqlite
        /// </summary>
        [PrimaryKey]
        public string Identifier { get; set; }

        private string _build;
        /// <summary>
        /// The corresponding <see cref="SPBuild"/>
        /// </summary>
        public string Build
        {
            get => _build;
            set
            {
                if(value != _build)
                {
                    _build = value;
                    if (!string.IsNullOrWhiteSpace(SkillID))
                    {
                        SetIdentifier(value, SkillID);
                    }
                }
            }
        }

        private int _milestone;
        /// <summary>
        /// The numeric ID
        /// </summary>
        public int Milestone
        {
            get => _milestone;
            set
            {
                if(value != _milestone)
                {
                    _milestone = value;
                    if(!string.IsNullOrWhiteSpace(SkillID) && !string.IsNullOrWhiteSpace(Build))
                    {
                        SetIdentifier(Build, SkillID);
                    }
                }
            }
        }

        private string _skillID;
        /// <summary>
        /// The SkillID
        /// </summary>
        public string SkillID
        {
            get => _skillID;
            set
            {
                if(value != _skillID)
                {
                    _skillID = value;
                    if (!string.IsNullOrWhiteSpace(Build))
                    {
                        SetIdentifier(Build, value);
                    }
                }
            }
        }

        /// <summary>
        /// The amount of level for <see cref="SkillID"/>
        /// </summary>
        public int Amount { get; set; }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets identifier for this object
        /// </summary>
        /// <param name="build"></param>
        /// <param name="skill"></param>
        private void SetIdentifier(string build, string skill) => Identifier = $"{build},{Milestone},{skill}";
        #endregion

        #region Ctor
        /// <summary>
        /// Full ctor
        /// </summary>
        /// <param name="build">the build-ID</param>
        /// <param name="milestone">the milestone-ID</param>
        /// <param name="skill">the skill-id</param>
        /// <param name="amount">the amount to level</param>
        public SPBuildMilestoneItem(string build, int milestone, string skill, int amount)
        {
            Build = build;
            Milestone = milestone;
            SkillID = skill;
            Amount = amount;
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="build"> ID of <see cref="SPBuild"/></param>
        /// <param name="milestone">ID of <see cref="Milestone"/></param>
        /// <param name="skill">ID of <see cref="Skill"/></param>
        public SPBuildMilestoneItem(string build, int milestone, string skill)
        {
            Build = build;
            Milestone = milestone;
            SkillID = skill;
            Amount = 0;
        }

        /// <summary>
        /// ctor for sqlite
        /// </summary>
        public SPBuildMilestoneItem()
        {

        }
        #endregion
    }
}