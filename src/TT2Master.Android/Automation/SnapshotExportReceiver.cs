using Android.App;
using Android.Content;
using Android.Util;
using System;
using TT2Master.Droid.Loggers;

namespace TT2Master.Droid.Automation
{
    /// <summary>
    /// Broadcast receiver to start the snapshot service after reboot
    /// </summary>
    [BroadcastReceiver]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted, "com.LovePatrolAlpha.TT2Master.SNAPSHOT_EXPORT" })]
    public class SnapshotExportReceiver : BroadcastReceiver
    {
        /// <summary>
        /// Gets fired from when timer is elapsed
        /// Here is the logic for local notifications
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                Log.Debug("TT2Master", $"pid {Android.OS.Process.MyPid()} SnapshotExportReceiver OnReceive");
                var h = new ClanAutoExportHelper();
                h.StartService();
                Log.Debug("TT2Master", $"pid {Android.OS.Process.MyPid()} SnapshotExportReceiver Service started");
            }
            catch (Exception ex)
            {
                AutoServiceLogger.WriteToLogFile($"SnapshotExportWorker.OnReceive() ERROR: {ex.Message}");
            }
        }
    }
}