using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TT2Master
{
    public class MenuItemGroup : ObservableCollection<MasterPageItem>, INotifyPropertyChanged
    {
        private bool _expanded;
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

        public string Title { get; set; }

        public string TitleWithItemCount => $"{Title} {ItemCount}";

        public string StateIcon => Expanded ? "\uf106" : "\uf107";

        public void InvertExpanded() => Expanded = !Expanded;

        public int ItemCount { get; set; }

        private bool _isEnabled = true;
        public bool IsEnabled 
        { 
            get => _isEnabled; 
            set 
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsEnabled"));
                }
            } 
        }

        public MenuItemGroup(string title, bool expanded = true)
        {
            Title = title;
            Expanded = expanded;
        }

        public MenuItemGroup(string title, bool expanded = true, bool isEnabled = true)
        {
            Title = title;
            Expanded = expanded;
            IsEnabled = isEnabled;
        }
    }
}
