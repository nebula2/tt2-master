using SQLite;
using System.Collections.Generic;

namespace TT2Master
{
    /// <summary>
    /// A Milestone for an SP-Build
    /// </summary>
    [Table("SPBUILDMILESTONE")]
    public class SPBuildMilestone
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
                if (value != _build)
                {
                    _build = value;
                    SetIdentifier(value);
                }
            }
        }

        private int _milestone { get; set; }
        /// <summary>
        /// The numeric ID
        /// </summary>
        public int Milestone
        {
            get => _milestone;
            set
            {
                if (value != _milestone)
                {
                    _milestone = value;
                    if (!string.IsNullOrWhiteSpace(Build))
                    {
                        SetIdentifier(Build);
                    }
                }
            }
        }

        /// <summary>
        /// The amount of SP required for this milestone
        /// </summary>
        public int SPReq { get; set; } = 0;

        /// <summary>
        /// List of Milestone-Items
        /// </summary>
        [Ignore]
        public List<SPBuildMilestoneItem> MilestoneItems { get; set; }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the identifier for this object
        /// </summary>
        /// <param name="build"></param>
        private void SetIdentifier(string build)
        {
            Identifier = $"{build},{Milestone}";

            //sync child
            foreach (var item in MilestoneItems)
            {
                item.Build = build;
                item.Milestone = Milestone;
            }
        }

        /// <summary>
        /// Initializes the fields that cannot be null
        /// </summary>
        private void InitFields() => MilestoneItems = new List<SPBuildMilestoneItem>();

        #endregion

        #region Ctor
        /// <summary>
        /// Full ctor
        /// </summary>
        /// <param name="build">the build-ID</param>
        /// <param name="milestone">the milestone-ID</param>
        /// <param name="spreq">the required sp points</param>
        public SPBuildMilestone(string build, int milestone, int spreq)
        {
            InitFields();

            Build = build;
            Milestone = milestone;
            SPReq = spreq;

        }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="build"> ID of <see cref="SPBuild"/></param>
        /// <param name="milestone">ID of <see cref="Milestone"/></param>
        /// <param name="skill">ID of <see cref="Skill"/></param>
        public SPBuildMilestone(string build, int milestone)
        {
            InitFields();

            Build = build;
            Milestone = milestone;
            SPReq = 0;
        }

        /// <summary>
        /// ctor for sqlite
        /// </summary>
        public SPBuildMilestone()
        {
            InitFields();
        }
        #endregion
    }
}
