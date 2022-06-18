using TT2Master.Droid;
using TT2Master.Interfaces;
using Xamarin.Forms;

/// <summary>
/// Closes app
/// </summary>
[assembly: Dependency(typeof(CloseApplicationHelper))]
namespace TT2Master.Droid
{
    public class CloseApplicationHelper : ICloseApplication
    {
        public void CloseApplication()
        {
            MainActivity.MainActivitySingleton.FinishAffinity();
        }
    }
}
