using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TT2Master.Model.Assets;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;

namespace TT2Master.Model
{
    public class InstallationSourceResult
    {
        /// <summary>
        /// Is this app installed via an official store installation?
        /// </summary>
        public bool IsOfficialStoreInstallation { get; private set; }

        /// <summary>
        /// Package name of the installer
        /// </summary>
        public string Installer { get; private set; }

        /// <summary>
        /// Additional information
        /// </summary>
        public string Information { get; set; }

        public static List<OfficialStore> OfficialStores;

        public InstallationSourceResult(string installer)
        {
            Installer = installer;
            IsOfficialStoreInstallation = OfficialStores.Any(x => x.StoreIds == installer);
        }

        public static void Init()
        {
            OfficialStores = AssetReader.GetInfoFile<OfficialStore, OfficialStoreMap>("officialStores");
        }

        public Dictionary<string, string> ToDict()
        {
            return new Dictionary<string, string>
            {
                {"Installer", Installer ?? "NULL" },
                {"IsOfficialStoreInstallation", IsOfficialStoreInstallation.ToString() },
                {"Information", Information ?? "NULL" },
            };
        }
    }
}
