using Android.App;
using Android.App.Job;
using System.Threading.Tasks;

namespace TT2Master.Droid.Automation
{
    [Service(Name = "com.LovePatrolAlpha.TT2Master.SnapshotExportJob"
        , Permission = "android.permission.BIND_JOB_SERVICE")]
    public class SnapshotExportJob : JobService
    {
        #region Public functions
        public override bool OnStartJob(JobParameters jobParams)
        {
            var export = new SnapshotExport();

            var expTask = Task.Run(async () =>
            {
                return await export.ExportSnapshotAsync();
            });
            expTask.Wait();
            bool r = expTask.Result;

            // Have to tell the JobScheduler the work is done. 
            JobFinished(jobParams, !r);

            return r;
        }

        // we don't want to reschedule the job if it is stopped or cancelled.
        public override bool OnStopJob(JobParameters jobParams) => false;
        #endregion
    }
}