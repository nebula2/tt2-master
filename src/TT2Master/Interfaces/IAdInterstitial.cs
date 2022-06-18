using System.Security;

namespace TT2Master
{
    [SecuritySafeCritical]
    public interface IAdInterstitial
    {
        /// <summary>
        /// shows the ad
        /// </summary>
        void ShowAd();

        /// <summary>
        /// Loads the ad
        /// </summary>
        void LoadAd();
    }
}