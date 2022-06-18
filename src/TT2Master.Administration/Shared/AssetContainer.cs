using System.Collections.Generic;

namespace TT2MasterAdministrationApp.Shared
{
    public class AssetContainer
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string ContainerReference { get; set; }

        /// <summary>
        /// Latest asset version in this container
        /// </summary>
        public string LatestVersion { get; set; }

        /// <summary>
        /// Amount of items in this container
        /// </summary>
        public int ItemsCount { get; set; }

        /// <summary>
        /// type of this container
        /// </summary>
        public AssetContainerType ContainerType { get; set; }

        /// <summary>
        /// container reference to production container
        /// </summary>
        public List<string> ProductionContainerReferences { get; set; }

        /// <summary>
        /// Asset provider
        /// </summary>
        public AssetProvider Provider { get; set; }
    }
}
