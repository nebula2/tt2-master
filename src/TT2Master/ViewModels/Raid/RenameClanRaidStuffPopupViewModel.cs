using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Raid;
using TT2Master.Resources;
using Xamarin.Forms;

namespace TT2Master.ViewModels.Raid
{
    public class RenameClanRaidStuffPopupViewModel : ViewModelBase
    {
        #region Member
        private string _newName = "";
        public string NewName { get => _newName; set => SetProperty(ref _newName, value); }

        private object _itemToRename;

        /// <summary>
        /// Command to save
        /// </summary>
        public ICommand SaveCommand { get; set; }

        private INavigationService _navigationService;
        private IPageDialogService _dialogService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public RenameClanRaidStuffPopupViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            SaveCommand = new DelegateCommand(async () => await SaveExecuteAsync());
            Title = AppResources.Rename;
        }
        #endregion

        #region CommandMethods
        /// <summary>
        /// Command to Save ban
        /// </summary>
        private async Task<bool> SaveExecuteAsync()
        {
            if(string.IsNullOrEmpty(NewName))
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.FillNameFirstText, AppResources.OKText);
                return false;
            }
            
            //store value
            if(_itemToRename is RaidTolerance tolerance)
            {
                // get items from db
                tolerance = await App.DBRepo.GetRaidToleranceByID(tolerance.Name);
                var relationships = await App.DBRepo.GetClanRaidToleranceRelationshipByToleranceID(tolerance.Name);

                // delete parent as name is PK
                await App.DBRepo.DeleteRaidToleranceByID(tolerance.Name);

                // update childs as they are stored by auto inc id
                tolerance.Name = NewName;
                foreach (var item in relationships)
                {
                    item.RaidToleranceId = NewName;
                    await App.DBRepo.UpdateClanRaidToleranceRelationshipAsync(item);
                }

                // insert item again
                await App.DBRepo.AddRaidToleranceAsync(tolerance);
            }
            else if(_itemToRename is RaidStrategy strategy)
            {
                // get items from db
                strategy = await App.DBRepo.GetRaidStrategyByID(strategy.Name);
                var relationships = await App.DBRepo.GetClanRaidEnemyStrategiesByStrategyID(strategy.Name);

                // delete parent as name is PK
                await App.DBRepo.DeleteRaidStrategyByID(strategy.Name);

                // update childs as they are stored by auto inc id
                strategy.Name = NewName;
                foreach (var item in relationships)
                {
                    item.RaidStrategyId = NewName;
                    await App.DBRepo.UpdateClanRaidEnemyStrategyAsync(item);
                }

                // insert item again
                await App.DBRepo.AddRaidStrategyAsync(strategy);
            }

            //leave this shit - i am done
            var result = await _navigationService.GoBackAsync();
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");
            return true;
        }
        #endregion

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("tolerance"))
            {
                _itemToRename = parameters["tolerance"] as RaidTolerance;
                NewName = (_itemToRename as RaidTolerance).Name;
            }
            else if (parameters.ContainsKey("strategy"))
            {
                _itemToRename = parameters["strategy"] as RaidStrategy;
                NewName = (_itemToRename as RaidStrategy).Name;
            }

            base.OnNavigatedTo(parameters);
        }
    }
}