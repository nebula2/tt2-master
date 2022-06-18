using Plugin.Connectivity;
using System.Collections.Generic;
using System.Linq;

namespace TT2Master
{
    /// <summary>
    /// In App purchaseable Items
    /// </summary>
    public static class PurchaseableItems
    {
        /// <summary>
        /// List of Items one can purchase
        /// </summary>
        public static List<PurchaseItem> PurchaseItems = new List<PurchaseItem>()
        {
            new PurchaseItem()
            {
                ID = "smallad",
                IsPurchased = false
            },
            new PurchaseItem()
            {
                ID = "noad",
                IsPurchased = false
            },
            new PurchaseItem()
            {
                ID = "supporter",
                IsPurchased = false
            },
            new PurchaseItem()
            {
                ID = "supporterovertime",
                IsPurchased = false
            },
        };

        /// <summary>
        /// Returns true if user has access to every function in this app
        /// </summary>
        /// <returns></returns>
        public static bool GetAllFuncsAccess() => PurchaseItems.Where(x => (x.ID == "supporter" || x.ID == "supporterovertime") && x.IsPurchased).Count() > 0;

        /// <summary>
        /// Returns true if the small ad should be shown
        /// </summary>
        /// <returns></returns>
        public static bool GetSmallAdVisible()
        {
            return PurchaseItems.Where(x => x.IsPurchased && (x.ID == "noad" || x.ID == "supporter" || x.ID == "supporterovertime")).Count() == 0
                && App.InstallationSourceInfo.IsOfficialStoreInstallation
                && CrossConnectivity.Current.IsConnected;
        }

        /// <summary>
        /// Returns true if the big ad should be shown
        /// </summary>
        /// <returns></returns>
        public static bool GetBigAdVisible()
        {
            return PurchaseItems.Where(x => x.IsPurchased).Count() == 0
                && App.InstallationSourceInfo.IsOfficialStoreInstallation
                && CrossConnectivity.Current.IsConnected;
        }
    }
}