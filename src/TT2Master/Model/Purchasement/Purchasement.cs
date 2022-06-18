using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Loggers;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Class to handle purchasement
    /// </summary>
    public static class Purchasement
    {
        /// <summary>
        /// Purchases an item async
        /// </summary>
        /// <param name="item">id of item to purchase</param>
        /// <returns></returns>
        public static Task<bool> MakeMorePurchase(PurchaseItem item, IPageDialogService dialogService)
        {
            item.IsPurchased = true;
            return Task.FromResult(true);
        }

        /// <summary>
        /// Asks server if item was purchased and returns a bool
        /// Also saves result in AppSettings (but only if purchased)
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static Task<bool> WasItemPurchased(string productId, IPageDialogService dialogService)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Checks for purchased Items
        /// </summary>
        /// <returns></returns>
        public static Task<bool> CheckPurchases(bool isJustCheckingOffline, IPageDialogService dialogService)
        {
            //Set IsPurchased
            foreach (var item in PurchaseableItems.PurchaseItems)
            {
                item.IsPurchased = true;
            }

            return Task.FromResult(true);
        }
    }
}