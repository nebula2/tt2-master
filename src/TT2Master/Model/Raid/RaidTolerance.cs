using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using SQLite;

namespace TT2Master.Model.Raid
{
    [Table("RAIDTOLERANCE")]
    public class RaidTolerance : BindableBase
    {
        private string _name;
        [PrimaryKey]
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private bool _isSaved;
        [Ignore]
        public bool IsSaved { get => _isSaved; set => SetProperty(ref _isSaved, value); }

        private string _description;
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        private OverkillCalculationType _overkillType;
        public OverkillCalculationType OverkillType
        {
            get => _overkillType; 
            set
            {
                if (value >= 0) SetProperty(ref _overkillType, value);
            }
        }

        private AttackAmountCalculationType _amountType;
        public AttackAmountCalculationType AmountType
        {
            get => _amountType;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _amountType, value);
                    OnAttackAmountChanged?.Invoke();
                }
            }
        }

        private AverageDamageCalculationType _averageType = AverageDamageCalculationType.Absolute;
        public AverageDamageCalculationType AverageType
        {
            get => _averageType;
            set
            {
                if (value >= 0) SetProperty(ref _averageType, value);
            }
        }

        private double _overkillTolerance;
        public double OverkillTolerance
        {
            get => _overkillTolerance;
            set
            {
                if (value >= 0) SetProperty(ref _overkillTolerance, value);
            }
        }

        private double _amountTolerance;
        public double AmountTolerance
        {
            get => _amountTolerance;
            set
            {
                if (value >= 0) SetProperty(ref _amountTolerance, value);
            }
        }

        private double _AverageTolerance;
        public double AverageTolerance
        {
            get => _AverageTolerance;
            set
            {
                if (value >= 0) SetProperty(ref _AverageTolerance, value);
            }
        }

        #region E + D
        public delegate void AttackAmountChangedCarrier();
        /// <summary>
        /// Occures when the enemy name has been changed
        /// </summary>
        public event AttackAmountChangedCarrier OnAttackAmountChanged;
        #endregion
    }
}
