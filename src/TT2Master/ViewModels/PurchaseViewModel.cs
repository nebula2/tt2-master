using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master
{
    public class PurchaseViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private IPageDialogService _dialogService;

        public ICommand PurchaseSmallAdsCommand { get; private set; }
        public ICommand PurchaseNoAdsCommand { get; private set; }
        public ICommand PurchaseSupporterCommand { get; private set; }
        public ICommand PurchaseSupporterAboCommand { get; private set; }
        #endregion

        #region Ctor
        public PurchaseViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = AppResources.PurchaseHeader;

            PurchaseSmallAdsCommand = new DelegateCommand(async () => await PurchaseSmallAdsExecute());
            PurchaseNoAdsCommand = new DelegateCommand(async () => await PurchaseNoAdsExecute());
            PurchaseSupporterCommand = new DelegateCommand(async () => await PurchaseSupporterExecute());
            PurchaseSupporterAboCommand = new DelegateCommand(async () => await PurchaseSupporterAboExecute());

            _dialogService = dialogService;
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Action to purchase small ads package
        /// </summary>
        /// <returns></returns>
        private async Task<bool> PurchaseSmallAdsExecute()
        {
            //check if above pack was purchased
            if (PurchaseableItems.PurchaseItems.Where(x => (x.ID == "noad" || x.ID == "supporter" || x.ID == "supporterovertime") && x.IsPurchased).Count() > 0)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.AlreadyPurchasedIncludingText, AppResources.OKText);
                return true;
            }

            //get purchasement object
            var pi = PurchaseableItems.PurchaseItems.Where(x => x.ID == "smallad").First();

            //check if it was purchased
            if (pi.IsPurchased)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.AlreadyPurchasedText, AppResources.OKText);
                return true;
            }

            //purchase
            bool purchasementMade = await Purchasement.MakeMorePurchase(pi, _dialogService);

            //check if purchased
            if (purchasementMade)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemPurchasedText, AppResources.OKText);
                return true;
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemNotPurchasedText, AppResources.OKText);
                return false;
            }
        }

        /// <summary>
        /// Action to purchase no ads package
        /// </summary>
        /// <returns></returns>
        private async Task<bool> PurchaseNoAdsExecute()
        {
            //check if above pack was purchased
            if (PurchaseableItems.PurchaseItems.Where(x => (x.ID == "supporter" || x.ID == "supporterovertime") && x.IsPurchased).Count() > 0)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.AlreadyPurchasedIncludingText, AppResources.OKText);
                return true;
            }

            //get purchase object
            var pi = PurchaseableItems.PurchaseItems.Where(x => x.ID == "noad").First();

            //check if this is already purchased
            if (pi.IsPurchased)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.AlreadyPurchasedText, AppResources.OKText);
                return true;
            }

            //purchase item
            bool purchasementMade = await Purchasement.MakeMorePurchase(pi, _dialogService);

            //alert if success or not
            if (purchasementMade)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemPurchasedText, AppResources.OKText);
                return true;
            }

            await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemNotPurchasedText, AppResources.OKText);
            return false;
        }

        /// <summary>
        /// Action to purchase supporter package
        /// </summary>
        /// <returns></returns>
        private async Task<bool> PurchaseSupporterExecute()
        {
            //get purchase item
            var pi = PurchaseableItems.PurchaseItems.Where(x => x.ID == "supporter").First();

            //check if it was already purchased
            if (pi.IsPurchased)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.AlreadyPurchasedText, AppResources.OKText);
                return true;
            }

            //purchase
            bool purchasementMade = await Purchasement.MakeMorePurchase(pi, _dialogService);

            //alert if success or not
            if (purchasementMade)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemPurchasedText, AppResources.OKText);
                return true;
            }

            await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemNotPurchasedText, AppResources.OKText);
            return false;
        }

        /// <summary>
        /// Action to purchase supporter package
        /// </summary>
        /// <returns></returns>
        private async Task<bool> PurchaseSupporterAboExecute()
        {
            //get purchase item
            var pi = PurchaseableItems.PurchaseItems.Where(x => x.ID == "supporterovertime").First();

            //check if it was already purchased
            if (pi.IsPurchased)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.AlreadyPurchasedText, AppResources.OKText);
                return true;
            }

            //purchase
            bool purchasementMade = await Purchasement.MakeMorePurchase(pi, _dialogService);

            //alert if success or not
            if (purchasementMade)
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemPurchasedText, AppResources.OKText);
                return true;
            }

            await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ItemNotPurchasedText, AppResources.OKText);
            return false;
        }
        #endregion
    }
}