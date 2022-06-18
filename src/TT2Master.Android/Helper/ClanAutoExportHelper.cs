using Android.App.Job;
using System;
using TT2Master.Droid;
using TT2Master.Droid.Automation;
using TT2Master.Droid.Loggers;
using TT2Master.Droid.Workers;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(ClanAutoExportHelper))]
namespace TT2Master.Droid
{
    public class ClanAutoExportHelper : IStartClanAutoExport
    {
        private const int _serviceId = 150;

        /// <summary>
        /// Starts Service if wished. else stops it
        /// </summary>
        public bool StartService(bool forceCloseBefore = false)
        {
            // for more info see here: https://docs.microsoft.com/en-us/xamarin/android/platform/android-job-scheduler
            try
            {
                var jobScheduler = (JobScheduler)Android.App.Application.Context.GetSystemService(Android.App.Application.JobSchedulerService);

                if (forceCloseBefore)
                {
                    jobScheduler.Cancel(_serviceId);
                }

                if (LocalSettingsORM.IsClanAutoExport)
                {
                    // check if there is already a service running
                    var dada = jobScheduler.GetPendingJob(_serviceId);

                    if (dada != null)
                    {
                        return true;
                    }

                    // Sample usage - creates a JobBuilder for a DownloadJob and sets the Job ID to 1.
                    var jobBuilder = Android.App.Application.Context.CreateJobBuilderUsingJobId<SnapshotExportJob>(_serviceId);

                    var jobInfo = jobBuilder
                        .SetPeriodic(LocalSettingsORM.ClanAutoExportSchedule * 3600000)    // Specifies that the job should be regularly run.
                        .SetPersisted(true)             // The job should perisist across device reboots.
                        .Build();                       // creates a JobInfo object.

                    int scheduleResult = jobScheduler.Schedule(jobInfo);

                    return JobScheduler.ResultSuccess == scheduleResult;
                }
                else
                {
                    jobScheduler.Cancel(_serviceId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                AutoServiceLogger.WriteToLogFile($"ClanAutoExportHelper.StartService() ERROR: {ex.Message}\n{ex.Data}");
                return false;
            }
        }
    }
}