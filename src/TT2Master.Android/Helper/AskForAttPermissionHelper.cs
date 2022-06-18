using System.Threading.Tasks;
using TT2Master.Droid;
using TT2Master.Interfaces;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Xamarin.Forms.Dependency(typeof(AskForAttPermissionHelper))]
namespace TT2Master.Droid
{
    public class AskForAttPermissionHelper : IAskForAttPermission
    {
        public async Task<bool> AskUserAsync()
        {
            return await Task.Run(() => true);
        }
    }
}