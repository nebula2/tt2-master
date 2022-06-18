using Prism.Mvvm;
using SQLite;

namespace TT2Master.Model.SP
{
    /// <summary>
    /// Describes a Skill optimizer setting
    /// </summary>
    [Table("SPOPTSKILLSETTING")]
    public class SPOptSkillSetting : BindableBase
    {
        #region Properties
        /// <summary>
        /// Identifier
        /// </summary>
        [PrimaryKey]
        public string Identifier { get; set; }

        private string _configuration;
        /// <summary>
        /// Configuration identifier
        /// </summary>
        public string Configuration
        {
            get => _configuration;
            set
            {
                if (value != _configuration)
                {
                    SetProperty(ref _configuration, value);
                    SetIdentifier(value, SkillId);
                }
            }
        }

        private string _skillId;
        /// <summary>
        /// Talent id
        /// </summary>
        public string SkillId
        {
            get => _skillId;
            set
            {
                if (value != _skillId)
                {
                    SetProperty(ref _skillId, value);
                    SetIdentifier(Configuration, value);
                }
            }
        }

        private bool _isHavingSpecialCalculation;
        /// <summary>
        /// Does this skill have a special calculation method?
        /// </summary>
        [Ignore]
        public bool IsHavingSpecialCalculation
        {
            get => _isHavingSpecialCalculation;
            set
            {
                SetProperty(ref _isHavingSpecialCalculation, value);
                IsSpecialOptionShown = value & IsTolerated;
            }
        }

        private bool _isSpecialCalculationEnabled;
        /// <summary>
        /// Is the special calculation method enabled?
        /// </summary>
        public bool IsSpecialCalculationEnabled
        {
            get => _isSpecialCalculationEnabled;
            set
            {
                SetProperty(ref _isSpecialCalculationEnabled, value);
                if (value && IsHavingSpecialCalculation)
                {
                    IsDmgOptionShown = false;
                    IsGoldOptionShown = false;
                    return;
                }
                else
                {
                    IsDmgOptionShown = IsDmgRelevant && IsTolerated;
                    IsGoldOptionShown = IsGoldRelevant && IsTolerated;
                }
            }
        }

        private double _defaultDmgReduction;
        /// <summary>
        /// The default damage reduction
        /// </summary>
        [Ignore]
        public double DefaultDmgReduction { get => _defaultDmgReduction; set => SetProperty(ref _defaultDmgReduction, value); }

        private double _defaultGoldReduction;
        /// <summary>
        /// The default gold reduction
        /// </summary>
        [Ignore]
        public double DefaultGoldReduction { get => _defaultGoldReduction; set => SetProperty(ref _defaultGoldReduction, value); }

        private double _customDmgReduction;
        /// <summary>
        /// The custom damage reduction set by the user
        /// </summary>
        public double CustomDmgReduction
        {
            get => _customDmgReduction;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _customDmgReduction, value);
                }
            }
        }

        private double _customGoldReduction;
        /// <summary>
        /// The custom gold reduction set by the user
        /// </summary>
        public double CustomGoldReduction
        {
            get => _customGoldReduction;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _customGoldReduction, value);
                }
            }
        }

        private int _fixedLevel;
        /// <summary>
        /// The fixed level for this talent (0 for infinity)
        /// </summary>
        public int FixedLevel
        {
            get => _fixedLevel;
            set
            {
                if (value < 0)
                {
                    return;
                }

                SetProperty(ref _fixedLevel, value);
            }
        }

        private bool _isDmgRelevant;
        /// <summary>
        /// Is this talent relevant for damage output?
        /// </summary>
        [Ignore]
        public bool IsDmgRelevant
        {
            get => _isDmgRelevant;
            set
            {
                SetProperty(ref _isDmgRelevant, value);
                if (IsSpecialCalculationEnabled && IsHavingSpecialCalculation)
                {
                    IsDmgOptionShown = false;
                }
                else
                {
                    IsDmgOptionShown = value && IsTolerated;
                }
            }
        }

        private bool _isGoldRelevant;
        /// <summary>
        /// Is this talent relevant for gold output?
        /// </summary>
        [Ignore]
        public bool IsGoldRelevant
        {
            get => _isGoldRelevant;
            set
            {
                SetProperty(ref _isGoldRelevant, value);
                if (IsSpecialCalculationEnabled && IsHavingSpecialCalculation)
                {
                    IsGoldOptionShown = false;
                }
                else
                {
                    IsGoldOptionShown = value && IsTolerated;
                }
            }
        }

        private bool _isTolerated = true;
        /// <summary>
        /// If not this is ignored
        /// </summary>
        public bool IsTolerated
        {
            get => _isTolerated;
            set
            {
                SetProperty(ref _isTolerated, value);

                IsSpecialOptionShown = IsHavingSpecialCalculation && value;
                IsFixedLevelShown = value;

                if (IsSpecialCalculationEnabled && IsHavingSpecialCalculation)
                {
                    IsDmgOptionShown = false;
                    IsGoldOptionShown = false;
                    return;
                }
                else
                {
                    IsDmgOptionShown = IsDmgRelevant && value;
                    IsGoldOptionShown = IsGoldRelevant && value;
                }
            }
        }

        private SPOptSkill _mySPOptSkill = new SPOptSkill();
        /// <summary>
        /// The corresponding <see cref="SPOptSkill"/> object
        /// </summary>
        [Ignore]
        public SPOptSkill MySPOptSkill { get => _mySPOptSkill; set => SetProperty(ref _mySPOptSkill, value); }

        private bool _isDmgOptionShown;
        /// <summary>
        /// Is the damage option shown in UI
        /// </summary>
        [Ignore]
        public bool IsDmgOptionShown { get => _isDmgOptionShown; set => SetProperty(ref _isDmgOptionShown, value); }

        private bool _isGoldOptionShown;
        /// <summary>
        /// Is the gold option shown in UI
        /// </summary>
        [Ignore]
        public bool IsGoldOptionShown { get => _isGoldOptionShown; set => SetProperty(ref _isGoldOptionShown, value); }

        private bool _isSpecialOptionShown;
        /// <summary>
        /// Is the special option shown in UI
        /// </summary>
        [Ignore]
        public bool IsSpecialOptionShown { get => _isSpecialOptionShown; set => SetProperty(ref _isSpecialOptionShown, value); }

        private bool _isFixedLevelShown = true;
        /// <summary>
        /// Is the fixed-level option shown in UI
        /// </summary>
        [Ignore]
        public bool IsFixedLevelShown { get => _isFixedLevelShown; set => SetProperty(ref _isFixedLevelShown, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public SPOptSkillSetting()
        {

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the identifier from configuration id and skill id
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="skill"></param>
        private void SetIdentifier(string configuration, string skill) => Identifier = $"{(configuration ?? "?")},{(skill ?? "?")}";
        #endregion

        #region public Methods
        /// <summary>
        /// Reverts custom values to default values
        /// </summary>
        public void UndoCustomValues()
        {
            IsTolerated = true;
            CustomDmgReduction = 0;
            CustomGoldReduction = 0;
            IsSpecialCalculationEnabled = true;
            FixedLevel = 0;
        }

        /// <summary>
        /// Returns a copy of this object
        /// </summary>
        /// <returns></returns>
        public SPOptSkillSetting Copy()
        {
            return new SPOptSkillSetting()
            {
                Configuration = this.Configuration,
                CustomDmgReduction = this.CustomDmgReduction,
                CustomGoldReduction = this.CustomGoldReduction,
                DefaultDmgReduction = this.DefaultDmgReduction,
                DefaultGoldReduction = this.DefaultGoldReduction,
                FixedLevel = this.FixedLevel,
                Identifier = this.Identifier,
                IsDmgOptionShown = this.IsDmgOptionShown,
                IsDmgRelevant = this.IsDmgRelevant,
                IsFixedLevelShown = this.IsFixedLevelShown,
                IsGoldOptionShown = this.IsGoldOptionShown,
                IsGoldRelevant = this.IsGoldRelevant,
                IsHavingSpecialCalculation = this.IsHavingSpecialCalculation,
                IsSpecialCalculationEnabled = this.IsSpecialCalculationEnabled,
                IsSpecialOptionShown = this.IsSpecialOptionShown,
                IsTolerated = this.IsTolerated,
                MySPOptSkill = this.MySPOptSkill,
                SkillId = this.SkillId,
            };
        }
        #endregion
    }
}