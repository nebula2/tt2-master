using SQLite;
using System;
using System.Linq;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// Holds Artifact Optimizer Settings
    /// </summary>
    [Table("ARTOPTSETTINGS")]
    public class ArtOptSettings
    {
        #region Properties
        [PrimaryKey]
        public string ID { get; set; }

        public int StepAmountId { get; set; } = 6;

        public double LifeTimeSpentPercentageOnAmount { get; set; } = 10;

        public double BoSRoyalty { get; set; } = 50;

        public int HeroDamageInt { get; set; } = 0;

        public int HeroBaseTypeInt { get; set; } = 0;

        public string Build { get; set; } = "_SHIP";

        public bool IsClickSuggestionEnabled { get; set; } = true;

        /// <summary>
        /// BoS Royalty in tournament
        /// </summary>
        public double BosTourneyRoyalty { get; set; } = 10;

        public double MinEfficiency { get; set; } = 1.05;

        public int MaxArtifactAmount { get; set; } = 30;

        public bool HasHerosMaxed { get; set; }
        #endregion

        #region Ctor
        public ArtOptSettings()
        {
            ID = "1";
        } 
        #endregion

        #region Public Functions
        /// <summary>
        /// Checks if the values in here are complete
        /// </summary>
        /// <returns></returns>
        public bool ValuesComplete()
        {
            return LifeTimeSpentPercentageOnAmount > 0
                && BoSRoyalty >= 0
                && BosTourneyRoyalty >= 0
                && HeroDamageInt >= 0
                && HeroBaseTypeInt >= 0
                && StepAmountId >= 0
                && MaxArtifactAmount > 0
                && MinEfficiency > 0;
        }

        /// <summary>
        /// Checks if the values in here are valid
        /// </summary>
        /// <returns></returns>
        public bool CheckIfValuesAreValid()
        {
            bool result = true;

            //StepAmount-ID
            var sa = ArtStepAmounts.StepAmounts.Where(x => x.ID == StepAmountId).FirstOrDefault();

            if (sa == null)
            {
                result = false;
            }

            IsClickSuggestionEnabled = true;
            LifeTimeSpentPercentageOnAmount = 10;

            return result;
        }

        public override string ToString()
        {

            return $@"ID        {ID                                 }
StepAmountId                    {StepAmountId                       }
LifeTimeSpentPercentageOnAmount {LifeTimeSpentPercentageOnAmount    }
BoSRoyalty                      {BoSRoyalty                         }
HeroDamageInt                   {HeroDamageInt                      }
HeroBaseTypeInt                 {HeroBaseTypeInt                    }
Build                           {Build                              }
IsClickSuggestionEnabled        {IsClickSuggestionEnabled           }
BosTourneyRoyalty               {BosTourneyRoyalty                  }
MinEfficiency                   {MinEfficiency                      }
MaxArtifactAmount               {MaxArtifactAmount                  }"
;
        } 
        #endregion
    }
}