using Android.OS;
using TT2Master.Droid;
using TT2Master.Interfaces;
using TT2Master.Model;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(ClientOSVersionHelper))]
namespace TT2Master.Droid
{
    public class ClientOSVersionHelper : IGetClientOSVersion
    {
        public int GetClientOSVersion() => (int)Build.VERSION.SdkInt;
    }
}
