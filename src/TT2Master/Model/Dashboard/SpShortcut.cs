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
using TT2Master.Resources;

namespace TT2Master.Model.Dashboard
{
    [ShortcutAttribute(1, "SP", true)]
    public class SpShortcut : DashboardShortcut
    {
        public override int ID { get; set; } = 1;

        public override string Header { get; set; } = AppResources.SkillPoints;

        private bool _hasContent = true;
        public override bool HasContent { get => _hasContent; set => SetProperty(ref _hasContent, value); }

        public override ICommand ItemTappedAction { get; protected set; }
        public override Func<Task> LoadItem { get; set; }

        public SpShortcut(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            LoadItem = async () =>
            {
                try
                {
                    SkillInfoHandler.FillSkills(App.Save);
                    Content = $"{SaveFile.SPReceived.ToString()}";
                }
                catch (Exception e)
                {
                    Content = "?";
                    Logger.WriteToLogFile($"ERROR SpShortcut:{e.Message}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            };

            ItemTappedAction = new DelegateCommand(() => { });
        }
    }
}
