using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TT2Master.Model.Dashboard
{
    public abstract class DashboardShortcut : BindableBase
    {
        public INavigationService _navigationService;
        public IPageDialogService _dialogService;

        public abstract int ID { get; set; }

        public string Destination { get; set; }

        public abstract string Header { get; set; }

        private string _content;
        public string Content { get => _content; set => SetProperty(ref _content , value); }

        public abstract bool HasContent { get; set; }

        public string Icon { get; set; }

        public bool HasIcon => !string.IsNullOrWhiteSpace(Icon);

        public string TextColor { get; set; } = "#FFFFFF";

        public abstract ICommand ItemTappedAction { get; protected set; }

        public abstract Func<Task> LoadItem { get; set; }

        public DashboardShortcut(INavigationService navigationService, IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
        }
    }
}
