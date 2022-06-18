using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Navigation;
using TT2Master.Resources;
using TT2Master.Views.Raid;

namespace TT2Master.Model.Dashboard
{
    [ShortcutAttribute(18, "ClanExportTitle", false)]
    public class ClanMemberExportShortcut : DashboardShortcut, INotifyPropertyChanged
    {
        public override int ID { get; set; } = 18;

        public override string Header { get; set; } = AppResources.ClanExportTitle;

        private bool _hasContent = false;
        public override bool HasContent { get => _hasContent; set => SetProperty(ref _hasContent, value); }

        public override ICommand ItemTappedAction { get; protected set; }
        public override Func<Task> LoadItem { get; set; }

        public ClanMemberExportShortcut(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            Destination = typeof(ClanExportPopupPage).Name;
            Icon = "";

            ItemTappedAction = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync("ClanExportPopupPage");
                Logger.WriteToLogFile($"Navigation Result to ClanExportPopupPage: {(result as Prism.Navigation.NavigationResult).Success} {(result as Prism.Navigation.NavigationResult).Exception}");
            });
        }
    }
}
