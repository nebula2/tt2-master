using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TT2Master.Droid
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "com.LovePatrolAlpha.TT2Master.UPDATE_WIDGET" })]
    public class TT2IntentReceiver : BroadcastReceiver
    {
        public static int Counter = 0;

        public override void OnReceive(Context context, Intent intent)
        {
            WidgetLogger.WriteToLogFile("TT2IntentReceiver.OnReceive: starting DoUpdate");

            DoUpdate(context);
        }

        /// <summary>
        /// Updates Widget
        /// </summary>
        /// <param name="context"></param>
        public static async void DoUpdate(Context context)
        {
            WidgetLogger.WriteToLogFile($"TT2IntentReceiver.DoUpdate(): start");

            //get manager
            var manager = AppWidgetManager.GetInstance(context);

            //create views
            var remoteViews = await TT2Widget.CreateView(context);

            //update widget
            manager.UpdateAppWidget(new ComponentName(context.PackageName, typeof(TT2Widget).Name), null);
            manager.UpdateAppWidget(new ComponentName(context.PackageName, typeof(TT2Widget).Name), remoteViews);

            int[] widgetIds = manager.GetAppWidgetIds(new ComponentName(context.PackageName, typeof(TT2Widget).Name));
            manager.NotifyAppWidgetViewDataChanged(widgetIds, Resource.Id.list1);

            //push update
            TT2Widget.PushWidgetUpdate(context.ApplicationContext, remoteViews);

            WidgetLogger.WriteToLogFile($"TT2IntentReceiver.DoUpdate(): end");
        }

        [Obsolete("Replaced by DoUpdate")]
        public static async void UpdateFromBtn(Context context)
        {
            WidgetLogger.WriteToLogFile($"Begin of UpdateFromBtn()");

            var latestWidget = new ComponentName(context, typeof(TT2IntentReceiver).Name);
            var manger = AppWidgetManager.GetInstance(context);
            int[] allWidgets = manger.GetAppWidgetIds(latestWidget);

            WidgetLogger.WriteToLogFile($"UpdateFromBtn: Got {allWidgets.Length} widgets to update");

            for (int i = 0; i < allWidgets.Length; i++)
            {
                WidgetLogger.WriteToLogFile($"UpdateFromBtn: Updating {i + 1} from {allWidgets.Length} ({allWidgets[i]})");

                var remoteViews = await TT2Widget.UpdateWidgetListView(context, i);

                WidgetLogger.WriteToLogFile($"UpdateFromBtn: received remoteViews. Calling TT2Widget.PushWidgetUpdate");

                //REMEMBER TO ALWAYS REFRESH YOUR BUTTON CLICK LISTENERS!!!
                //WidgetLogger.WriteToLogFile($"OnUpdate: Setting OnClickPendingIntent.");
                //remoteViews.SetOnClickPendingIntent(Resource.Id.reload, TT2Widget.BuildButtonPendingIntent(context));

              TT2Widget.PushWidgetUpdate(context.ApplicationContext, remoteViews);

                WidgetLogger.WriteToLogFile($"UpdateFromBtn: Finished updating.");
            }
        }
    }
}
