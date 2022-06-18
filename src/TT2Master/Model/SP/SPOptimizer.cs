using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master.Model.SP
{
    public class SPOptimizer
    {
        #region Properties
        /// <summary>
        /// Simulation for SP Follower
        /// </summary>
        public List<List<SPOptSkill>> UpgradeSimulation { get; set; } = new List<List<SPOptSkill>>();

        /// <summary>
        /// The current config
        /// </summary>
        public SPOptConfiguration SelectedConfig { get; set; }

        public bool IsIgnoredIfNotEnoughSP { get; set; } = LocalSettingsORM.SPIgnoreNotEnoughSP;

        /// <summary>
        /// Current SP you have
        /// </summary>
        public int CurrentSP { get; set; }

        /// <summary>
        /// Available SP you can spend
        /// </summary>
        public int AvailableSP { get; set; }

        /// <summary>
        /// All dem SP you have spend
        /// </summary>
        public int SPSpent { get; set; }

        /// <summary>
        /// All dem SP you have spend in red branch
        /// </summary>
        public int SPSpentRed { get; set; }

        /// <summary>
        /// All dem SP you have spend in yellow branch
        /// </summary>
        public int SPSpentYellow { get; set; }

        /// <summary>
        /// All dem SP you have spend in blue branch
        /// </summary>
        public int SPSpentBlue { get; set; }

        /// <summary>
        /// All dem SP you have spend in green branch
        /// </summary>
        public int SPSpentGreen { get; set; }

        public string AttentionText { get; set; } = "";

        /// <summary>
        /// Holds the collection of upgrades in the correct order in which they have to be made (the single steps)
        /// </summary>
        public List<SPOptSkill> UpgradeCollection { get; set; }

        public static List<SPOptSkill> UnignoredTree { get; set; } = new List<SPOptSkill>();

        private readonly SaveFile _save;
        #endregion

        #region Initialization Methods
        /// <summary>
        /// Initializes and loads equipment handler to have access to sets
        /// </summary>
        /// <returns></returns>
        private bool LoadEquipInformation()
        {
            try
            {
                EquipmentHandler.OnProblemHaving += EquipmentHandler_OnProblemHaving;

                EquipmentHandler.LoadSetInformation(_save);

                EquipmentHandler.OnProblemHaving -= EquipmentHandler_OnProblemHaving;

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"Could not load equip values: {ex.Message}"));
                return false;
            }
        }

        /// <summary>
        /// Loads Sp values from savefile. How much is spent and how much is available and so on
        /// </summary>
        private bool LoadSPValues()
        {
            try
            {
                var chosenMode = (SPOptMode)SelectedConfig.ModeInt;

                switch (chosenMode)
                {
                    case SPOptMode.Continuous:
                        //Set current SP and available SP from Save file
                        CurrentSP = SaveFile.SPReceived;
                        SPSpent = SaveFile.SPSpent;
                        AvailableSP = CurrentSP - SPSpent;
                        break;
                    case SPOptMode.Creation:
                        //Set CurrentSP from savefile. Rest is not used
                        CurrentSP = SaveFile.SPReceived;
                        SPSpent = 0;
                        AvailableSP = CurrentSP;
                        break;
                    default:
                        break;
                }

                LoadSPSpentOnBranches(SelectedConfig.SkillSettings.Select(x => x.MySPOptSkill).ToList());

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"Could not load SP values: {ex.Message}"));
                CurrentSP = 0;
                AvailableSP = 0;
                return false;
            }
        }

        /// <summary>
        /// creates a new skilltree and does not add ignored skills or skills that depend on ignored skills
        /// </summary>
        private bool CreateUnignoredSkillTree()
        {
            UnignoredTree = new List<SPOptSkill>();

            // set ignoration (only works without recursion because the list is ordered
            for (int i = 0; i < SelectedConfig.SkillSettings.Count; i++)
            {
                CheckSkillIgnore(SelectedConfig.SkillSettings[i]);
            }

            // populate unignored tree
            foreach (var item in SelectedConfig.SkillSettings)
            {
                // ensure max level from sets
                item.MySPOptSkill.EnsureMaxLevelFromSets();

                if (!item.MySPOptSkill.IsIgnored)
                {
                    UnignoredTree.Add(item.MySPOptSkill);
                }
            }

            return UnignoredTree.Count != 0;
        }

        /// <summary>
        /// Checks if the skill is not tolerated. if that is the case the skill will be set to ignored
        /// This method also sets skills to not tolerated which depend on this skill.
        /// <para/>In order for this to work the SelectedConfig.SkillSettings needs to be ordered
        /// </summary>
        /// <param name="parent">skill to check</param>
        private void CheckSkillIgnore(SPOptSkillSetting parent)
        {
            // null check
            if (parent == null)
            {
                return;
            }

            // if not ignored just skip this
            if (parent.IsTolerated)
            {
                parent.MySPOptSkill.IsIgnored = false;
                return;
            }

            // get dependent children
            var children = SelectedConfig.SkillSettings.Where(x => x.MySPOptSkill.TalentReq == parent.SkillId)?.ToList();

            // return if there are none
            if (children == null)
            {
                return;
            }

            // set tolerated value
            foreach (var item in children)
            {
                item.IsTolerated = false;
            }

            // set parent to ignored
            parent.MySPOptSkill.IsIgnored = true;
        }

        /// <summary>
        /// Set the fixed levels which the user chose
        /// Important note: Optimization for those skills is not done (it is done when you have them unlocked! at the specified level)
        /// </summary>
        private bool SetFixedLevels()
        {
            try
            {
                foreach (var item in SelectedConfig.SkillSettings)
                {
                    item.MySPOptSkill.FixedLevel = item.FixedLevel > 0
                        ? item.FixedLevel
                        : 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs(ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Initializes the initial Skill levels for the chosen <see cref="SPOptMode"/>
        /// </summary>
        private bool InitializeSkillLevelsForMode()
        {
            try
            {
                var chosenMode = (SPOptMode)SelectedConfig.ModeInt;

                switch (chosenMode)
                {
                    case SPOptMode.Continuous:
                        // Nothing to do. We should already have our skills with the correct level
                        break;
                    case SPOptMode.Creation:
                        foreach (var item in SelectedConfig.SkillSettings)
                        {
                            item.MySPOptSkill.CurrentLevel = 0;
                        }

                        break;
                    default:
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs(ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Configures the SPOptSkill inside SPOptSkillSetting
        /// </summary>
        /// <returns></returns>
        private bool InitializeSkillsForConfiguration()
        {
            try
            {
                foreach (var item in SelectedConfig.SkillSettings)
                {
                    // set chosen weights
                    item.MySPOptSkill.ChosenDamageWeight = item.CustomDmgReduction > 0 ? item.CustomDmgReduction : item.DefaultDmgReduction;
                    item.MySPOptSkill.ChosenGoldWeight = item.CustomGoldReduction > 0 ? item.CustomGoldReduction : item.DefaultGoldReduction;

                    // set special calculation
                    item.MySPOptSkill.IsSpecialCalculationEnabled = item.MySPOptSkill.IsHavingSpecialCalculation && item.IsSpecialCalculationEnabled;

                    // set sources
                    item.MySPOptSkill.DmgSource = (SPOptDamageSource)SelectedConfig.DamageSourceInt;
                    item.MySPOptSkill.GoldSource = (GoldType)SelectedConfig.GoldSourceInt;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs(ex.Message));
                return false;
            }
        }
        #endregion

        #region Optimization methods
        /// <summary>
        /// Sets the unlocked state in an ordered list of skills
        /// </summary>
        /// <param name="skills">Skills to check unlock state</param>
        private void UpdateUnlockedState(List<SPOptSkill> skills)
        {
            // set unlocked-related properties
            foreach (var item in skills)
            {
                int relevantSpSpent = 0;
                switch (item.Branch)
                {
                    case "BranchRed":
                        relevantSpSpent = SPSpentRed;
                        break;
                    case "BranchYellow":
                        relevantSpSpent = SPSpentYellow;
                        break;
                    case "BranchBlue":
                        relevantSpSpent = SPSpentBlue;
                        break;
                    case "BranchGreen":
                        relevantSpSpent = SPSpentGreen;
                        break;
                    default:
                        break;
                }

                item.IsSpReqFulfilled = relevantSpSpent >= item.SPReq;

                // set remaining sp required for branch to upgrade skill
                item.BranchUpgradeCost = item.IsSpReqFulfilled ? 0 : item.SPReq - relevantSpSpent;

                // set parents
                if (!item.IsUnlocked)
                {
                    item.NotUnlockedParents = new List<string>();
                    FetchUnlockedParents(item, item.NotUnlockedParents);
                }

                // I only need to do this once because the list is ordered!

                // update unlocked
                item.IsUnlocked = item.NotUnlockedParents.Count == 0;

                // set parent sp required
                item.ParentUpgradeCost = GetParentUpgradeCost(item);
                item.SetCompleteUpgradeCost();
            }

            OnLogMePlease?.Invoke(this, new InformationEventArgs("Updated unlocked states"));
        }

        /// <summary>
        /// Sums up the amount of SP you need to invest to upgrade the parents
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        private int GetParentUpgradeCost(SPOptSkill skill)
        {
            int spReqInSkills = 0; // required SP in skills

            if (skill == null)
            {
                return 0;
            }

            if (skill.IsUnlocked)
            {
                return 0;
            }

            for (int i = 0; i < skill.NotUnlockedParents.Count; i++)
            {
                spReqInSkills += SelectedConfig.SkillSettings.Where(x => x.MySPOptSkill.TalentID == skill.NotUnlockedParents[i]).FirstOrDefault().MySPOptSkill.GetUpgradeCost();
            }

            return spReqInSkills;
        }

        /// <summary>
        /// Gets the list of parent skills recursively
        /// </summary>
        /// <param name="sPOpt"></param>
        /// <param name="parents"></param>
        private void FetchUnlockedParents(SPOptSkill sPOpt, List<string> parents)
        {

            // return if there are no parents at all
            if (sPOpt.TalentReq == "None")
            {
                return;
            }

            // get first parent
            var parentSkill = SelectedConfig.SkillSettings.Where(x => x.MySPOptSkill.TalentID == sPOpt.TalentReq).FirstOrDefault()?.MySPOptSkill;

            // return if null
            if (parentSkill == null)
            {
                return;
            }

            // return if unlocked and the level is greater 0
            if (parentSkill.IsUnlocked && parentSkill.CurrentLevel > 0)
            {
                return;
            }

            // add this to parents
            parents.Add(parentSkill.TalentID);

            // check again for parent skill
            FetchUnlockedParents(parentSkill, parents);
        }

        /// <summary>
        /// Check things like "could fixed levels be reached?" post optimization
        /// </summary>
        private void DoPostOptimizationChecks()
        {
            var finalList = CreateUpgradedSkillList();

            // place this thing at top
            AttentionText = $"- {string.Format(AppResources.SpOptSpSpentText, AvailableSP)}\n{AttentionText}";

            foreach (var item in finalList)
            {
                // ignored fixed level
                if (item.IsIgnored && item.FixedLevel > 0)
                {
                    AttentionText += $"- {string.Format(AppResources.IgnoredSkillWithFixedLevel, item.Name)}\n";
                }
                // fixed level could not be reached
                else if (item.CurrentLevel < item.FixedLevel)
                {
                    AttentionText += $"- {string.Format(AppResources.IgnoredSkillFixedLevelNotReached, item.Name, item.FixedLevel, item.CurrentLevel)}\n";
                }
            }

            if(UpgradeCollection?.Count > 0)
            {
                #region Efficiency info
                if (Math.Round(UpgradeCollection[UpgradeCollection.Count - 1].WeightedTotalEfficiency, 2) <= 1.00d)
                {
                    AttentionText += $"{AppResources.SpEfficiencyWarning}\n\n";
                }
                #endregion

                #region ForbiddenContract info
                if(finalList.Where(x => x.TalentID == "ForbiddenContract").FirstOrDefault()?.CurrentLevel > 0)
                {
                    AttentionText += $"{AppResources.ForbiddenContractSpOptInfo}\n";
                }
                #endregion

                #region Multi Cast skills
                var soulBlade = UnignoredTree.FirstOrDefault(x => x.TalentID == "SoulBlade");
                if (soulBlade != null &&
                    soulBlade.ChosenDamageWeight > 0 &&
                    soulBlade?.FixedLevel == 0 && 
                    finalList.FirstOrDefault(x => x.TalentID == soulBlade.TalentID)?.CurrentLevel == 0)
                {
                    AttentionText += $"- Please consider setting a fixed level for {soulBlade.Name} as the calculation is not finished yet. You gain Multi-Cast Stacks on Level 1, 6 and 11.\n";
                }

                var hbmcs = UnignoredTree.FirstOrDefault(x => x.TalentID == "HelperBoostMultiCastSkill");
                if (hbmcs != null &&
                    hbmcs.ChosenDamageWeight > 0 &&
                    hbmcs?.FixedLevel == 0 &&
                    finalList.FirstOrDefault(x => x.TalentID == hbmcs.TalentID)?.CurrentLevel == 0)
                {
                    AttentionText += $"- Please consider setting a fixed level for {hbmcs.Name} as the calculation is not finished yet. You gain Multi-Cast Stacks on Level 1, 6 and 11.\n";
                }

                var tbmcs = UnignoredTree.FirstOrDefault(x => x.TalentID == "TapBoostMultiCastSkill");
                if (tbmcs != null &&
                    tbmcs?.ChosenDamageWeight > 0 &&
                    tbmcs?.FixedLevel == 0 &&
                    finalList.FirstOrDefault(x => x.TalentID == tbmcs.TalentID)?.CurrentLevel == 0)
                {
                    AttentionText += $"- Please consider setting a fixed level for {tbmcs.Name} as the calculation is not finished yet. You gain Multi-Cast Stacks on Level 1, 6 and 11.\n";
                }
                #endregion
            }
        }

        /// <summary>
        /// Sums up the spent sp per branch and sets it to the corresponding sp spent on branch xy property
        /// </summary>
        private void LoadSPSpentOnBranches(List<SPOptSkill> spendingList)
        {
            SPSpentRed = spendingList.Where(x => x.Branch == "BranchRed").Sum(n => n.GetSpSpentAmount());
            SPSpentYellow = spendingList.Where(x => x.Branch == "BranchYellow").Sum(n => n.GetSpSpentAmount());
            SPSpentBlue = spendingList.Where(x => x.Branch == "BranchBlue").Sum(n => n.GetSpSpentAmount());
            SPSpentGreen = spendingList.Where(x => x.Branch == "BranchGreen").Sum(n => n.GetSpSpentAmount());
        }

        /// <summary>
        /// Optimizes. Here lies the main loop
        /// </summary>
        private void Optimize()
        {
            int runId = 0;

            while (AvailableSP > -1 * LocalSettingsORM.SPOverclockAmount)
            {
                if(UnignoredTree == null)
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs("UnignoredTree is null"));
                }

                // check if skills are maxed out
                if (UnignoredTree.Where(x => !x.IsOptimizationDone)?.Count() == 0)
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs("Every skill is maxed out. Nothing to do here"));
                    break;
                }

                OnLogMePlease?.Invoke(this, new InformationEventArgs($"Got {UnignoredTree.Count} items in unignored tree"));

                // Update unlocked state and spent SP on branches
                LoadSPSpentOnBranches(UnignoredTree);
                UpdateUnlockedState(UnignoredTree);

                // copy current upgrade list
                var tmpList = new List<SPOptSkill>();
                foreach (var item in UnignoredTree)
                {
                    tmpList.Add(item.Clone());
                }

                // do efficiency calculation on each item
                SetEfficiencyValues(tmpList);

                var skill = GetSkillToUpgradeFromCalculatedList(tmpList);

                if (skill == null)
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"Skill is null. returning optimization"));
                    break;
                }

                if (string.IsNullOrWhiteSpace(skill?.TalentID))
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"talent id is null or empty. returning"));
                    break;
                }

                // When on creation mode, just create something which the user can afford
                if ((SPOptMode)SelectedConfig.ModeInt == SPOptMode.Creation && skill.UpgradeCost > AvailableSP)
                {
                    AttentionText += $"{string.Format(AppResources.SpOptNextSkillButNotEnoughSp, skill.Name, skill.UpgradeCost)}\n";
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"Creational Mode: UpgradeCost of {skill.UpgradeCost} is greater than available SP {AvailableSP} - Overclock {LocalSettingsORM.SPOverclockAmount}"));
                    break;
                }
                else if (skill.UpgradeCost > AvailableSP - LocalSettingsORM.SPOverclockAmount)
                {
                    AttentionText += $"{string.Format(AppResources.SpOptNextSkillIncludedButNotEnoughSp, skill.Name, skill.UpgradeCost, AvailableSP - LocalSettingsORM.SPOverclockAmount)}\n";
                }

                if (Math.Round(skill.WeightedTotalEfficiency,4) <=1.0000d && skill.UpgradeReason == SpSkillUpgradeReason.Efficiency)
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"Next skill has literally no efficiency boost ({skill.Name}). At this moment the player has {AvailableSP} SP left"));
                    break;
                }

                skill.UpgradeStepId = runId;

                UpgradeSkill(skill);

                runId++;
            }

            if (OnOptiLogMePlease != null)
            {
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs("\nResult List:"));
                foreach (var item in UpgradeCollection)
                {
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"Skill: {item.TalentID} Level: {item.CurrentLevel} Cost: {item.UpgradeCost} Eff: {item.WeightedTotalEfficiency} Reason: {item.Information}"));
                }
            }
        }

        /// <summary>
        /// Upgrades a skill and stuff that depends on that
        /// </summary>
        /// <param name="skill"></param>
        private void UpgradeSkill(SPOptSkill skill)
        {
            UpgradeCollection.Add(skill.Clone()); // insert a copy instead of a reference

            // upgrade the skill and set sp spent and sp available
            SPSpent += skill.UpgradeCost;
            AvailableSP -= skill.UpgradeCost;

            // Upgrade Config skill
            UnignoredTree.Where(x => x.TalentID == skill.TalentID).FirstOrDefault().Upgrade();

            // Add current state to simulation
            AddSnapshotToSimulation();
        }

        /// <summary>
        /// Checks what skill is the best to upgrade in the given list
        /// </summary>
        /// <param name="calculatedList">list of skills on which the efficiency calculation was made</param>
        /// <returns>skill to upgrade. null is shitty</returns>
        private SPOptSkill GetSkillToUpgradeFromCalculatedList(List<SPOptSkill> calculatedList)
        {
            var lstToPickFrom = calculatedList.OrderByDescending(x => x.WeightedTotalEfficiency).ToList();

            // pick most efficient skill to go for
            var nextSkill = IsIgnoredIfNotEnoughSP && ((SPOptMode)SelectedConfig.ModeInt != SPOptMode.Creation)
                ? lstToPickFrom.Where(n => n.UpgradeCost <= AvailableSP + LocalSettingsORM.SPOverclockAmount).FirstOrDefault()
                : lstToPickFrom.FirstOrDefault();
            //var nextSkill = calculatedList.OrderByDescending(x => x.WeightedTotalEfficiency).FirstOrDefault();

            #region Normal case - skill is okay
            // null check
            if (nextSkill == null)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs("got a null item as nextSkill. quitting..."));
                return new SPOptSkill();
            }

            // check if efficiency is 0
            if (nextSkill.WeightedTotalEfficiency == 0)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"got a 0 efficiency item as nextSkill ({nextSkill.TalentID}). quitting..."));
                return new SPOptSkill();
            }

            // if everything is okay, return this skill
            if (nextSkill.IsUnlocked && nextSkill.BranchUpgradeCost == 0)
            {
                nextSkill.Information = nextSkill.FixedLevel > 0 ? AppResources.FixedLevel : AppResources.Efficiency;
                nextSkill.UpgradeReason = nextSkill.FixedLevel > 0 ? SpSkillUpgradeReason.FixedLevel : SpSkillUpgradeReason.Efficiency;
                return nextSkill;
            }
            #endregion

            // find parent to upgrade if this skill is not unlocked
            SPOptSkill finalUpgradeSkill;

            #region Branch Upgrade if all parents are unlocked
            // are the parents unlocked?
            if (nextSkill.NotUnlockedParents.Count == 0)
            {
                // then we have a branch problem. Upgrade most efficient upgradeable skill in branch
                finalUpgradeSkill = calculatedList.OrderByDescending(x => x.WeightedTotalEfficiency).Where(x => x.WeightedTotalEfficiency > 0
                        && x.IsUnlocked
                        && !x.IsOptimizationDone
                        && x.BranchUpgradeCost == 0
                        && x.Branch == nextSkill.Branch).FirstOrDefault();

                if (finalUpgradeSkill != null)
                {
                    finalUpgradeSkill.Information = string.Format(AppResources.BranchUpgradeParent, nextSkill.Name, nextSkill.BranchUpgradeCost);
                    finalUpgradeSkill.UpgradeReason = SpSkillUpgradeReason.ParentUpgrade;
                    return finalUpgradeSkill;
                }
            }
            #endregion

            #region Normal Parent Upgrade
            // get last not unlocked parent (min TIER)
            var lastParent = calculatedList.Where(x => x.TalentID == nextSkill.NotUnlockedParents[nextSkill.NotUnlockedParents.Count - 1]).FirstOrDefault();

            // null check
            if (lastParent == null)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"could not get last parent for ({nextSkill.TalentID}). quitting..."));
                return new SPOptSkill();
            }

            // can you upgrade the parent?
            if (lastParent.IsUnlocked && lastParent.BranchUpgradeCost == 0)
            {
                finalUpgradeSkill = lastParent;
                finalUpgradeSkill.Information = string.Format(AppResources.UpgradeParent, nextSkill.Name);
                finalUpgradeSkill.UpgradeReason = SpSkillUpgradeReason.ParentUpgrade;
                return finalUpgradeSkill;
            }
            #endregion

            #region Branch Grandparent Upgrade
            // if last parent cannot be upgraded - we have branch issues
            // find best unlocked upgradeable skill in branch to spent points
            finalUpgradeSkill = calculatedList.OrderByDescending(x => x.WeightedTotalEfficiency).Where(x => x.WeightedTotalEfficiency > 0
                        && x.IsUnlocked
                        && !x.IsOptimizationDone
                        && x.BranchUpgradeCost == 0
                        && x.Branch == nextSkill.Branch).FirstOrDefault();

            finalUpgradeSkill.Information = string.Format(AppResources.BranchUpgradeGrandParent, nextSkill.Name, lastParent.Name, lastParent.BranchUpgradeCost);
            finalUpgradeSkill.UpgradeReason = SpSkillUpgradeReason.ParentUpgrade;
            return finalUpgradeSkill;
            #endregion
        }

        /// <summary>
        /// This method is responsible for the efficiency calculation
        /// </summary>
        /// <param name="skills">skills to calculate the efficiency for</param>
        private void SetEfficiencyValues(List<SPOptSkill> skills)
        {
            ResetEfficiencyValues(skills);

            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].CalculateEfficiency(_save);
            }

            OnLogMePlease?.Invoke(this, new InformationEventArgs("set efficiency values"));
        }

        /// <summary>
        /// This method calls the function to reset efficiency related values on each item in the given list
        /// </summary>
        /// <param name="skills">list to reset</param>
        private void ResetEfficiencyValues(List<SPOptSkill> skills)
        {
            if (skills == null)
            {
                return;
            }

            foreach (var item in skills)
            {
                item.ResetEfficiencyValues();
            }
        }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor
        /// </summary>
        public SPOptimizer(SPOptConfiguration config, SaveFile save)
        {
            _save = save;
            SelectedConfig = config;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// This method does the optimization run by calling needed methods in the correct order
        /// </summary>
        public void DoRun()
        {
            Optimize();
            DoPostOptimizationChecks();
        }

        /// <summary>
        /// Initializes all Properties for the optimization process
        /// </summary>
        public bool Initialize()
        {
            UpgradeCollection = new List<SPOptSkill>();
            bool equipSetsLoaded = LoadEquipInformation();
            bool spLoaded = LoadSPValues();
            bool skillInit = InitializeSkillLevelsForMode();
            bool skillSetup = InitializeSkillsForConfiguration();
            bool fixedLvl = SetFixedLevels();
            bool treeOkay = CreateUnignoredSkillTree();
            _ = AddSnapshotToSimulation();

            return spLoaded && skillInit && skillSetup && fixedLvl && treeOkay && equipSetsLoaded;
        }
        #endregion

        #region Stuff
        /// <summary>
        /// Adds a snapshot to <see cref="_upgradeSimulation"/> by calling <see cref="CreateUpgradedSkillList"/>
        /// </summary>
        /// <returns></returns>
        private bool AddSnapshotToSimulation()
        {
            try
            {
                UpgradeSimulation.Add(CreateUpgradedSkillList());

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"AddSnapshotToSimulation Error: {ex.Message}"));
                return false;
            }
        }

        /// <summary>
        /// Puts the skill collections back together to a single upgraded list and returns it
        /// </summary>
        /// <returns><see cref="List{T}<see cref="SPOptSkill"/>"/> with result for every skill</returns>
        private List<SPOptSkill> CreateUpgradedSkillList()
        {
            var lst = new List<SPOptSkill>();
            foreach (var item in SelectedConfig.SkillSettings)
            {
                // get skill from currentState (now upgraded collection)
                var upgradedSkill = UnignoredTree.Where(x => x.TalentID == item.SkillId).FirstOrDefault();

                if (upgradedSkill == null)
                {
                    // skill is not in currentState tree. add item
                    lst.Add(item.MySPOptSkill.Clone());
                }
                else
                {
                    // take upgraded skill
                    lst.Add(upgradedSkill.Clone());
                }
            }

            return lst;
        }
        #endregion

        #region E + D
        private void EquipmentHandler_OnProblemHaving(Exception e) 
        {
            try
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs($"EquipmentHandler Error: {e.Message}"));
            }
            catch (Exception) { }
        }


        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void LogCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when <see cref="Initialize"/> has something to log
        /// </summary>
        public event LogCarrier OnLogMePlease;
        public event LogCarrier OnOptiLogMePlease;
        #endregion
    }
}