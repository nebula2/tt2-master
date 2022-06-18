using Android.OS;
using TT2Master.Droid;
using TT2Master.Interfaces;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Xamarin.Forms.Dependency(typeof(WindowModeHelper))]
namespace TT2Master.Droid
{
    public class WindowModeHelper : IGetWindowMode
    {
        public bool IsFullScreen()
        {
            try
            {
                if (Build.VERSION.SdkInt <= Android.OS.BuildVersionCodes.M) return true;

                return !MainActivity.MainActivitySingleton.IsInMultiWindowMode &&
                       !MainActivity.MainActivitySingleton.IsInPictureInPictureMode;
            }
            catch (System.Exception)
            {
                return true;
            }
        }
    }
}