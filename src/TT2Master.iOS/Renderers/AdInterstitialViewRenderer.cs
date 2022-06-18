using TT2Master.iOS.Renderers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdInterstitialViewRenderer))]
namespace TT2Master.iOS.Renderers
{
    /// <summary>
    /// custom renderer for big ad
    /// </summary>
    public class AdInterstitialViewRenderer : IAdInterstitial
    {
        public void LoadAd()
        {
            return;
        }

        public void ShowAd()
        {
        }
    }
}