using System;
using System.Collections.Generic;
using TT2Master.Helpers;
using TT2Master.Model.Assets;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// Handler for ArtifactCostInfo.csv
    /// </summary>
    public static class ArtifactCostHandler
    {
        #region Private member
        private static bool _isInitialized;
        #endregion

        #region Properties
        /// <summary>
        /// List of ArtifactCosts filled from ArtifactCostInfo.csv
        /// </summary>
        public static List<ArtifactCost> ArtifactCosts { get; private set; }
        #endregion

        #region Public Functions
        /// <summary>
        /// Get the cost of artifact purchasement by giving the amount of owned artifacts
        /// </summary>
        /// <param name="ownedArts"></param>
        /// <returns></returns>
        public static double CostSum(int ownedArts, int enchantedArtifacts)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            double result = 0;

            int purchasedAmount = ownedArts + enchantedArtifacts;

            for (int i = 0; i < ArtifactCosts.Count; i++)
            {
                if (i < purchasedAmount)
                {
                    result += ArtifactCosts[i].RelicCost;
                }
            }

            return result;
        }

        /// <summary>
        /// Initializes cost collections from asset manager
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            try
            {
                ArtifactCosts = AssetReader.GetInfoFile<ArtifactCost, ArtifactCostMap>(InfoFileEnum.ArtifactCostInfo);

                _isInitialized = true;

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("ArtifactCostHandler.Initialize", new CustErrorEventArgs(ex));
                return false;
            }
        }
        #endregion

        #region events and delegates
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