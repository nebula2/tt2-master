using Microcharts;
using Prism.Navigation;
using Prism.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Loggers;
using TT2Master.Model.Tournament;
using TT2Master.Resources;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// ViewModel for Player Comparison
    /// </summary>
    public class MemberComparisonViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;
        private readonly INavigationService _navigationService;

        #region Properties
        private Player _sourcePlayer = new Player();
        public Player SourcePlayer { get => _sourcePlayer; set => SetProperty(ref _sourcePlayer, value); }

        private Player _targetPlayer = new Player();
        public Player TargetPlayer { get => _targetPlayer; set => SetProperty(ref _targetPlayer, value); }

        private bool _isTournamentComparison = false;
        public bool IsTournamentComparison { get => _isTournamentComparison; set => SetProperty(ref _isTournamentComparison, value); }

        private bool _showCharts = true;
        public bool ShowCharts { get => _showCharts; set => SetProperty(ref _showCharts, value); }

        private List<StatTime> _times = StatTimes.Times;
        public List<StatTime> Times { get => _times; set => SetProperty(ref _times, value); }

        private int _statTimeId = 0;
        public int StatTimeId
        {
            get => _statTimeId;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _statTimeId, value);

                    UpdateChartsView();
                }
            }
        }

        private List<Snapshot> _snapshots = new List<Snapshot>();

        private List<ChartEntry> _msEntriesSource;
        public List<ChartEntry> MsEntriesSource { get => _msEntriesSource; set => SetProperty(ref _msEntriesSource, value); }

        private LineChart _msChartSource;
        public LineChart MsChartSource { get => _msChartSource; set => SetProperty(ref _msChartSource, value); }

        private List<ChartEntry> _msEntriesTarget;
        public List<ChartEntry> MsEntriesTarget { get => _msEntriesTarget; set => SetProperty(ref _msEntriesTarget, value); }

        private LineChart _msChartTarget;
        public LineChart MsChartTarget { get => _msChartTarget; set => SetProperty(ref _msChartTarget, value); }
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public MemberComparisonViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;

            Title = AppResources.PlayerComparison;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Gets the Player in Clan for given PlayerId
        /// </summary>
        /// <param name="Id">PlayerId</param>
        /// <returns>Player from ThisClan</returns>
        private Player LoadClanPlayerData(string Id)
        {
            var currPlayer = new Player();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                try
                {
                    currPlayer = App.Save.ThisClan.ClanMember.Where(x => x.PlayerId == Id).FirstOrDefault();
                    OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ClanMemberComparisonVM Error LoadPlayerData: {e.Message}");
                }
            }


            return currPlayer;
        }

        /// <summary>
        /// Gets the Player in Tournament for given PlayerId
        /// </summary>
        /// <param name="Id">PlayerId</param>
        /// <returns>Player from ThisClan</returns>
        private Player LoadTourneyPlayerData(string Id)
        {
            var currPlayer = new Player();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                try
                {
                    currPlayer = TournamentHandler.TM.Members.Where(x => x.PlayerId == Id).FirstOrDefault();
                    OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
                }
                catch (Exception e)
                {
                    Logger.WriteToLogFile($"ClanMemberComparisonVM Error LoadPlayerData: {e.Message}");
                }
            }


            return currPlayer;
        }

        /// <summary>
        /// Does the initialization when in Clan comparison mode
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        private async Task<bool> InitializeForClanComparison(string sourceId, string targetId)
        {
            try
            {
                SourcePlayer = LoadClanPlayerData(sourceId);
                TargetPlayer = LoadClanPlayerData(targetId);

                _snapshots = await App.DBRepo.GetAllSnapshotAsync();

                foreach (var item in _snapshots)
                {
                    item.MemberSnapshotItems = await App.DBRepo.GetAllMemberSnapshotItemAsync(item.ID);
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"InitializeForClanComparison Error: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Does the initialization when in Tournament comparison mode
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        private bool InitializeForTournamentComparison(string sourceId, string targetId)
        {
            try
            {
                SourcePlayer = LoadTourneyPlayerData(sourceId);
                TargetPlayer = LoadTourneyPlayerData(targetId);

                _snapshots = new List<Snapshot>();

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"InitializeForClanComparison Error: {e.Message}");
                return false;
            }
        }

        private void LoadEntriesForClanComparison()
        {
            try
            {
                #region Getting Data
                //get relevant snapshots grouped by date and ordered by timestamp ascending
                var relevantSnaps = _snapshots
                    .Where(x => x.Timestamp >= DateTime.Now.AddDays(-Times[StatTimeId].Days))
                    .GroupBy(g => g.Timestamp.ToString("yyyyMMdd"))
                    .Select(s => s.First())
                    .OrderBy(n => n.Timestamp).ToList();

                MsEntriesSource = new List<ChartEntry>();
                MsEntriesTarget = new List<ChartEntry>();

                var sourceSnaps = new List<MemberSnapshotItem>();
                var targetSnaps = new List<MemberSnapshotItem>();

                if (relevantSnaps == null)
                {
                    Logger.WriteToLogFile($"ClanMemberDetail Did not find any relevant snaps: {App.DBRepo.StatusMessage}");
                    relevantSnaps = new List<Snapshot>();
                }

                //get items for both member
                for (int i = 0; i < relevantSnaps.Count; i++)
                {
                    var sourceUser = relevantSnaps[i].MemberSnapshotItems.Where(x => x.PlayerId == SourcePlayer.PlayerId).FirstOrDefault();
                    var targetUser = relevantSnaps[i].MemberSnapshotItems.Where(x => x.PlayerId == TargetPlayer.PlayerId).FirstOrDefault();

                    if (sourceUser == null && targetUser == null)
                    {
                        continue;
                    }
                    else
                    {
                        if(sourceUser != null)
                        {
                            sourceSnaps.Add(sourceUser);
                        }

                        if (targetUser != null)
                        {
                            targetSnaps.Add(targetUser);
                        }
                    }
                }
                #endregion

                #region Populate Entries
                //populate entries
                for (int i = 0; i < sourceSnaps.Count; i++)
                {
                    //Add entry
                    MsEntriesSource.Add(new ChartEntry((float)sourceSnaps[i].StageMax)
                    {
                        Color = SKColor.Parse("#23B391"),
                        ValueLabel = sourceSnaps[i].StageMax.ToString(),
                        Label = relevantSnaps[i].Timestamp.ToString("MM.dd"),
                    });
                }

                for (int i = 0; i < targetSnaps.Count; i++)
                {
                    //Add entry
                    MsEntriesTarget.Add(new ChartEntry((float)targetSnaps[i].StageMax)
                    {
                        Color = SKColor.Parse("#B33123"),
                        ValueLabel = targetSnaps[i].StageMax.ToString(),
                        Label = relevantSnaps[i].Timestamp.ToString("MM.dd"),
                    });
                }
                #endregion
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"LoadEntriesForClanComparison Error: {e.Message}");
                MsEntriesSource = new List<ChartEntry>();
                MsEntriesTarget = new List<ChartEntry>();
            }
        }

        private void LoadEntriesForTournamentComparison()
        {
            try
            {
                // TODO Get Tournament snapshot data and make ChartEntries for them
                MsEntriesSource = new List<ChartEntry>();
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"LoadEntriesForClanComparison Error: {e.Message}");
                MsEntriesSource = new List<ChartEntry>();
            }
        }

        /// <summary>
        /// Updates the Chart
        /// </summary>
        private void UpdateChartsView()
        {
            try
            {
                if (IsTournamentComparison)
                {
                    LoadEntriesForTournamentComparison();
                }
                else
                {
                    LoadEntriesForClanComparison();
                }

                #region Populate Charts
                MsChartSource = new LineChart()
                {
                    Entries = MsEntriesSource,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    Margin = 30,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    LabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };
                MsChartSource.MinValue = MsEntriesSource.Min( x => x.Value);
                MsChartSource.MaxValue = MsEntriesSource.Max(x => x.Value);

                MsChartTarget = new LineChart()
                {
                    Entries = MsEntriesTarget,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    Margin = 30,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    LabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };
                MsChartTarget.MinValue = MsEntriesTarget.Min(x => x.Value);
                MsChartTarget.MaxValue = MsEntriesTarget.Max(x => x.Value);
                #endregion
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"MemberComparison Update Charts Error: {e.Message}");

                MsChartSource = new LineChart()
                {
                    Entries = MsEntriesSource,
                    LineMode = LineMode.Straight,
                    LabelTextSize = 24,
                    Margin = 30,
                    BackgroundColor = SKColor.Parse("#344860"),
                    ValueLabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    LabelOrientation = Times[StatTimeId].Days <= 7 ? Orientation.Horizontal : Orientation.Vertical,
                    AnimationDuration = TimeSpan.FromMilliseconds(150),
                };
            }
        }
        #endregion

        #region Navigation override
        /// <summary>
        /// When navigating to this, load values from parameters
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("tournament"))
            {
                IsTournamentComparison = JfTypeConverter.ForceBool(parameters["tournament"].ToString());
                ShowCharts = !IsTournamentComparison; // until this is finished for tournament do not show it
            }

            if(IsTournamentComparison && parameters.ContainsKey("source") && parameters.ContainsKey("target"))
            {
                InitializeForTournamentComparison(parameters["source"].ToString(), parameters["target"].ToString());
            }
            else if(parameters.ContainsKey("source") && parameters.ContainsKey("target"))
            {
                await InitializeForClanComparison(parameters["source"].ToString(), parameters["target"].ToString());
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);
                await _navigationService.GoBackAsync();
            }

            UpdateChartsView();

            base.OnNavigatedTo(parameters);
        } 
        #endregion
    }
}