using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Assets;
using TT2Master.Model.SP;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// Class for handling the TT2 SkillInfo-File
    /// </summary>
    public static class SkillInfoHandler
    {
        #region Properties
        /// <summary>
        /// List of Skills the Game provides
        /// </summary>
        public static List<Skill> Skills { get; set; } = new List<Skill>();

        /// <summary>
        /// List of <see cref="SPOptSkill"/> for optimization stuff
        /// </summary>
        public static List<SPOptSkill> OptSkills { get; set; } = new List<SPOptSkill>();

        /// <summary>
        /// List of Reductions
        /// </summary>
        public static List<SPSkillReduction> SkillReductions { get; private set; } = new List<SPSkillReduction>();

        /// <summary>
        /// Settings for special calculation skills
        /// </summary>
        public static List<SPSpecialCalculationSetting> SpecialCalculationSettings = new List<SPSpecialCalculationSetting>()
        {
            new SPSpecialCalculationSetting()
            {
                TalentId = "CloneSkillBoost",
                MethodName = "CalculateCloneSkillBoostEfficiency",
                IsAdditive = true,
            },
            new SPSpecialCalculationSetting()
            {
                TalentId = "PetQTE",
                MethodName = "CalculatePetQTEEfficiency",
                IsAdditive = true,
            },
            new SPSpecialCalculationSetting()
            {
                TalentId = "HelperBoost",
                MethodName = "CalculateHelperBoostEfficiency",
                IsAdditive = false,
            },
        };
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads known Skills
        /// </summary>
        public static bool LoadSkills()
        {
            try
            {
                #region Setup
                OnLogMePlease?.Invoke("SkillInfoHandler", new InformationEventArgs("SkillInfoHandler.LoadSkillsAsync"));
                // Get Info file
                Skills = AssetReader.GetInfoFile<Skill, SkillMap>(InfoFileEnum.SkillTreeInfo20);
                #endregion

                foreach (var item in Skills)
                {
                    item.Name = ("Skill." + item.TalentID).TranslatedString();
                }

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("SkillInfoHandler", new CustErrorEventArgs(ex));
                return false;
            }
        }

        /// <summary>
        /// Fills known Skills from Savefile
        /// </summary>
        public static void FillSkills(SaveFile save)
        {
            OnLogMePlease?.Invoke("SkillInfoHandler", new InformationEventArgs($"SkillInfoHandler: FillSkills()"));
            try
            {
                if (LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    FillSkillsFromSaveFile(save);
                }
                else
                {
                    FillSkillsFromExportFile(save);
                }
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("SkillInfoHandler", new CustErrorEventArgs(ex));
            }
        }

        private static void FillSkillsFromSaveFile(SaveFile save)
        {
            foreach (var token in save.SkillTreeModel)
            {
                //Get index of skill in list
                try
                {
                    int index = Skills.FindIndex(x => x.TalentID == token.Key);

                    //known Skill
                    if (index >= 0)
                    {
                        //Set currentLevel from JSON by TalentID
                        Skills[index].CurrentLevel = JfTypeConverter.ForceInt(((string)token.Value));
                    }
                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke("SkillInfoHandler", new InformationEventArgs($"SkillInfoHandler: FillSkills() -> Error on setting level: {ex.Message}\n\n{ex.Data}"));
                }

            }
        }


        private static void FillSkillsFromExportFile(SaveFile save)
        {
            if(save.SkillTreeModel == null)
            {
                return;
            }

            foreach (var token in save.SkillTreeModel)
            {
                //Get index of skill in list
                try
                {
                    int index = Skills.FindIndex(x => x.Name == token.Key ||
                        x.Name == "Searching Light" && token.Key == "Searing Light" ||
                        x.Name == "Soul Blade" && token.Key == "Dagger Storm" ||
                        x.Name == "Poisoned Blade" && token.Key == "Poison Edge" ||
                        x.Name == "Divine Blessing" && token.Key == "Divine Supremacy" ||
                        x.Name == "Phantom Vengeance" && token.Key == "Phantom Supremacy"
                    );

                    //known Skill
                    if (index >= 0)
                    {
                        //Set currentLevel from JSON by TalentID
                        Skills[index].CurrentLevel = JfTypeConverter.ForceInt(((string)token.Value));
                    }
                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke("SkillInfoHandler", new InformationEventArgs($"SkillInfoHandler: FillSkills() -> Error on setting level: { ex.Message}\n\n{ ex.Data}"));
                }
            }

            var spOwned = JfTypeConverter.ForceInt(save.ProfileData["Skill Points Owned"].Value<string>());
            SaveFile.SPSpent = Skills.Sum(s => s.GetSpSpentAmount());
            
            // Adress bug in export. 
            //Sometimes "Skill Points Owned" contains the available amount of SP and sometimes it contains the amount of collected SP.
            if(SaveFile.SPSpent > spOwned) 
            {
                SaveFile.SPReceived = SaveFile.SPSpent + spOwned;
            }
            else 
            {
                SaveFile.SPReceived = spOwned;
            }
        }

        /// <summary>
        /// Loads and fills <see cref="OptSkills"/> if needed.
        /// </summary>
        /// <param name="freshLoad">force a fresh reload</param>
        public static void PopulateOptSkills(SaveFile save, bool freshLoad = false)
        {
            try
            {
                // do nothing if you don't need to
                if (OptSkills.Count > 0 && OptSkills.Count == Skills.Count && !freshLoad)
                {
                    return;
                }

                // Load and populate Skills
                LoadSkills();
                FillSkills(save);

                // read in reductions
                LoadSkillReductions();

                OptSkills = new List<SPOptSkill>();

                foreach (var item in Skills)
                {
                    // create new item
                    var tmp = new SPOptSkill()
                    {
                        A = item.A,
                        B = item.B,
                        C = item.C,
                        Cost = item.Cost,
                        BonusTypeA = item.BonusTypeA,
                        BonusTypeB = item.BonusTypeB,
                        BonusTypeC = item.BonusTypeC,
                        BonusTypeD = item.BonusTypeD,
                        BonusAmountD = item.BonusAmountD,
                        Branch = item.Branch,
                        Class = item.Class,
                        CurrentLevel = item.CurrentLevel,
                        MaxLevel = item.MaxLevel,
                        Name = item.Name,
                        Note = item.Note,
                        S = item.S,
                        Slot = item.Slot,
                        SplashEffectAtLevel = item.SplashEffectAtLevel,
                        SPReq = item.SPReq,
                        TalentID = item.TalentID,
                        TalentReq = item.TalentReq,
                        Tier = item.Tier,
                        TierNum = item.TierNum,
                    };

                    // set reduction and stuff
                    tmp.DefaultReduction = SkillReductions.Where(x => x.SkillId == tmp.TalentID).FirstOrDefault();

                    // set special calculation
                    tmp.IsHavingSpecialCalculation = IsSkillHavingSpecialCalculation(tmp.TalentID);

                    // set MS unlocked
                    var ms = save.ThisPlayer?.StageMax ?? 0;
                    tmp.IsAvailable = ms >= tmp.S[0];

                    OptSkills.Add(tmp);
                }
            }
            catch (Exception e)
            {
                OnProblemHaving?.Invoke("SkillInfoHandler", new CustErrorEventArgs(e));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads reductions for skills
        /// </summary>
        /// <returns></returns>
        private static bool LoadSkillReductions()
        {
            SkillReductions = new List<SPSkillReduction>();

            try
            {
                SkillReductions = AssetReader.GetInfoFile< SPSkillReduction, SPSkillReductionMap>("skillReductions");
                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("SkillInfoHandler", new CustErrorEventArgs(ex));
                return false;
            }
        }

        /// <summary>
        /// Checks if the given skill has a special calculation in the SP Optimizer
        /// </summary>
        /// <param name="talentId">ID of the Skill (or TalentID)</param>
        /// <returns></returns>
        private static bool IsSkillHavingSpecialCalculation(string talentId) => SpecialCalculationSettings.Where(x => x.TalentId == talentId).Count() > 0;
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
        #endregion
    }
}