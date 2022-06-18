using System;
using System.Collections.Generic;
using System.Text;
using TT2Master.Helpers;
using TT2Master.Model.Assets;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master.Model.Raid
{
    /// <summary>
    /// Handles some raid info files
    /// RaidAreaInfo, RaidEnemyInfo, RaidLevelInfo
    /// </summary>
    public static class RaidInfoHandler
    {
        #region Properties
        private static bool _isInitialized;

        public static List<RaidAreaInfo> AreaInfos { get; private set; } = new List<RaidAreaInfo>();

        public static List<RaidLevelInfo> LevelInfos { get; private set; } = new List<RaidLevelInfo>();

        public static List<RaidEnemyInfo> EnemyInfos { get; private set; } = new List<RaidEnemyInfo>();
        #endregion

        #region private methods
        private static bool LoadAreaInfos()
        {
            try
            {
                AreaInfos = AssetReader.GetInfoFile<RaidAreaInfo, RaidAreaInfoMap>(InfoFileEnum.RaidAreaInfo);

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("RaidInfoHandler.LoadAreaInfos", new CustErrorEventArgs(ex));
                return false;
            }
        }

        private static bool LoadRaidLevelInfos()
        {
            try
            {
                LevelInfos = AssetReader.GetInfoFile<RaidLevelInfo, RaidLevelInfoMap>(InfoFileEnum.RaidLevelInfo);

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("RaidInfoHandler.LoadRaidLevelInfos", new CustErrorEventArgs(ex));
                return false;
            }
        }

        private static bool LoadEnemyInfos()
        {
            try
            {
                EnemyInfos = AssetReader.GetInfoFile<RaidEnemyInfo, RaidEnemyInfoMap>(InfoFileEnum.RaidEnemyInfo);

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("RaidInfoHandler.LoadEnemyInfos", new CustErrorEventArgs(ex));
                return false;
            }
        }
        #endregion

        #region public functions
        public static bool LoadRaidInfos()
        {
            OnLogMePlease?.Invoke("RaidInfoHandler", new InformationEventArgs("LoadRaidInfos start"));

            if (_isInitialized)
            {
                OnLogMePlease?.Invoke("RaidInfoHandler", new InformationEventArgs("LoadRaidInfos is already initialized"));
                return true;
            }

            bool result = true;

            result &= LoadAreaInfos();
            result &= LoadRaidLevelInfos();
            result &= LoadEnemyInfos();

            OnLogMePlease?.Invoke("RaidInfoHandler", new InformationEventArgs($"LoadRaidInfos end: success -> {result}"));

            _isInitialized = true;
            return result;
        }
        #endregion

        #region events and delegates
        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public static event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}
