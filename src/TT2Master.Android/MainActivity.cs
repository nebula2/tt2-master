using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using LabelHtml.Forms.Plugin.Droid;
using Plugin.CurrentActivity;
using Prism;
using Prism.Ioc;
using TT2Master.Droid.Automation;
using Xamarin.Forms;

namespace TT2Master.Droid
{
    [Activity(Label = "TT2Master"
        , Icon = "@drawable/new_logo"
        , RoundIcon = "@drawable/new_logo_round"
        , Theme = "@style/MainTheme"
        , MainLauncher = true
        , ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)
        ]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// Singleton instance of MainActivity
        /// </summary>
        public static MainActivity MainActivitySingleton;

        public MainActivity()
        {
            MainActivitySingleton = this;
        }

        /// <summary>
        /// Kinda like the CTOR
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            DependencyService.Register<ChromeCustomTabsBrowser>();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //For Popup Messages
            //Rg.Plugins.Popup.Popup.Init(this, bundle);
            Rg.Plugins.Popup.Popup.Init(this);

            // Xamarin.Essentials
            Xamarin.Essentials.Platform.Init(this, bundle);

            HtmlLabelRenderer.Initialize();

            // set rpm service started intent extra
            OnNewIntent(Intent);

            // Services
            CheckForStartedServices(bundle);

            try
            {
                using var builder = new StrictMode.VmPolicy.Builder();
                StrictMode.SetVmPolicy(builder.Build());
            }
            catch (System.Exception)
            { }

            EnsureGlobalizations();

            //Xamarin
            Xamarin.Forms.Forms.Init(this, bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            LoadApplication(new App(new AndroidInitializer()));
        }

        /// <summary>
        /// Override for permission service
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            try
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch (System.Exception)
            { }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Override OnBackPressed to handle popups
        /// </summary>
        public override void OnBackPressed()
        {
            try
            {
                Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
            }
            catch (System.Exception) { }
        }

        #region Services
        /// <summary>
        /// Override OnNewIntent to check for running services
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
        {
            if (intent == null)
            {
                return;
            }

            CheckForStartedServices(intent.Extras);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            try
            {
                outState?.PutBoolean(AutomationService.SERVICE_STARTED_KEY, AutomationServiceHelper.IsStarted);
            }
            catch (System.Exception) { }
            
            base.OnSaveInstanceState(outState);
        }

        private void CheckForStartedServices(Bundle bundle)
        {
            if (bundle == null)
            {
                return;
            }

            try
            {
                AutomationServiceHelper.IsStarted = bundle.GetBoolean(AutomationService.SERVICE_STARTED_KEY, false);
            }
            catch (System.Exception) { }
        }
        #endregion

        /// These classes won't be linked away because of the code,
        /// but we also won't have to construct unnecessarily either,
        /// hence the if statement with (hopefully) impossible
        /// runtime condition.
        ///
        /// This is to resolve crash at CultureInfo.CurrentCulture
        /// when language is set to Thai. See
        /// https://github.com/xamarin/Xamarin.Forms/issues/4037
        private void EnsureGlobalizations()
        {
            if (Environment.DirectoryDocuments == "_never_POSSIBLE_")
            {
                new System.Globalization.ChineseLunisolarCalendar();
                new System.Globalization.HebrewCalendar();
                new System.Globalization.HijriCalendar();
                new System.Globalization.JapaneseCalendar();
                new System.Globalization.JapaneseLunisolarCalendar();
                new System.Globalization.KoreanCalendar();
                new System.Globalization.KoreanLunisolarCalendar();
                new System.Globalization.PersianCalendar();
                new System.Globalization.TaiwanCalendar();
                new System.Globalization.TaiwanLunisolarCalendar();
                new System.Globalization.ThaiBuddhistCalendar();
                new System.Globalization.UmAlQuraCalendar();
            }
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            // Register any platform specific implementations
        }
    }
}