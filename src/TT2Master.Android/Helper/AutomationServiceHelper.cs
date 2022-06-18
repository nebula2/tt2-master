using Android.Content;
using System;
using TT2Master.Droid;
using TT2Master.Droid.Automation;
using TT2Master.Model.Types;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to handle automation foreground service
/// </summary>
[assembly: Dependency(typeof(AutomationServiceHelper))]
namespace TT2Master.Droid
{
    public class AutomationServiceHelper : IAutomationService
    {
        #region Properties
        /// <summary>
        /// True if the service is started
        /// </summary>
        public static bool IsStarted { get; set; }

        /// <summary>
        /// Intent to start the servie
        /// </summary>
		Intent _startServiceIntent;

        /// <summary>
        /// Intent to stop the service
        /// </summary>
		Intent _stopServiceIntent;

        /// <summary>
        /// BoS-Percentage
        /// </summary>
        public static PDouble BosPercentage { get; set; } = new PDouble(0);

        /// <summary>
        /// Amount of free SP
        /// </summary>
        public static PInt AvailableSP { get; set; } = new PInt(0);

        /// <summary>
        /// Costs for next skill
        /// </summary>
        public static PInt NextSkillCost { get; set; } = new PInt(0);

        /// <summary>
        /// Amount of non optimal equipent parts
        /// </summary>
        public static PInt EquipNotOptimal { get; set; } = new PInt(0);

        /// <summary>
        /// Amount of Artifacts to upgrade
        /// </summary>
        public static PInt ArtifactsToUpgrage { get; set; } = new PInt(0);

        public static PInt DiamondFairy { get; set; } = new PInt(0);

        public static PInt FatFairy { get; set; } = new PInt(0);

        public static PInt FreeEquipment { get; set; } = new PInt(0);

        /// <summary>
        /// True if update required
        /// </summary>
        public static bool UiUpdateRequired { get; set; } = false;
        #endregion

        #region Helper
        /// <summary>
        /// Sets up the service
        /// </summary>
        private bool SetUp()
        {
            bool success;

            try
            {
                _startServiceIntent = new Intent(Android.App.Application.Context, typeof(AutomationService));
                _startServiceIntent.SetAction(AutomationService.ACTION_START_SERVICE);

                _stopServiceIntent = new Intent(Android.App.Application.Context, typeof(AutomationService));
                _stopServiceIntent.SetAction(AutomationService.ACTION_STOP_SERVICE);

                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the foreground automation service
        /// </summary>
        /// <returns></returns>
        public bool StartService()
        {
            if (_startServiceIntent == null)
            {
                SetUp();
            }

            Android.App.Application.Context.StartService(_startServiceIntent);

            IsStarted = true;

            return true;
        }

        /// <summary>
        /// Stops the foreground automation service
        /// </summary>
        /// <returns></returns>
        public bool StopService()
        {
            if (_stopServiceIntent == null)
            {
                SetUp();
            }

            Android.App.Application.Context.StopService(_stopServiceIntent);
            IsStarted = false;

            return true;
        }

        /// <summary>
        /// Forces an update of the values
        /// </summary>
        /// <returns></returns>
        public bool UpdateContent() => UiUpdateRequired = true;

        public static void CheckForUpdate()
        {
            if(BosPercentage.HasChanged
                || AvailableSP.HasChanged
                || NextSkillCost.HasChanged
                || EquipNotOptimal.HasChanged
                || ArtifactsToUpgrage.HasChanged)
            {
                UiUpdateRequired = true;
            }
        }
        #endregion
    }
}