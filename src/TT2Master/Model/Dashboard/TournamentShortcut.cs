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
using TT2Master.Model.Tournament;
using TT2Master.Resources;

namespace TT2Master.Model.Dashboard
{
    [ShortcutAttribute(3, "Tournament", false)]
    public class TournamentShortcut : DashboardShortcut
    {
        public override int ID { get; set; } = 3;

        public override string Header { get; set; } = AppResources.Tournament;

        private bool _hasContent = false;
        public override bool HasContent { get => _hasContent; set => SetProperty(ref _hasContent, value); }

        public override ICommand ItemTappedAction { get; protected set; }
        public override Func<Task> LoadItem { get; set; }

        public TournamentShortcut(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService, dialogService)
        {
            Destination = typeof(ArtOptImageGridPage).Name;

            ItemTappedAction = new DelegateCommand(async () =>
            {
                try
                {
                    if (TournamentHandler.IsPlayerInTournament(true))
                    {
                        var result = await _navigationService.NavigateAsync(NavigationConstants.ChildNavigationPath<DashboardPage, TournamentMembersPage>());
                        Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                    }
                    else
                    {
                        await _dialogService.DisplayAlertAsync(AppResources.Tournament
                            , string.Format(AppResources.NextTournamentInfo
                                , SaveFile.Tournament.PrizeText
                                , SaveFile.Tournament.StartTime.Date.ToString("yyyy.MM.dd")
                                , SaveFile.Tournament.BonusAmount
                                , SaveFile.Tournament.BonusType), AppResources.OKText);
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ShowTournamentExecute Error: {e.Message}");
                }
            });
        }
    }
}
