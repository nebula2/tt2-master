using SQLite;
using System.Linq;

namespace TT2Master.Model.Arti
{
    [Table("ARTIFACTIGNODICT")]
    public class ArtifactBuildIgno
    {
        #region Properties
        /// <summary>
        /// Combination of <see cref="Build"/> and <see cref="ArtifactID"/> for DB
        /// </summary>
        [PrimaryKey]
        public string BuildArt { get; set; }

        private string _build;
        /// <summary>
        /// Corresponding <see cref="ArtifactBuild"/>
        /// </summary>
        public string Build
        {
            get => _build;
            set
            {
                _build = value;
                if (!string.IsNullOrWhiteSpace(ArtifactID))
                {
                    SetBuildArt(value, ArtifactID);
                }
            }
        }

        private string _artifactID;
        /// <summary>
        /// Ignored ArtifactID
        /// </summary>
        public string ArtifactID
        {
            get => _artifactID;
            set
            {
                _artifactID = value;
                if (!string.IsNullOrWhiteSpace(Build))
                {
                    SetBuildArt(Build, value);
                }
            }
        }

        [Ignore]
        public bool IsIgnored { get; set; }

        [Ignore]
        public string Name { get => ArtifactID?.TranslatedString() ?? "New Artifact"; }

        [Ignore]
        public string ImagePath => $"{ArtifactID}";
        #endregion

        #region Ctors
        /// <summary>
        /// Ctor with IsIgnored
        /// </summary>
        /// <param name="build"></param>
        /// <param name="artId"></param>
        /// <param name="igno"></param>
        public ArtifactBuildIgno(string build, string artId, bool igno) : this (build, artId)
        {
            IsIgnored = igno;
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="build"></param>
        /// <param name="artId"></param>
        public ArtifactBuildIgno(string build, string artId)
        {
            Build = build;
            ArtifactID = artId;
            IsIgnored = true;
        }

        /// <summary>
        /// Ctor for sqlite
        /// </summary>
        public ArtifactBuildIgno()
        {

        }

        #endregion

        #region private methods
        /// <summary>
        /// Sets PK-String
        /// </summary>
        /// <param name="build"></param>
        /// <param name="artId"></param>
        private void SetBuildArt(string build, string artId) => BuildArt = $"{Build}, {ArtifactID}";
        #endregion
    }
}
