using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Mvvm;
using SQLite;
using TT2Master.Model.SP;
using TT2Master.Resources;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// Spell points build
    /// </summary>
    [Table("SPBUILD")]
    public class SPBuild : BindableBase
    {
        #region Properties
        private string _iD;

        /// <summary>
        /// Identifier
        /// </summary>
        [PrimaryKey]
        public string ID
        {
            get => _iD;
            set
            {
                if(value != _iD)
                {
                    SetProperty(ref _iD, value);

                    UpdateChilds();
                }
            }
        }

        private bool _editable = true;
        /// <summary>
        /// False if user can not edit this build
        /// </summary>
        public bool Editable
        {
            get => _editable;
            set
            {
                if(value != _editable)
                {
                    SetProperty(ref _editable, value);
                }
            }
        }

        private string _ownerId;
        /// <summary>
        /// ID of owner
        /// </summary>
        public string OwnerId { get => _ownerId; set => SetProperty(ref _ownerId, value); }

        private string _ownerName;
        /// <summary>
        /// Name of owner
        /// </summary>
        public string OwnerName { get => _ownerName; set => SetProperty(ref _ownerName, value); }

        private string _name;
        /// <summary>
        /// Name of build
        /// </summary>
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private int _blueAmount;
        /// <summary>
        /// Amount of SP in blue tree
        /// </summary>
        public int BlueAmount { get => _blueAmount; set => SetProperty(ref _blueAmount, value); }
        private int _redAmount;
        /// <summary>
        /// Amount of SP in red tree
        /// </summary>
        public int RedAmount { get => _redAmount; set => SetProperty(ref _redAmount, value); }
        private int _greenAmount;
        /// <summary>
        /// Amount of SP in green tree
        /// </summary>
        public int GreenAmount { get => _greenAmount; set => SetProperty(ref _greenAmount, value); }
        private int _yellowAmount;
        /// <summary>
        /// Amount of SP in yellow tree
        /// </summary>
        public int YellowAmount { get => _yellowAmount; set => SetProperty(ref _yellowAmount, value); }

        private string _description;
        /// <summary>
        /// A descriptive text for this build
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if(value != _description)
                {
                    SetProperty(ref _description, value);
                }
            }
        }

        private string _version;
        /// <summary>
        /// The TT2 version this build is designed for
        /// </summary>
        public string Version
        {
            get => _version;
            set
            {
                if(value != _version)
                {
                    SetProperty(ref _version, value);
                }
            }
        }

        private List<SPBuildMilestone> _milestones = new List<SPBuildMilestone>();
        /// <summary>
        /// List of <see cref="SPBuildMilestone"/>
        /// </summary>
        [Ignore]
        public List<SPBuildMilestone> Milestones
        {
            get => _milestones;
            set
            {
               if(value != _milestones)
                {
                    SetProperty(ref _milestones, value);
                }
            }
        }
        #endregion

        #region private Methods
        /// <summary>
        /// Updates the Build-ID in childs
        /// </summary>
        private void UpdateChilds()
        {
            foreach (var item in Milestones)
            {
                item.Build = ID;
            }
        }

        /// <summary>
        /// Initializes fields that need to be initialized
        /// </summary>
        private void InitializeFields()
        {
            Milestones = new List<SPBuildMilestone>();
            OwnerId = "?";
            OwnerName = "?";
            Name = "?";
        }
        #endregion

        #region ctor
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="name">Identifier</param>
        public SPBuild(string name)
        {
            InitializeFields();

            ID = name;
        }

        /// <summary>
        /// Ctor to use from SP Optimizer for Sp Follower
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="version"></param>
        /// <param name="ownerId"></param>
        /// <param name="ownerName"></param>
        public SPBuild(List<List<SPOptSkill>> skills, string version, string ownerId = "", string ownerName = "")
        {
            InitializeFields();

            OwnerId = string.IsNullOrWhiteSpace(ownerId) ? App.Save.ThisPlayer.PlayerId : ownerId;
            OwnerName = string.IsNullOrWhiteSpace(ownerName) ? App.Save.ThisPlayer.PlayerName : ownerName;
            Description = string.Format(AppResources.OptimizedBuildFrom, $" {OwnerName}");
            Editable = true;
            Version = version ?? "?";
            ID = $"{OwnerId}_{Version}";

            BlueAmount =   skills[skills.Count - 1].Where(x => x.Branch == "BranchBlue").Sum(n => n.GetSpSpentAmount());
            RedAmount =    skills[skills.Count - 1].Where(x => x.Branch == "BranchRed").Sum(n => n.GetSpSpentAmount());
            GreenAmount =  skills[skills.Count - 1].Where(x => x.Branch == "BranchGreen").Sum(n => n.GetSpSpentAmount());
            YellowAmount = skills[skills.Count - 1].Where(x => x.Branch == "BranchYellow").Sum(n => n.GetSpSpentAmount());

            // Build milestones
            for (int i = 0; i < skills.Count; i++)
            {
                int spSum = skills[i].Sum(n => n.GetSpSpentAmount());

                var ms = new SPBuildMilestone(ID, i, spSum);

                for (int k = 0; k < skills[i].Count; k++)
                {
                    var msi = new SPBuildMilestoneItem()
                    {
                        Build = ms.Build,
                        Milestone = ms.Milestone,
                        SkillID = skills[i][k].TalentID,
                        Amount = JfTypeConverter.ForceInt(skills[i][k].CurrentLevel)
                    };

                    ms.MilestoneItems.Add(msi);
                }

                Milestones.Add(ms);
            }
        }

        /// <summary>
        /// Ctor from <see cref="ClanMessage"/>
        /// </summary>
        /// <param name="msg"></param>
        public SPBuild(ClanMessage msg)
        {
            InitializeFields();

            if (msg != null)
            {
                OwnerId = msg.PlayerIdFrom;
                OwnerName = msg.MemberName;
                Description = string.Format(AppResources.SharedBuildFrom, $" {OwnerName}");
                Editable = true;

                string[] build = msg.Message.Split(',');

                Version = build[0];
                ID = $"{OwnerId}_{build[1]}";
                BlueAmount = JfTypeConverter.ForceInt(build[2]);
                RedAmount = JfTypeConverter.ForceInt(build[3]);
                GreenAmount = JfTypeConverter.ForceInt(build[4]);
                YellowAmount = JfTypeConverter.ForceInt(build[5]);

                #region Read in Milestone
                var ms = new SPBuildMilestone(ID, 1, BlueAmount + RedAmount + GreenAmount + YellowAmount);

                for (int i = 6; i < build.Length; i++)
                {
                    string[] skill = build[i].Split(':');

                    var msi = new SPBuildMilestoneItem()
                    {
                        Build = ms.Build,
                        Milestone = ms.Milestone,
                        SkillID = skill[0],
                        Amount = JfTypeConverter.ForceInt(skill[1])
                    };

                    ms.MilestoneItems.Add(msi);
                }

                Milestones.Add(ms); 
                #endregion
            }
        }

        /// <summary>
        /// Base ctor
        /// </summary>
        public SPBuild()
        {
            InitializeFields();
        }
        #endregion
    }
}
