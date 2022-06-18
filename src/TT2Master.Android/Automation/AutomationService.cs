using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Droid.Loggers;
using TT2Master.Model;
using TT2Master.Model.Arti;
using TT2Master.Model.Equip;
using TT2Master.Model.SP;
using Xamarin.Forms;

namespace TT2Master.Droid.Automation
{
    [Service]
    public class AutomationService : Service, IAutomationReceiver
    {
        #region member and properties
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 12345;
        public int DELAY_BETWEEN_LOG_MESSAGES = 60 * 1000; // milliseconds
        public const string SERVICE_STARTED_KEY = "has_auto_service_been_started";
        public const string BROADCAST_MESSAGE_KEY = "auto_broadcast_message";
        public const string NOTIFICATION_BROADCAST_ACTION = "TT2Master.Notification.Action";
        public const string ACTION_START_SERVICE = "TT2Master.action.START_AUTO_SERVICE";
        public const string ACTION_STOP_SERVICE = "TT2Master.action.STOP_AUTO_SERVICE";
        public const string ACTION_MAIN_ACTIVITY = "TT2Master.action.MAIN_ACTIVITY";

        //private static readonly string _tAG = typeof(AutomationService).FullName;

        bool _isStarted;
        Handler _handler;
        Action _runnable;
        readonly string _channelId = "tt2m_automation_service";
        NotificationManager _notificationManager;

        private Exception _exception = null;
        #endregion

        #region business logic
        /// <summary>
        /// What to do
        /// </summary>
        private void RunningAction()
        {
            _exception = null;

            if (_isStarted)
            {
                // start clan export service
                var h = new ClanAutoExportHelper();
                h.StartService();

                // walk through optimizers
                try
                {
                    // create dbRepo
                    var dbh = new DBPathHelper();
                    var dbRepo = new DBRepository(dbh.DBPath("tt2master.db3"));

                    // read save file and get clan
                    var save = new SaveFile(dbRepo);
                    SaveFile.OnError += SaveFile_OnError;
                    SaveFile.OnLogMePlease += SaveFile_OnLogMePlease;

                    var t = Task.Run(async () =>
                    {
                        #region SaveFile
                        bool isSaveOkay = await save.Initialize(loadPlayer: true, loadAccountModel: true, loadClan: true);
                        SaveFile.OnError -= SaveFile_OnError;
                        SaveFile.OnLogMePlease -= SaveFile_OnLogMePlease;
                        #endregion

                        if(!await CalculateArtifactStuff(dbRepo, save))
                        {
                            return;
                        }

                        if(!await CalculateSkillStuff(dbRepo, save))
                        {
                            return;
                        }

                        if(!await CalculateEquipmentStuff(dbRepo, save))
                        {
                            return;
                        }
                        
                        if(!CalculateDiamondFairyStuff(save))
                        {
                            return;
                        }
                        
                        if(!CalculateFatFairyStuff(save))
                        {
                            return;
                        }
                        
                        if(!CalculateFreeEquipStuff(save))
                        {
                            return;
                        }

                        AutomationServiceHelper.CheckForUpdate();
                    });

                    t.Wait();
                }
                catch (Exception ex)
                {
                    AutoServiceLogger.WriteToLogFile($"RunningAction ERROR {ex.Message}\n{ex.Data}");
                }

                if (_handler == null)
                {
                    _handler = new Handler();
                }

                _handler.PostDelayed(_runnable, DELAY_BETWEEN_LOG_MESSAGES);

                if (AutomationServiceHelper.UiUpdateRequired)
                {
                    UpdateNotification(GetMessage());
                    AutomationServiceHelper.UiUpdateRequired = false;
                }
            }
        }

        /// <summary>
        /// Calculates equipment related stuff
        /// </summary>
        /// <param name="dbRepo"></param>
        /// <returns></returns>
        private async Task<bool> CalculateEquipmentStuff(DBRepository dbRepo, SaveFile save)
        {
            if (!LocalSettingsORM.IsAutoEquipCheck)
            {
                return true;
            }

            try
            {
                var opt = new EquipOptimizer(dbRepo, save);

                bool success = await opt.ReloadList();

                if (success)
                {
                    int shitty = 0;

                    if(opt.MySwords.Count > 0)
                    {
                        var item = opt.MySwords.OrderByDescending(x => x.EfficiencyValue).FirstOrDefault();
                        shitty += item == null ? 0 : item.Equipped ? 0 : 1;
                    }

                    if (opt.MyHats.Count > 0)
                    {
                        var item = opt.MyHats.OrderByDescending(x => x.EfficiencyValue).FirstOrDefault();
                        shitty += item == null ? 0 : item.Equipped ? 0 : 1;
                    }

                    if (opt.MyChests.Count > 0)
                    {
                        var item = opt.MyChests.OrderByDescending(x => x.EfficiencyValue).FirstOrDefault();
                        shitty += item == null ? 0 : item.Equipped ? 0 : 1;
                    }

                    if (opt.MyAuras.Count > 0)
                    {
                        var item = opt.MyAuras.OrderByDescending(x => x.EfficiencyValue).FirstOrDefault();
                        shitty += item == null ? 0 : item.Equipped ? 0 : 1;
                    }

                    if (opt.MySlashs.Count > 0)
                    {
                        var item = opt.MySlashs.OrderByDescending(x => x.EfficiencyValue).FirstOrDefault();
                        shitty += item == null ? 0 : item.Equipped ? 0 : 1;
                    }

                    AutomationServiceHelper.EquipNotOptimal.CurrentValue = shitty;
                    AutoServiceLogger.WriteToLogFile($"EquipNotOptimal {AutomationServiceHelper.EquipNotOptimal.CurrentValue}");
                }
                else
                {
                    _exception = new Exception("Equip load error");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }

        /// <summary>
        /// Calculates skill related stuff
        /// </summary>
        /// <param name="dbRepo"></param>
        /// <returns></returns>
        private async Task<bool> CalculateSkillStuff(DBRepository dbRepo, SaveFile save)
        {
            if (!LocalSettingsORM.IsAutoSkillCheck)
            {
                return true;
            }

            try
            {
                // Load configuration if passed in parameters. if not check for default one
                var config = await SPOptConfigFactory.LoadConfigAsync(dbRepo, save, LocalSettingsORM.DefaultSPConfiguration ?? "", true);

                if(config == null)
                {
                    _exception = new Exception("SP config issue");
                    return false;
                }

                var opt = new SPOptimizer(config, save);

                if (!opt.Initialize())
                {
                    _exception = new Exception("SP init issue");
                    return false;
                }

                opt.DoRun();

                AutomationServiceHelper.AvailableSP.CurrentValue = SaveFile.SPReceived - SaveFile.SPSpent;

                AutomationServiceHelper.NextSkillCost.CurrentValue = opt.UpgradeCollection == null 
                    ? 0 
                    : opt.UpgradeCollection.Count == 0 
                        ? 0 
                        : opt.UpgradeCollection.FirstOrDefault().UpgradeCost;

                AutoServiceLogger.WriteToLogFile($"Available SP {AutomationServiceHelper.AvailableSP.CurrentValue} to nextSkillCost: {AutomationServiceHelper.NextSkillCost.CurrentValue}");

                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }

        /// <summary>
        /// Calculates artifact related stuff
        /// </summary>
        /// <param name="dbRepo"></param>
        /// <returns></returns>
        private async Task<bool> CalculateArtifactStuff(DBRepository dbRepo, SaveFile save)
        {
            try
            {
                #region BoS
                ArtifactHandler.LoadArtifacts();
                ArtifactHandler.FillArtifacts(save);

                AutomationServiceHelper.BosPercentage.CurrentValue = LocalSettingsORM.UseMasterBoSDisplay
                    ? ArtifactHandler.CalculateLifeTimeSpentPercentage(ArtifactHandler.Artifacts.Where(x => x.ID == "Artifact22").First().RelicsSpent)
                    : ArtifactHandler.CalculateLifeTimeSpentPercentageForDummies(ArtifactHandler.Artifacts.Where(x => x.ID == "Artifact22").First().RelicsSpent);

                AutoServiceLogger.WriteToLogFile($"BosPercentage: {AutomationServiceHelper.BosPercentage.CurrentValue}");
                #endregion

                #region Artifact Optimizer
                if (!LocalSettingsORM.IsAutoArtifactCheck)
                {
                    return true;
                }

                var artOptSet = await dbRepo.GetArtOptSettingsByID("1") ?? new ArtOptSettings();

                if (string.IsNullOrWhiteSpace(artOptSet.Build))
                {
                    artOptSet.Build = "_SHIP";
                }

                var artBuild = await App.DBRepo.GetArtifactBuildByName(artOptSet.Build);

                var artOpt = new ArtifactOptimization(artOptSet, artBuild, save, false, null);

                var artOptResult = await artOpt.GetOptimizedList();

                // if there is no message to be displayed everything went correct
                if (!artOptResult.IsMessageNeeded)
                {
                    AutomationServiceHelper.ArtifactsToUpgrage.CurrentValue = artOptResult.OptimizedList.Count;
                    AutoServiceLogger.WriteToLogFile($"ArtifactsToUpgrage: {AutomationServiceHelper.ArtifactsToUpgrage.CurrentValue}");
                }
                else
                {
                    AutoServiceLogger.WriteToLogFile($"artOptResult message needed: {artOptResult.Content}");
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }

        private bool CalculateDiamondFairyStuff(SaveFile save)
        {
            try
            {
                if (!LocalSettingsORM.IsDiamondFairyWished)
                {
                    return true;
                }

                AutomationServiceHelper.DiamondFairy.CurrentValue = save.NumDailyDiamondFairiesCollected;
                AutoServiceLogger.WriteToLogFile($"DiamondFairy: {AutomationServiceHelper.DiamondFairy.CurrentValue}");

                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }
        private bool CalculateFatFairyStuff(SaveFile save)
        {
            try
            {
                if (!LocalSettingsORM.IsFatFairyWished)
                {
                    return true;
                }

                AutomationServiceHelper.FatFairy.CurrentValue = save.NumDailyPerkFairiesCollected;
                AutoServiceLogger.WriteToLogFile($"DiamondFairy: {AutomationServiceHelper.FatFairy.CurrentValue}");

                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }
        private bool CalculateFreeEquipStuff(SaveFile save)
        {
            try
            {
                if (!LocalSettingsORM.IsFreeEquipWished)
                {
                    return true;
                }

                AutomationServiceHelper.FreeEquipment.CurrentValue = SaveFile.ServerEquipmentLeftToFarmMax - save.ServerEquipmentLeftToFarm;
                AutoServiceLogger.WriteToLogFile($"DiamondFairy: {AutomationServiceHelper.FreeEquipment.CurrentValue}");

                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }

        private string GetMessage()
        {
            if (_exception != null)
            {
                return _exception.Message;
            }

            string result = $"BoS: {Math.Round(AutomationServiceHelper.BosPercentage.CurrentValue, 2)} %";

            if (LocalSettingsORM.IsAutoArtifactCheck)
            {
                result += $"\nArt-Upgrades: {AutomationServiceHelper.ArtifactsToUpgrage.CurrentValue}";
            }

            if (LocalSettingsORM.IsAutoSkillCheck)
            {
                result += $"\nSP-Upgrades: {AutomationServiceHelper.AvailableSP.CurrentValue}/{AutomationServiceHelper.NextSkillCost.CurrentValue}";
            }

            if (LocalSettingsORM.IsAutoEquipCheck)
            {
                result += $"\nEquipment-Upgrades: {AutomationServiceHelper.EquipNotOptimal.CurrentValue}";
            }
            
            if (LocalSettingsORM.IsDiamondFairyWished)
            {
                if(AutomationServiceHelper.DiamondFairy.CurrentValue < SaveFile.NumDailyDiamondFairiesCollectedMax)
                {
                    result += $"\nDiamond-Fairies: {AutomationServiceHelper.DiamondFairy.CurrentValue}/{SaveFile.NumDailyDiamondFairiesCollectedMax}";
                }
            }
            
            if (LocalSettingsORM.IsFatFairyWished)
            {
                if(AutomationServiceHelper.FatFairy.CurrentValue < SaveFile.NumDailyPerkFairiesCollectedMax)
                {
                    result += $"\nFat fairy: {AutomationServiceHelper.FatFairy.CurrentValue}/{SaveFile.NumDailyPerkFairiesCollectedMax}";
                }
            }
            
            if (LocalSettingsORM.IsFreeEquipWished)
            {
                if (AutomationServiceHelper.FreeEquipment.CurrentValue < SaveFile.ServerEquipmentLeftToFarmMax)
                {
                    result += $"\nFree equipment: {AutomationServiceHelper.FreeEquipment.CurrentValue}/{SaveFile.ServerEquipmentLeftToFarmMax}";
                }
            }

            return result;
        } 
        #endregion

        #region Service must haves
        /// <summary>
        /// Handles creation
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();

            DELAY_BETWEEN_LOG_MESSAGES = LocalSettingsORM.AutoServiceSchedule * 60 * 1000;

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                string name = GetString(Resource.String.app_name);
                var mChannel = new NotificationChannel(_channelId, name, Android.App.NotificationImportance.Default);
                _notificationManager.CreateNotificationChannel(mChannel);
            }

            _handler = new Handler();

            // This Action Refreshes the notification
            _runnable = RunningAction;
        }

        /// <summary>
        /// Handles on start
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="flags"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action.Equals(ACTION_START_SERVICE))
            {
                if (_isStarted)
                {
                }
                else
                {
                    try
                    {
                        RegisterForegroundService();
                        _handler.PostDelayed(_runnable, DELAY_BETWEEN_LOG_MESSAGES);
                        _isStarted = true;
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(Android.App.Application.Context, e.Message, ToastLength.Long).Show();
                    }
                }
            }
            else if (intent.Action.Equals(ACTION_STOP_SERVICE))
            {
                try
                {
                    StopForeground(true);
                    StopSelf();
                }
                catch (Exception e)
                {
                    Toast.MakeText(Android.App.Application.Context, e.Message, ToastLength.Long).Show();
                }

                _isStarted = false;
                MessagingCenter.Send<IAutomationReceiver, string>(this, "RecordingCommand", "stop");
            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// Implementation of abstract service
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        public override IBinder OnBind(Intent intent) =>
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            null;

        /// <summary>
        /// Handles destroy of service
        /// </summary>
        public override void OnDestroy()
        {
            // Stop the handler.
            try
            {
                _handler.RemoveCallbacks(_runnable);
            }
            catch (Exception)
            { }

            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);

            _isStarted = false;
            base.OnDestroy();
        }

        /// <summary>
        /// Registeres the foreground service
        /// </summary>
        void RegisterForegroundService()
        {
            try
            {
                var notification = BuildNotification();

                // Enlist this instance of the service as a foreground service
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            catch (Exception e)
            {
                Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long);
            }
        }

        /// <summary>
        /// Builds a PendingIntent that will display the main activity of the app. This is used when the 
        /// user taps on the notification; it will take them to the main activity of the app.
        /// </summary>
        /// <returns>The content intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        /// <summary>
        /// Builds the Notification.Action that will allow the user to stop the service via the
        /// notification in the status bar
        /// </summary>
        /// <returns>The stop service action.</returns>
        Notification.Action BuildStopServiceAction()
        {
            try
            {
                var stopServiceIntent = new Intent(this, GetType());
                stopServiceIntent.SetAction(ACTION_STOP_SERVICE);
                var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

                var builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause,
                                                              GetText(Resource.String.stop_service),
                                                              stopServicePendingIntent);
                return builder.Build();
            }
            catch (Exception e)
            {
                AutoServiceLogger.WriteToLogFile($"BuildStopServiceAction Error: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Rebuilds the notification
        /// </summary>
        /// <param name="message"></param>
        private void UpdateNotification(string message = null)
        {
            if (_handler == null)
            {
                _handler = new Handler();
            }

            _handler.PostDelayed(_runnable, DELAY_BETWEEN_LOG_MESSAGES);

            var notif = BuildNotification(message);

            if (notif != null)
            {
                _notificationManager.Notify(SERVICE_RUNNING_NOTIFICATION_ID, notif);
            }
        }

        /// <summary>
        /// Builds foreground notification
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Notification BuildNotification(string message = null)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    string name = GetString(Resource.String.app_name);
                    var mChannel = new NotificationChannel(_channelId, name, Android.App.NotificationImportance.Default);
                    _notificationManager.CreateNotificationChannel(mChannel);
                }

                Notification.Style style = new Notification.BigTextStyle();

                var notification = new Notification.Builder(this, _channelId)
                    .SetContentTitle(Resources.GetString(Resource.String.app_name))
                    .SetContentText(string.IsNullOrWhiteSpace(message) ? Resources.GetString(Resource.String.notification_text) : message)
                    .SetSmallIcon(Resource.Drawable.ic_stat_name)
                    .SetContentIntent(BuildIntentToShowMainActivity())
                    .SetOngoing(true)
                    .SetStyle(style)
                    .AddAction(BuildStopServiceAction())
                    .Build();

                return notification;
            }
            catch (Exception e)
            {
                AutoServiceLogger.WriteToLogFile($"BuildNotification Error: {e.Message}");
                return null;
            }
        } 
        #endregion

        #region E + D
        private void SaveFile_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => AutoServiceLogger.WriteToLogFile($"SnapshotExport.SaveFile_OnLogMePlease: {e.Information}");
        private void SaveFile_OnError(object sender, Helpers.CustErrorEventArgs e) => AutoServiceLogger.WriteToLogFile($"SnapshotExport.SaveFile_OnError: {e.MyException.Message}");
        private void SnapshotFactory_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => AutoServiceLogger.WriteToLogFile($"SnapshotExport.SnapshotFactory_OnLogMePlease: {e.Information}");
        #endregion
    }
}