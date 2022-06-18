using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TT2Master.Shared.Raids;

namespace TT2Master.ViewModels.Raid
{
    public class RaidSeedEnemyViewModel : ViewModelBase
    {
        private readonly RaidSeed _seed;
        private readonly RaidSeedEnemy _enemy;

        private int _tier;
        public int Tier { get => _tier; set => SetProperty(ref _tier, value); }
        
        private int _level;
        public int Level { get => _level; set => SetProperty(ref _level, value); }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private string _enemyId;
        public string EnemyId { get => _enemyId; set => SetProperty(ref _enemyId, value); }
        
        private double _totalHp;
        public double TotalHp { get => _totalHp; set => SetProperty(ref _totalHp, value); }
        
        private double _bodyHead;
        public double BodyHead { get => _bodyHead; set => SetProperty(ref _bodyHead, value); }
        
        private double _bodyChestUpper;
        public double BodyChestUpper { get => _bodyChestUpper; set => SetProperty(ref _bodyChestUpper, value); }
        
        private double _bodyArmUpperRight;
        public double BodyArmUpperRight { get => _bodyArmUpperRight; set => SetProperty(ref _bodyArmUpperRight, value); }
        
        private double _bodyArmUpperLeft;
        public double BodyArmUpperLeft { get => _bodyArmUpperLeft; set => SetProperty(ref _bodyArmUpperLeft, value); }
        
        private double _bodyLegUpperRight;
        public double BodyLegUpperRight { get => _bodyLegUpperRight; set => SetProperty(ref _bodyLegUpperRight, value); }

        private double _bodyLegUpperLeft;
        public double BodyLegUpperLeft { get => _bodyLegUpperLeft; set => SetProperty(ref _bodyLegUpperLeft, value); }

        private double _bodyHandRight;
        public double BodyHandRight { get => _bodyHandRight; set => SetProperty(ref _bodyHandRight, value); }

        private double _bodyHandLeft;
        public double BodyHandLeft { get => _bodyHandLeft; set => SetProperty(ref _bodyHandLeft, value); }

        private string _areaBuffBonusType;
        public string AreaBuffBonusType { get => _areaBuffBonusType; set => SetProperty(ref _areaBuffBonusType, value); }

        private double _areaBuffBonusAmount;
        public double AreaBuffBonusAmount { get => _areaBuffBonusAmount; set => SetProperty(ref _areaBuffBonusAmount, value); }

        private string _areaDebuffBonusType;
        public string AreaDebuffBonusType { get => _areaDebuffBonusType; set => SetProperty(ref _areaDebuffBonusType, value); }

        private double _areaDebuffBonusAmount;
        public double AreaDebuffBonusAmount { get => _areaDebuffBonusAmount; set => SetProperty(ref _areaDebuffBonusAmount, value); }

        public string SpawnSequenceText => string.Join(", ", _seed.SpawnSequence ?? new string[] { });

        public RaidSeedEnemyViewModel(RaidSeed seed, RaidSeedEnemy enemy) : base(null)
        {
            _seed = seed;
            _enemy = enemy;

            Init();
        }

        private void Init()
        {
            Tier = int.Parse(_seed.Tier);
            Level = int.Parse(_seed.Level);
            Name = _enemy.Name;
            EnemyId = _enemy.EnemyId;
            TotalHp = _enemy.TotalHp;

            BodyHead = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyHead").TotalHp;
            BodyChestUpper = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyChestUpper").TotalHp;
            BodyArmUpperRight = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyArmUpperRight").TotalHp;
            BodyArmUpperLeft = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyArmUpperLeft").TotalHp;
            BodyLegUpperRight = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyLegUpperRight").TotalHp;
            BodyLegUpperLeft = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyLegUpperLeft").TotalHp;
            BodyHandRight = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyHandRight").TotalHp;
            BodyHandLeft = _enemy.Parts.FirstOrDefault(x => x.PartId == "BodyHandLeft").TotalHp;

            var ab = _seed.AreaBuffs?.FirstOrDefault();
            AreaBuffBonusType = ab?.BonusType;
            AreaBuffBonusAmount = ab?.BonusAmount ?? 0;

            var db = _enemy.AreaDebuffs?.FirstOrDefault();
            AreaDebuffBonusType = db?.BonusType;
            AreaDebuffBonusAmount = db?.BonusAmount ?? 0;
        }

        public string GetShortString() => $"{Tier}-{Level}";
        public string GetLongString() => $"{Tier}-{Level} {(AreaBuffBonusAmount == 0 ? "" : $"({AreaBuffBonusAmount} {AreaBuffBonusType})")}: {SpawnSequenceText}";
    }
}
