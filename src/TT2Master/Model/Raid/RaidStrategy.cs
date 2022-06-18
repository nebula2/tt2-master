using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using SQLite;

namespace TT2Master.Model.Raid
{
    [Table("RAIDSTRATEGY")]
    public class RaidStrategy : BindableBase
    {
        private string _name;
        [PrimaryKey]
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private string _enemyId;
        public string EnemyId
        {
            get => _enemyId; 
            set
            {
                if(value != _enemyId)
                {
                    SetProperty(ref _enemyId, value);
                }
            }
        }

        private bool _isSaved;
        [Ignore]
        public bool IsSaved { get => _isSaved; set => SetProperty(ref _isSaved, value); }

        private string _enemyName;
        [Ignore]
        public string EnemyName
        {
            get => _enemyName; set
            {
                if(value != _enemyName)
                {
                    SetProperty(ref _enemyName, value);
                    OnEnemyNameChanged?.Invoke(value);
                }
            }
        }

        private string _description;
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        private EnemyAttackType _head;
        public EnemyAttackType Head
        {
            get => _head; 
            set
            {
                if(value >= 0) SetProperty(ref _head, value);
            }
        }

        private EnemyAttackType _torso;
        public EnemyAttackType Torso
        {
            get => _torso; set
            {
                if (value >= 0) SetProperty(ref _torso, value);
            }
        }

        private EnemyAttackType _leftShoulder;
        public EnemyAttackType LeftShoulder
        {
            get => _leftShoulder; set
            {
                if (value >= 0) SetProperty(ref _leftShoulder, value);
            }
        }

        private EnemyAttackType _rightShoulder;
        public EnemyAttackType RightShoulder
        {
            get => _rightShoulder; set
            {
                if (value >= 0) SetProperty(ref _rightShoulder, value);
            }
        }

        private EnemyAttackType _leftHand;
        public EnemyAttackType LeftHand
        {
            get => _leftHand; set
            {
                if (value >= 0) SetProperty(ref _leftHand, value);
            }
        }

        private EnemyAttackType _rightHand;
        public EnemyAttackType RightHand
        {
            get => _rightHand; set
            {
                if (value >= 0) SetProperty(ref _rightHand, value);
            }
        }

        private EnemyAttackType _leftLeg;
        public EnemyAttackType LeftLeg
        {
            get => _leftLeg; set
            {
                if (value >= 0) SetProperty(ref _leftLeg, value);
            }
        }

        private EnemyAttackType _rightLeg;
        public EnemyAttackType RightLeg
        {
            get => _rightLeg; set
            {
                if (value >= 0) SetProperty(ref _rightLeg, value);
            }
        }

        [Ignore]
        public string ImagePath => GetImagePath();

        private string GetImagePath() => $"{EnemyName}";

        #region E + D
        public delegate void EnemyNameChangeCarrier(string newValue);
        /// <summary>
        /// Occures when the enemy name has been changed
        /// </summary>
        public event EnemyNameChangeCarrier OnEnemyNameChanged;
        #endregion
    }
}
