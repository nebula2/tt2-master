using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Navigation;
using TT2Master.Resources;

namespace TT2Master.Model.Dashboard
{
    [ShortcutAttribute(0, "ArtifactHeader", true)]
    public class ArtifactsShortcut : DashboardShortcut
    {
        public override int ID { get; set; } = 0;
        public override string Header { get; set; } = AppResources.ArtifactHeader;
        public override ICommand ItemTappedAction { get; protected set; }
        public override Func<Task> LoadItem { get; set; }

        private bool _hasContent = true;
        public override bool HasContent { get => _hasContent; set => SetProperty(ref _hasContent, value); }

        public ArtifactsShortcut(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            Destination = typeof(ArtifactOverviewPage).Name;

            LoadItem = async () =>
            {
                try
                {
                    ArtifactHandler.FillArtifacts(App.Save);
                    Content = $"{ArtifactHandler.Artifacts.Where(x => x.Level > 0).Count()}/{ArtifactHandler.Artifacts.Count}";
                }
                catch (Exception e)
                {
                    Content = "?";
                    Logger.WriteToLogFile($"ERROR ArtifactsShortcut:{e.Message}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                }
            };

            ItemTappedAction = new DelegateCommand(async () =>
            {
                var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<DashboardPage, ArtifactOverviewPage>());

                Logger.WriteToLogFile($"Navigation Result to {nameof(ArtifactOverviewPage)}: {(result as Prism.Navigation.NavigationResult).Success} {(result as Prism.Navigation.NavigationResult).Exception}");
            });
        }
    }
}
