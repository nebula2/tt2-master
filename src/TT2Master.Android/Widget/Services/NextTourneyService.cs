using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TT2Master.Droid
{
    /// <summary>
    /// Service class to get next Tournament Info
    /// </summary>
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS")]
    public class NextTourneyService : RemoteViewsService
    {
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //return base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.Sticky;
        }

        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            string AppWidgetId = intent.Data.SchemeSpecificPart;

            var lp = new NextTourneyListProvider(ApplicationContext);

            return lp;
        }
    }
}