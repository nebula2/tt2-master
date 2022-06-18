using Android.Widget;
using TT2Master.Droid;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(MessageHelper))]
namespace TT2Master.Droid
{
    public class MessageHelper : ISendMessage
    {
        public void LongAlert(string message) => Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();

        public void ShortAlert(string message) => Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
    }
}