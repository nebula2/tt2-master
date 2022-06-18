using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master.Model.SP
{
    /// <summary>
    /// Class to describe an optimizable skill with all needed methods for handling efficiency calculation
    /// </summary>
    public class SPOptSkill : Skill, INotifyPropertyChanged
    {
        #region Properties
        private bool _isHavingSpecialCalculation;
        /// <summary>
        /// Does this Skill have a special kind of calculation?
        /// </summary>
        public bool IsHavingSpecialCalculation { get => _isHavingSpecialCalculation; set => SetProperty(ref _isHavingSpecialCalculation, value); }

        private SPSkillReduction _defaultReduction;
        /// <summary>
        /// The default reductions for this skill
        /// </summary>
        public SPSkillReduction DefaultReduction { get => _defaultReduction; set => SetProperty(ref _defaultReduction, value); }

        #region Optimization-Properties
        private SpSkillUpgradeReason _upgradeReason;
        public SpSkillUpgradeReason UpgradeReason { get => _upgradeReason; set => SetProperty(ref _upgradeReason, value); }

        private int _upgradeStepId;
        /// <summary>
        /// ID of upgrade
        /// </summary>
        public int UpgradeStepId { get => _upgradeStepId; set => SetProperty(ref _upgradeStepId, value); }

        #region Efficiency
        private double _purePrimaryEffectAtLevel;
        /// <summary>
        /// The pure primary unweighted effect at the current level
        /// </summary>
        public double PurePrimaryEffectAtLevel { get => _purePrimaryEffectAtLevel; set => SetProperty(ref _purePrimaryEffectAtLevel, value); }

        private double _pureSecondaryEffectAtLevel;
        /// <summary>
        /// The pure secondary unweighted effect at the current level
        /// </summary>
        public double PureSecondaryEffectAtLevel { get => _pureSecondaryEffectAtLevel; set => SetProperty(ref _pureSecondaryEffectAtLevel, value); }

        private double _purePrimaryEffectNextLevel;
        /// <summary>
        /// The pure primary unweighted effect at the next level
        /// </summary>
        public double PurePrimaryEffectNextLevel { get => _purePrimaryEffectNextLevel; set => SetProperty(ref _purePrimaryEffectNextLevel, value); }

        private double _pureSecondaryEffectNextLevel;
        /// <summary>
        /// The pure secondary unweighted effect at the next level
        /// </summary>
        public double PureSecondaryEffectNextLevel { get => _pureSecondaryEffectNextLevel; set => SetProperty(ref _pureSecondaryEffectNextLevel, value); }

        private double _chosenDamageWeight;
        /// <summary>
        /// The chosen damage weight to calculate the reduction
        /// </summary>
        public double ChosenDamageWeight { get => _chosenDamageWeight; set => SetProperty(ref _chosenDamageWeight, value); }

        private double _chosenGoldWeight;
        /// <summary>
        /// The chosen gold weight to calculate the reduction
        /// </summary>
        public double ChosenGoldWeight { get => _chosenGoldWeight; set => SetProperty(ref _chosenGoldWeight, value); }

        private double _weightedPrimaryEffectAtLevel;
        /// <summary>
        /// The weighted primary effect at the current level
        /// </summary>
        public double WeightedPrimaryEffectAtLevel { get => _weightedPrimaryEffectAtLevel; set => SetProperty(ref _weightedPrimaryEffectAtLevel, value); }

        private double _weightedSecondaryEffectAtLevel;
        /// <summary>
        /// The weighted secondary effect at the current level
        /// </summary>
        public double WeightedSecondaryEffectAtLevel { get => _weightedSecondaryEffectAtLevel; set => SetProperty(ref _weightedSecondaryEffectAtLevel, value); }

        private double _weightedPrimaryEffectNextLevel;
        /// <summary>
        /// The weighted primary effect at the next level
        /// </summary>
        public double WeightedPrimaryEffectNextLevel { get => _weightedPrimaryEffectNextLevel; set => SetProperty(ref _weightedPrimaryEffectNextLevel, value); }

        private double _weightedSecondaryEffectNextLevel;
        /// <summary>
        /// The weighted secondary effect at the next level
        /// </summary>
        public double WeightedSecondaryEffectNextLevel { get => _weightedSecondaryEffectNextLevel; set => SetProperty(ref _weightedSecondaryEffectNextLevel, value); }

        private double _weightedTotalEfficiency;
        /// <summary>
        /// The total weighted efficiency. This value is used to determine the most efficient skill
        /// </summary>
        public double WeightedTotalEfficiency
        {
            get => _weightedTotalEfficiency;
            set => SetProperty(ref _weightedTotalEfficiency, value);
        }

        private double _specialEfficiency;
        /// <summary>
        /// The special efficiency which is valid for non-additive
        /// </summary>
        public double SpecialEfficiency { get => _specialEfficiency; set => SetProperty(ref _specialEfficiency, value); }
        #endregion

        #region Accessibility
        private int _fixedLevel;
        /// <summary>
        /// Fixed level
        /// </summary>
        public int FixedLevel { get => _fixedLevel; set => SetProperty(ref _fixedLevel, value); }

        private bool _isUnlocked;
        /// <summary>
        /// True if this skill is fully unlocked (SP requirements, parent unlocks and MS requirements met)
        /// </summary>
        public bool IsUnlocked { get => _isUnlocked; set => SetProperty(ref _isUnlocked, value); }

        private bool _isIgnored;
        /// <summary>
        /// True if this skill is ignored for optimization
        /// </summary>
        public bool IsIgnored { get => _isIgnored; set => SetProperty(ref _isIgnored, value); }

        private bool _isAvailable;
        /// <summary>
        /// True, if MS requirements are met
        /// </summary>
        public bool IsAvailable { get => _isAvailable; set => SetProperty(ref _isAvailable, value); }

        private bool _isSpReqFulfilled;
        /// <summary>
        /// True if there are enough Sp spent in the corresponding branch
        /// </summary>
        public bool IsSpReqFulfilled { get => _isSpReqFulfilled; set => SetProperty(ref _isSpReqFulfilled, value); }

        private int _equipSetMaxLevelIncreaseAmount = 0;
        private bool _isSetMaxLevelIncreaseSet = false;
        #endregion

        private List<string> _notUnlockedParents = new List<string>();
        /// <summary>
        /// List of not unlocked parents. Where the string represents the skill-id of the parent skill
        /// </summary>
        public List<string> NotUnlockedParents { get => _notUnlockedParents; set => SetProperty(ref _notUnlockedParents, value); }

        private int _upgradeCost;
        /// <summary>
        /// The SP cost to upgrade this skill to the next level
        /// </summary>
        //public int UpgradeCost { get => _upgradeCost; set => SetProperty(ref _upgradeCost, value); }
        public int UpgradeCost 
        { 
            get 
            { 
                _upgradeCost = GetUpgradeCost();
                return _upgradeCost;

            }
            set
            {
                SetProperty(ref _upgradeCost, GetUpgradeCost());
                //_upgradeCost=> SetProperty(ref _upgradeCost, value);
            }
        } 

        private int _parentUpgradeCost;
        /// <summary>
        /// Costs to upgrade parents to at least level 1 (including SP spent in branch)
        /// </summary>
        public int ParentUpgradeCost { get => _parentUpgradeCost; set => SetProperty(ref _parentUpgradeCost, value); }

        private int _branchUpgradeCost;
        /// <summary>
        /// The amount of SP needed in the branch needed to unlock this skill (what is left of the basic requirement)
        /// </summary>
        public int BranchUpgradeCost { get => _branchUpgradeCost; set => SetProperty(ref _branchUpgradeCost, value); }

        private bool _isOptimizationDone;
        /// <summary>
        /// Is the optimization process done?
        /// This can be if you enter a fixed level or this skill is at max level
        /// </summary>
        public bool IsOptimizationDone { get => _isOptimizationDone; set => SetProperty(ref _isOptimizationDone, value); }

        private string _information;
        /// <summary>
        /// An informative text for the user. Could provide information about why this upgrade is made or a reason for the level
        /// </summary>
        public string Information { get => _information; set => SetProperty(ref _information, value); }

        private bool _isSpecialCalculationEnabled;
        /// <summary>
        /// Is the special calculation for this skill enabled?
        /// </summary>
        public bool IsSpecialCalculationEnabled { get => _isSpecialCalculationEnabled; set => SetProperty(ref _isSpecialCalculationEnabled, value); }

        private SPOptDamageSource _dmgSource;
        /// <summary>
        /// Is the special calculation for this skill enabled?
        /// </summary>
        public SPOptDamageSource DmgSource { get => _dmgSource; set => SetProperty(ref _dmgSource, value); }

        private GoldType _goldSource;
        /// <summary>
        /// Is the special calculation for this skill enabled?
        /// </summary>
        public GoldType GoldSource { get => _goldSource; set => SetProperty(ref _goldSource, value); }
        #endregion
        #endregion

        #region Ctor
        /// <summary>
        /// ctor
        /// </summary>
        public SPOptSkill() { }
        #endregion

        #region Public Methods
        public SPOptSkill Clone()
        {
            return new SPOptSkill()
            {
                A = A,
                B = B,
                C = C,
                Cost = Cost,
                BonusTypeA = BonusTypeA,
                BonusTypeB = BonusTypeB,
                BonusTypeC = BonusTypeC,
                BonusTypeD = BonusTypeD,
                BonusAmountD = BonusAmountD,
                Branch = Branch,
                BranchUpgradeCost = BranchUpgradeCost,
                ChosenDamageWeight = ChosenDamageWeight,
                ChosenGoldWeight = ChosenGoldWeight,
                CurrentLevel = CurrentLevel,
                DefaultReduction = DefaultReduction,
                DmgSource = DmgSource,
                FixedLevel = FixedLevel,
                GoldSource = GoldSource,
                Information = Information,
                IsAvailable = IsAvailable,
                IsHavingSpecialCalculation = IsHavingSpecialCalculation,
                IsIgnored = IsIgnored,
                IsOptimizationDone = IsOptimizationDone,
                IsSpecialCalculationEnabled = IsSpecialCalculationEnabled,
                IsSpReqFulfilled = IsSpReqFulfilled,
                IsUnlocked = IsUnlocked,
                MaxLevel = MaxLevel,
                Name = Name,
                Note = Note,
                NotUnlockedParents = NotUnlockedParents,
                ParentUpgradeCost = ParentUpgradeCost,
                PurePrimaryEffectAtLevel = PurePrimaryEffectAtLevel,
                PurePrimaryEffectNextLevel = PurePrimaryEffectNextLevel,
                PureSecondaryEffectAtLevel = PureSecondaryEffectAtLevel,
                PureSecondaryEffectNextLevel = PureSecondaryEffectNextLevel,
                S = S,
                Slot = Slot,
                SpecialEfficiency = SpecialEfficiency,
                SplashEffectAtLevel = SplashEffectAtLevel,
                SPReq = SPReq,
                TalentID = TalentID,
                Class = Class,
                TalentReq = TalentReq,
                TierNum = TierNum,
                Tier = Tier,
                UpgradeCost = UpgradeCost,
                UpgradeStepId = UpgradeStepId,
                WeightedPrimaryEffectAtLevel = WeightedPrimaryEffectAtLevel,
                WeightedPrimaryEffectNextLevel = WeightedPrimaryEffectNextLevel,
                WeightedSecondaryEffectAtLevel = WeightedSecondaryEffectAtLevel,
                WeightedSecondaryEffectNextLevel = WeightedSecondaryEffectNextLevel,
                WeightedTotalEfficiency = WeightedTotalEfficiency,
                SetLevelIncreaseAmount = SetLevelIncreaseAmount,
                UpgradeReason = UpgradeReason,
            };
        }

        public void SetCompleteUpgradeCost()
        {
            // CumCost ist zu krass, dann haben die skills gar keine effizienz mehr. mal gucken, was hier sinnvoll ist
            // UpgradeCost = GetUpgradeCost() + ParentUpgradeCost + Math.Max(BranchUpgradeCost - ParentUpgradeCost, 0);
            UpgradeCost = GetUpgradeCost();
        }

        /// <summary>
        /// Upgrades the skill to the next level
        /// </summary>
        public void Upgrade()
        {
            SetFixedLevelOverloadStuff();

            if (CurrentLevel == 0)
            {
                CurrentLevel += 1 + _equipSetMaxLevelIncreaseAmount;

                if (SetLevelIncreaseAmount == 0)
                {
                    SetLevelIncreaseAmount = _equipSetMaxLevelIncreaseAmount;
                }
            }
            else
            {
                CurrentLevel += CurrentLevel < MaxLevel ? 1 : 0;

                if (SetLevelIncreaseAmount == 0)
                {
                    SetLevelIncreaseAmount = _equipSetMaxLevelIncreaseAmount;
                }
            }

            CheckIfOptimizationIsDone();
        }

        /// <summary>
        /// Resets efficiency values to 0
        /// </summary>
        public void ResetEfficiencyValues()
        {
            PurePrimaryEffectAtLevel = 0;
            PureSecondaryEffectAtLevel = 0;
            PurePrimaryEffectNextLevel = 0;
            PureSecondaryEffectNextLevel = 0;

            WeightedPrimaryEffectAtLevel = 0;
            WeightedSecondaryEffectAtLevel = 0;
            WeightedPrimaryEffectNextLevel = 0;
            WeightedSecondaryEffectNextLevel = 0;

            SpecialEfficiency = 0;

            WeightedTotalEfficiency = 0;
        }

        public void CalculateEfficiency(SaveFile save)
        {
            SetFixedLevelOverloadStuff();

            // return if done
            if (IsOptimizationDone)
            {
                ResetEfficiencyValues();
                return;
            }
            
            // fixed level
            if(FixedLevel > 0)
            {
                WeightedTotalEfficiency = GetFixedLevelEfficiency();
                return;
            }

            bool isSpecialCalculationValid = false;
            var setting = SkillInfoHandler.SpecialCalculationSettings.Where(x => x.TalentId == TalentID).FirstOrDefault();
            MethodInfo method = null;

            // special calculation
            if (IsHavingSpecialCalculation && IsSpecialCalculationEnabled && setting != null)
            {
                var t = GetType();
                method = t.GetMethod(setting.MethodName, BindingFlags.Instance | BindingFlags.NonPublic);

                if(method != null)
                {
                    isSpecialCalculationValid = true;
                }
            }

            // replacive special calculation
            if (isSpecialCalculationValid && !setting.IsAdditive)
            {
                method.Invoke(this, new object[] { save });
                return;
            }
            // set additive special efficiency
            else if (isSpecialCalculationValid)
            {
                method.Invoke(this, new object[] { save });
            }

            // normal calculation. Some skills have seperate calculations
            switch (TalentID)
            {
                case "HelperInspiredWeaken":
                    CalculateHelperInspiredWeakenEff();
                    break;
                case "TapDmgFromHelpers":
                    CalculateTapDmgFromHelpersEff();
                    break;
                case "PetQTE":
                    CalculatePetQTEEff();
                    break;
                case "HelperDmgSkillBoost":
                    CalculateHelperDmgSkillBoostEff();
                    break;
                case "CloneDmg":
                    CalculateCloneDmgEff();
                    break;
                case "MultiMonsters":
                    CalculateMultiMonstersEff();
                    break;
                case "PetOfflineDmg":
                    CalculatePetOfflineDmgEff();
                    break;
                case "InactiveClanShip":
                    CalculateInactiveClanShipEff();
                    break;
                case "OfflineCloneDmg":
                    CalculateOfflineCloneDmgEff();
                    break;
                case "ClanShipStun":
                    CalculateClanShipStunEff();
                    break;
                case "CritSkillBoost":
                    CalculateCritSkillBoostEff();
                    break;
                case "AutoAdvance":
                    CalculateAutoAdvanceEff();
                    break;
                case "ChestGold":
                    CalculateChestGoldEff();
                    break;
                case "Fairy":
                    CalculateFairyEff();
                    break;
                case "MidasSkillBoost":
                    CalculateMidasSkillBoostEff();
                    break;
                case "PetGoldQTE":
                    CalculatePetGoldQTEEff();
                    break;
                case "HeavyStrikes":
                    CalculateHeavyStrikesEff();
                    break;
                #region 3.8
                case "UltraDagger":
                    CalculateUltraDaggerEff();
                    break;
                case "StrokeOfLuck":
                    CalculateStrokeOfLuckEff();
                    break;
                case "PoisonedBlade":
                    CalculatePoisonedBladeEff();
                    break;
                case "Cloaking":
                    CalculateCloakingEff();
                    break;
                case "ForbiddenContract":
                    CalculateForbiddenContractEff();
                    break;
                case "SoulBlade":
                case "HelperBoostMultiCastSkill":
                case "TapBoostMultiCastSkill":
                case "BurstDamageMultiCastSkill":
                case "ShadowCloneMultiCastSkill":
                    CalculateMultiCastSkills();
                    break;
                case "GuidedBlade":
                    CalculateGuidedBladeEff();
                    break;
                case "HelperDmgQTE":
                    CalculateHelperDmgQTEEff();
                    break;
                case "Frenzy":
                    CalculateFrenzyEfficiency();
                    break;
                case "BossDmgQTE":
                    CalculateBossDmgQTEEfficiency();
                    break;
                case "ManaMonster":
                    CalculateManaMonsterEfficiency();
                    break;                
                case "BossTimer":
                    CalculateBossTimerEfficiency();
                    break;
                #endregion
                default:
                    DoNormalEfficiencyCalculation();
                    break;
            }
        }

        #region Special Calculation Methods
#pragma warning disable IDE0051 // Remove unused private members
                               /// <summary>
                               /// Skip-Skill calculation for Shadow Clone builds
                               /// </summary>
        private void CalculateCloneSkillBoostEfficiency(SaveFile save)
#pragma warning restore IDE0051 // Remove unused private members
        {
            // Get value from skip skill calculator and set SpecialEfficiency
            if(DmgSource != SPOptDamageSource.ShadowClone)
            {
                SpecialEfficiency = 0;
                return;
            }

            var skill = SPSkipHandler.CalculateCloneSkillBoostEfficiency(save);

            // set special efficiency
            SpecialEfficiency = CurrentLevel >= skill.CurrentLevel ? 0 : 88888;
        }

#pragma warning disable IDE0051 // Remove unused private members
                               /// <summary>
                               /// Skip-Skill calculation for Heavenly Strike builds
                               /// </summary>

#pragma warning disable IDE0051 // Remove unused private members
                               /// <summary>
                               /// Skip-Skill calculation for Pet-Builds
                               /// </summary>
        private void CalculatePetQTEEfficiency(SaveFile save)
#pragma warning restore IDE0051 // Remove unused private members
        {
            // Get value from skip skill calculator and set SpecialEfficiency
            if(DmgSource != SPOptDamageSource.Pet)
            {
                SpecialEfficiency = 0;
                return;
            }

            var skill = SPSkipHandler.CalculatePetQTEEfficiency(save);

            SpecialEfficiency = CurrentLevel >= skill.CurrentLevel ? 0 : 88888;
        }

#pragma warning disable IDE0051 // Remove unused private members
                               /// <summary>
                               /// Special Calculation of Tactical Insight
                               /// </summary>
        private void CalculateHelperBoostEfficiency(SaveFile save)
#pragma warning restore IDE0051 // Remove unused private members
        {
            // do special replacive efficiency calculation
            // Pure effects
            PurePrimaryEffectAtLevel = GetPrimaryEffectForLevel(CurrentLevel);
            PureSecondaryEffectAtLevel = GetSecondaryEffectForLevel(CurrentLevel);
            PurePrimaryEffectNextLevel = GetPrimaryEffectForLevel(CurrentLevel + 1);
            PureSecondaryEffectNextLevel = GetSecondaryEffectForLevel(CurrentLevel + 1);

            double dmgWeight = 0;

            switch (DmgSource)
            {
                case SPOptDamageSource.ClanShip:
                    dmgWeight = 108;
                    break;
                case SPOptDamageSource.ShadowClone:
                    dmgWeight = 109.2;
                    break;
                case SPOptDamageSource.HeavenlyStrike:
                    dmgWeight = 111.5;
                    break;
                case SPOptDamageSource.Pet:
                    dmgWeight = 111.5;
                    break;
                default:
                    break;
            }

            double goldWeight = 0;

            switch (GoldSource)
            {
                case GoldType.pHoM:
                    goldWeight = 56;
                    break;
                case GoldType.BossGold:
                    goldWeight = 56;
                    break;
                case GoldType.ChestersonGold:
                    goldWeight = 65;
                    break;
                case GoldType.NormalMobGold:
                    goldWeight = 56;
                    break;
                case GoldType.FairyGold:
                    goldWeight = 83;
                    break;
                default:
                    break;
            }

            double weight = Math.Max((dmgWeight + goldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow((PurePrimaryEffectNextLevel + 1) / (PurePrimaryEffectAtLevel + 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow((PureSecondaryEffectNextLevel + 1) / (PureSecondaryEffectAtLevel + 1), weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;

        }
        #endregion
        #endregion

        #region private methods
        private void CheckIfOptimizationIsDone() 
        { 
            IsOptimizationDone = (FixedLevel > 0 && FixedLevel == CurrentLevel) || CurrentLevel == MaxLevel;
        }
        public void EnsureMaxLevelFromSets()
        {
            SetFixedLevelOverloadStuff();

            MaxLevel += _equipSetMaxLevelIncreaseAmount;

            if (CurrentLevel > 0)
            {
                CurrentLevel += _equipSetMaxLevelIncreaseAmount;
                SetLevelIncreaseAmount = _equipSetMaxLevelIncreaseAmount;
            }

            CheckIfOptimizationIsDone();
        }
        private void SetFixedLevelOverloadStuff()
        {
            if (_isSetMaxLevelIncreaseSet)
            {
                return;
            }

            switch (TalentID)
            {
                // YELLOW
                case "HelperDmgQTE":
                case "ClanShipStun":
                case "HelperBoostMultiCastSkill":
                    var set = EquipmentHandler.EquipmentSets.Where(x => x.Set == "ElectricWarlord").FirstOrDefault();
                    _equipSetMaxLevelIncreaseAmount = set == null ? 0 : set.Completed ? (int)set.Amount1 : 0;
                    break;
                // GREEN
                case "SoulBlade":
                case "ForbiddenContract":
                case "GuidedBlade":
                    var setRogue = EquipmentHandler.EquipmentSets.Where(x => x.Set == "EarthRogue").FirstOrDefault();
                    _equipSetMaxLevelIncreaseAmount = setRogue == null ? 0 : setRogue.Completed ? (int)setRogue.Amount1 : 0;
                    break;
                // RED
                case "Frenzy":
                case "BossDmgQTE":
                case "TapBoostMultiCastSkill":
                    var warriorSet = EquipmentHandler.EquipmentSets.Where(x => x.Set == "FireKnight").FirstOrDefault();
                    _equipSetMaxLevelIncreaseAmount = warriorSet == null ? 0 : warriorSet.Completed ? (int)warriorSet.Amount1 : 0;
                    break;
                // BLUE
                case "ManaMonster":
                case "CritSkillBoost":
                case "BossTimer":
                    var mongoSet = EquipmentHandler.EquipmentSets.Where(x => x.Set == "WaterSorcerer").FirstOrDefault();
                    _equipSetMaxLevelIncreaseAmount = mongoSet == null ? 0 : mongoSet.Completed ? (int)mongoSet.Amount1 : 0;
                    break;
                default:
                    break;
            }

            _isSetMaxLevelIncreaseSet = true;
        }

        private void SetPureValues()
        {
            int levelToCalculateWith = CurrentLevel == 0 ? CurrentLevel + _equipSetMaxLevelIncreaseAmount : CurrentLevel;

            // Pure effects
            PurePrimaryEffectAtLevel = GetPrimaryEffectForLevel(levelToCalculateWith);
            PureSecondaryEffectAtLevel = GetSecondaryEffectForLevel(levelToCalculateWith);
            PurePrimaryEffectNextLevel = GetPrimaryEffectForLevel(levelToCalculateWith + 1);
            PureSecondaryEffectNextLevel = GetSecondaryEffectForLevel(levelToCalculateWith + 1);
        }

        /// <summary>
        /// Calculates Efficiency which fits for most skills
        /// </summary>
        private void DoNormalEfficiencyCalculation()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1), weight / UpgradeCost);

            WeightedTotalEfficiency = Math.Pow(WeightedPrimaryEffectNextLevel, (DefaultReduction.IsSecondaryRelevant ? WeightedSecondaryEffectNextLevel : 1)) + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for ManaMonster
        /// </summary>
        private void CalculateManaMonsterEfficiency()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1), weight / UpgradeCost);

            WeightedTotalEfficiency = Math.Pow(WeightedPrimaryEffectNextLevel, (DefaultReduction.IsSecondaryRelevant ? WeightedSecondaryEffectNextLevel : 1)) + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for BossTimer
        /// </summary>
        private void CalculateBossTimerEfficiency()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1), weight / UpgradeCost);

            WeightedTotalEfficiency = Math.Pow(WeightedPrimaryEffectNextLevel, (DefaultReduction.IsSecondaryRelevant ? WeightedSecondaryEffectNextLevel : 1)) + SpecialEfficiency;
        }
        
        /// <summary>
        /// Calculates Efficiency for frenzy - barbaric fury
        /// </summary>
        private void CalculateFrenzyEfficiency()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1), weight / UpgradeCost);

            WeightedTotalEfficiency = Math.Pow(WeightedPrimaryEffectNextLevel, (DefaultReduction.IsSecondaryRelevant ? WeightedSecondaryEffectNextLevel : 1)) + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for BossDmgQTE - barbaric fury
        /// </summary>
        private void CalculateBossDmgQTEEfficiency()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1), weight / UpgradeCost);

            WeightedTotalEfficiency = Math.Pow(WeightedPrimaryEffectNextLevel, (DefaultReduction.IsSecondaryRelevant ? WeightedSecondaryEffectNextLevel : 1)) + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Cleaving Strike
        /// </summary>
        private void CalculateHeavyStrikesEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);
            double secWeight = 0;
            double critChance = 0.5;

            switch (DmgSource)
            {
                case SPOptDamageSource.ClanShip:
                    secWeight = 0.5;
                    break;
                case SPOptDamageSource.ShadowClone:
                    secWeight = 1.5;
                    break;
                case SPOptDamageSource.HeavenlyStrike:
                    secWeight = 0;
                    break;
                case SPOptDamageSource.Pet:
                    secWeight = 1;
                    break;
                default:
                    break;
            }

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow((PurePrimaryEffectNextLevel + 0.15) / (Math.Max(PurePrimaryEffectAtLevel, 1) + 0.15), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow((critChance + PureSecondaryEffectNextLevel) / (critChance + PureSecondaryEffectAtLevel), secWeight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel * WeightedSecondaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Heart of Midas 
        /// </summary>
        private void CalculatePetGoldQTEEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);
            double goldWeight = GoldSource == GoldType.pHoM ? Reductions.GoldToDamageReduction : 0;

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow((75 - PureSecondaryEffectAtLevel) / (75 - PureSecondaryEffectNextLevel), goldWeight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel * WeightedSecondaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Midas Ultimate
        /// </summary>
        private void CalculateMidasSkillBoostEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);
            double goldWeight = (GoldSource == GoldType.FairyGold ? 1 : ChosenGoldWeight) * Reductions.GoldToDamageReduction;

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1), goldWeight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel * WeightedSecondaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Fairy Charm
        /// </summary>
        private void CalculateFairyEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(1 + PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            WeightedPrimaryEffectNextLevel = Math.Pow((1 + PurePrimaryEffectNextLevel) / (1 + PurePrimaryEffectAtLevel), weight / UpgradeCost);

            double modsecNext = 120 - PureSecondaryEffectNextLevel;
            double modsecNow =  120 - PureSecondaryEffectAtLevel;
            WeightedSecondaryEffectNextLevel = Math.Pow(modsecNow / modsecNext, weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel * WeightedSecondaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Spoils of War
        /// </summary>
        private void CalculateChestGoldEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = 0;

            double modPrimary = PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1);
            double modSecondary = (0.7 + PureSecondaryEffectNextLevel) / (0.7 + PureSecondaryEffectAtLevel);

            WeightedPrimaryEffectNextLevel = Math.Pow(modPrimary * modSecondary, weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for silent march
        /// </summary>
        private void CalculateAutoAdvanceEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            WeightedPrimaryEffectNextLevel = CurrentLevel == 0
                ? Math.Pow(1.6, weight / UpgradeCost)
                : Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);

            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Lighting Strike Efficiency
        /// </summary>
        private void CalculateCritSkillBoostEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(GetLightningStrikeReduction(PurePrimaryEffectAtLevel, PureSecondaryEffectAtLevel), weight);
            WeightedSecondaryEffectAtLevel = 0;

            double nextLsRed = GetLightningStrikeReduction(PurePrimaryEffectNextLevel, PureSecondaryEffectNextLevel);
            double currLsRed = GetLightningStrikeReduction(PurePrimaryEffectAtLevel, PureSecondaryEffectAtLevel);

            WeightedPrimaryEffectNextLevel = Math.Pow(currLsRed / nextLsRed, weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Returns the Lightning Strike reduction for given efficiency values. 
        /// </summary>
        /// <param name="primaryEffect"></param>
        /// <param name="secondaryEffect"></param>
        /// <returns>Life reduction on titan</returns>
        private double GetLightningStrikeReduction(double primaryEffect, double secondaryEffect)
        {
            double result = 1 - primaryEffect;

            for (int i = 1; i < 32; i++)
            {
                double eff = primaryEffect * Math.Pow(secondaryEffect, i);
                result *= 1 - eff;
            }

            return result;
        }

        /// <summary>
        /// Calculates Anchoring Shot Efficiency
        /// </summary>
        private void CalculateClanShipStunEff()
        {
            #region Set logic
            SetPureValues();

            // if max level reached from set then there is nothing left to do
            if (CurrentLevel >= MaxLevel)
            {
                WeightedTotalEfficiency = 1;
                return;
            } 
            #endregion

            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            WeightedSecondaryEffectNextLevel = 0;

            if(CurrentLevel <= _equipSetMaxLevelIncreaseAmount)
            {
                double effMultiplier = DmgSource == SPOptDamageSource.Pet || DmgSource == SPOptDamageSource.HeavenlyStrike ? 1.15 : 1.2;

                WeightedPrimaryEffectNextLevel = Math.Pow(effMultiplier, weight);
            }
            else
            {
                WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);
            }

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Shadow Assassin Efficiency
        /// </summary>
        private void CalculateOfflineCloneDmgEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            if (CurrentLevel == 0)
            {
                WeightedPrimaryEffectNextLevel = Math.Pow(100, (DmgSource == SPOptDamageSource.ShadowClone ? 0.6 : 0) * weight) / 2;
                WeightedSecondaryEffectNextLevel = 0;

                WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
                return;
            }

            WeightedPrimaryEffectNextLevel = PurePrimaryEffectAtLevel == 0 ? Math.Pow(1.5, weight) : Math.Pow(PurePrimaryEffectNextLevel / PurePrimaryEffectNextLevel, weight / UpgradeCost); ;
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / PureSecondaryEffectAtLevel, weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel * WeightedSecondaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Ghost Ship Efficiency
        /// </summary>
        private void CalculateInactiveClanShipEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            if (CurrentLevel == 0)
            {
                WeightedPrimaryEffectNextLevel = Math.Pow(100, (DmgSource == SPOptDamageSource.ClanShip ? 0.5 : 0) * weight) / 2;
                WeightedSecondaryEffectNextLevel = 0;

                WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
                return;
            }

            WeightedPrimaryEffectNextLevel = PurePrimaryEffectAtLevel == 0 ? Math.Pow(1.5, weight) : Math.Pow(PurePrimaryEffectNextLevel / PurePrimaryEffectNextLevel, weight / UpgradeCost); ;
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / PureSecondaryEffectAtLevel, weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel * WeightedSecondaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Twilight's Veil Efficiency
        /// </summary>
        private void CalculatePetOfflineDmgEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            if (CurrentLevel == 0)
            {
                WeightedPrimaryEffectNextLevel = Math.Pow(100, (DmgSource == SPOptDamageSource.Pet ? 0.7 : 0) * weight) / 2;
                WeightedSecondaryEffectNextLevel = 0;

                WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
                return;
            }

            WeightedPrimaryEffectNextLevel = PurePrimaryEffectAtLevel == 0 ? Math.Pow(1.5, weight) : Math.Pow(PurePrimaryEffectNextLevel / PurePrimaryEffectNextLevel, weight / UpgradeCost); ;
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / PureSecondaryEffectAtLevel, weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Ambush Efficiency
        /// </summary>
        private void CalculateMultiMonstersEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);
            double multiChance = 0.25;

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            double modNextLvl = ((6 + PurePrimaryEffectNextLevel) / 2 * (multiChance + PureSecondaryEffectNextLevel) + 1);
            double modCurrLvl = ((6 + PurePrimaryEffectAtLevel) / 2 * (multiChance + PureSecondaryEffectNextLevel) + 1);

            WeightedPrimaryEffectNextLevel = Math.Pow(modNextLvl / modCurrLvl, weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Phantom Vengeance
        /// </summary>
        private void CalculateCloneDmgEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);

            double priEffMod = PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1);
            double secEffMod = (4 * PureSecondaryEffectNextLevel) / (4 * PureSecondaryEffectAtLevel);

            WeightedPrimaryEffectNextLevel = Math.Pow(priEffMod * secEffMod, weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow(PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1), weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Heroic Might
        /// </summary>
        private void CalculateHelperDmgSkillBoostEff()
        {
            //Get primary effect of Inspired Shot to know eff boost
            var iw = SPOptimizer.UnignoredTree.Where(x => x.TalentID == "HelperInspiredWeaken").FirstOrDefault();
            double iwEff = 1;

            if (iw != null)
            {
                iwEff = iw.GetPrimaryEffectForLevel(iw.CurrentLevel);
            }

            SetPureValues();

            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = Math.Pow(Math.Max(PureSecondaryEffectAtLevel, 1), weight);
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / Math.Max(PurePrimaryEffectAtLevel, 1), weight / UpgradeCost);

            double priEffModifier = PureSecondaryEffectNextLevel / Math.Max(PureSecondaryEffectAtLevel, 1);
            double secEffModifier = (1 + iwEff * 8 * PureSecondaryEffectNextLevel / 0.14) / (1 + iwEff * 8 * PureSecondaryEffectAtLevel / 0.14);

            WeightedSecondaryEffectNextLevel = Math.Pow(priEffModifier * secEffModifier, weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }

        /// <summary>
        /// Calculates Efficiency for Lightning Burst
        /// </summary>
        private void CalculatePetQTEEff()
        {
            //Get secondary effect of Barbaric Fury to know how many extra attacks you get
            var hm = SPOptimizer.UnignoredTree.Where(x => x.TalentID == "Frenzy").FirstOrDefault();
            double frencyEff = 0;
            double petAtk = 0;

            if (hm != null)
            {
                frencyEff = Math.Max(hm.GetSecondaryEffectForLevel(hm.CurrentLevel), 1); // set min of 1 so that you do not divide by 0
            }

            petAtk = ((2 / 3) + 10 + frencyEff) / 20;

            SetPureValues();

            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);


            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = 0;

            double modifiedMultiplierOne = (20 * petAtk + PurePrimaryEffectNextLevel) / (20 * petAtk + PurePrimaryEffectAtLevel) * 0.5;
            double modifiedMultiplierTwo = (15 * petAtk + PurePrimaryEffectNextLevel + 3) / (15 * petAtk + PurePrimaryEffectAtLevel + 3) * 0.5;

            WeightedPrimaryEffectNextLevel = Math.Pow(modifiedMultiplierOne + modifiedMultiplierTwo, weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = CurrentLevel == 0 ? Math.Pow(1.25, weight) : WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for Chivalric order
        /// </summary>
        private void CalculateTapDmgFromHelpersEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(PurePrimaryEffectAtLevel + 0.0043, weight);
            WeightedSecondaryEffectAtLevel = 0;

            double effMod = CurrentLevel == 0 ? 1.5 : (PurePrimaryEffectNextLevel + 0.0043) / (PurePrimaryEffectAtLevel + 0.0043);

            WeightedPrimaryEffectNextLevel = Math.Pow(effMod, weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }


        /// <summary>
        /// Calculates Efficiency for Astral Awakening
        /// </summary>
        private void CalculateHelperDmgQTEEff()
        {
            SetPureValues();
            // if max level reached from set then there is nothing left to do
            if (CurrentLevel >= MaxLevel)
            {
                WeightedTotalEfficiency = 1;
                return;
            }

            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);
                
            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / PurePrimaryEffectAtLevel, 5 * weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
        }

        #region 3.8 skills
        /// <summary>
        /// Calculates Efficiency for Summon Dagger
        /// </summary>
        private void CalculateUltraDaggerEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);
            if (CurrentLevel == 0)
            {
                WeightedPrimaryEffectNextLevel = Math.Pow(1.5, weight / UpgradeCost);
                WeightedSecondaryEffectNextLevel = 0;

                WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel + SpecialEfficiency;
                return;
            }
            else
            {
                WeightedTotalEfficiency = 1;
            }
        }

        /// <summary>
        /// Calculates Efficiency for StrokeOfLuck
        /// </summary>
        private void CalculateStrokeOfLuckEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);
            if (CurrentLevel == 0)
            {
                WeightedPrimaryEffectNextLevel = Math.Pow(5, weight / UpgradeCost);

                WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
                return;
            }

            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / (PurePrimaryEffectAtLevel == 0 ? 1 : PurePrimaryEffectAtLevel), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = Math.Pow((999 * PureSecondaryEffectNextLevel + 1) / (999 * PureSecondaryEffectAtLevel + 1), Reductions.GoldToDamageReduction / UpgradeCost);

            // total
            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel * WeightedSecondaryEffectNextLevel + SpecialEfficiency;
        }

        /// <summary>
        /// Calculates Efficiency for PoisonedBlade
        /// </summary>
        private void CalculatePoisonedBladeEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            WeightedPrimaryEffectNextLevel = Math.Pow((1 + PurePrimaryEffectNextLevel) / (1 + PurePrimaryEffectAtLevel), weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }

        /// <summary>
        /// Calculates Efficiency for Cloaking
        /// </summary>
        private void CalculateCloakingEff()
        {
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / (PurePrimaryEffectAtLevel == 0 ? 1 : PurePrimaryEffectAtLevel), weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }

        /// <summary>
        /// Calculates Efficiency for ForbiddenContract
        /// </summary>
        private void CalculateForbiddenContractEff()
        {
            #region Set logic
            SetPureValues();

            // if max level reached from set then there is nothing left to do
            if (CurrentLevel >= MaxLevel)
            {
                WeightedTotalEfficiency = 1;
                return;
            }
            #endregion

            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / (PurePrimaryEffectAtLevel == 0 ? 1 : PurePrimaryEffectAtLevel), weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }

        /// <summary>
        /// Calculates Efficiency for multi cast skills
        /// </summary>
        private void CalculateMultiCastSkills()
        {
            #region Set logic
            SetPureValues();

            // if max level reached from set then there is nothing left to do
            if (CurrentLevel >= MaxLevel)
            {
                WeightedTotalEfficiency = 1;
                return;
            }
            #endregion

            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // multiply for multi cast
            PurePrimaryEffectAtLevel *= PureSecondaryEffectAtLevel;
            PurePrimaryEffectNextLevel *= PureSecondaryEffectNextLevel;

            WeightedPrimaryEffectNextLevel = Math.Pow(PurePrimaryEffectNextLevel / (PurePrimaryEffectAtLevel == 0 ? 1 : PurePrimaryEffectAtLevel), weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }

        /// <summary>
        /// Calculates Efficiency for GuidedBlade
        /// </summary>
        private void CalculateGuidedBladeEff()
        {
            #region Set logic
            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // if max level reached from set then there is nothing left to do
            if (CurrentLevel >= MaxLevel)
            {
                WeightedTotalEfficiency = 1;
                return;
            }
            #endregion

            WeightedPrimaryEffectNextLevel = Math.Pow(
                PurePrimaryEffectNextLevel * PureSecondaryEffectNextLevel / (PurePrimaryEffectAtLevel == 0 ? 1 : PurePrimaryEffectAtLevel * PureSecondaryEffectAtLevel)
                , weight / UpgradeCost);

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }
        #endregion

        /// <summary>
        /// Calculates Efficiency for Inspired Shot
        /// </summary>
        private void CalculateHelperInspiredWeakenEff()
        {
            //Get secondary effect of Heroic Might to know how many helpers are affected by inspiration
            var hm = SPOptimizer.UnignoredTree.Where(x => x.TalentID == "HelperDmgSkillBoost").FirstOrDefault();
            double hmSecEff = 1;

            if(hm != null)
            {
                hmSecEff = hm.GetSecondaryEffectForLevel(hm.CurrentLevel);
            }

            SetPureValues();
            double weight = Math.Max((ChosenDamageWeight + ChosenGoldWeight * Reductions.GoldToDamageReduction), 0.000001);

            // weighted effects
            WeightedPrimaryEffectAtLevel = Math.Pow(Math.Max(PurePrimaryEffectAtLevel, 1), weight);
            WeightedSecondaryEffectAtLevel = 0;
            WeightedPrimaryEffectNextLevel = Math.Pow((1 + 8 * PurePrimaryEffectNextLevel * hmSecEff / 0.14) / Math.Max((1 + 8 * PurePrimaryEffectAtLevel * hmSecEff / 0.14), 1), weight / UpgradeCost);
            WeightedSecondaryEffectNextLevel = 0;

            WeightedTotalEfficiency = WeightedPrimaryEffectNextLevel;
        }

        /// <summary>
        /// Returns the Efficiency for a skill with a fixed level
        /// </summary>
        /// <returns></returns>
        private double GetFixedLevelEfficiency() => CurrentLevel < FixedLevel ? 99999 : 0.000000;
        #endregion

        #region INPC
        /// <summary>
        /// Gets fired when a property changes it's value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Set a property and call <see cref="PropertyChanged"/> to notify view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (value == null ? field != null : !value.Equals(field))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
        #endregion
    }
}