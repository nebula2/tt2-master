using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Assets;
using TT2Master.Model.Reporting;
using TT2Master.Resources;
using System.Reflection;
using TT2Master.Shared.Helper;

namespace TT2Master.ViewModels.Reporting
{
    /// <summary>
    /// Settings
    /// </summary>
    public class ClanMemberRaidStatsReportViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Member for displaying dialogs
        /// </summary>
        private readonly IPageDialogService _dialogService;

        INavigationService _navigationService;


        private ObservableCollection<Player> _member = new ObservableCollection<Player>();
        public ObservableCollection<Player> Member { get => _member; set => SetProperty(ref _member, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public ClanMemberRaidStatsReportViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;


            Title = AppResources.MemberRaidStatsReport;
        }
        #endregion

        #region Command Methods

        #endregion

        #region Private Methods
        /// <summary>
        /// Reloads <see cref="Snapshots"/> from DB and sorts the list
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReloadSnapshotsAsync(int id)
        {
            var memberSns = await App.DBRepo.GetAllMemberSnapshotItemAsync(id);

            if (memberSns == null || memberSns.Count == 0)
            {
                return false;
            }

            Member = new ObservableCollection<Player>();

            foreach (var item in memberSns)
            {
                Member.Add(item.ConvertToPlayer());
            }

            return true;
        }
        #endregion

        #region E + D
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            #region Passed players for comparison
            if (parameters.ContainsKey("id"))
            {
                try
                {
                    int idToExport = JfTypeConverter.ForceInt(parameters["id"].ToString());

                    if (idToExport >= 0)
                    {
                        //Load from DB
                        var sn = await App.DBRepo.GetSnapshotByID(idToExport);
                        var memberSn = new List<MemberSnapshotItem>();

                        if (sn == null)
                        {
                            sn = new Snapshot();
                        }

                        bool snLoaded = await ReloadSnapshotsAsync(idToExport);
                    }
                    else
                    {
                        await _dialogService.DisplayAlertAsync(AppResources.ErrorOccuredText, "could not parse id", AppResources.OKText);
                        var result = await _navigationService.GoBackAsync();
                        Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
                    }
                }
                catch (System.Exception e)
                {
                    Logger.WriteToLogFile($"ClanMemberOverview Error: Could fetch snapshot data {e.Message}");
                }
            }
            else
            {
                Member = new ObservableCollection<Player>(App.Save.ThisClan.ClanMember);
            }
            #endregion

            // Sort the List
            if (Member != null)
            {
                Member.OrderByDescending(x => x.StageMax).ThenBy(n => n.PlayerId);
            }

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}