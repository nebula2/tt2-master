using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Prism.Mvvm;
using SQLite;
using TT2Master.Shared.Models;

namespace TT2Master.Model.SP
{
    /// <summary>
    /// Configuration for SP Optimizer
    /// </summary>
    [Table("SPOPTCONFIGURATION")]
    public class SPOptConfiguration : BindableBase
    {
        #region Properties
        private string _name;
        /// <summary>
        /// Name given by the user as Identifier
        /// </summary>
        [PrimaryKey]
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    SetProperty(ref _name, value);

                    UpdateChilds(value);
                }
            }
        }

        private int _modeInt;
        /// <summary>
        /// Optimization mode
        /// </summary>
        public int ModeInt
        {
            get => _modeInt;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _modeInt, value);
                    ModeString = ((SPOptMode)value).ToString().TranslatedString();
                }
            }
        }

        private int _damageSourceInt;
        /// <summary>
        /// Damage source
        /// </summary>
        public int DamageSourceInt
        {
            get => _damageSourceInt;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _damageSourceInt, value);
                    DamageSourceString = ((SPOptDamageSource)value).ToString().TranslatedString();
                    OnDmgSourceChanged?.Invoke();
                }
            }
        }

        private int _goldSourceInt;
        /// <summary>
        /// Gold source
        /// </summary>
        public int GoldSourceInt
        {
            get => _goldSourceInt;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _goldSourceInt, value);
                    GoldSourceString = ((GoldType)value).ToString().TranslatedString();
                    OnGoldSourceChanged?.Invoke();
                }
            }
        }

        private ObservableCollection<SPOptSkillSetting> _skillSettings = new ObservableCollection<SPOptSkillSetting>();
        /// <summary>
        /// Settings for each skill
        /// </summary>
        [Ignore]
        public ObservableCollection<SPOptSkillSetting> SkillSettings
        {
            get => _skillSettings; 
            set
            {
                SetProperty(ref _skillSettings, value);
                UpdateChilds(Name);
            }
        }

        private string _modeString;
        /// <summary>
        /// Optimization mode as string
        /// </summary>
        public string ModeString { get => _modeString; set => SetProperty(ref _modeString, value); }

        private string _damageSourcecString;
        /// <summary>
        /// Damage source as string
        /// </summary>
        public string DamageSourceString { get => _damageSourcecString; set => SetProperty(ref _damageSourcecString, value); }

        private string _goldSourceString;
        /// <summary>
        /// Gold source as string
        /// </summary>
        public string GoldSourceString { get => _goldSourceString; set => SetProperty(ref _goldSourceString, value); }

        private string _description;
        /// <summary>
        /// A description for this
        /// </summary>
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// ctor
        /// </summary>
        public SPOptConfiguration()
        {

        } 
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates child objects so that they can update their parental identifiers and such
        /// </summary>
        /// <param name="name"></param>
        private void UpdateChilds(string name)
        {
            foreach (var item in SkillSettings)
            {
                item.Configuration = name;
            }
        }
        #endregion

        #region E+D
        public delegate void DmgSourceChangeCarrier();
        /// <summary>
        /// Occures when the damage source has been changed
        /// </summary>
        public event DmgSourceChangeCarrier OnDmgSourceChanged;

        public delegate void GoldSourceChangeCarrier();
        /// <summary>
        /// Occures when the gold source has been changed
        /// </summary>
        public event GoldSourceChangeCarrier OnGoldSourceChanged;
        #endregion
    }
}
