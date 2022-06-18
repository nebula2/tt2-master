using Foundation;
using LabelHtml.Forms.Plugin.iOS;
using Prism;
using Prism.Ioc;
using System;
using System.Linq;
using UIKit;


namespace TT2Master.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UINavigationBar.Appearance.TintColor = UIColor.Black;
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White };

            UIButton.Appearance.SetTitleColor(UIColor.White, UIControlState.Normal);

            app.StatusBarStyle = UIStatusBarStyle.LightContent;

            Rg.Plugins.Popup.Popup.Init();

            HtmlLabelRenderer.Initialize();
            EnsureGlobalizations();

            // xamarin
            Xamarin.Forms.Forms.Init();

            LoadApplication(new App(new iOSInitializer()));

            return  base.FinishedLaunching(app, options);
        }

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
            string[] smelly = { "hello :*" };
            if (smelly.Contains("_never_POSSIBLE_"))
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

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {

        }
    }
}
