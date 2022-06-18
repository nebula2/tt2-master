using Plugin.Clipboard;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Clan;
using TT2Master.Resources;

namespace TT2Master
{
    public class TournamentResultPopupViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<TournamentResultMember> _members = new ObservableCollection<TournamentResultMember>();
        public ObservableCollection<TournamentResultMember> Members { get => _members; set => SetProperty(ref _members, value); }

        public ICommand CancelCommand { get; private set; }

        private IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;
        #endregion

        #region Ctor
        public TournamentResultPopupViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            Title = "Tournament";

            _dialogService = dialogService;
            _navigationService = navigationService;

            CancelCommand = new DelegateCommand(async () => 
            {
                var result = await _navigationService.GoBackAsync();
                Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            } );
        }
        #endregion

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.ContainsKey("item"))
            {
                Members = new ObservableCollection<TournamentResultMember>();
            }
            else
            {
                try
                {
                    Members = (parameters["item"] as TournamentResult).Member;
                }
                catch (Exception)
                {
                }
            }

            base.OnNavigatedTo(parameters);
        }
    }
}