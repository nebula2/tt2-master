using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// Describes an Artifact in optimization-context
    /// </summary>
    public class ArtifactToOptimize : Artifact, ICloneable, INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// Current Rank
        /// </summary>
        public string Rank { get; private set; }

        /// <summary>
        /// Amount to Level
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Amount to click the upgrade button in TT2
        /// </summary>
        public int ClickAmount { get; set; }

        /// <summary>
        /// Amount deliminated
        /// </summary>
        public string AmountString => AmountToString(); //needs no OnPropertyChanged cause list is rebuild on reload

        /// <summary>
        /// Current Efficiency
        /// </summary>
        public double Efficiency { get; set; }

        /// <summary>
        /// Active Ratio for weight
        /// </summary>
        public double ActiveRatio { get; set; }

        private double _inPercent;

        /// <summary>
        /// Percentage of current Relics
        /// </summary>
        public double InPercent
        {
            get => _inPercent;
            set => _inPercent = value;
        }

        private double _currPercentage;
        /// <summary>
        /// Current Percentage
        /// </summary>
        public double CurrPercentage { get => _currPercentage; set => _currPercentage = value > 100 ? 100 : value; }

        public bool IsClickEnabled { get; set; } = true;

        public string UpgradeReason { get; set; }

        /// <summary>
        /// How many times is this artifact winner in a row?
        /// </summary>
        public int SubstitutionCount { get; set; }

        private bool _isTaggedAsDone;

        public bool IsTaggedAsDone { get => _isTaggedAsDone; set => SetProperty(ref _isTaggedAsDone, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Base ctor
        /// </summary>
        public ArtifactToOptimize() : base()
        {

        }
        
        /// <summary>
        /// 
        /// </summary>
        public ArtifactToOptimize(int rank) : base()
        {
            SetRank(rank);
        }

        /// <summary>
        /// Ctor from <see cref="Artifact"/> (base type)
        /// </summary>
        /// <param name="art"></param>
        public ArtifactToOptimize(Artifact art) : base()
        {
            ID = art.ID;
            SortIndex = art.SortIndex;
            GeneralTier = art.GeneralTier;
            InternalName = art.InternalName;
            Level = art.Level;
            RelicsSpent = art.RelicsSpent;
            Name = art.Name;
            Description = art.Description;
            MaxLevel = art.MaxLevel;
            Effect = art.Effect;
            EffectPerLevel = art.EffectPerLevel;
            GrowthExpo = art.GrowthExpo;
            DamageBonus = art.DamageBonus;
            CostCoefficient = art.CostCoefficient;
            CostExpo = art.CostExpo;
        }
        #endregion

        #region Public Methods
        public void SetRank(int value) => Rank = $"{(value < 10 ? "0" : "")}{value.ToString()}.";

        /// <summary>
        /// Sets the new values
        /// </summary>
        public void SetNewValues()
        {
            Level += Amount;
            RelicsSpent += CostAtLevel(0);

            // try to increase values at artifact handler
            try
            {
                var art = ArtifactHandler.Artifacts.Where(x => x.ID == ID).FirstOrDefault();
                art.Level = Level;
                art.RelicsSpent = RelicsSpent;
            }
            catch (Exception)
            { }
        }

        public void AddValuesFromOtherArtifact(ArtifactToOptimize artifact)
        {
            Level += artifact.Amount;
            RelicsSpent += CostAtLevel(0);
            ClickAmount += artifact.ClickAmount;
            InPercent += artifact.InPercent;
            Amount += artifact.Amount;
            SubstitutionCount++;
        }

        public override string ToString()
        {
            return $"ID {ID}\tSortIndex {SortIndex}\nLevel {Level}\tRelicsSpent {RelicsSpent}\nName {Name}\tDescription {Description}\nMaxLevel {MaxLevel}\tEffect {Effect}\nEffectPerLevel {EffectPerLevel }\tGrowthExpo {GrowthExpo}"
            + $"DamageBonus {DamageBonus}\tCostCoefficient {CostCoefficient}\tCostExpo {CostExpo}";
        }

        public object Clone() => (ArtifactToOptimize)MemberwiseClone();
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds commas or dots in thousands place for a number
        /// </summary>
        /// <returns></returns>
        private string AmountToString() => Amount.ToString();
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