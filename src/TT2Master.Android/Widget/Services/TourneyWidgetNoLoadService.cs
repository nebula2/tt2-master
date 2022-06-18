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
    /// Service class to get Info when something went wrong
    /// </summary>
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS")]
    public class NoLoadService : RemoteViewsService
    {
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //return base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.Sticky;
        }

        //public override IBinder OnBind(Intent intent) => null;

        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            string AppWidgetId = intent.Data.SchemeSpecificPart;

            var lp = new NoLoadListProvider(ApplicationContext);

            return lp;
        }
    }
}