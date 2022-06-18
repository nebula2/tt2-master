using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Admob
{
    /// <summary>
    /// Ads max handler which should prevent ad violation
    /// </summary>
    public static class AdsMaxHandler
    {
        private const int _maxAdsClickablePerDay = 5;

        private static void CheckAdLimit()
        {
            if (string.IsNullOrEmpty(LocalSettingsORM.AdLimitResetDay))
            {
                return;
            }

            if (!DateTime.TryParse(LocalSettingsORM.AdLimitResetDay, out var result))
            {
                LocalSettingsORM.AdLimitResetDay = null;
                return;
            }
            else if ((DateTime.Now.Date >= result.Date))
            {
                LocalSettingsORM.AdLimitResetDay = null;
                return;
            }
        }

        private static void CheckDailyCounter()
        {
            if (string.IsNullOrEmpty(LocalSettingsORM.AdCounterResetDay))
            {
                return;
            }

            if (!DateTime.TryParse(LocalSettingsORM.AdCounterResetDay, out var result))
            {
                LocalSettingsORM.AdCounterResetDay = null;
                LocalSettingsORM.AdsClickedTodayCounter = 0;
                return;
            }
            else if ((DateTime.Now.Date >= result.Date))
            {
                LocalSettingsORM.AdCounterResetDay = null;
                LocalSettingsORM.AdsClickedTodayCounter = 0;
                return;
            }
        }

        /// <summary>
        /// Increments the amount of ads clicked and disables ads if the max amount is reached
        /// </summary>
        public static void IncrementAdClickedCounter()
        {
            LocalSettingsORM.AdsClickedTodayCounter++;
            LocalSettingsORM.AdCounterResetDay = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            if (LocalSettingsORM.AdsClickedTodayCounter >= _maxAdsClickablePerDay)
            {
                LocalSettingsORM.AdLimitResetDay = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// Checks if max ads reached is valid and fixes if not
        /// </summary>
        public static void CheckForMaxAdsReached()
        {
            CheckAdLimit();
            CheckDailyCounter();
        }

        public static bool IsAllowedToShowAds()
        {
            CheckForMaxAdsReached();

            return string.IsNullOrEmpty(LocalSettingsORM.AdLimitResetDay);
        }
    }
}
