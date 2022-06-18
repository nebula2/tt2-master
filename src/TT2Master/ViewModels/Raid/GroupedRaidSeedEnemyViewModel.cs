using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace TT2Master.ViewModels.Raid
{
    public class GroupedRaidSeedEnemyViewModel : ObservableCollection<RaidSeedEnemyViewModel>, INotifyPropertyChanged
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }

        private bool _expanded = false;
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Expanded"));
                    OnPropertyChanged(new PropertyChangedEventArgs("StateIcon"));
                }
            }
        }

        public string StateIcon => Expanded ? "\uf106" : "\uf107";

        public void InvertExpanded() => Expanded = !Expanded;

        public GroupedRaidSeedEnemyViewModel(string longName, string shortName, bool expanded = false)
        {
            LongName = longName;
            ShortName = shortName;
            Expanded = expanded;
        }
    }
}
