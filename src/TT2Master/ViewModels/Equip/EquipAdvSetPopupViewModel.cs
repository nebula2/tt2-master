using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master
{
    public class EquipAdvSetPopupViewModel : ViewModelBase
    {
        #region Member
        private EquipAdvSettings _currentSettings = new EquipAdvSettings();
        public EquipAdvSettings CurrentSettings { get => _currentSettings; set => SetProperty(ref _currentSettings, value); }

        private EquipBuildEnum _equipBuild = EquipBuildEnum.Ship;
        public EquipBuildEnum EquipBuild { get => _equipBuild; set => SetProperty(ref _equipBuild, value); }

        private List<string> _equipBuilds = Enum.GetNames(typeof(EquipBuildEnum)).Select(x => x.TranslatedString()).ToList();
        public List<string> EquipBuilds { get => _equipBuilds; set => SetProperty(ref _equipBuilds, value); }

        private GoldType _currGoldType = GoldType.pHoM;
        public GoldType CurrGoldType { get => _currGoldType; set => SetProperty(ref _currGoldType, value); }

        private List<string> _goldTypes = Enum.GetNames(typeof(GoldType)).Select(x => x.TranslatedString()).ToList();
        public List<string> GoldTypes { get => _goldTypes; set => SetProperty(ref _goldTypes, value); }

        private HeroBaseType _currHeroBaseType = HeroBaseType.NotSet;
        public HeroBaseType CurrHeroBaseType { get => _currHeroBaseType; set => SetProperty(ref _currHeroBaseType, value); }

        private List<string> _heroBaseTypes = Enum.GetNames(typeof(HeroBaseType)).Select(x => x.TranslatedString()).ToList();
        public List<string> HeroBaseTypes { get => _heroBaseTypes; set => SetProperty(ref _heroBaseTypes, value); }

        private HeroDmgType _currHeroDmgType = HeroDmgType.NotSet;
        public HeroDmgType CurrHeroDmgType { get => _currHeroDmgType; set => SetProperty(ref _currHeroDmgType, value); }

        private List<string> _heroDmgTypes = Enum.GetNames(typeof(HeroDmgType)).Select(x => x.TranslatedString()).ToList();
        public List<string> HeroDmgTypes { get => _heroDmgTypes; set => SetProperty(ref _heroDmgTypes, value); }

        private INavigationService _navigationService;

        public ICommand SaveCommand { get; private set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        public EquipAdvSetPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            Title = AppResources.SettingsHeader;

            SaveCommand = new DelegateCommand(async () => await SaveExecuteAsync());
        }
        #endregion

        #region CommandMethods
        /// <summary>
        /// Command to Save category
        /// </summary>
        private async Task<bool> SaveExecuteAsync()
        {
            Logger.WriteToLogFile("ArtOpt.SaveExecuteAsync: Going to save");
            FillSettingsFromProperties();

            //store value
            Logger.WriteToLogFile($"ArtOpt.SaveExecuteAsync: going to save settings");
            int res = await App.DBRepo.UpdateEquipAdvSettingsAsync(CurrentSettings);

            Logger.WriteToLogFile($"ArtOpt.SaveExecuteAsync: saved {res}.\n{CurrentSettings.ToString()}\nGoing back");
            //leave this shit - i am done
            var result = await _navigationService.GoBackAsync(new NavigationParameters() { { "id", CurrentSettings.ID } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return true;
        }
        #endregion

        #region Helper

        #endregion

        #region Population
        /// <summary>
        /// Fills Settings from Properties
        /// </summary>
        private void FillSettingsFromProperties()
        {
            Logger.WriteToLogFile("Filling settings from properties");
            CurrentSettings = new EquipAdvSettings()
            {
                CurrentBuild = EquipBuild,
                CurrentGoldType = CurrGoldType,
                CurrentHeroDmgType = CurrHeroDmgType,
                CurrentHeroType = CurrHeroBaseType,
            };

            Logger.WriteToLogFile("done that");
        }

        /// <summary>
        /// Fills Properties from Settings
        /// </summary>
        private void FillPropertiesFromSettings()
        {
            Logger.WriteToLogFile("Filling properties from settings");
            try
            {
                EquipBuild = CurrentSettings.CurrentBuild;
                CurrGoldType = CurrentSettings.CurrentGoldType;
                CurrHeroDmgType = CurrentSettings.CurrentHeroDmgType;
                CurrHeroBaseType = CurrentSettings.CurrentHeroType;

                Logger.WriteToLogFile("done doing that");
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ArtOptSettings.FillPropsFromSettings: Error {ex.Message}");
            }
        }
        #endregion 

        #region Override
        
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.ContainsKey("id"))
            {
                Logger.WriteToLogFile("EquipAdvSet.OnNavigatedTo: No id given. Creating new CurrentSettings");
                CurrentSettings = new EquipAdvSettings();
            }
            else
            {
                Logger.WriteToLogFile("EquipAdvSet.OnNavigatedTo: id given. loading from db");
                //Load from DB
                CurrentSettings = await App.DBRepo.GetEquipAdvSettingsByID(parameters["id"].ToString());

                if (CurrentSettings == null)
                {

                    Logger.WriteToLogFile("EquipAdvSet.OnNavigatedTo: No currentSettings got from db");
                    CurrentSettings = new EquipAdvSettings();
                }
                else
                {
                    Logger.WriteToLogFile($"EquipAdvSet.OnNavigatedTo: loaded currentsettings from db.\n{_currentSettings.ToString()}");
                }
            }

            FillPropertiesFromSettings();

            OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}