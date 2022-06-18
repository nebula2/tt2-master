using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Helpers;
using TT2Master.Model.SP;
using TT2Master.Resources;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master
{
    public class SPSplashCalculatorViewModel : ViewModelBase
    {
        #region Properties
        private readonly IPageDialogService _dialogService;

        #region Settings
        private SplashSnapEnum _splashSnap;
        public SplashSnapEnum SplashSnap
        {
            get => _splashSnap;
            set
            {
                if (value != _splashSnap && value >= 0)
                {
                    SetProperty(ref _splashSnap, value, LoadStageValues);
                    LocalSettingsORM.SetSplashSnapSetting(((int)value).ToString());
                }
            }
        }

        private List<string> _splashSnaps = Enum.GetNames(typeof(SplashSnapEnum)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Splash Snap choices
        /// </summary>
        public List<string> SplashSnaps { get => _splashSnaps; set => SetProperty(ref _splashSnaps, value); }

        private SplashSkillEnum _splashSkill;
        public SplashSkillEnum SplashSkill
        {
            get => _splashSkill;
            set
            {
                if (value != _splashSkill && value >= 0)
                {
                    SetProperty(ref _splashSkill, value, LoadStageValues);
                    LocalSettingsORM.SetSplashSkillSetting(((int)value).ToString());
                }
            }
        }

        private List<string> _splashSkills = Enum.GetNames(typeof(SplashSkillEnum)).Select(x => x.TranslatedString()).ToList<string>();
        /// <summary>
        /// Available Splash Skill choices
        /// </summary>
        public List<string> SplashSkills { get => _splashSkills; set => SetProperty(ref _splashSkills, value); }

        private int _maxStage = (int)App.Save.ThisPlayer.StageMax;
        public int MaxStage
        {
            get => _maxStage;
            set
            {
                if (value != _maxStage)
                {
                    SetProperty(ref _maxStage, value, LoadStageValues);
                }
            }
        }

        public string MaxStageString
        {
            get => MaxStage.ToString();
            set
            {
                int i = JfTypeConverter.ForceInt(value, 1);
                if(i != MaxStage)
                {
                    MaxStage = JfTypeConverter.ForceInt(value, 1);
                    OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
                    LoadStageValues();
                }
            }
        }

        private int _normalTitanCount = FormulaHelper.GetTitanCount((int)App.Save.ThisPlayer.StageMax);
        public int NormalTitanCount { get => _normalTitanCount; set => SetProperty(ref _normalTitanCount, value); }

        private int _passiveSkillReduction;
        public int PassiveSkillReduction { get => _passiveSkillReduction; set => SetProperty(ref _passiveSkillReduction, value); }

        private string _agSetCompleted = AppResources.NoText;
        public string AGSetCompleted { get => _agSetCompleted; set => SetProperty(ref _agSetCompleted, value); }

        private bool _apSetCompleted = false;
        public bool APSetCompleted { get => _apSetCompleted; set => SetProperty(ref _apSetCompleted, value); }

        private int _stagesToSplash;
        public int StagesToSplash { get => _stagesToSplash; set => SetProperty(ref _stagesToSplash, value); }

        private double _apSetBonus = 0;
        public double APSetBonus { get => _apSetBonus; set => SetProperty(ref _apSetBonus, value); }

        private int _baseSkip = 3;
        public int BaseSkip { get => _baseSkip; set => SetProperty(ref _baseSkip, value); }

        private int _arcaneBargainLevel = 0;
        private int _intimidatingPresenceLevel = 0;
        private int _powerSurgeLevel = 0;
        private int _mysticalImpactLevel = 0;
        private int _antiTitanCannonLevel = 0;
        #endregion

        #region Skills
        private Skill _edSkill = new Skill();
        public Skill EDSkill { get => _edSkill; set => SetProperty(ref _edSkill, value); }

        private Skill _arSkill = new Skill();
        [Obsolete("Logic has changed with 3.2")]
        public Skill ARSkill { get => _arSkill; set => SetProperty(ref _arSkill, value); }

        private Skill _lbSkill = new Skill();
        public Skill LBSkill { get => _lbSkill; set => SetProperty(ref _lbSkill, value); }

        private Skill _aaSkill = new Skill();
        public Skill AASkill { get => _aaSkill; set => SetProperty(ref _aaSkill, value); }

        private ActiveSkill _heavenlyStrikeSkill = new ActiveSkill();
        public ActiveSkill HeavenlyStrikeSkill { get => _heavenlyStrikeSkill; set => SetProperty(ref _heavenlyStrikeSkill, value); }

        private string _isEDSkillSkipGood = AppResources.NoText;
        public string IsEDSkillSkipGood { get => _isEDSkillSkipGood; set => SetProperty(ref _isEDSkillSkipGood, value); }

        private string _isARSkillSkipGood = AppResources.NoText;
        public string IsARSkillSkipGood { get => _isARSkillSkipGood; set => SetProperty(ref _isARSkillSkipGood, value); }

        private string _isLBSkillSkipGood = AppResources.NoText;
        public string IsLBSkillSkipGood { get => _isLBSkillSkipGood; set => SetProperty(ref _isLBSkillSkipGood, value); }

        private string _isAASkillSkipGood = AppResources.NoText;
        public string IsAASkillSkipGood { get => _isAASkillSkipGood; set => SetProperty(ref _isAASkillSkipGood, value); }

        private double _edSPSpent = 0;
        public double EDSPSpent { get => _edSPSpent; set => SetProperty(ref _edSPSpent, value); }

        private double _arSPSpent = 0;
        public double ARSPSpent { get => _arSPSpent; set => SetProperty(ref _arSPSpent, value); }

        private int _lbSPSpent = 0;
        public int LBSPSpent { get => _lbSPSpent; set => SetProperty(ref _lbSPSpent, value); }

        private int _aaSPSpent = 0;
        public int AASPSpent { get => _aaSPSpent; set => SetProperty(ref _aaSPSpent, value); }

        private bool _isEDSkillSelected = false;
        public bool IsEDSkillSelected { get => _isEDSkillSelected; set => SetProperty(ref _isEDSkillSelected, value); }

        private bool _isARSkillSelected = false;
        public bool IsARSkillSelected { get => _isARSkillSelected; set => SetProperty(ref _isARSkillSelected, value); }

        private bool _isLBSkillSelected = false;
        public bool IsLBSkillSelected { get => _isLBSkillSelected; set => SetProperty(ref _isLBSkillSelected, value); }

        private bool _isAASkillSelected = false;
        public bool IsAASkillSelected { get => _isAASkillSelected; set => SetProperty(ref _isAASkillSelected, value); }

        #endregion

        #endregion

        #region Ctor
        public SPSplashCalculatorViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;

            Title = AppResources.SPSplashCalculator;

            try
            {
                SplashSnap = (SplashSnapEnum)JfTypeConverter.ForceInt(LocalSettingsORM.GetSplashSnapSetting());
            }
            catch (Exception)
            {
                SplashSnap = SplashSnapEnum.WithoutSnap;
            }

            try
            {
                SplashSkill = (SplashSkillEnum)JfTypeConverter.ForceInt(LocalSettingsORM.GetSplashSkillSetting());
            }
            catch (Exception)
            {
                SplashSkill = SplashSkillEnum.EternalDarkness;
            }
        }
        #endregion

        #region Private Methods
        private void LoadStageValues()
        {
            try
            {
                EquipmentHandler.FillEquipment(App.Save);
                EquipmentHandler.LoadSetInformation(App.Save);

                AGSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "HighTecLightning").Count() > 0 ? AppResources.YesText : AppResources.NoText;
                APSetCompleted = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Platinum").Count() > 0;

                var player = App.Save.ThisClan.ClanMember.Where(x => x.PlayerId == App.Save.ThisPlayer.PlayerId).FirstOrDefault();

                FormulaHelper.LoadPassiveSkillCosts(EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Mech").Count() > 0);
                _arcaneBargainLevel = FormulaHelper.GetArcaneBargainLevel(App.Save.ThisPlayer.DustSpent);
                _intimidatingPresenceLevel = FormulaHelper.GetIntimidatingPresenceLevel(player.TotalSkillPoints);
                _powerSurgeLevel = FormulaHelper.GetPowerSurgeLevel(player.TotalPetLevels);
                _mysticalImpactLevel = FormulaHelper.GetMysticalImpactLevel(player.TitanPoints);
                _antiTitanCannonLevel = FormulaHelper.GetAntiTitanCannonLevel(HelpersFactory.GetTotalMasteriesLevel());

                PassiveSkillReduction = _arcaneBargainLevel + _intimidatingPresenceLevel;

                // AP Set bonus
                APSetBonus = 1;
                if (APSetCompleted)
                {
                    var apSet = EquipmentHandler.EquipmentSets.Where(x => x.Set == "Platinum").FirstOrDefault();
                    int cp = App.Save.CraftingPower;

                    APSetBonus = FormulaHelper.GetEquipmentSetBonus(apSet, cp, 2);
                }

                BaseSkip = AGSetCompleted == AppResources.YesText ? 11 : 3;
                BaseSkip = 0; //Das ist splashing. Hier geht es um skipping

                NormalTitanCount = FormulaHelper.GetTitanCount((int)App.Save.ThisPlayer.StageMax);
                //StagesToSplash = (int)((NormalTitanCount - PassiveSkillReduction - (AGSetCompleted ? 11 : 3)) * apSetBonus) / (SplashSnap == SplashSnapEnum.SingleSnap ? 2 : 1);
                StagesToSplash = (int)(NormalTitanCount - PassiveSkillReduction) / (SplashSnap == SplashSnapEnum.SingleSnap ? 2 : 1);
                if (SplashSnap == SplashSnapEnum.DoubleSnap)
                {
                    StagesToSplash /= 2;
                    StagesToSplash /= 2;
                }

                //Load Skills
                SkillInfoHandler.OnProblemHaving += SkillInfoHandler_OnProblemHaving; ;

                if (!SkillInfoHandler.LoadSkills())
                {
                    OptimizeLogger.WriteToLogFile($"SPSplashCalculator: Could not load Skils");
                }

                OptimizeLogger.WriteToLogFile($"SPOptimizer.ReloadAsync: filling skills");
                SkillInfoHandler.FillSkills(App.Save);

                SkillInfoHandler.OnProblemHaving -= SkillInfoHandler_OnProblemHaving;

                // Load active skills
                ActiveSkillHandler.OnProblemHaving += SkillInfoHandler_OnProblemHaving;
                ActiveSkillHandler.LoadSkills();
                ActiveSkillHandler.OnProblemHaving -= SkillInfoHandler_OnProblemHaving;

                EDSkill = SkillInfoHandler.Skills.Where(x => x.TalentID == "CloneSkillBoost").FirstOrDefault();
                LBSkill = SkillInfoHandler.Skills.Where(x => x.TalentID == "PetQTE").FirstOrDefault();
                AASkill = SkillInfoHandler.Skills.Where(x => x.TalentID == "ClanShipDmg").FirstOrDefault();

                HeavenlyStrikeSkill = ActiveSkillHandler.ActiveSkills.Where(x => x.ActiveSkillID == "BurstDamage").FirstOrDefault();

                //Set the current Level to the value which is required for splashing
                EDSkill.CurrentLevel = GetSplashSkillLevel(EDSkill, _mysticalImpactLevel + _arcaneBargainLevel);
                EDSPSpent = EDSkill.GetSpSpentAmount();
                IsEDSkillSkipGood = EDSkill.SplashEffectAtLevel >= StagesToSplash ? AppResources.YesText : AppResources.NoText;
                IsEDSkillSelected = SplashSkill == SplashSkillEnum.EternalDarkness;

                HeavenlyStrikeSkill.CurrentLevel = GetActiveSkillLevel(HeavenlyStrikeSkill, _mysticalImpactLevel + _arcaneBargainLevel);
                ARSPSpent = HeavenlyStrikeSkill.GetCostSpentAmount();
                IsARSkillSkipGood = HeavenlyStrikeSkill.SplashEffectAtLevel >= StagesToSplash ? AppResources.YesText : AppResources.NoText;
                IsARSkillSelected = SplashSkill == SplashSkillEnum.HeavenlyStrike;

                LBSkill.CurrentLevel = GetSplashSkillLevel(LBSkill, _powerSurgeLevel + _arcaneBargainLevel);
                LBSPSpent = LBSkill.GetSpSpentAmount();
                IsLBSkillSkipGood = LBSkill.SplashEffectAtLevel >= StagesToSplash ? AppResources.YesText : AppResources.NoText;
                IsLBSkillSelected = SplashSkill == SplashSkillEnum.LightningBurst;

                AASkill.CurrentLevel = GetSplashSkillLevel(AASkill, _antiTitanCannonLevel + _arcaneBargainLevel);
                AASPSpent = AASkill.GetSpSpentAmount();
                IsAASkillSkipGood = AASkill.SplashEffectAtLevel >= StagesToSplash ? AppResources.YesText : AppResources.NoText;
                //IsAASkillSelected = SplashSkill == SplashSkillEnum.AerialAssault;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"LoadStageValues Error: {e.Message}");
            }

            OnPropertyChanged(new PropertyChangedEventArgs("EDSkill"));
            OnPropertyChanged(new PropertyChangedEventArgs("HeavenlyStrikeSkill"));
            OnPropertyChanged(new PropertyChangedEventArgs("LBSkill"));
            OnPropertyChanged(new PropertyChangedEventArgs("AASkill"));
        }

        private int GetSplashSkillLevel(Skill skill, int passiveSkillBoost)
        {
            // Splash Effect is located in B

            try
            {
                int skillLevel = 0;
                int possibleSkip = 0;
                for (int i = 0; i < skill.B.Count; i++)
                {
                    possibleSkip = (int)((BaseSkip + passiveSkillBoost + skill.B[i]) * APSetBonus);

                    if (i == skill.B.Count -1)
                    {
                        skillLevel = i;
                        skill.SetSplashEffect(possibleSkip);
                        break;
                    }

                    if (possibleSkip >= StagesToSplash)
                    {
                        skillLevel = i;
                        skill.SetSplashEffect(possibleSkip);
                        break;
                    }
                }

                skill.SetSplashEffect(possibleSkip);
                return skillLevel;

            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Could not Get Splash Skill level: {e.Message}");
                return skill.MaxLevel;
            }
        }

        private int GetActiveSkillLevel(ActiveSkill skill, int passiveSkillBoost)
        {
            // Splash Effect is located in B

            try
            {
                int skillLevel = 0;
                int possibleSkip = 0;
                for (int i = 0; i < skill.SecondaryAmount.Count; i++)
                {
                    possibleSkip = (int)((BaseSkip + passiveSkillBoost + skill.SecondaryAmount[i]) * APSetBonus);

                    if (i == skill.SecondaryAmount.Count - 1)
                    {
                        skillLevel = i;
                        skill.SetSplashEffect(possibleSkip);
                        break;
                    }

                    if (possibleSkip >= StagesToSplash)
                    {
                        skillLevel = i;
                        skill.SetSplashEffect(possibleSkip);
                        break;
                    }
                }

                skill.SetSplashEffect(possibleSkip);
                return skillLevel;

            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Could not Get Splash Skill level: {e.Message}");
                return skill.SecondaryAmount.Count;
            }
        }

        private void SkillInfoHandler_OnProblemHaving(object sender, CustErrorEventArgs e) => Logger.WriteToLogFile($"SPSplashCalculator Skill Handler Error on {sender.ToString()}: {e.MyException.Message}\n{e.MyException.Data}");
        #endregion

        #region Helper

        #endregion

        #region Override
        /// <summary>
        /// When Navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadStageValues();

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}

