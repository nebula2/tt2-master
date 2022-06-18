using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using SQLite;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Raid
{
    /// <summary>
    /// Decribes a raid tier which a user manually creates
    /// will be stored in db
    /// </summary>
    [Table("CLANRAID")]
    public class ClanRaid : BindableBase
    {
        #region Clan raid definition properties
        private int _id;
        [PrimaryKey, AutoIncrement]
        public int ID { get => _id; set => SetProperty(ref _id, value); }

        private DateTime _createdDate;
        public DateTime CreatedDate { get => _createdDate; set => SetProperty(ref _createdDate, value); }

        private int _tier;
        public int Tier
        {
            get => _tier;
            set
            {
                if (value != _tier)
                {
                    SetProperty(ref _tier, value);
                    OnTierChanged?.Invoke();
                }
            }
        }

        private int _level;
        public int Level
        {
            get => _level;
            set
            {
                if (value != _level)
                {
                    SetProperty(ref _level, value);
                    OnLevelChanged?.Invoke();
                }
            }
        }

        private string _description;
        public string Description { get => _description; set => SetProperty(ref _description, value); } 
        #endregion

        #region Helper Properties
        private RaidLevelInfo _levelInfo;
        [Ignore]
        public RaidLevelInfo LevelInfo { get => _levelInfo; set => SetProperty(ref _levelInfo, value); }

        private List<RaidEnemyInfo> _enemyInfos;
        [Ignore]
        public List<RaidEnemyInfo> EnemyInfos { get => _enemyInfos; set => SetProperty(ref _enemyInfos, value); }

        private RaidAreaInfo _areaInfo;
        [Ignore]
        public RaidAreaInfo AreaInfo { get => _areaInfo; set => SetProperty(ref _areaInfo, value); }

        private List<ClanRaidResult> _result;
        [Ignore]
        public List<ClanRaidResult> Result { get => _result; set => SetProperty(ref _result, value); }

        private bool _isSaved;
        [Ignore]
        public bool IsSaved { get => _isSaved; set => SetProperty(ref _isSaved, value); }

        private RaidTolerance _tolerance = new RaidTolerance();
        [Ignore]
        public RaidTolerance Tolerance { get => _tolerance; set => SetProperty(ref _tolerance, value); }

        private List<RaidStrategy> _strategies;
        [Ignore]
        public List<RaidStrategy> Strategies { get => _strategies; set => SetProperty(ref _strategies, value); }
        #endregion

        #region E + D
        public delegate void TierChangedCarrier();
        /// <summary>
        /// Occures when the tier has changed
        /// </summary>
        public event TierChangedCarrier OnTierChanged;

        public delegate void LevelChangedCarrier();
        /// <summary>
        /// Occures when the tier has changed
        /// </summary>
        public event LevelChangedCarrier OnLevelChanged;
        #endregion
    }
}
