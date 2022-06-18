using TT2Master.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdInterstitialDroid))]
namespace TT2Master.Droid
{
    /// <summary>
    /// Big ad implementation
    /// </summary>
    public class AdInterstitialDroid : IAdInterstitial
    {
        public AdInterstitialDroid()
        {
        }

        public void LoadAd()
        {
            return;
        }

        public void ShowAd()
        {
            return;
        }
    }
}