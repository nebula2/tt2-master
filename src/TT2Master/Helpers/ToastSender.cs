using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master.Helpers
{
    static class ToastSender
    {
        public static async Task SendToastAsync(string msg, IPageDialogService dialogService = null)
        {
            if(Device.RuntimePlatform == Device.Android)
            {
                Xamarin.Forms.DependencyService.Get<ISendMessage>().LongAlert(msg);
            }
            else if (dialogService != null)
            {
                await dialogService.DisplayAlertAsync(AppResources.InfoHeader, msg, AppResources.OKText);
            }

        }
    }
}
