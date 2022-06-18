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
using Plugin.Connectivity;

namespace TT2Master.Droid
{
    [BroadcastReceiver(Label = "@string/widget_name")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widgetinfo")]
    public class TT2Widget : AppWidgetProvider
    {
        #region Properties
        /// <summary>
        /// If some shit happens, the exception message will be stored here.
        /// </summary>
        public static string ShitHappenedHere = "";

        public static int randomNumber = new Random().Next(1, 500) * 1000;
        #endregion

        /// <summary>
        /// Gets called on Update
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appWidgetManager"></param>
        /// <param name="appWidgetIds"></param>
        public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            try
            {
                #region Logfile
                //delete log file
                WidgetLogger.DeleteLogFile();
                WidgetLogger.WriteLog = true;
                WidgetLogger.WriteToLogFile("TT2Widget.OnUpdate: starting OnUpdate");
                #endregion

                #region Updating each widget

                var remoteViews = await CreateView(context);
                WidgetLogger.WriteToLogFile($"TT2Widget.OnUpdate: received remoteViews");

                WidgetLogger.WriteToLogFile($"TT2Widget.OnUpdate: Updating {appWidgetIds.Length} Widgets");

                //int[] widgetIds = manager.GetAppWidgetIds(new ComponentName(context.PackageName, typeof(TT2Widget).Name));
                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.list1);

                appWidgetManager.UpdateAppWidget(appWidgetIds, null);
                appWidgetManager.UpdateAppWidget(appWidgetIds, remoteViews);

                WidgetLogger.WriteToLogFile($"TT2Widget.OnUpdate: Finished updating.");
                #endregion

                base.OnUpdate(context, appWidgetManager, appWidgetIds);
            }
            catch (Exception)
            { throw; }
            finally
            {
            }
        }

        /// <summary>
        /// PendingIntent for updatebutton
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static PendingIntent BuildButtonPendingIntent(Context context)
        {
            var intent = new Intent(context, typeof(TT2IntentReceiver));
            intent.SetAction("com.LovePatrolAlpha.TT2Master.UPDATE_WIDGET");

            return PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        /// <summary>
        /// Pushes Widget-Update
        /// </summary>
        /// <param name="context"></param>
        /// <param name="remoteViews"></param>
        public static void PushWidgetUpdate(Context context, RemoteViews remoteViews)
        {
            var myWidget = new ComponentName(context, Java.Lang.Class.FromType(typeof(TT2Widget)).Name);
            var manager = AppWidgetManager.GetInstance(context);

            int[] widgetIds = manager.GetAppWidgetIds(new ComponentName(context.PackageName, typeof(TT2Widget).Name));
            manager.NotifyAppWidgetViewDataChanged(widgetIds, Resource.Id.list1);

            manager.UpdateAppWidget(myWidget, null);
            manager.UpdateAppWidget(myWidget, remoteViews);
        }

        /// <summary>
    /// Helper to update widget
    /// </summary>
    /// <param name="context"></param>
    /// <param name="appWidgetId"></param>
    /// <returns></returns>
        [Obsolete("Replaced by CreateView")]
        public static async Task<RemoteViews> UpdateWidgetListView(Context context, int appWidgetId)
        {
            WidgetLogger.WriteToLogFile($"In UpdateWidgetListView");

            //Create the container
            var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_layout);

            string PACKAGE_NAME = context.PackageName;
            var svcIntent = new Intent();

            #region set svcIntent
            try
            {
                svcIntent = await CreateIntentAsync(context);
            }
            catch (Exception ex)
            {
                if(ex != null)
                {
                    WidgetLogger.WriteToLogFile($"UpdateWidgetListView: creating NoLoadService");
                }
                else
                {
                    WidgetLogger.WriteToLogFile($"UpdateWidgetListView: Got null exception...");
                }

                //Set shit that happened so NoLoad can access the data
                ShitHappenedHere = ex.Message;
                svcIntent = new Intent(context, typeof(NoLoadService));
            }
            finally
            {
                WidgetLogger.WriteToLogFile($"UpdateWidgetListView: SetOnClickPendingIntent");
                remoteViews.SetOnClickPendingIntent(Resource.Id.reload, TT2Widget.BuildButtonPendingIntent(context));
            }
            #endregion

            try
            {
                WidgetLogger.WriteToLogFile($"UpdateWidgetListView: Trying to fill Intent");
                //build up the whole crap with the correct Intent
                svcIntent.SetPackage(PACKAGE_NAME);
                svcIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);

                svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(Android.Content.IntentUriType.AndroidAppScheme)));

                //Set last Update-Time
                remoteViews.SetTextViewText(Resource.Id.last_update, $"Last Updated: {DateTime.Now.ToString("yyyy.MM.dd H:mm:ss")}");

                remoteViews.SetRemoteAdapter(Resource.Id.list1, svcIntent);
            }
            catch (Exception ex)
            {
                if(ex == null)
                {
                    WidgetLogger.WriteToLogFile($"UpdateWidgetListView: exception (null)");
                }
                else
                {
                    WidgetLogger.WriteToLogFile($"UpdateWidgetListView: exception -> {ex.Message}\n{ex.Data.ToString()}");
                }
            }

            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: finished. Returning remoteViews");

            return remoteViews;
        }

        /// <summary>
        /// Creates RemoteViews-Object without appWidgetId
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<RemoteViews> CreateView(Context context)
        {
            WidgetLogger.WriteToLogFile($"In CreateView");

            //Create the container
            var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_layout);

            string PACKAGE_NAME = context.PackageName;
            var svcIntent = new Intent();

            #region set svcIntent
            try
            {
                svcIntent = await CreateIntentAsync(context);
                
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    WidgetLogger.WriteToLogFile($"CreateView: creating NoLoadService");
                }
                else
                {
                    WidgetLogger.WriteToLogFile($"CreateView: Got null exception...");
                }

                //Set shit happened so NoLoad can access the data
                ShitHappenedHere = ex.Message;
                svcIntent = new Intent(context, typeof(NoLoadService));
            }
            finally
            {
                WidgetLogger.WriteToLogFile($"CreateView: SetOnClickPendingIntent");
                remoteViews.SetOnClickPendingIntent(Resource.Id.reload, TT2Widget.BuildButtonPendingIntent(context));
            }
            #endregion

            try
            {
                WidgetLogger.WriteToLogFile($"CreateView: Trying to fill Intent");
                //build up the whole crap with the correct Intent
                svcIntent.SetPackage(PACKAGE_NAME);
                svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(Android.Content.IntentUriType.AndroidAppScheme)));
                svcIntent.SetData(Android.Net.Uri.FromParts("content", (DateTime.Now.Ticks + randomNumber).ToString(), null));

                //Set last Update-Time
                remoteViews.SetTextViewText(Resource.Id.last_update, $"Last Updated: {DateTime.Now.ToString("yyyy.MM.dd H:mm:ss")}");

                remoteViews.SetRemoteAdapter(Resource.Id.list1, svcIntent);
            }
            catch (Exception ex)
            {
                if (ex == null)
                {
                    WidgetLogger.WriteToLogFile($"CreateView: exception (null)");
                }
                else
                {
                    WidgetLogger.WriteToLogFile($"CreateView: exception -> {ex.Message}\n{ex.Data.ToString()}");
                }
            }

            WidgetLogger.WriteToLogFile($"CreateView: finished. Returning remoteViews");

            return remoteViews;
        }

        #region Helper
        /// <summary>
        /// Creates the correct intent for the widget
        /// </summary>
        /// <returns></returns>
        private static async Task<Intent> CreateIntentAsync(Context context)
        {
            #region Init Tournament-Info
            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: Calling ti-Init");
            //check if tournament is running
            bool tiInit = await TournamentInfo.Init();

            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: ti-Init = {tiInit}");

            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: Calling tr");
            //Is a tournament running?
            bool tr = false;
            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: tr = {tr}");
            #endregion

            #region No Tournament running
            if (!tr)
            {
                WidgetLogger.WriteToLogFile($"UpdateWidgetListView: !tr. Calling NextTourneyService");
                return new Intent(context, typeof(NextTourneyService));
            }
            #endregion

            #region Loading data of running tournament
            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: tr. Calling Getting Tourney Data");
            bool allLoaded = await TournamentInfo.InitServerApiAsync();
            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: allLoaded = {allLoaded}");

            if (!allLoaded)
            {
                ShitHappenedHere = "could not init Server-stuff";
                return new Intent(context, typeof(NoLoadService));
            }

            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: consuming member");

            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: consuming member successful? {allLoaded}");

            if (!allLoaded || TournamentInfo.Members.Count == 0)
            {
                ShitHappenedHere = "could not consume member";
                WidgetLogger.WriteToLogFile($"UpdateWidgetListView: could not consume member -> creating NoLoadService");
                return new Intent(context, typeof(NoLoadService));
            }

            WidgetLogger.WriteToLogFile($"UpdateWidgetListView: creating RunningTournamentService");
            #endregion

            //return RunningTournament
            return new Intent(context, typeof(RunningTournamentService));
        } 
        #endregion

        #region Informational overrides
        /// <summary>
        /// OnRestored override to remap <see cref="AppWidgetIdsStored"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="oldWidgetIds"></param>
        /// <param name="newWidgetIds"></param>
        public override void OnRestored(Context context, int[] oldWidgetIds, int[] newWidgetIds)
        {
            WidgetLogger.WriteToLogFile("OnRestored");

            //string oldIds = IntArrayToString(TT2IntentReceiver.AppWidgetIdsStored);
            //string newIds = IntArrayToString(newWidgetIds);

            //WidgetLogger.WriteToLogFile($"OnRestored: {oldIds} -> {newIds}");

            //TT2IntentReceiver.AppWidgetIdsStored = newWidgetIds;

            base.OnRestored(context, oldWidgetIds, newWidgetIds);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            WidgetLogger.WriteToLogFile($"In OnReceive");

            base.OnReceive(context, intent);
        }

        public override void OnDisabled(Context context)
        {
            WidgetLogger.WriteToLogFile($"On Disabled");

            base.OnDisabled(context);
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            WidgetLogger.WriteToLogFile($"On Deleted");

            base.OnDeleted(context, appWidgetIds);
        }

        public override void OnEnabled(Context context)
        {
            WidgetLogger.WriteToLogFile($"On Enabled");

            base.OnEnabled(context);
        }

        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            WidgetLogger.WriteToLogFile($"On AppWidgetOptionsChanged");

            base.OnAppWidgetOptionsChanged(context, appWidgetManager, appWidgetId, newOptions);
        }

        protected override void JavaFinalize()
        {
            WidgetLogger.WriteToLogFile($"Java Finalize");

            base.JavaFinalize();
        }

        protected override void Dispose(bool disposing)
        {
            WidgetLogger.WriteToLogFile($"Dispose");

            base.Dispose(disposing);
        } 
        #endregion
    }
}