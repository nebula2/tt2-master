using TT2Master.Droid;
using TT2Master.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(MediationTestHelper))]
namespace TT2Master.Droid
{
    public class MediationTestHelper : ITestAdmobMediation
    {
        public void StartMediationTest()
        {
            return;
        }
    }
}
