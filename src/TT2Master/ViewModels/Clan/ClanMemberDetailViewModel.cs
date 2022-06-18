using Microcharts;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model;
using TT2Master.Model.Arti;
using TT2Master.Model.Helpers;
using TT2Master.Model.Tournament;
using TT2Master.Resources;

namespace TT2Master
{
    public class ClanMemberDetailViewModel : ViewModelBase
    {
        #region Properties
        private bool _isLoadingDataFromSavefile = true;
        public bool IsLoadingDataFromSavefile { get => _isLoadingDataFromSavefile; set => SetProperty(ref _isLoadingDataFromSavefile, value); }

        private bool _isLoadingDataFromSavefileAndMyself = true;
        public bool IsLoadingDataFromSavefileAndMyself { get => _isLoadingDataFromSavefileAndMyself; set => SetProperty(ref _isLoadingDataFromSavefileAndMyself, value); }
        
        private bool _isClanQuestDataAvailable = true;
        public bool IsClanQuestDataAvailable { get => _isClanQuestDataAvailable; set => SetProperty(ref _isClanQuestDataAvailable, value); }

        private Player _currentPlayer;
        public Player CurrentPlayer { get => _currentPlayer; set => SetProperty(ref _currentPlayer, value); }

        private bool _isTournament = false;
        public bool IsTournament { get => _isTournament; set => SetProperty(ref _isTournament, value); }

        private bool _isMyself = false;
        public bool IsMyself { get => _isMyself; set => SetProperty(ref _isMyself, value); }

        private bool _isNotMyself = true;
        public bool IsNotMyself { get => _isNotMyself; set => SetProperty(ref _isNotMyself, value); }

        private bool _showCharts = true;
        public bool ShowCharts { get => _showCharts; set => SetProperty(ref _showCharts, value); }

        private bool _isNavigateEnabled = true;
        public bool IsNavigateEnabled { get => _isNavigateEnabled; set => SetProperty(ref _isNavigateEnabled, value); }

        private string _msIncreaseText = "";
        public string MsIncreaseText { get => _msIncreaseText; set => SetProperty(ref _msIncreaseText, value); }

        private string _tpIncreaseText = "";
        public string TpIncreaseText { get => _tpIncreaseText; set => SetProperty(ref _tpIncreaseText, value); }

        private string _cqIncreaseText = "";
        public string CqIncreaseText { get => _cqIncreaseText; set => SetProperty(ref _cqIncreaseText, value); }

        private string _bosProgress = "";
        public string BoSProgress { get => _bosProgress; set => SetProperty(ref _bosProgress, value); }

        private string _artifactProgress = "";
        public string ArtifactProgress { get => _artifactProgress; set => SetProperty(ref _artifactProgress, value); }

        private string _spProgress = "";
        public string SpProgress { get => _spProgress; set => SetProperty(ref _spProgress, value); }

        private string _mSetsProgress = "";
        public string MSetsProgress { get => _mSetsProgress; set => SetProperty(ref _mSetsProgress, value); }

        private string _prestigeIncreaseText = "";
        public string PrestigeIncreaseText { get => _prestigeIncreaseText; set => SetProperty(ref _prestigeIncreaseText, value); }

        private double _ltrValue = 0;
        public double LTRValue { get => _ltrValue; set => SetProperty(ref _ltrValue, value); }

        private double _craftingPower = 0;
        public double CraftingPower { get => _craftingPower; set => SetProperty(ref _craftingPower, value); }

        #region Passive Skill Level
        private int _antiTitanCannonLevel = 0;
        public int AntiTitanCannonLevel { get => _antiTitanCannonLevel; set => SetProperty(ref _antiTitanCannonLevel, value); }

        private int _arcaneBargainLevel = 0;
        public int ArcaneBargainLevel { get => _arcaneBargainLevel; set => SetProperty(ref _arcaneBargainLevel, value); }

        private int _intimidatingPresenceLevel = 0;
        public int IntimidatingPresenceLevel { get => _intimidatingPresenceLevel; set => SetProperty(ref _intimidatingPresenceLevel, value); }

        private int _mysticalImpactLevel = 0;
        public int MysticalImpactLevel { get => _mysticalImpactLevel; set => SetProperty(ref _mysticalImpactLevel, value); }

        private int _powerSurgeLevel = 0;
        public int PowerSurgeLevel { get => _powerSurgeLevel; set => SetProperty(ref _powerSurgeLevel, value); }

        private int _silentMarchLevel = 0;
        public int SilentMarchLevel { get => _silentMarchLevel; set => SetProperty(ref _silentMarchLevel, value); }
        #endregion

        private List<ChartEntry> _entries;
        public List<ChartEntry> Entries { get => _entries; set => SetProperty(ref _entries, value); }

        private List<ChartEntry> _prestigeEntries;
        public List<ChartEntry> PrestigeEntries { get => _prestigeEntries; set => SetProperty(ref _prestigeEntries, value); }

        private LineChart _lChart;
        public LineChart LChart { get => _lChart; set => SetProperty(ref _lChart, value); }

        private LineChart _prestigeChart;
        public LineChart PrestigeChart { get => _prestigeChart; set => SetProperty(ref _prestigeChart, value); }

        private int _statTimeId = 0;
        public int StatTimeId
        {
            get => _statTimeId;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _statTimeId, value);

                    if (_snapshots != null && CurrentPlayer != null)
                    {
                        UpdateChartsView();
                    }
                }
            }
        }

        private List<StatTime> _times = StatTimes.Times;
        public List<StatTime> Times { get => _times; set => SetProperty(ref _times, value); }

        private List<Snapshot> _snapshots = new List<Snapshot>();

        private bool _isNavigatingToOtherMember = false;

        public ICommand BanCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand GoToPreviousCommand { get; private set; }
        public ICommand GoToNextCommand { get; private set; }

        private readonly IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public ClanMemberDetailViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;

            IsLoadingDataFromSavefile = LocalSettingsORM.IsReadingDataFromSavefile;

            BanCommand = new DelegateCommand(async () => await BanAsync());
            ExportCommand = new DelegateCommand(async () => await ExportAsync());
            GoToNextCommand = new DelegateCommand(async () => await GoToNextAsync());
            GoToPreviousCommand = new DelegateCommand(async () => await GoToPreviousAsync());
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the Charts
        /// </summary>
        private void UpdateChartsView()
        {
            if (IsTournament)
            {
                return;
            }

            try
            {
                #region Getting Data
                //get relevant snapshots grouped by date and ordered by timestamp ascending
                var relevantSnaps = _snapshots
                    .Where(x => x.Timestamp >= DateTime.Now.AddDays(-Times[StatTimeId].Days))
                    .GroupBy(g => g.Timestamp.ToString("yyyyMMdd"))
                    .Select(s => s.First())
                    .OrderBy(n => n.Timestamp).ToList();

                Entries = new List<ChartEntry>();
                PrestigeEntries = new List<ChartEntry>();

                var userSnaps = new List<MemberSnapshotItem>();

                if (relevantSnaps == null)
                {
                    Logger.WriteToLogFile($"ClanMemberDetail Did not find any relevant snaps: {App.DBRepo.StatusMessage}");
                    relevantSnaps = new List<Snapshot>();
                }

                //get item for this clanmember
                for (int i = 0; i < relevantSnaps.Count; i++)
                {
                    var user = relevantSnaps[i].MemberSnapshotItems.Where(x => x.PlayerId == CurrentPlayer.PlayerId).FirstOrDefault();

                    if (user == null)
                    {
                        continue;
                    }
                    else
                    {
                        userSnaps.Add(user);
                    }
                }
                #endregion

                #region Populate Entries
                //populate entries
                for (int i = 0; i < userSnaps.Count; i++)
                {
                    //Add entry
                    Entries.Add(new ChartEntry((float)userSnaps[i].StageMax)
                    {
                        Color = SKColor.Parse("#23B391"),
                        ValueLabel = userSnaps[i].StageMax.ToString(),
                        Label = relevantSnaps[i].Timestamp.ToString("MM.dd"),
                    });

                    PrestigeEntries.Add(new ChartEntry((float)userSnaps[i].PrestigeCount)
                    {
                        Color = SKColor.Parse("#23B391"),
                        ValueLabel = userSnaps[i].PrestigeCount.ToString(),
                        Label = relevantSnaps[i].Timestamp.ToString("MM.dd"),
                    });
                }
                #endregion

                #region Inc-Values
                if (userSnaps.Count > 1)
                {
                    MsIncreaseText = string.Format(AppResources.IncreasedXStages, (userSnaps[userSnaps.Count - 1].StageMax - userSnaps[0].StageMax).ToString());
                    TpIncreaseText = $"TP+ {(userSnaps[userSnaps.Count - 1].TitanPoints - userSnaps[0].TitanPoints).ToString()}";
                    CqIncreaseText = $"TC+ {(userSnaps[userSnaps.Count - 1].RaidTicketsCollected - userSnaps[0].RaidTicketsCollected).ToString()}";
                    PrestigeIncreaseText = string.Format(AppResources.IncreasedXPrestige, (userSnaps[userSnaps.Count - 1].PrestigeCount - userSnaps[0].PrestigeCount).ToString());
                }
                else
                {
                    MsIncreaseText = string.Format(AppResources.IncreasedXStages, "?");
                    TpIncreaseText = $"TP+ ?";
                    CqIncreaseText = $"TQ+ ?";
                    PrestigeIncreaseText = string.Format(AppResources.IncreasedXPrestige, "?");
                }
                #endregion

                #region Populate Charts
                LChart = new LineChart()
                {
                    Entries = Entries,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    Margin = 30,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    LabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };
                LChart.MinValue = (float)userSnaps[0].StageMax;
                LChart.MaxValue = (float)userSnaps[userSnaps.Count - 1].StageMax;

                PrestigeChart = new LineChart()
                {
                    Entries = PrestigeEntries,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    Margin = 30,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    LabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };
                PrestigeChart.MinValue = (float)userSnaps[0].PrestigeCount;
                PrestigeChart.MaxValue = (float)userSnaps[userSnaps.Count - 1].PrestigeCount;
                #endregion
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanMemberDetail Update Charts Error: {e.Message}");

                Entries = new List<ChartEntry>();
                LChart = new LineChart()
                {
                    Entries = Entries,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    Margin = 30,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    LabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };

                PrestigeEntries = new List<ChartEntry>();
                PrestigeChart = new LineChart()
                {
                    Entries = PrestigeEntries,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    Margin = 30,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelOrientation = Orientation.Horizontal,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };
            }
        }

        private async Task<bool> BanAsync()
        {
            if (IsMyself)
            {
                return true;
            }

            bool choice = await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.SureToBanText, AppResources.OKText, AppResources.CancelText);

            if (!choice)
            {
                return false;
            }

            //Check if this player is already banned
            bool banned = await IsCurrentPlayerBanned();

            if (banned)
            {
                await ToastSender.SendToastAsync(AppResources.PlayerAlreadyBanned, _dialogService);

                return false;
            }

            //Navigate to BanPopup
            var result = await _navigationService.NavigateAsync("BanPlayerPopupPage", new NavigationParameters() { { "dude", CurrentPlayer.PlayerName } });
            Logger.WriteToLogFile($"Navigation Result: \n{(result as Prism.Navigation.NavigationResult).Success}\n {(result as Prism.Navigation.NavigationResult).Exception}");

            return true;
        }

        private async Task<bool> ExportAsync()
        {
            try
            {
                string screenshotData = Xamarin.Forms.DependencyService.Get<ICreateScreenshot>().Capture();

                await _dialogService.DisplayAlertAsync("Saved", screenshotData, "cancel");

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"CMD Screenie Error: {e.Message}");
                return false;
            }
        }

        private async Task<bool> IsCurrentPlayerBanned()
        {
            try
            {
                string[] banned = await App.DBRepo.GetBannedPlayersArrayAsync();

                return banned.Contains(CurrentPlayer.PlayerId);
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanMemberDetailVM Error: {e.Message}");
                return false;
            }
        }

        private void LoadDataForMyself()
        {
            #region BoS
            try
            {
                double perc = LocalSettingsORM.UseMasterBoSDisplay
                    ? ArtifactHandler.CalculateLifeTimeSpentPercentage(ArtifactHandler.Artifacts.Where(x => x.ID == "Artifact22").First().RelicsSpent)
                    : ArtifactHandler.CalculateLifeTimeSpentPercentageForDummies(ArtifactHandler.Artifacts.Where(x => x.ID == "Artifact22").First().RelicsSpent);

                BoSProgress = $"{Math.Round(perc, 2)} %";
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"CMD Error BoS:{e.Message}");
            }
            #endregion

            #region Sp Progress
            SpProgress = $"{SaveFile.SPReceived.ToString()}";
            #endregion

            #region Mystic Sets
            try
            {
                bool setsLoaded = EquipmentHandler.LoadSetInformation(App.Save);
                if (setsLoaded)
                {
                    int setCount = EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.SetType == "Mythic").Count();
                    MSetsProgress = setCount.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"LoadDataForMyself Error Sets:{e.Message}");
            }
            #endregion

            #region Passive Skills
            try
            {
                FormulaHelper.LoadPassiveSkillCosts(EquipmentHandler.EquipmentSets.Where(x => x.Completed && x.Set == "Mech").Count() > 0);
                AntiTitanCannonLevel = FormulaHelper.GetAntiTitanCannonLevel(HelpersFactory.GetTotalMasteriesLevel());
                ArcaneBargainLevel = FormulaHelper.GetArcaneBargainLevel(CurrentPlayer.DustSpent);
                IntimidatingPresenceLevel = FormulaHelper.GetIntimidatingPresenceLevel(CurrentPlayer.TotalSkillPoints);
                MysticalImpactLevel = FormulaHelper.GetMysticalImpactLevel(CurrentPlayer.TitanPoints);
                PowerSurgeLevel = FormulaHelper.GetPowerSurgeLevel(CurrentPlayer.TotalPetLevels);
                SilentMarchLevel = FormulaHelper.GetSilentMarchLevel((int)CurrentPlayer.StageMax);
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"LoadDataForMyself Error Passive Skills:{e.Message}");
            }

            #endregion

            #region LTR
            try
            {
                LTRValue = App.Save.CurrentRelics + ArtifactHandler.GetLifeTimeSpentOnAll()
                    + ArtifactCostHandler.CostSum(ArtifactHandler.Artifacts.Where(x => x.Level > 0).Count(), ArtifactHandler.Artifacts.Where(x => x.EnchantmentLevel > 0).Count());
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanMemberDetailVM LTR Error:{e.Message}");
                LTRValue = 0;
            }
            #endregion
        }

        private async Task<bool> GoToPreviousAsync()
        {
            if (!IsNavigateEnabled)
            {
                return true;
            }

            // Get tournament member rank
            if (CurrentPlayer.ClanRank == 1)
            {
                return true;
            }

            _isNavigatingToOtherMember = true;

            if (IsTournament)
            {
                string destinationMember = TournamentHandler.TM.Members.Where(x => x.ClanRank == CurrentPlayer.ClanRank - 1).FirstOrDefault().PlayerId;
                return await LoadTourneyPlayerData(destinationMember);
            }
            else
            {
                string destinationMember = App.Save.ThisClan.ClanMember.Where(x => x.ClanRank == CurrentPlayer.ClanRank - 1).FirstOrDefault().PlayerName;
                return await LoadPlayerData(destinationMember);
            }

        }

        private async Task<bool> GoToNextAsync()
        {
            if (!IsNavigateEnabled)
            {
                return true;
            }

            _isNavigatingToOtherMember = true;

            if (IsTournament)
            {
                // Get tourney member rank
                if (CurrentPlayer.ClanRank == TournamentHandler.TM.Members.Max(x => x.ClanRank))
                {
                    return true;
                }

                string destinationMember = TournamentHandler.TM.Members.Where(x => x.ClanRank == CurrentPlayer.ClanRank + 1).FirstOrDefault().PlayerId;
                return await LoadTourneyPlayerData(destinationMember);
            }
            else
            {
                // Get clan member rank
                if (CurrentPlayer.ClanRank == App.Save.ThisClan.ClanMember.Max(x => x.ClanRank))
                {
                    return true;
                }

                string destinationMember = App.Save.ThisClan.ClanMember.Where(x => x.ClanRank == CurrentPlayer.ClanRank + 1).FirstOrDefault().PlayerName;
                return await LoadPlayerData(destinationMember);
            }
        }

        private async Task<bool> LoadPlayerData(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                CurrentPlayer = new Player();
            }
            else
            {
                try
                {
                    if(App.Save.ThisPlayer?.PlayerName == memberName)
                    {
                        CurrentPlayer = App.Save.ThisPlayer;
                    }
                    else if(App.Save.ThisClan != null)
                    {
                        CurrentPlayer = App.Save.ThisClan.ClanMember.Where(x => x.PlayerName == memberName).FirstOrDefault() ?? new Player();
                    }

                    CurrentPlayer.IsBanned = await IsCurrentPlayerBanned();
                    OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
                    Title = LocalSettingsORM.IsReadingDataFromSavefile
                        ? CurrentPlayer.PlayerName ?? ""
                        : AppResources.Profile;

                    CraftingPower = IsLoadingDataFromSavefile ? FormulaHelper.GetCraftingPower(CurrentPlayer.CraftingShardsSpent) : App.Save.CraftingPower;
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ClanMemberDetailVM Error LoadPlayerData: {e.Message}");
                }
            }

            try
            {
                // is this myself or someone else?
                IsMyself = CurrentPlayer?.PlayerId == App.Save.ThisPlayer?.PlayerId;
                IsNotMyself = !IsMyself;
                IsLoadingDataFromSavefileAndMyself = IsMyself && IsLoadingDataFromSavefile;


                if (IsMyself)
                {
                    LoadDataForMyself();
                }

                #region Artifacts
                try
                {
                    ArtifactHandler.FillArtifacts(App.Save);
                    ArtifactProgress = $"{CurrentPlayer.ArtifactCount}/{ArtifactHandler.Artifacts.Count}";
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"CMD Error Arts:{e.Message}");
                }
                #endregion

                _snapshots = await App.DBRepo.GetAllSnapshotAsync();

                foreach (var item in _snapshots)
                {
                    item.MemberSnapshotItems = await App.DBRepo.GetAllMemberSnapshotItemAsync(item.ID);
                }

                UpdateChartsView();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanMemberDetailVM Error LoadPlayerData: {e.Message}");
            }

            return true;
        }

        private async Task<bool> LoadTourneyPlayerData(string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                CurrentPlayer = new Player();
            }
            else
            {
                try
                {
                    CurrentPlayer = TournamentHandler.TM.Members.Where(x => x.PlayerId == memberId).FirstOrDefault();
                    CurrentPlayer.IsBanned = await IsCurrentPlayerBanned();
                    OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
                    Title = CurrentPlayer.PlayerName;

                    CraftingPower = FormulaHelper.GetCraftingPower(CurrentPlayer.CraftingShardsSpent);
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ClanMemberDetailVM Error LoadPlayerData: {e.Message}");
                }
            }

            try
            {
                // is this myself or someone else?
                IsMyself = CurrentPlayer.PlayerId == App.Save.ThisPlayer?.PlayerId;
                IsNotMyself = !IsMyself;
                IsLoadingDataFromSavefileAndMyself = IsMyself && IsLoadingDataFromSavefile;

                if (IsMyself)
                {
                    LoadDataForMyself();
                }

                #region Artifacts
                try
                {
                    ArtifactHandler.FillArtifacts(App.Save);
                    ArtifactProgress = $"{CurrentPlayer.ArtifactCount}/{ArtifactHandler.Artifacts.Count}";
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"CMD Error Arts:{e.Message}");
                }
                #endregion

                _snapshots = await App.DBRepo.GetAllSnapshotAsync();

                foreach (var item in _snapshots)
                {
                    item.MemberSnapshotItems = await App.DBRepo.GetAllMemberSnapshotItemAsync(item.ID);
                }

                UpdateChartsView();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"ClanMemberDetailVM Error LoadPlayerData: {e.Message}");
            }

            return true;
        }
        #endregion

        #region Navigation overrides
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("isTournament"))
            {
                IsTournament = true;
                ShowCharts = false;
            }

            IsClanQuestDataAvailable = LocalSettingsORM.IsReadingDataFromSavefile && ShowCharts;

            if (parameters.ContainsKey("member") && IsTournament)
            {
                await LoadTourneyPlayerData(parameters["member"].ToString());
            }
            else if (parameters.ContainsKey("member"))
            {
                await LoadPlayerData(parameters["member"]?.ToString());
            }
            else
            {
                await LoadPlayerData("");
            }

            if (parameters.ContainsKey("profile"))
            {
                IsNavigateEnabled = false;
            }


            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            if (_isNavigatingToOtherMember)
            {

            }

            base.OnNavigatedFrom(parameters);
        }
        #endregion
    }
}