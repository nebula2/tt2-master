
using TT2Master.Interfaces;
using TT2Master.iOS;
using Xamarin.Forms;

/// <summary>
/// Implementation of Interface to access Filesystem
/// </summary>
[assembly: Dependency(typeof(MediationTestHelper))]
namespace TT2Master.iOS
{
    public class MediationTestHelper : ITestAdmobMediation
    {
        public void StartMediationTest()
        {
            return;
        }
    }
}
