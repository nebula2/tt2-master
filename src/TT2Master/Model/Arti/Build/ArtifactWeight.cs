using SQLite;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// Describes a single Weight-Setting of <see cref="ArtifactBuild"/>
    /// </summary>
    [Table("ARTIFACTWEIGHT")]
    public class ArtifactWeight
    {
        #region Properties
        /// <summary>
        /// Corresponding <see cref="ArtifactBuild"/>
        /// </summary>
        [PrimaryKey]
        public string BuildAndArtifact { get; set; }

        private string _build;
        /// <summary>
        /// Build-Identifier
        /// </summary>
        public string Build
        {
            get => _build;
            set
            {
                _build = value;
                if (!string.IsNullOrWhiteSpace(ArtifactId))
                {
                    SetBuildAndArtifact(value, ArtifactId);
                }
            }
        }

        private string _artifactId;
        /// <summary>
        /// Artifact-Identifier
        /// </summary>
        public string ArtifactId
        {
            get => _artifactId;
            set
            {
                _artifactId = value;
                if (!string.IsNullOrWhiteSpace(Build))
                {
                    SetBuildAndArtifact(Build, value);
                }
            }
        }

        /// <summary>
        /// Weight for this category
        /// </summary>
        public double Weight
        {
            get;
            set;
        }

        /// <summary>
        /// Weight as string to bind in text entry
        /// </summary>
        [Ignore]
        public string WeightString { get => Weight.ToString(); set => Weight = JfTypeConverter.ForceDoubleUniversal(value); }

        private Artifact _artifactBehind;

        [Ignore]
        public Artifact ArtifactBehind { get => _artifactBehind; set => _artifactBehind = value; }

        #endregion

        #region private Methods
        /// <summary>
        /// Builds up Primary Key
        /// </summary>
        /// <param name="build"></param>
        /// <param name="category"></param>
        private void SetBuildAndArtifact(string build, string artifact) => BuildAndArtifact = $"{build},{artifact}";
        #endregion

        #region Ctors
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="build"></param>
        /// <param name="artifact">Artifact-ID (eg. Artifact22)</param>
        public ArtifactWeight(string build, string artifact)
        {
            Build = build;
            ArtifactId = artifact ?? "Artifact0";
            SetBuildAndArtifact(Build, ArtifactId);

            Weight = 1;
        }

        /// <summary>
        /// Ctor for sqlite
        /// </summary>
        public ArtifactWeight()
        {

        } 
        #endregion
    }
}