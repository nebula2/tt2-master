using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Raid
{
    public class RaidAnalysisHeaderData : BindableBase
    {
        private int _totalAttacks;
        public int TotalAttacks { get => _totalAttacks; set => SetProperty(ref _totalAttacks, value); }

        private int _amountOfWaves;
        public int AmountOfWaves { get => _amountOfWaves; set => SetProperty(ref _amountOfWaves, value); }

        private double _totalDamage;
        public double TotalDamage { get => _totalDamage; set => SetProperty(ref _totalDamage, value); }

        private double _damagePerAttack;
        public double DamagePerAttack { get => _damagePerAttack; set => SetProperty(ref _damagePerAttack, value); }
        
        private double _totalOverkillAmount;
        public double TotalOverkillAmount { get => _totalOverkillAmount; set => SetProperty(ref _totalOverkillAmount, value); }
        
        private double _totalOverkillPercentage;
        public double TotalOverkillPercentage { get => _totalOverkillPercentage; set => SetProperty(ref _totalOverkillPercentage, value); }

    }
}
