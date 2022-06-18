using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Assets;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// Class that handles ArtifactInfo and holds current Artifacts
    /// </summary>
    public static class ArtifactHandler
    {
        #region Properties
        /// <summary>
        /// List of Artifacts the Game provides
        /// Execute <see cref="LoadArtifacts"/> and <see cref="FillArtifacts"/> before use
        /// </summary>
        public static List<Artifact> Artifacts { get; private set; }

        private static double _divider = 0;

        private static bool _artifactsLoaded = false;
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads known Artifacts from Infofile
        /// </summary>
        public static bool LoadArtifacts()
        {
            try
            {
                OnLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs("LoadArtifactsAsync"));

                // Load items from amazon csv infofile
                Artifacts = AssetReader.GetInfoFile<Artifact, ArtifactMap>(InfoFileEnum.ArtifactInfo);

                foreach (var item in Artifacts)
                {
                    item.Name = item.ID.TranslatedString();
                }

                OnLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: loaded info file with {Artifacts.Count} items"));

                _artifactsLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("ActiveSkillHandler", new InformationEventArgs($"ArtifactHandler: Exception at LoadArtifacts: { ex.Message}\n\n { ex.Data}\n\n"));
                OnProblemHaving?.Invoke("ArtifactHandler", new CustErrorEventArgs(ex));
                return false;
            }
        }

        /// <summary>
        /// Fills known Artifacts from Savefile
        /// </summary>
        public static void FillArtifacts(SaveFile save)
        {
            OnLogMePlease?.Invoke("ActiveSkillHandler", new InformationEventArgs($"ArtifactHandler: FillArtifacts()"));
            try
            {
                if (!_artifactsLoaded)
                {
                    LoadArtifacts();
                }

                if(save.ArtifactModel != null)
                {
                    if (LocalSettingsORM.IsReadingDataFromSavefile) 
                    {
                        FillArtifactsFromSaveFile(save);
                    }
                    else 
                    {
                        FillArtifactsFromExportFile(save);
                    }
                }

                //Set _divider
                RecalculateDivider();
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("ActiveSkillHandler", new InformationEventArgs($"ArtifactHandler: FillArtifacts() -> Error { ex.Message}\n\n{ ex.Data}"));
                OnProblemHaving?.Invoke("ArtifactHandler", new CustErrorEventArgs(ex));
            }
        }

        private static void FillArtifactsFromSaveFile(SaveFile save)
        {
            foreach (var token in save.ArtifactModel)
            {
                //Get index of artifact in list
                int artifactIndex = Artifacts.FindIndex(x => x.ID == token.Key);

                //known Artifact
                if (artifactIndex >= 0)
                {
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: Pupulating Artifact: {Artifacts[artifactIndex].ID}"));
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: found value for level: {(string)token.Value.SelectToken("level.$content")}"));
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: found value for RelicsSpent: {(string)token.Value.SelectToken("relics_spent.$content")}"));

                    //Set artifactIndex from JSON
                    Artifacts[artifactIndex].Level = JfTypeConverter.ForceDoubleUniversal((string)token.Value.SelectToken("level.$content"));
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: set value for level after conversion: {Artifacts[artifactIndex].Level}"));

                    Artifacts[artifactIndex].RelicsSpent = Artifacts[artifactIndex].CostAtLevel(0);

                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: setting default categories"));

                    // Set enchantment level
                    Artifacts[artifactIndex].EnchantmentLevel = JfTypeConverter.ForceDoubleUniversal((string)token.Value.SelectToken("enchantment_level.$content"));
                }

                //unknown Artifact - ignore everything but Artifact-Keys
                else if (token.Key.Contains("Artifact"))
                {
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: elseif new Artifact with level/ relicsSpent {(string)token.Value.SelectToken("level.$content")} {(string)token.Value.SelectToken("relics_spent.$content")}"));
                    Artifacts.Add(new Artifact()
                    {
                        ID = token.Key,
                        Description = "???",
                        Level = JfTypeConverter.ForceDoubleUniversal((string)token.Value.SelectToken("level.$content")),
                        RelicsSpent = JfTypeConverter.ForceDoubleUniversal((string)token.Value.SelectToken("relics_spent.$content")),
                        EnchantmentLevel = JfTypeConverter.ForceDoubleUniversal((string)token.Value.SelectToken("enchantment_level.$content")),
                    });
                }
            }
        }

        private static void FillArtifactsFromExportFile(SaveFile save) 
        {
            if(save.ArtifactModel == null)
            {
                return;
            }

            foreach (var token in save.ArtifactModel)
            {
                //Get index of artifact in list
                int artifactIndex = Artifacts.FindIndex(x => x.InternalName == token.Key 
                    || x.InternalName == "Lucky Foot of Al-Mi-Raj" && token.Key == "Lucky Foot of Al-mi'raj");

                //known Artifact
                if (artifactIndex >= 0)
                {
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: Pupulating Artifact: {Artifacts[artifactIndex].ID}"));
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: found value for level: {(string)token.Value["level"]}"));

                    //Set artifactIndex from JSON
                    Artifacts[artifactIndex].Level = JfTypeConverter.ForceDoubleUniversal((string)token.Value["level"]);
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: set value for level after conversion: {Artifacts[artifactIndex].Level}"));

                    Artifacts[artifactIndex].RelicsSpent = Artifacts[artifactIndex].CostAtLevel(0);

                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: setting default categories"));

                    // Set enchantment level
                    var isEnchanted = (string)token.Value["enchanted"] == "True";
                    Artifacts[artifactIndex].EnchantmentLevel = isEnchanted ? 1.0 : 0.0;
                }

                //unknown Artifact - ignore everything but Artifact-Keys
                else if (token.Key.Contains("Artifact"))
                {
                    OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"ArtifactHandler: elseif new Artifact with level/ relicsSpent {(string)token.Value["level"]}"));

                    var isEnchanted = (string)token.Value["enchanted"] == "True";
                    Artifacts[artifactIndex].EnchantmentLevel = isEnchanted ? 1.0 : 0.0;

                    Artifacts.Add(new Artifact()
                    {
                        ID = token.Key,
                        Description = "???",
                        Level = JfTypeConverter.ForceDoubleUniversal((string)token.Value["level"]),
                        RelicsSpent = 0.0,
                        EnchantmentLevel = isEnchanted ? 1.0 : 0.0,
                    });
                }
            }
        }

        /// <summary>
        /// Gets the minimum Level of all Artifacts.
        /// Artifacts with max level are not included
        /// </summary>
        /// <returns></returns>
        public static double GetMinLevelOfAllArtifacts()
        {
            var minArt = Artifacts.Where(x => x.Level > 0 && x.MaxLevel == 0).Aggregate((a1, a2) => a1.Level < a2.Level ? a1 : a2);
            return minArt.Level;
        }

        /// <summary>
        /// Returnes the sum of LifeTimeSpent from all Relics in sum
        /// </summary>
        /// <returns></returns>
        public static double GetLifeTimeSpentOnAll()
        {
            double tmp = 0; //return val
            int i = 1; //counter for logger

            OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"GetLifeTimeSpentOnAll: Start summing spent relics of {Artifacts.Count} Artifacts"));

            foreach (var item in Artifacts)
            {
                tmp += item.RelicsSpent;

                i++;
            }

            OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"GetLifeTimeSpentOnAll: sum is {tmp}"));

            return tmp;
        }

        /// <summary>
        /// Use before <see cref="CalculateLifeTimeSpentPercentage(double)"/>
        /// </summary>
        public static void RecalculateDivider() => _divider = GetLifeTimeSpentOnAll(); //normalization of value. Put here to speed calculation up

        /// <summary>
        /// Calculates percentage of LifetimeSpent
        /// </summary>
        /// <param name="relicsYouGot">relics you got</param>
        /// <returns>percentage of relicsYouGot from LifeTimeSpentOnAll</returns>
        public static double CalculateLifeTimeSpentPercentage(double relicsYouGot)
        {
            if (relicsYouGot == 0)
            {
                OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"CalculateLifeTimeSpentPercentage: Relics you got is 0. returning"));

                return 0;
            }

            OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"CalculateLifeTimeSpentPercentage: Calculating percentage with w * 100 / g, where w = {relicsYouGot}/{_divider} and g = 1"));

            double w = (relicsYouGot / _divider);
            int g = 1;

            double result = w * 100 / g;

            OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"CalculateLifeTimeSpentPercentage: Calculated {result}"));

            return result;
        }

        /// <summary>
        /// Calculates percentage of LifetimeSpent for dummies. Use this if UseMasterBoSDisplay is false -.-
        /// </summary>
        /// <param name="relicsYouGot">relics you got</param>
        /// <returns>percentage of relicsYouGot from LifeTimeSpentOnAll</returns>
        public static double CalculateLifeTimeSpentPercentageForDummies(double relicsYouGot)
        {
            if (relicsYouGot == 0)
            {
                OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"CalculateLifeTimeSpentPercentageForDummies: Relics you got is 0. returning"));

                return 0;
            }

            OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"CalculateLifeTimeSpentPercentageForDummies: Calculating percentage with w * 100 / g, where w = {relicsYouGot}/{_divider} and g = 1"));

            double w = (relicsYouGot / SaveFile.RelicsReceived);

            double result = w * 100;

            OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"CalculateLifeTimeSpentPercentageForDummies: Calculated {result}"));

            return result;
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

        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void LogCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when <see cref="Initialize"/> has something to log
        /// </summary>
        public static event LogCarrier OnLogMePlease;
        /// <summary>
        /// Raised when <see cref="Initialize"/> has something to log
        /// </summary>
        public static event LogCarrier OnOptiLogMePlease;
        #endregion
    }
}