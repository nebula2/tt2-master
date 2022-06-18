using Prism.Mvvm;
using SQLite;
using System.Collections.Generic;
using System.ComponentModel;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    [Table("ARTIFACTBUILD")]
    public class ArtifactBuild : BindableBase
    {
        #region Properties
        
        private string _name;
        /// <summary>
        /// Name of Build
        /// </summary>
        [PrimaryKey]
        public string Name
        {
            get => _name; set
            {
                SetProperty(ref _name, value);

                //Reset Build Name
                UpdateChilds();
            }
        }

        private bool _editable = true;
        /// <summary>
        /// False if user can not edit this build
        /// </summary>
        public bool Editable { get => _editable; set => SetProperty(ref _editable, value); }

        private GoldType _goldSource;
        /// <summary>
        /// Gold source for this build
        /// </summary>
        public GoldType GoldSource
        {
            get => _goldSource;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _goldSource, value);
                }
            }
        }

        private List<ArtifactWeight> _artWeights;
        /// <summary>
        /// Dictionary of Damagetype and weight for optimizer
        /// </summary>
        [Ignore]
        public List<ArtifactWeight> CategoryWeights
        {
            get => _artWeights;
            set
            {
                SetProperty(ref _artWeights, value);
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }
        }

        private List<ArtifactBuildIgno> _artsIgnored;
        /// <summary>
        /// List of ignored Artifacts
        /// </summary>
        [Ignore]
        public List<ArtifactBuildIgno> ArtsIgnored
        {
            get => _artsIgnored;
            set
            {
                SetProperty(ref _artsIgnored, value);
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }
        }

        [Ignore]
        public bool IsDeleted { get; set; } = false;
        #endregion

        #region Ctor
        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="name"></param>
        public ArtifactBuild(string name) : base()
        {
            Name = name;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ArtifactBuild()
        {
            InitializeFields();
        }
        #endregion

        #region Private Methods
        private void InitializeFields()
        {
            CategoryWeights = new List<ArtifactWeight>();
            ArtsIgnored = new List<ArtifactBuildIgno>();
        }

        /// <summary>
        /// Updates Buildname in child lists
        /// </summary>
        private void UpdateChilds()
        {
            foreach (var item in CategoryWeights)
            {
                item.Build = Name;
            }

            foreach (var item in ArtsIgnored)
            {
                item.Build = Name;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Overriden ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;
        #endregion
    }
}