using TT2Master.Interfaces;
using TT2Master.iOS.Helper;
using Xamarin.Forms;

[assembly: Dependency(typeof(WindowModeHelper))]
namespace TT2Master.iOS.Helper
{
    public class WindowModeHelper : IGetWindowMode
    {
        public bool IsFullScreen()
        {
            return true;
        }
    }
}