using Android.App;
using Android.Content;
using Android.OS;
using System;

namespace TT2Master.Droid
{
    [Activity(Label = "OidcCallbackActivity")]
    [IntentFilter(new[] { Intent.ActionView },
            Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
            DataScheme = "xamarinformsclients")]
    public class OidcCallbackActivity : Activity
    {
        public static event Action<string> Callbacks;

        public OidcCallbackActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Callbacks?.Invoke(Intent.DataString);

            Finish();

            StartActivity(typeof(MainActivity));
        }
    }
}