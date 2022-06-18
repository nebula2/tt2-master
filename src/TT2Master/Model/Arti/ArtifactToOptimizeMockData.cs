using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// Just some mock data for artifact optimizer visual settings
    /// </summary>
    public class ArtifactToOptimizeMockData
    {
        /// <summary>
        /// Mock data
        /// </summary>
        public static List<ArtifactToOptimize> MockData = InitMockData();

        /// <summary>
        /// Returns a List of mock data
        /// </summary>
        /// <returns></returns>
        private static List<ArtifactToOptimize> InitMockData()
        {
            var lst = new List<ArtifactToOptimize>();

            for (int i = 1; i < 30; i++)
            {
                lst.Add(new ArtifactToOptimize(i)
                {
                    ID = "Artifact" + i,
                    SortIndex = i,
                    Amount = 1,
                    ActiveRatio = 1,
                    ClickAmount = 1,
                    CostCoefficient = 1,
                    CostExpo = 1,
                    CurrPercentage = 5,
                    DamageBonus = 1,
                    Description = "Test",
                    Effect = "No Effect",
                    Efficiency = 2,
                    Level = 20,
                    UpgradeReason = "Test",
                    SubstitutionCount = 1,
                    RelicsSpent = 1,
                    //CategoryOutput = "Category",
                    DiscoveryPool = 1,
                    EffectPerLevel = 1,
                    EnchantmentLevel = 1,
                    EnchantmentMagnitude = 1,
                    EnchantmentMult = 1,
                    EnchantmentPool = 1,
                    GeneralTier = 1,
                    InPercent = 5,
                    IsClickEnabled = true,
                    MaxLevel = 500,
                });
            }

            return lst;
        }
    }
}
