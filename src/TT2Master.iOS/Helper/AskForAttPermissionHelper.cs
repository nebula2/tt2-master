using System.Threading.Tasks;
using TT2Master.Interfaces;
using TT2Master.iOS.Helper;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AskForAttPermissionHelper))]
namespace TT2Master.iOS.Helper
{
    public class AskForAttPermissionHelper : IAskForAttPermission
    {
        public async Task<bool> AskUserAsync()
        {
            // only ask on ios 14+
            if (!UIDevice.CurrentDevice.CheckSystemVersion(14, 0)) return true;

            // return if we already asked stuff
            if (LocalSettingsORM.ATTUserChoice > 0) return true;

            var result = await AppTrackingTransparency.ATTrackingManager.RequestTrackingAuthorizationAsync();

            LocalSettingsORM.ATTUserChoice = (int)result;
            return true;
        }
    }
}