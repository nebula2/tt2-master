using Microcharts;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TT2Master.Loggers;
using TT2Master.Model.Raid;
using TT2Master.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.Clipboard;
using TT2Master.Helpers;
using Newtonsoft.Json.Serialization;
using ImTools;
using TT2Master.Model.Drawing;
using TT2Master.ExtensionMethods;
using TT2Master.Shared.Helper;
using Microsoft.AppCenter.Analytics;
using TT2Master.Shared.Models;

namespace TT2Master.ViewModels.Raid
{

    public class ClanRaidDetailViewModel : ViewModelBase
    {
        #region Properties
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;

        private ClanRaid _currentRaid = new ClanRaid();
        /// <summary>
        /// The current config
        /// </summary>
        public ClanRaid CurrentRaid
        {
            get => _currentRaid;
            set
            {
                if (value != _currentRaid)
                {
                    SetProperty(ref _currentRaid, value);
                }
            }
        }

        #region General
        private List<RaidStrategy> _availableRaidStrategies;

        private ObservableCollection<RaidTolerance> _availableTolerances;
        public ObservableCollection<RaidTolerance> AvailableTolerances { get => _availableTolerances; set => SetProperty(ref _availableTolerances, value); }

        private List<int> _availableTiers;
        /// <summary>
        /// Available Tiers
        /// </summary>
        public List<int> AvailableTiers { get => _availableTiers; set => SetProperty(ref _availableTiers, value); }

        private List<int> _availableLevels;
        /// <summary>
        /// Available levels
        /// </summary>
        public List<int> AvailableLevels { get => _availableLevels; set => SetProperty(ref _availableLevels, value); }

        private string _enemy1;
        public string Enemy1 { get => _enemy1; set => SetProperty(ref _enemy1, value); }

        private string _enemy2;
        public string Enemy2 { get => _enemy2; set => SetProperty(ref _enemy2, value); }

        private string _enemy3;
        public string Enemy3 { get => _enemy3; set => SetProperty(ref _enemy3, value); }

        private bool _isEnemy1Visible;
        public bool IsEnemy1Visible { get => _isEnemy1Visible; set => SetProperty(ref _isEnemy1Visible, value); }

        private bool _isEnemy2Visible;
        public bool IsEnemy2Visible { get => _isEnemy2Visible; set => SetProperty(ref _isEnemy2Visible, value); }

        private bool _isEnemy3Visible;
        public bool IsEnemy3Visible { get => _isEnemy3Visible; set => SetProperty(ref _isEnemy3Visible, value); }

        private ObservableCollection<string> _strategiesForEnemy1;
        public ObservableCollection<string> StrategiesForEnemy1 { get => _strategiesForEnemy1; set => SetProperty(ref _strategiesForEnemy1, value); }

        private ObservableCollection<string> _strategiesForEnemy2;
        public ObservableCollection<string> StrategiesForEnemy2 { get => _strategiesForEnemy2; set => SetProperty(ref _strategiesForEnemy2, value); }

        private ObservableCollection<string> _strategiesForEnemy3;
        public ObservableCollection<string> StrategiesForEnemy3 { get => _strategiesForEnemy3; set => SetProperty(ref _strategiesForEnemy3, value); }

        private string _strategy1Name;
        public string Strategy1Name { get => _strategy1Name; set => SetProperty(ref _strategy1Name, value); }

        private string _strategy2Name;
        public string Strategy2Name { get => _strategy2Name; set => SetProperty(ref _strategy2Name, value); }

        private string _strategy3Name;
        public string Strategy3Name { get => _strategy3Name; set => SetProperty(ref _strategy3Name, value); }
        #endregion

        #region Analysis
        private RaidAnalysisHeaderData _analysisHeaderData = new RaidAnalysisHeaderData();
        public RaidAnalysisHeaderData AnalysisHeaderData { get => _analysisHeaderData; set => SetProperty(ref _analysisHeaderData, value); }

        private ObservableCollection<ClanRaidAttackFlaw> _attackFlaws = new ObservableCollection<ClanRaidAttackFlaw>();
        public ObservableCollection<ClanRaidAttackFlaw> AttackFlaws { get => _attackFlaws; set => SetProperty(ref _attackFlaws, value); }

        private ObservableCollection<GroupedClanRaidAttackFlaw> _attackFlawGrouping;
        public ObservableCollection<GroupedClanRaidAttackFlaw> AttackFlawGrouping { get => _attackFlawGrouping; set => SetProperty(ref _attackFlawGrouping, value); }

        #endregion

        #region Details
        private ObservableCollection<RaidResultAnalysisEntry> _raidResultAnalysisList;
        public ObservableCollection<RaidResultAnalysisEntry> RaidResultAnalysisList { get => _raidResultAnalysisList; set => SetProperty(ref _raidResultAnalysisList, value); }
        #endregion

        public ICommand SaveCommand { get; private set; }
        public ICommand PasteResultCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand ReloadAnalysisCommand { get; private set; }
        public ICommand InfoCommand { get; private set; }

        #endregion

        #region Ctor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public ClanRaidDetailViewModel(INavigationService navigationService, IPageDialogService dialogService) : base(navigationService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = AppResources.Analysis;

            InitializeCommands();
        }
        #endregion

        #region Private Methods

        #region Helper
        private void InitializeCommands()
        {
            SaveCommand = new DelegateCommand(async () => await SaveConfigurationAsync());
            PasteResultCommand = new DelegateCommand(async () => await PasteResultsAsync());
            ExportCommand = new DelegateCommand(async () => await ExportAsync());
            ReloadAnalysisCommand = new DelegateCommand(async () => await ReloadAnalysisAsync());
            InfoCommand = new DelegateCommand(async () => await ShowInfoAsync());
        }

        private void ResetCurrentRaidStrategies()
        {
            CurrentRaid.Strategies = new List<RaidStrategy>();
            if (IsEnemy1Visible)
            {
                CurrentRaid.Strategies.Add(_availableRaidStrategies.Where(x => x.Name == Strategy1Name).First());
            }
            if (IsEnemy2Visible)
            {
                CurrentRaid.Strategies.Add(_availableRaidStrategies.Where(x => x.Name == Strategy2Name).First());
            }
            if (IsEnemy3Visible)
            {
                CurrentRaid.Strategies.Add(_availableRaidStrategies.Where(x => x.Name == Strategy3Name).First());
            }
        }

        private List<OffStratAttacker> GetOffStratAttackers()
        {
            var idiots = CurrentRaid.Result.Where(x => x.IsOneOfWorstOverkills).ToList();

            var result = new List<OffStratAttacker>();

            foreach (var item in idiots)
            {
                if (item.OverkillHead > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.Head,
                        DamageDealt = (int)item.OverkillHead,
                    });
                }

                if (item.OverkillLeftArm > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.RightShoulder,
                        DamageDealt = (int)item.OverkillLeftArm,
                    });
                }

                if (item.OverkillRightArm > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.LeftShoulder,
                        DamageDealt = (int)item.OverkillRightArm,
                    });
                }

                if (item.OverkillLeftHand > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.RightHand,
                        DamageDealt = (int)item.OverkillRightHand,
                    });
                }

                if (item.OverkillRightHand > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.LeftHand,
                        DamageDealt = (int)item.OverkillRightHand,
                    });
                }

                if (item.OverkillTorso > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.Torso,
                        DamageDealt = (int)item.OverkillTorso,
                    });
                }

                if (item.OverkillLeftLeg > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.RightLeg,
                        DamageDealt = (int)item.OverkillLeftLeg,
                    });
                }

                if (item.OverkillRightLeg > 0)
                {
                    result.Add(new OffStratAttacker
                    {
                        Player = item.PlayerName,
                        TitanDamage = (int)item.TitanDamage,
                        OffStratDamage = (int)item.OverkillAmount,
                        OffStratDamagePercent = item.OverkillAmount * 100 / item.TitanDamage,
                        TitanNumber = item.TitanNumber,
                        TitanName = item.TitanName,
                        BodyPart = AppResources.LeftLeg,
                        DamageDealt = (int)item.OverkillRightLeg,
                    });
                }
            }

            result = result.OrderByDescending(x => x.OffStratDamage).ThenByDescending(x => x.DamageDealt).ToList();

            return result;
        }
        #endregion

        #region command methods
        private async Task<bool> ExportAsync()
        {
            try
            {
                // promt user
                string response = await _dialogService.DisplayActionSheetAsync(AppResources.WhatDoYouWant
                , AppResources.CancelText
                , AppResources.DestroyText
                , new string[] {
                    AppResources.ExportRaidTacticsImage
                    , AppResources.ExportRaidAnalysisImage
                    , AppResources.ExportRaidResultToClipboard
                    , AppResources.ExportRaidResultToCsvFile
                    , AppResources.ExportRaidResultToShare
                    , AppResources.ExportPartsAnalysis

                    , AppResources.ExportDetailsToClipboard
                    , AppResources.ExportDetailsToCsvFile
                    , AppResources.ExportDetailsToShare
                });

                // handle user cancel
                if (response == AppResources.CancelText || response == AppResources.DestroyText)
                {
                    return false;
                }

                if (response == AppResources.ExportRaidAnalysisImage)
                {
                    try
                    {
                        if (!GetRaidIsAnalyzable())
                        {
                            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.FillAllFieldsFirst, AppResources.OKText);
                            return false;
                        }

                        ResetCurrentRaidStrategies();

                        if (CurrentRaid == null)
                        {
                            throw new Exception($"{nameof(CurrentRaid)} is null");
                        }
                        if (AnalysisHeaderData == null)
                        {
                            throw new Exception($"{nameof(AnalysisHeaderData)} is null");
                        }
                        if (AttackFlawGrouping == null)
                        {
                            throw new Exception($"{nameof(AttackFlawGrouping)} is null");
                        }
                        if (RaidResultAnalysisList == null)
                        {
                            throw new Exception($"{nameof(RaidResultAnalysisList)} is null");
                        }

                        var drawer = new RaidAnalysisDrawer(CurrentRaid
                                        , AnalysisHeaderData
                                        , AttackFlawGrouping.ToList()
                                        , RaidResultAnalysisList.ToList());

                        drawer.OnLogMePlease += Drawer_OnLogMePlease;
                        drawer.Draw();

                        var result = await drawer.SaveImage();
                        drawer.OnLogMePlease -= Drawer_OnLogMePlease;

                        await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + result.Item2, AppResources.OKText);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLogFile($"ERROR: ExportAsync Image: {ex.Message}\n{ex.Data}\n{ex.StackTrace}");
                        await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                        return false;
                    }
                }

                if (response == AppResources.ExportPartsAnalysis)
                {
                    try
                    {
                        if (!GetRaidIsAnalyzable())
                        {
                            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.FillAllFieldsFirst, AppResources.OKText);
                            return false;
                        }

                        ResetCurrentRaidStrategies();

                        var offStrats = GetOffStratAttackers();

                        await FileHelper.WriteAndShareFileAsCsvAsync(offStrats, $"off_strat_{CurrentRaid.Tier}-{CurrentRaid.Level}.csv");

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLogFile($"ERROR: ExportAsync Image: {ex.Message}\n{ex.Data}\n{ex.StackTrace}");
                        await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                        return false;
                    }
                }

                if (response == AppResources.ExportRaidTacticsImage)
                {
                    try
                    {
                        if (!GetRaidIsTacticsReady())
                        {
                            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.FillAllStrategiesAndTolerancesFirst, AppResources.OKText);
                            return false;
                        }

                        ResetCurrentRaidStrategies();

                        var drawer = new RaidTacticsDrawer(CurrentRaid);

                        drawer.Draw();

                        var result = await drawer.SaveImage();

                        await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + result.Item2, AppResources.OKText);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLogFile($"ERROR: ExportAsync ExportRaidTacticsImage: {ex.Message}\n{ex.Data}\n{ex.StackTrace}");
                        await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                        return false;
                    }
                }

                if (CurrentRaid.Result == null || CurrentRaid.Result.Count() == 0)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.YouNeedToAddRaidResultsBefore, AppResources.OKText);
                    return false;
                }

                string rawCsv = "";
                string fileName = "";

                if (response == AppResources.ExportRaidResultToClipboard
                    || response == AppResources.ExportRaidResultToCsvFile
                    || response == AppResources.ExportRaidResultToShare)
                {
                    rawCsv = CurrentRaid.Result.ToCsv();
                    fileName = $"raidResult_{CurrentRaid.Tier}-{CurrentRaid.Level}.csv";
                }
                else if (response == AppResources.ExportDetailsToClipboard
                    || response == AppResources.ExportDetailsToCsvFile
                    || response == AppResources.ExportDetailsToShare)
                {
                    rawCsv = RaidResultAnalysisList.ToCsv();
                    fileName = $"raidDetail_{CurrentRaid.Tier}-{CurrentRaid.Level}.csv";
                }

                if (response == AppResources.ExportRaidResultToClipboard
                    || response == AppResources.ExportDetailsToClipboard)
                {
                    CrossClipboard.Current.SetText(rawCsv);
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.Copied, AppResources.OKText);
                }
                else if (response == AppResources.ExportRaidResultToCsvFile
                    || response == AppResources.ExportDetailsToCsvFile)
                {
                    var path = FileHelper.WriteFileToDownloads(rawCsv, fileName);
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ExportedToText + $" {path}", AppResources.OKText);
                }
                else if (response == AppResources.ExportRaidResultToShare
                    || response == AppResources.ExportDetailsToShare)
                {
                    await FileHelper.WriteAndShareFileAsync(rawCsv, fileName);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: ExportAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);

                return false;
            }
        }

        private async Task<bool> PasteResultsAsync()
        {
            try
            {
                if (!CurrentRaid.IsSaved)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.SaveFirstText, AppResources.OKText);
                    return false;
                }

                var text = await Clipboard.GetTextAsync();

                if (string.IsNullOrEmpty(text))
                {
#if DEBUG
                    text = @"PlayerName,PlayerCode,TotalRaidAttacks,TitanNumber,TitanName,TitanDamage,ArmorHead,ArmorTorso,ArmorLeftArm,ArmorRightArm,ArmorLeftHand,ArmorRightHand,ArmorLeftLeg,ArmorRightLeg,BodyHead,BodyTorso,BodyLeftArm,BodyRightArm,BodyLeftHand,BodyRightHand,BodyLeftLeg,BodyRightLeg,SkeletonHead,SkeletonTorso,SkeletonLeftArm,SkeletonRightArm,SkeletonLeftHand,SkeletonRightHand,SkeletonLeftLeg,SkeletonRightLeg
{D}Kaempes,mgmm7eb,27,0,Jukk the Overseer,40854322,0,2645,0,0,0,0,12865723,1075938,1927629,0,2281511,2342679,2194279,2301525,15191423,670967,0,0,0,0,0,0,0,0
{D}Kaempes,mgmm7eb,27,1,Takedar the Reborn,45269289,709648,8790617,581305,766741,478069,737336,2030733,2818525,12475468,7977204,1476429,814828,861309,1154430,1231925,2361341,0,0,0,0,0,0,100,3272
{D}Kaempes,mgmm7eb,27,2,Mohaca the Gale,47403134,985543,10940,514240,668148,715404,678590,8140336,193838,1476408,0,1423853,4644374,3976404,1236769,19780503,2954438,0,0,0,0,3339,0,0,0
{D}Sundancee,7rde459,24,0,Jukk the Overseer,39543079,8394526,101040,1165345,3281525,474520,393768,267024,1283845,6904762,0,1875608,1830259,1632037,2033060,8997485,908268,0,0,0,0,0,0,0,0
{D}Sundancee,7rde459,24,1,Takedar the Reborn,41296860,4857686,21265799,353195,428960,430770,457991,600956,744462,2165015,0,1603416,2141886,1836569,1943808,1416189,1050151,0,0,0,0,0,0,0,0
{D}Sundancee,7rde459,24,2,Mohaca the Gale,38321176,12620944,75416,1095032,1249396,1055333,1269451,1101250,907039,8603999,0,1153394,1430503,899446,1936126,4119949,803891,0,0,0,0,0,0,0,0
{D} Party (11.1),8b5yb,28,0,Jukk the Overseer,34855946,0,154272,0,0,0,0,0,326303,19855592,0,5789844,1123907,4953249,1250426,922348,480001,0,0,0,0,0,0,0,0
{D} Party (11.1),8b5yb,28,1,Takedar the Reborn,35589841,9845161,12005171,367806,3103910,650147,1948616,693265,466549,2735991,0,701704,789064,424419,479775,744224,634033,0,0,0,0,0,0,0,0
{D} Party (11.1),8b5yb,28,2,Mohaca the Gale,47001172,9622851,9800,2053913,521010,2148614,739380,295901,674495,841137,0,558255,757723,693207,1002279,26711339,371263,0,0,0,0,0,0,0,0
{D} Tobstar,wqwb49,24,0,Jukk the Overseer,38719996,7356070,46899,799788,699810,753542,527514,4985662,696963,8861459,0,2160241,1818334,1535675,1714800,5550881,1212352,0,0,0,0,0,0,0,0
{D} Tobstar,wqwb49,24,1,Takedar the Reborn,40635245,7980793,5217082,1475626,1206007,754132,1155279,794416,939408,1668456,1642676,1331606,804390,692961,1057912,12526639,1387855,0,0,0,0,0,0,0,0
{D} Tobstar,wqwb49,24,2,Mohaca the Gale,36192052,975763,6826,1552181,1606703,1200398,1188263,10375448,961720,1262799,0,1686847,1415572,7629303,963091,4177840,1189292,0,0,0,0,0,0,0,0
{D}PRÄTORIANERIN,ek38mb7,24,0,Jukk the Overseer,38048799,7711288,15041,568186,2939018,401500,1784326,507947,5137233,7712054,0,1686688,3393005,2387443,2029169,1313980,461352,0,0,564,0,0,0,0,0
{D}PRÄTORIANERIN,ek38mb7,24,1,Takedar the Reborn,38632691,7223464,11572870,676593,3584505,511183,1807578,268453,3770493,0,4983803,0,2362734,0,1871009,0,0,0,0,0,0,0,0,0,0
{D}PRÄTORIANERIN,ek38mb7,24,2,Mohaca the Gale,35497111,421789,274949,2231532,798759,1994918,567002,6735998,4132397,1158456,0,1294152,4450880,1454843,1568507,816644,7595577,701,0,0,0,0,0,0,0
{D} VolleWindel,eqq6m,24,0,Jukk the Overseer,36327011,4011586,63572,262720,436722,389652,684125,495730,10501505,695455,0,661979,770148,798122,652636,620962,15282090,0,0,0,0,0,0,0,0
{D} VolleWindel,eqq6m,24,1,Takedar the Reborn,43107984,1056369,3813513,0,0,0,0,276974,4316731,8320976,4585957,880906,1032385,7602311,2537901,789630,7894324,0,0,0,0,0,0,0,0
{D} VolleWindel,eqq6m,24,2,Mohaca the Gale,31765069,921283,62400,620118,397591,851215,635755,369588,9789302,2102057,0,954299,1297444,861143,4625152,816827,7460889,0,0,0,0,0,0,0,0
{D} LiLVanny,x8x6b5,20,0,Jukk the Overseer,40356653,0,143134,0,0,0,0,0,5155459,19283204,0,1701653,1816118,3561272,1862310,2942854,3889869,0,0,778,0,0,0,0,0
{D} LiLVanny,x8x6b5,20,1,Takedar the Reborn,43439866,6656810,2495280,0,0,0,0,0,392017,18050162,1103235,4692819,641290,3344278,793582,4110784,1159604,0,0,0,0,0,0,0,0
{D} LiLVanny,x8x6b5,20,2,Mohaca the Gale,18396628,0,85865,0,0,0,0,1282328,8302596,899198,0,1884207,500440,1757438,1138778,1809125,736648,0,0,0,0,0,0,0,0
{D} Ahmon-shi,wkb7q8,23,0,Jukk the Overseer,30143050,356390,208438,1025219,1026840,792032,743542,8025199,1631940,553288,0,981502,6743299,714502,954573,5396793,988500,0,0,0,985,0,0,0,0
{D} Ahmon-shi,wkb7q8,23,1,Takedar the Reborn,34804912,6447995,4077826,996865,1016053,919416,1123452,995128,789819,5779318,12659035,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Ahmon-shi,wkb7q8,23,2,Mohaca the Gale,33098042,610599,293983,1052788,1500440,1120516,4067244,6652755,681693,998753,0,2968906,1641626,1144197,2087796,7056671,1220068,0,0,0,0,0,0,0,0
{D} J4bster,pd2kepr,22,0,Jukk the Overseer,35171114,3776725,3837,0,9108697,0,3395185,0,0,3529954,0,833695,12102829,858191,597265,492614,472116,0,0,0,0,0,0,0,0
{D} J4bster,pd2kepr,22,1,Takedar the Reborn,32127730,3074383,0,0,6671506,0,3261206,0,0,3256986,15863646,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} J4bster,pd2kepr,22,2,Mohaca the Gale,29398553,448210,1905,7610686,396850,2308228,3148086,552690,523084,0,0,121489,11821324,705435,1293986,263441,203132,0,0,0,0,0,0,0,0
{D} Greed,gm8er93,26,0,Jukk the Overseer,23383129,5711794,9189,2323939,519139,2480265,1953988,488818,248750,0,0,0,0,250653,470592,8925997,0,0,0,0,0,0,0,0,0
{D} Greed,gm8er93,26,1,Takedar the Reborn,31077233,5533701,3466856,466096,376308,354664,429307,295880,4134761,12402425,3617230,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Greed,gm8er93,26,2,Mohaca the Gale,42202191,9183713,423832,520022,459114,467513,436985,556554,3139020,9147377,0,494156,304254,1444426,582701,1394561,13647956,0,0,0,0,0,0,0,0
{D}Kenpatchi,pg6bdgr,24,0,Jukk the Overseer,14963593,0,2628,0,0,0,0,0,0,9101469,0,563634,466257,3931948,293694,603959,0,0,0,0,0,0,0,0,0
{D}Kenpatchi,pg6bdgr,24,1,Takedar the Reborn,47587340,17718619,8668938,764464,1124848,491840,700707,904125,616305,9056271,4436750,138826,1119501,220200,606933,359572,659435,0,0,0,0,0,0,0,0
{D}Kenpatchi,pg6bdgr,24,2,Mohaca the Gale,29585872,8230816,66194,0,0,0,0,0,0,10193732,0,1285469,891557,6442663,1278993,664110,532334,0,0,0,0,0,0,0,0
{D}Dom,x4n5kb3,26,0,Jukk the Overseer,27845826,3331698,667,5729147,395897,260368,312469,4020092,314951,3251432,0,0,0,8942586,476875,414254,394816,0,0,233,333,0,0,0,0
{D}Dom,x4n5kb3,26,1,Takedar the Reborn,29315093,6055881,490896,808443,647295,1109129,0,409245,555463,841971,352263,1049001,9745801,3959078,1571347,825928,893345,0,0,0,0,0,0,0,0
{D}Dom,x4n5kb3,26,2,Mohaca the Gale,33826036,5245197,38683,478943,3679115,713485,851644,850337,1465008,7864789,0,628086,819765,747880,701293,4112414,5629390,0,0,0,0,0,0,0,0
{D} Torti,mymgw9y,28,0,Jukk the Overseer,20401351,5062459,61747,297741,685755,382357,3104626,265267,2668879,0,0,0,0,0,0,3766304,4106211,0,0,0,0,0,0,0,0
{D} Torti,mymgw9y,28,1,Takedar the Reborn,30190978,8099689,4349623,647192,573102,703127,684600,425337,1163191,5105233,3974290,279491,449274,359307,476532,224038,2676946,0,0,0,0,0,0,0,0
{D} Torti,mymgw9y,28,2,Mohaca the Gale,35750438,5118278,229958,638202,616915,314220,591928,2420993,2794552,4773062,0,198817,2887765,5115604,486787,2851796,6711491,0,0,0,0,0,61,0,0
{D} Oggy,8grrywe,24,0,Jukk the Overseer,28446231,6204450,3865,3013400,824325,2245121,585412,603239,681563,6590330,0,2813462,705234,2279465,584295,680202,631864,0,0,0,0,0,0,0,0
{D} Oggy,8grrywe,24,1,Takedar the Reborn,30218898,2559243,5984998,0,0,0,0,3727170,352836,911573,6365228,7008714,937696,746780,756794,478545,389315,0,0,0,0,0,0,0,0
{D} Oggy,8grrywe,24,2,Mohaca the Gale,27398488,0,16734,0,0,0,0,393149,7771556,8707216,0,960280,846187,1149657,4319249,3011037,223419,0,0,0,0,0,0,0,0
{D}Flx,kmyr5bp,25,0,Jukk the Overseer,21839367,520791,121955,576049,473406,415180,8950086,555907,479448,0,0,344034,313515,299607,261917,8527466,0,0,0,0,0,0,0,0,0
{D}Flx,kmyr5bp,25,1,Takedar the Reborn,32926948,7069014,431843,1445235,1712282,1704737,5072566,1262703,1449118,5951446,6827998,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}Flx,kmyr5bp,25,2,Mohaca the Gale,29510155,882523,27092,1125431,705933,5687305,955016,3373276,868390,0,0,1241893,1068329,844935,9231459,992259,2505396,911,0,0,0,0,0,0,0
{D} Snake90DD,kb52mw,24,0,Jukk the Overseer,14171775,5036068,8942,4908034,641657,3160849,416223,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Snake90DD,kb52mw,24,1,Takedar the Reborn,44423110,9543869,1079418,8259762,1019402,7016208,613502,553845,625215,6925849,8786035,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Snake90DD,kb52mw,24,2,Mohaca the Gale,24876004,4114870,47396,4411251,404069,2965126,265345,524176,465767,0,0,4884630,490583,4610501,404293,756850,531142,0,0,0,0,0,0,0,0
{D}Asklepie,49352e3,20,0,Jukk the Overseer,33622340,0,5670,0,0,0,0,0,4416687,18338322,0,3367918,0,36511,11239,0,7445990,0,0,0,0,0,0,0,0
{D}Asklepie,49352e3,20,1,Takedar the Reborn,17956373,0,0,0,0,0,0,0,0,14568644,3387729,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}Asklepie,49352e3,20,2,Mohaca the Gale,29730047,12217881,644,0,0,0,0,0,0,9138284,0,5446899,2906310,20025,0,0,0,0,0,0,0,0,0,0,0
{D}alphacentauri,7nd8my,24,0,Jukk the Overseer,25356432,2940080,644,413274,587644,8394774,282653,380129,424119,5351257,0,0,0,0,0,5880225,697683,3945,0,0,0,0,0,0,0
{D}alphacentauri,7nd8my,24,1,Takedar the Reborn,28909807,3407773,658258,436078,392045,5115291,314006,3703556,354942,7885148,6642705,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}alphacentauri,7nd8my,24,2,Mohaca the Gale,26673597,7811846,0,559287,623546,3229067,440145,459462,315359,7606959,0,3280516,539191,371721,552003,484298,400190,0,0,0,0,0,0,0,0
{D} Aldamon,4569qby,28,0,Jukk the Overseer,23065785,7123807,97630,322366,381795,2935525,192586,333801,292812,6972379,0,306908,349666,2865853,205636,459532,225485,0,0,0,0,0,0,0,0
{D} Aldamon,4569qby,28,1,Takedar the Reborn,32375221,6869986,473971,237694,303531,2994027,299559,414499,174516,3945249,9280717,0,0,0,0,0,7381127,0,0,0,0,0,340,0,0
{D} Aldamon,4569qby,28,2,Mohaca the Gale,23655572,418729,102891,425223,328810,441011,470313,423765,253844,6614408,0,1054436,753363,2696057,1121114,3635972,4915631,0,0,0,0,0,0,0,0
{D}r4i_,n5x5mdb,24,0,Jukk the Overseer,26506339,4875272,1009,2665622,4112079,432032,344324,399872,331007,7668573,0,0,3679157,1997387,0,0,0,0,0,0,0,0,0,0,0
{D}r4i_,n5x5mdb,24,1,Takedar the Reborn,27802783,5236221,3443414,4375567,291635,133334,311093,148301,471574,7538367,2348199,0,1368550,0,2127440,0,0,0,0,0,9083,0,0,0,0
{D}r4i_,n5x5mdb,24,2,Mohaca the Gale,24054206,6814503,10999,0,0,0,0,0,0,8416944,0,3574335,4219343,378373,639707,0,0,0,0,0,0,0,0,0,0
{D}Novum,drry54k,24,0,Jukk the Overseer,26823880,2763970,1212,588814,518228,408870,4854926,413341,3539124,2642314,0,0,0,553466,9345838,562865,630905,0,0,0,0,0,0,0,0
{D}Novum,drry54k,24,1,Takedar the Reborn,26649625,3221824,921819,445852,402673,364111,5023669,231232,3155213,591688,3475758,0,0,0,0,1914022,6901303,0,0,0,0,0,0,454,0
{D}Novum,drry54k,24,2,Mohaca the Gale,24198335,2658458,136006,559285,643764,2975731,5023008,331104,339697,1831653,0,732257,472257,610254,7378743,204117,300666,1328,0,0,0,0,0,0,0
{D} DieSensation,nxkq5q,24,0,Jukk the Overseer,27769168,0,0,0,0,0,0,2663408,0,5806564,0,2821954,3661485,0,4788649,0,8027107,0,0,0,0,0,0,0,0
{D} DieSensation,nxkq5q,24,1,Takedar the Reborn,25895771,2678606,3487344,5100,0,2487533,3510537,0,0,2812334,3070186,0,3075647,0,997329,0,3771152,0,0,0,0,0,0,0,0
{D} DieSensation,nxkq5q,24,2,Mohaca the Gale,23131092,2657461,1314,0,4515957,0,2750526,0,2369400,2569095,0,51460,0,3542231,2389413,0,2284231,0,0,0,0,0,0,0,0
{D}Stockchen,59knw9y,27,0,Jukk the Overseer,22633465,7493869,84004,798488,761519,391901,575688,384807,450568,7609150,0,714342,885975,609560,762410,538955,572221,0,0,0,0,0,0,0,0
{D}Stockchen,59knw9y,27,1,Takedar the Reborn,24439954,2904694,13098222,664564,702310,239416,335141,330417,562772,2249828,79151,605549,725594,518740,460080,561212,402258,0,0,0,0,0,0,0,0
{D}Stockchen,59knw9y,27,2,Mohaca the Gale,29173459,5953044,7368,962238,856952,436716,1146762,686708,871002,6628866,0,1111353,867873,489237,860817,2031638,6262394,0,0,0,0,0,482,0,0
{D}☆Coyote☆,9r5nkxp,24,0,Jukk the Overseer,12114382,2469463,75723,349996,377470,220879,366413,3259788,4994646,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}☆Coyote☆,9r5nkxp,24,1,Takedar the Reborn,36500583,751426,9324472,531161,651132,399478,280359,8035253,3169154,2517599,8496035,519609,293118,282515,585699,347522,316043,0,0,0,0,0,0,0,0
{D}☆Coyote☆,9r5nkxp,24,2,Mohaca the Gale,22192970,2452590,1132,389147,313715,389976,398071,275151,6949434,0,0,756687,2409964,245350,305707,4862826,2443214,0,0,0,0,0,0,0,0
{D} Smainchoker,9q7256b,20,0,Jukk the Overseer,28970962,478850,0,429174,330947,5240139,459023,4306533,3037665,5327856,0,514908,476469,507906,438382,4055792,3367312,0,0,0,0,0,0,0,0
{D} Smainchoker,9q7256b,20,1,Takedar the Reborn,27876098,4940409,4117272,407291,516981,218075,486156,457718,2966430,7854713,5911047,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Smainchoker,9q7256b,20,2,Mohaca the Gale,13110155,4881544,12709,1190291,1054494,851747,878215,3574451,666699,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}Kupferschmidt,qnwr2,20,0,Jukk the Overseer,27991310,3332946,338,3161859,0,0,898,0,7291093,3322417,0,7828757,182432,206889,122416,2541262,0,0,0,0,0,0,0,0,0
{D}Kupferschmidt,qnwr2,20,1,Takedar the Reborn,15743564,3144318,3506336,0,0,0,0,0,0,0,0,3289816,5798050,4027,1014,0,0,0,0,0,0,0,0,0,0
{D}Kupferschmidt,qnwr2,20,2,Mohaca the Gale,25781084,2790713,0,0,1993453,0,790886,0,8468563,109459,0,5765359,131986,550407,158354,0,5021900,0,0,0,0,0,0,0,0
{D} ElChico,47qwrqx,24,0,Jukk the Overseer,22783615,6347949,0,323743,453536,193939,419452,3007315,363361,6973116,0,233458,334151,287077,333172,3140815,372524,0,0,0,0,0,0,0,0
{D} ElChico,47qwrqx,24,1,Takedar the Reborn,24363249,4237383,3314787,181729,563670,150106,101882,3172749,560381,5249492,0,770470,597898,566029,519910,3524495,852260,0,0,0,0,0,0,0,0
{D} ElChico,47qwrqx,24,2,Mohaca the Gale,21489808,5661340,0,626299,562224,223615,402713,2544760,599854,5989943,0,592258,523599,573509,279971,485802,2423914,0,0,0,0,0,0,0,0
{D} ShurikenJr,yk46dn6,24,0,Jukk the Overseer,22989705,7042819,21212,291769,279268,276859,170716,222956,3013589,0,0,0,0,0,0,0,11670512,0,0,0,0,0,0,0,0
{D} ShurikenJr,yk46dn6,24,1,Takedar the Reborn,23368672,6448530,2641465,0,0,0,0,0,0,6700369,5571379,2006927,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} ShurikenJr,yk46dn6,24,2,Mohaca the Gale,20161182,0,0,0,0,0,0,1863364,1993987,12007366,0,486779,353009,267919,470459,349537,2368757,0,0,0,0,0,0,0,0
{D} ShinY,pk4d78,24,0,Jukk the Overseer,21594224,5841136,0,2118756,2839242,0,0,0,0,5798386,0,0,0,0,4996703,0,0,0,0,0,0,0,0,0,0
{D} ShinY,pk4d78,24,1,Takedar the Reborn,22909725,4758512,2320395,4050572,0,0,0,0,0,6202311,5577933,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} ShinY,pk4d78,24,2,Mohaca the Gale,19217633,7641205,278,0,0,0,0,1811861,0,5526695,0,4236117,0,1475,0,0,0,0,0,0,0,0,0,0,0
{D} Fenriz,22ep28m,24,0,Jukk the Overseer,21445090,2784746,99707,724757,1040816,642167,547596,557175,3689133,1027087,0,701223,833809,996329,454779,3068678,4277079,0,0,0,0,0,0,0,0
{D} Fenriz,22ep28m,24,1,Takedar the Reborn,20216687,781930,10790248,656451,678925,529857,486518,907194,490844,874375,361977,818964,376158,640691,494571,756631,571343,0,0,0,0,0,0,0,0
{D} Fenriz,22ep28m,24,2,Mohaca the Gale,20800713,915822,57813,665219,1005840,669868,601054,2446063,4239079,759888,0,816277,1133510,1026133,834114,2603694,3026331,0,0,0,0,0,0,0,0
{D} Quellwasser,drp846k,28,0,Jukk the Overseer,16667224,31229,689173,1245138,27137,972844,2551866,4318990,24177,0,0,0,0,0,0,4259414,2547169,0,0,0,0,82,0,0,0
{D} Quellwasser,drp846k,28,1,Takedar the Reborn,20943052,63251,102796,1334360,3296363,872299,2434133,1178774,86970,64330,3251718,1206109,64190,828772,3378311,2671692,108977,0,0,0,0,0,0,0,0
{D} Quellwasser,drp846k,28,2,Mohaca the Gale,22353841,50369,334494,1404329,3621209,1094547,48019,1152509,66152,52720,0,2020570,2497476,3422102,163185,6274310,151842,0,0,0,0,0,0,0,0
{D} Mavis,k8knqeq,20,0,Jukk the Overseer,24589435,7026565,288,0,0,0,0,0,0,9517729,0,510141,1812382,426386,4585830,432971,277138,0,0,0,0,0,0,0,0
{D} Mavis,k8knqeq,20,1,Takedar the Reborn,24525874,2450509,2999965,0,0,0,0,430726,4065740,2544052,3269752,604616,432384,668173,546873,2493762,4019289,0,0,0,0,0,28,0,0
{D} Mavis,k8knqeq,20,2,Mohaca the Gale,10421940,0,0,0,0,0,0,0,0,4670088,0,0,0,0,0,0,5751851,0,0,0,0,0,0,0,0
{D}R4uschI,93r8e,20,0,Jukk the Overseer,10252602,2939532,0,1459845,0,279361,0,5573862,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}R4uschI,93r8e,20,1,Takedar the Reborn,23924436,3509588,5371317,0,0,0,0,2393833,0,4431260,8218437,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}R4uschI,93r8e,20,2,Mohaca the Gale,20146821,3112948,3265,2301938,0,0,0,0,0,2734540,0,2411565,0,4836282,0,4746280,0,0,0,0,0,0,0,0,0
{D} Elenya,qm9k5eq,24,0,Jukk the Overseer,16567671,1959511,998,158025,3349477,181004,227639,169724,2414746,2063208,0,0,0,3610738,1892867,281506,257294,0,0,334,206,0,386,0,0
{D} Elenya,qm9k5eq,24,1,Takedar the Reborn,17988760,290115,467701,356254,346922,3338365,0,319519,301697,300502,410179,6636228,2820222,451514,846868,550376,552292,0,0,0,0,0,0,0,0
{D} Elenya,qm9k5eq,24,2,Mohaca the Gale,15404556,3421233,6700,336755,492612,381926,2032313,591666,356718,4940761,0,347439,505062,412491,476107,531874,570893,0,0,0,0,0,0,0,0
{D} LuckyLuke,5x98g6,12,0,Jukk the Overseer,31746682,42397,61349,0,0,0,0,453447,575831,14464107,0,11737627,892325,1666790,941380,549872,361553,0,0,0,0,0,0,0,0
{D} LuckyLuke,5x98g6,12,1,Takedar the Reborn,15629799,4418949,856821,470013,711347,330508,0,226431,288203,0,0,0,0,0,8327523,0,0,0,0,0,0,0,0,0,0
{D} LuckyLuke,5x98g6,12,2,Mohaca the Gale,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Sparta,4my8k,24,0,Jukk the Overseer,8656404,0,615,0,0,0,0,0,0,2057173,0,626091,1940970,590981,3440572,0,0,0,0,0,0,0,0,0,0
{D} Sparta,4my8k,24,1,Takedar the Reborn,17991960,2053428,429091,610511,2802024,172639,0,477462,308452,3368248,3901143,0,0,0,3364001,0,502127,0,0,31,815,0,1981,0,0
{D} Sparta,4my8k,24,2,Mohaca the Gale,19256081,1455490,1568,0,2803951,0,1528266,0,0,1325605,0,0,2007184,0,7768501,0,2358311,0,0,0,7200,0,0,0,0
{D} Waldbaum,bym3ny7,24,0,Jukk the Overseer,9227099,1881482,1352,347742,297218,228279,160926,4377046,198334,1734717,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Waldbaum,bym3ny7,24,1,Takedar the Reborn,22304837,442594,2233104,388142,475122,622031,464028,7486997,1971179,276245,2005718,340803,203720,357377,267191,4563600,206977,0,0,0,0,0,0,0,0
{D} Waldbaum,bym3ny7,24,2,Mohaca the Gale,14310515,1582588,29016,313337,236717,261769,275785,4127313,341253,0,0,313567,292506,341314,453760,5317912,423671,0,0,0,0,0,0,0,0
{D}Exolio,rn8pbd,14,0,Jukk the Overseer,9209732,4079320,0,0,0,0,0,0,0,0,0,0,0,0,0,4216419,913993,0,0,0,0,0,0,0,0
{D}Exolio,rn8pbd,14,1,Takedar the Reborn,13423974,351659,4525146,1992119,1754132,1800267,1964845,1035802,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}Exolio,rn8pbd,14,2,Mohaca the Gale,20187405,5986609,11291,2435922,0,2261709,0,1494731,0,0,0,1013756,1091601,1688793,1726561,2476427,0,0,0,0,0,0,0,0,0
{D} Lio,bgy6nn7,24,0,Jukk the Overseer,11977888,4217271,33925,507612,529988,408034,475834,457192,209822,3852711,0,0,0,278536,386221,303769,316703,0,0,243,21,0,0,0,0
{D} Lio,bgy6nn7,24,1,Takedar the Reborn,15065701,1872123,2607356,561919,726304,598120,722721,563212,510389,3955812,190760,443953,570204,456986,592995,352864,339977,0,0,0,0,0,0,0,0
{D} Lio,bgy6nn7,24,2,Mohaca the Gale,12510571,2510891,31124,389556,619247,398044,375127,422612,1456499,2349199,0,408366,1681858,530442,515935,420168,401496,0,0,0,0,0,0,0,0
{D} Uecco,n28p2g,16,0,Jukk the Overseer,19552602,6720996,108096,633075,775362,539106,604000,584843,423972,6809615,0,518287,390481,285716,510297,362817,285931,0,0,0,0,0,0,0,0
{D} Uecco,n28p2g,16,1,Takedar the Reborn,9669845,3897081,2701571,585139,760126,461289,466366,333850,464420,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Uecco,n28p2g,16,2,Mohaca the Gale,9723087,2265923,6612,668600,4562872,618471,650294,491590,458722,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Phil,3w7wm28,24,0,Jukk the Overseer,11517919,145709,591,602840,117948,4311362,112029,134005,134967,1182277,0,0,0,164444,1469546,3141927,0,0,0,0,0,266,0,0,0
{D} Phil,3w7wm28,24,1,Takedar the Reborn,19100605,1751864,0,2437723,0,0,0,0,1024985,3820713,5222631,669580,0,2693891,1478747,0,0,0,0,466,0,0,0,0,0
{D} Phil,3w7wm28,24,2,Mohaca the Gale,6877691,0,813,0,0,0,0,1280132,0,0,0,1180724,0,810836,1078545,1211,2520906,0,0,4040,0,0,479,0,0
{D} Ursaklein,n5dw8m6,16,0,Jukk the Overseer,9642592,7027961,0,0,0,0,2614631,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Ursaklein,n5dw8m6,16,1,Takedar the Reborn,18145817,4565210,1791487,0,0,0,2466109,0,0,4911228,2681216,0,0,1730565,0,0,0,0,0,0,0,0,0,0,0
{D} Ursaklein,n5dw8m6,16,2,Mohaca the Gale,8337619,6003709,0,0,0,0,2333909,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D}Chris,25r3gm,24,0,Jukk the Overseer,12267955,210893,4638,0,0,0,0,220501,2311993,294789,0,532981,645224,1509622,1863312,467627,4206370,0,0,0,0,0,0,0,0
{D}Chris,25r3gm,24,1,Takedar the Reborn,12276410,227286,3441655,0,0,0,0,227340,1151411,239052,3869605,736928,647880,403016,573886,414854,343491,0,0,0,0,0,0,0,0
{D}Chris,25r3gm,24,2,Mohaca the Gale,11226724,0,32373,0,0,0,1582859,234590,2550959,3465457,0,717664,696454,592596,469693,463267,420807,0,0,0,0,0,0,0,0
{D}GoldGates,9w3kp3q,12,0,Jukk the Overseer,12534164,0,608,0,0,0,0,2352741,4938377,2673294,0,0,2478466,0,30616,60059,0,0,0,0,0,0,0,0,0
{D}GoldGates,9w3kp3q,12,1,Takedar the Reborn,11680709,0,0,0,0,0,0,0,0,2550460,6133,0,0,4606694,0,4516461,0,0,0,0,0,959,0,0,0
{D}GoldGates,9w3kp3q,12,2,Mohaca the Gale,11074935,0,0,0,0,0,0,0,0,6483027,0,2442829,2149078,0,0,0,0,0,0,0,0,0,0,0,0
{D} Niack,dxqdp8,13,0,Jukk the Overseer,20835908,1883958,19879,0,0,0,0,6325738,373224,1996271,0,759692,905929,865580,837269,6466676,401686,0,0,0,0,0,0,0,0
{D} Niack,dxqdp8,13,1,Takedar the Reborn,1634678,0,0,0,0,0,0,0,0,1633618,1060,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} Niack,dxqdp8,13,2,Mohaca the Gale,11441461,474199,530,504668,299962,291056,418713,5838373,461996,0,0,0,0,0,0,3151960,0,0,0,0,0,0,0,0,0
{D} Redfox1607,4nmx8nq,16,0,Jukk the Overseer,7034573,2114364,12041,0,0,0,0,1647135,1972929,0,0,69323,42955,1138650,37173,0,0,0,0,0,0,0,0,0,0
{D} Redfox1607,4nmx8nq,16,1,Takedar the Reborn,13691499,1792338,2163181,241637,206297,1862286,153203,61857,160784,1551378,1942269,0,0,3556263,0,0,0,0,0,0,0,0,0,0,0
{D} Redfox1607,4nmx8nq,16,2,Mohaca the Gale,6741191,2128470,2628,176025,273097,1744931,270787,1949969,195280,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} BenFreaks,wq443pm,0,0,Jukk the Overseer,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} BenFreaks,wq443pm,0,1,Takedar the Reborn,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
{D} BenFreaks,wq443pm,0,2,Mohaca the Gale,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
#else 
                    return false;
#endif

                }

                var parser = new ClanRaidResultParser(App.DBRepo, CurrentRaid.ID, text);

                parser.OnLogMePlease += Parser_OnLogMePlease;
                parser.OnProblemHaving += async (s, e) => await Parser_OnProblemHaving(s, e);

                bool success = await parser.SaveRaidResultAsync();

                parser.OnLogMePlease -= Parser_OnLogMePlease;
                parser.OnProblemHaving -= async (s, e) => await Parser_OnProblemHaving(s, e);

                if (success)
                {
                    CurrentRaid.Result = await App.DBRepo.GetClanRaidResultsByParentID(CurrentRaid.ID);
                    await LoadLevelInformationAsync();
                    await LoadAnalysisDataAsync();

                    await ToastSender.SendToastAsync(AppResources.FinishedText, _dialogService);
                }

                return success;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: PaseResultsAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                return false;
            }
        }

        private async Task<bool> SaveConfigurationAsync()
        {
            try
            {
                // save item
                if (CurrentRaid.IsSaved)
                {
                    var saveAccepted = await _dialogService.DisplayAlertAsync(AppResources.OKText, AppResources.DoYouWantToOverwrite, AppResources.YesText, AppResources.NoText);

                    // if user does not want to overwrite -> do not save
                    if (!saveAccepted)
                    {
                        return false;
                    }

                    int updAmount = await App.DBRepo.UpdateClanRaidAsync(CurrentRaid);
                    updAmount += await UpdateRaidStrategiesInDbAsync();
                    updAmount += await UpdateRaidTolerancesInDbAsync();

                    Logger.WriteToLogFile($"updated configurations: {updAmount}");

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ChangesSavedText, AppResources.OKText);
                }
                else
                {
                    // set date if needed
                    if (!CurrentRaid.IsSaved)
                    {
                        CurrentRaid.CreatedDate = DateTime.Now;
                    }

                    int confSavedCount = await App.DBRepo.AddClanRaidAsync(CurrentRaid);
                    confSavedCount += await UpdateRaidStrategiesInDbAsync();
                    confSavedCount += await UpdateRaidTolerancesInDbAsync();

                    Logger.WriteToLogFile($"Saved configurations: {confSavedCount}");

                    CurrentRaid.IsSaved = true;

                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ChangesSavedText, AppResources.OKText);
                }

                // load level info just in case this has not happened already
                if (CurrentRaid.Level > 0 && CurrentRaid.Tier > 0)
                {
                    await LoadLevelInformationAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToLogFile($"Error saving configuration: {e.Message}\n{e.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);

                return false;
            }
        }

        private async Task<bool> ReloadAnalysisAsync()
        {
            try
            {
                if (!CurrentRaid.IsSaved)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.SaveFirstText, AppResources.OKText);
                    return false;
                }

                if (!(CurrentRaid.Result.Count() > 0))
                {
                    await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.YouNeedToAddRaidResultsBefore, AppResources.OKText);
                    return false;
                }

                await LoadAnalysisDataAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"Error ReloadAnalysisAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);

                return false;
            }
        }

        private async Task<bool> ShowInfoAsync()
        {
            try
            {
                await _dialogService.DisplayAlertAsync(AppResources.InfoHeader, AppResources.ClanRaidDetailInformations, AppResources.OKText);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"Error ReloadAnalysisAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, AppResources.ErrorOccuredText, AppResources.OKText);

                return false;
            }
        }
        #endregion

        #region Level and tier info
        private List<int> GetAvailableTiers() => RaidInfoHandler.LevelInfos.Select(x => x.TierID).Distinct().ToList();

        private List<int> GetAvailableLevels() => RaidInfoHandler.LevelInfos.Where(x => x.TierID == CurrentRaid.Tier).Select(x => x.LevelID).ToList();

        private async Task LoadLevelInformationAsync()
        {
            try
            {
                CurrentRaid.LevelInfo = RaidInfoHandler.LevelInfos.Where(x => x.TierID == CurrentRaid.Tier && x.LevelID == CurrentRaid.Level).FirstOrDefault();

                ExtractEnemiesFromLevelInfo();

                if (CurrentRaid.IsSaved)
                {
                    await LoadStrategyInformationFromDb();
                }
                else
                {
                    await LoadAvailableStrategies();
                }

                MakeStrategiesValid();
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: LoadLevelInformation: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
            }
        }

        private async Task LoadRaidInformation()
        {
            try
            {
                // raid result
                CurrentRaid.Result = await App.DBRepo.GetClanRaidResultsByParentID(CurrentRaid.ID);

                // level info
                await LoadLevelInformationAsync();

                // strategies
                await LoadStrategyInformationFromDb();

                // tolerances
                await LoadToleranceInformationFromDbAsync();

                // analysis
                await LoadAnalysisDataAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: LoadRaidInformation: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
            }
        }

        private void ExtractEnemiesFromLevelInfo()
        {
            if (CurrentRaid.LevelInfo == null)
            {
                return;
            }

            string[] enemyArray = CurrentRaid.LevelInfo.EnemyIDs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < enemyArray.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Enemy1 = RaidInfoHandler.EnemyInfos.Where(x => x.EnemyId == enemyArray[i]).FirstOrDefault()?.Name;
                        if (!string.IsNullOrEmpty(Enemy1))
                        {
                            IsEnemy1Visible = true;
                        }
                        break;
                    case 1:
                        Enemy2 = RaidInfoHandler.EnemyInfos.Where(x => x.EnemyId == enemyArray[i]).FirstOrDefault()?.Name;
                        if (!string.IsNullOrEmpty(Enemy2))
                        {
                            IsEnemy2Visible = true;
                        }
                        break;
                    case 2:
                        Enemy3 = RaidInfoHandler.EnemyInfos.Where(x => x.EnemyId == enemyArray[i]).FirstOrDefault()?.Name;
                        if (!string.IsNullOrEmpty(Enemy3))
                        {
                            IsEnemy3Visible = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Strategy
        private async Task<int> UpdateRaidStrategiesInDbAsync()
        {
            try
            {
                // delete old ones
                await App.DBRepo.DeleteClanRaidEnemyStrategiesByClanRaidId(CurrentRaid.ID);

                int amount = 0;

                // insert new ones
                if (Strategy1Name != null)
                {
                    amount += await App.DBRepo.AddClanRaidEnemyStrategyAsync(new ClanRaidEnemyStrategy { ClanRaidId = CurrentRaid.ID, RaidStrategyId = Strategy1Name });
                }
                if (Strategy2Name != null)
                {
                    amount += await App.DBRepo.AddClanRaidEnemyStrategyAsync(new ClanRaidEnemyStrategy { ClanRaidId = CurrentRaid.ID, RaidStrategyId = Strategy2Name });
                }
                if (Strategy3Name != null)
                {
                    amount += await App.DBRepo.AddClanRaidEnemyStrategyAsync(new ClanRaidEnemyStrategy { ClanRaidId = CurrentRaid.ID, RaidStrategyId = Strategy3Name });
                }

                return amount;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: UpdateRaidStrategiesInDbAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                return 0;
            }
        }

        private async Task<int> UpdateRaidTolerancesInDbAsync()
        {
            try
            {
                // delete old ones
                await App.DBRepo.DeleteClanRaidToleranceRelationshipByClanRaidId(CurrentRaid.ID);

                int amount = 0;

                // insert new ones
                if (CurrentRaid.Tolerance == null || string.IsNullOrEmpty(CurrentRaid.Tolerance.Name))
                {
                    return 0;
                }

                amount += await App.DBRepo.AddClanRaidToleranceRelationshipAsync(new ClanRaidToleranceRelationship
                {
                    ClanRaidId = CurrentRaid.ID,
                    RaidToleranceId = CurrentRaid.Tolerance.Name,
                });

                return amount;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: UpdateRaidTolerancesInDbAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
                return 0;
            }
        }

        private void MakeStrategiesValid()
        {
            if (!string.IsNullOrEmpty(Strategy1Name) && !string.IsNullOrEmpty(Enemy1))
            {
                // get strategy
                var strat = _availableRaidStrategies.Where(x => x.Name == Strategy1Name).FirstOrDefault();

                if (strat == null)
                {
                    Strategy1Name = null;
                }
                else
                {
                    if (strat.EnemyName != Enemy1)
                    {
                        Strategy1Name = null;
                    }
                }
            }

            if (!string.IsNullOrEmpty(Strategy2Name) && !string.IsNullOrEmpty(Enemy2))
            {
                // get strategy
                var strat = _availableRaidStrategies.Where(x => x.Name == Strategy2Name).FirstOrDefault();

                if (strat == null)
                {
                    Strategy2Name = null;
                }
                else
                {
                    if (strat.EnemyName != Enemy2)
                    {
                        Strategy2Name = null;
                    }
                }
            }

            if (!string.IsNullOrEmpty(Strategy3Name) && !string.IsNullOrEmpty(Enemy3))
            {
                // get strategy
                var strat = _availableRaidStrategies.Where(x => x.Name == Strategy3Name).FirstOrDefault();

                if (strat == null)
                {
                    Strategy3Name = null;
                }
                else
                {
                    if (strat.EnemyName != Enemy3)
                    {
                        Strategy3Name = null;
                    }
                }
            }
        }

        private async Task LoadStrategyInformationFromDb()
        {
            await LoadAvailableStrategies();

            var strategies = await App.DBRepo.GetClanRaidEnemyStrategiesByClanRaidID(CurrentRaid.ID);

            foreach (var item in strategies)
            {
                // get related strategy
                var strat = _availableRaidStrategies?.Where(x => x.Name == item.RaidStrategyId).FirstOrDefault();

                if (strat == null)
                {
                    continue;
                }

                if (strat.EnemyName == Enemy1)
                {
                    Strategy1Name = strat.Name;
                }
                else if (strat.EnemyName == Enemy2)
                {
                    Strategy2Name = strat.Name;
                }
                else if (strat.EnemyName == Enemy3)
                {
                    Strategy3Name = strat.Name;
                }
            }
        }

        private async Task LoadAvailableStrategies()
        {
            _availableRaidStrategies = await App.DBRepo.GetAllRaidStrategiesAsync() ?? new List<RaidStrategy>();

            foreach (var item in _availableRaidStrategies)
            {
                item.EnemyName = RaidInfoHandler.EnemyInfos.Where(x => x.EnemyId == item.EnemyId).FirstOrDefault()?.Name;
            }

            if (IsEnemy1Visible)
            {
                StrategiesForEnemy1 = new ObservableCollection<string>(_availableRaidStrategies.Where(x => x.EnemyName == Enemy1).Select(x => x.Name).ToList());
            }

            if (IsEnemy2Visible)
            {
                StrategiesForEnemy2 = new ObservableCollection<string>(_availableRaidStrategies.Where(x => x.EnemyName == Enemy2).Select(x => x.Name).ToList());
            }

            if (IsEnemy3Visible)
            {
                StrategiesForEnemy3 = new ObservableCollection<string>(_availableRaidStrategies.Where(x => x.EnemyName == Enemy3).Select(x => x.Name).ToList());
            }

        }
        #endregion

        #region Tolerances
        private async Task LoadToleranceInformationFromDbAsync()
        {
            try
            {
                await LoadAvailableTolerances();

                var relationships = await App.DBRepo.GetClanRaidToleranceRelationshipByClanRaidID(CurrentRaid.ID);

                // only take one!
                if (relationships == null || relationships.Count == 0)
                {
                    CurrentRaid.Tolerance = new RaidTolerance();
                    return;
                }

                var relevant = relationships.First();

                var tolerance = AvailableTolerances?.Where(x => x.Name == relevant.RaidToleranceId).FirstOrDefault();

                if (tolerance == null)
                {
                    CurrentRaid.Tolerance = new RaidTolerance();
                    return;
                }

                CurrentRaid.Tolerance = tolerance;
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: LoadToleranceInformationFromDbAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
            }
        }

        private async Task LoadAvailableTolerances()
        {
            if (AvailableTolerances == null || AvailableTolerances.Count == 0)
            {
                AvailableTolerances = new ObservableCollection<RaidTolerance>(await App.DBRepo.GetAllRaidTolerancesAsync() ?? new List<RaidTolerance>());
            }
        }
        #endregion

        #region analysis
        private bool GetRaidIsAnalyzable()
        {
            if (CurrentRaid.Result == null || CurrentRaid.Result.Count == 0)
            {
                return false;
            }

            if (IsEnemy1Visible && string.IsNullOrEmpty(Strategy1Name))
            {
                return false;
            }

            if (IsEnemy2Visible && string.IsNullOrEmpty(Strategy2Name))
            {
                return false;
            }

            if (IsEnemy3Visible && string.IsNullOrEmpty(Strategy3Name))
            {
                return false;
            }

            if (CurrentRaid.Tolerance == null || string.IsNullOrWhiteSpace(CurrentRaid.Tolerance.Name))
            {
                return false;
            }

            return true;
        }

        private bool GetRaidIsTacticsReady()
        {
            if (IsEnemy1Visible && string.IsNullOrEmpty(Strategy1Name))
            {
                return false;
            }

            if (IsEnemy2Visible && string.IsNullOrEmpty(Strategy2Name))
            {
                return false;
            }

            if (IsEnemy3Visible && string.IsNullOrEmpty(Strategy3Name))
            {
                return false;
            }

            if (CurrentRaid.Tolerance == null || string.IsNullOrWhiteSpace(CurrentRaid.Tolerance.Name))
            {
                return false;
            }

            return true;
        }

        private async Task LoadAnalysisDataAsync()
        {
            try
            {
                if (!GetRaidIsAnalyzable())
                {
                    return;
                }

                List<(string code, int attacks)> attacks = CurrentRaid.Result.Select(x => (x.PlayerCode, x.TotalRaidAttacks)).Distinct().ToList();

                AnalysisHeaderData = new RaidAnalysisHeaderData();

                AnalysisHeaderData.TotalAttacks = attacks.Select(x => x.attacks).Sum();
                AnalysisHeaderData.TotalDamage = CurrentRaid.Result.Sum(x => x.TitanDamage);

                var highestAttackAmount = CurrentRaid.Result.Max(x => x.TotalRaidAttacks);

                AnalysisHeaderData.AmountOfWaves = GetWaveAmount(CurrentRaid.LevelInfo.AttacksPerReset, highestAttackAmount);

                AnalysisHeaderData.DamagePerAttack = CurrentRaid.Result.Select(x => x.TitanDamage).Sum() / AnalysisHeaderData.TotalAttacks;
                (AnalysisHeaderData.TotalOverkillAmount, AnalysisHeaderData.TotalOverkillPercentage) = await GetOverkillValues();

                SetAverageDamage();

                LoadAttackFlaws();

                LoadPlayerDetails();
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR: LoadAnalysisDataAsync: {ex.Message}\n{ex.Data}");
                await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, ex.Message, AppResources.OKText);
            }
        }

        private RaidStrategy GetRaidStrategy(string enemyName)
        {
            if (enemyName.ToLower().Contains(Enemy1.ToLower()))
            {
                return _availableRaidStrategies.Where(x => x.Name == Strategy1Name).FirstOrDefault();
            }
            if (enemyName.ToLower().Contains(Enemy2.ToLower()))
            {
                return _availableRaidStrategies.Where(x => x.Name == Strategy2Name).FirstOrDefault();
            }
            if (enemyName.ToLower().Contains(Enemy3.ToLower()))
            {
                return _availableRaidStrategies.Where(x => x.Name == Strategy3Name).FirstOrDefault();
            }

            return null;
        }

        private async Task<(double overkillAmount, double overkillPercentage)> GetOverkillValues()
        {
            double amount = 0, percentage = 0, totalDamageDealt = 0;

            foreach (var item in CurrentRaid.Result)
            {
                double tmpAmount = 0;
                totalDamageDealt += item.TitanDamage;

                // get strategy
                var strat = GetRaidStrategy(item.TitanName);

                if (strat == null)
                {
                    var error = $"Could not find strategy for { item.TitanName}.\nMake sure you selected the correct tier and level.";
                    Logger.WriteToLogFile($"ERROR GetOverkillValues: {error}");
                    await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, error, AppResources.OKText);
                    return (0, 0);
                }

                // strategy independent overkill
                tmpAmount += item.SkeletonHead
                    + item.SkeletonTorso
                    + item.SkeletonLeftArm
                    + item.SkeletonRightArm
                    + item.SkeletonLeftHand
                    + item.SkeletonRightHand
                    + item.SkeletonLeftLeg
                    + item.SkeletonRightLeg
                    ;

                item.OverkillHead = item.SkeletonHead;
                item.OverkillTorso = item.SkeletonTorso;
                item.OverkillLeftArm = item.SkeletonLeftArm;
                item.OverkillRightArm = item.SkeletonRightArm;
                item.OverkillLeftHand = item.SkeletonLeftHand;
                item.OverkillRightHand = item.SkeletonRightHand;
                item.OverkillLeftLeg = item.SkeletonLeftLeg;
                item.OverkillRightLeg = item.SkeletonRightLeg;

                // strategy dependent overkill
                if (strat.Head == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorHead;
                    item.OverkillHead += item.ArmorHead;
                }

                if (strat.Torso == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorTorso;
                    item.OverkillTorso += item.ArmorTorso;
                }

                if (strat.LeftShoulder == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorRightArm;
                    item.OverkillRightArm += item.ArmorRightArm;
                }

                if (strat.RightShoulder == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorLeftArm;
                    item.OverkillLeftArm += item.ArmorLeftArm;
                }

                if (strat.LeftHand == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorRightHand;
                    item.OverkillRightHand += item.ArmorRightHand;
                }

                if (strat.RightHand == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorLeftHand;
                    item.OverkillLeftHand += item.ArmorLeftHand;
                }

                if (strat.LeftLeg == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorRightLeg;
                    item.OverkillRightLeg += item.ArmorRightLeg;
                }

                if (strat.RightLeg == EnemyAttackType.No)
                {
                    tmpAmount += item.ArmorLeftLeg;
                    item.OverkillLeftLeg += item.ArmorLeftLeg;
                }

                amount += tmpAmount;

                item.OverkillAmount = tmpAmount;
            }

            percentage = amount * 100 / totalDamageDealt;

            return (amount, percentage);
        }

        private void SetAverageDamage()
        {
            foreach (var item in CurrentRaid.Result)
            {
                if (item.TotalRaidAttacks == 0)
                {
                    item.AverageDamage = 0;
                    continue;
                }

                var playerResults = CurrentRaid.Result.Where(x => x.PlayerCode == item.PlayerCode).ToList();

                item.AverageDamage = (playerResults.Sum(x => x.TitanDamage) - playerResults.Sum(x => x.OverkillAmount)) / item.TotalRaidAttacks;
            }
        }

        /// <summary>
        /// If the amount of attacks divided by 4 has no remaining then this is the amount of waves.
        /// If it has a remainder then there is a wave that has been begun but was not completed as the titan died before
        /// </summary>
        /// <param name="attacks"></param>
        /// <returns></returns>
        private int GetWaveAmount(double attacksPerReset, int attacks) => (int) Math.Ceiling(attacks / attacksPerReset);

        private bool GetIsWorstParticipent(ClanRaidResult item)
        {
            switch (CurrentRaid.Tolerance.AmountType)
            {
                case AttackAmountCalculationType.AbsoluteInAttacks:
                    return item.TotalRaidAttacks < CurrentRaid.Tolerance.AmountTolerance;
                case AttackAmountCalculationType.AbsoluteInWavesIncludingLastWave:
                    return item.TotalRaidAttacks < (Math.Max(0, AnalysisHeaderData.AmountOfWaves - CurrentRaid.Tolerance.AmountTolerance) * 4);
                case AttackAmountCalculationType.AbsoluteInWavesExcludingLastWave:
                    return item.TotalRaidAttacks < (Math.Max(0, AnalysisHeaderData.AmountOfWaves - CurrentRaid.Tolerance.AmountTolerance - 1) * 4);
                case AttackAmountCalculationType.RelativeFromAllAttacksSum:
                    return item.TotalRaidAttacks < AnalysisHeaderData.TotalAttacks * CurrentRaid.Tolerance.AmountTolerance * 0.01;
                default:
                    return false;
            }
        }

        private bool GetIsWorstOverkill(ClanRaidResult item)
        {
            switch (CurrentRaid.Tolerance.OverkillType)
            {
                case OverkillCalculationType.Absolute:
                    return item.OverkillAmount > CurrentRaid.Tolerance.OverkillTolerance;
                case OverkillCalculationType.RelativeFromAllPlayerDamage:
                    return item.OverkillAmount > AnalysisHeaderData.TotalDamage * CurrentRaid.Tolerance.OverkillTolerance * 0.01;
                case OverkillCalculationType.RelativeFromCurrentPlayerDamage:
                    return item.OverkillAmount > CurrentRaid.Result.Where(x => x.PlayerCode == item.PlayerCode).Sum(x => x.TitanDamage) * CurrentRaid.Tolerance.OverkillTolerance * 0.01;
                default:
                    return false;
            }
        }

        private bool GetIsBelowAvgDamage(ClanRaidResult item)
        {
            switch (CurrentRaid.Tolerance.AverageType)
            {
                case AverageDamageCalculationType.Absolute:
                    return item.AverageDamage < CurrentRaid.Tolerance.AverageTolerance;
                default:
                    return false;
            }
        }

        private void LoadAttackFlaws()
        {
            foreach (var item in CurrentRaid.Result)
            {
                item.IsOneOfWorstOverkills = GetIsWorstOverkill(item);
                item.IsOneOfWorstParticipents = GetIsWorstParticipent(item);
                item.IsBelowMinAverageDamage = GetIsBelowAvgDamage(item);
            }

            var players = CurrentRaid.Result.Select(x => x.PlayerCode).Distinct();

            AttackFlaws = new ObservableCollection<ClanRaidAttackFlaw>();

            foreach (var item in players)
            {
                // get player results
                var playerResults = CurrentRaid.Result.Where(x => x.PlayerCode == item).ToList();
                var raidAttacks = playerResults.FirstOrDefault()?.TotalRaidAttacks ?? 0;

                if (playerResults.Any(x => x.IsOneOfWorstParticipents))
                {
                    AttackFlaws.Add(new ClanRaidAttackFlaw
                    {
                        Flaw = FlawType.BelowMinAttacks,
                        FlawText = Enum.GetName(typeof(FlawType), FlawType.BelowMinAttacks).TranslatedString(),
                        PlayerId = item,
                        PlayerName = playerResults.FirstOrDefault()?.PlayerName,
                        Value = raidAttacks,
                    });

                    // if a player got 0 attacks i do not want him included in other parts of the list
                    if (raidAttacks == 0)
                    {
                        continue;
                    }
                }

                if (playerResults.Any(x => x.IsBelowMinAverageDamage))
                {
                    AttackFlaws.Add(new ClanRaidAttackFlaw
                    {
                        Flaw = FlawType.BelowAvgDamage,
                        FlawText = Enum.GetName(typeof(FlawType), FlawType.BelowAvgDamage).TranslatedString(),
                        PlayerId = item,
                        PlayerName = playerResults.FirstOrDefault()?.PlayerName,
                        Value = playerResults.FirstOrDefault()?.AverageDamage ?? 0,
                    });
                }

                if(playerResults.Where(x => x.IsOneOfWorstOverkills).Any())
                {
                    AttackFlaws.Add(new ClanRaidAttackFlaw
                    {
                        Flaw = FlawType.AboveMaxOverKill,
                        FlawText = Enum.GetName(typeof(FlawType), FlawType.AboveMaxOverKill).TranslatedString(),
                        PlayerId = item,
                        PlayerName = playerResults.FirstOrDefault()?.PlayerName,
                        Value = playerResults.Sum(x => x.OverkillAmount),
                    });
                }               
            }

            // sort
            var tmpFlws = AttackFlaws.OrderBy(x => x.Flaw).ThenBy(x => x.PlayerName).ToList();
            AttackFlaws = new ObservableCollection<ClanRaidAttackFlaw>(tmpFlws);

            // grouping
            List<(string code, string name)> attacks = AttackFlaws.Select(x => (x.FlawText[0].ToString().ToUpper(), x.FlawText)).Distinct().ToList();
            AttackFlawGrouping = new ObservableCollection<GroupedClanRaidAttackFlaw>();
            foreach (var item in attacks)
            {
                var group = new GroupedClanRaidAttackFlaw() { LongName = item.name, ShortName = item.code };

                if(item.name == FlawType.BelowMinAttacks.GetDescription().TranslatedString())
                {
                    foreach (var child in AttackFlaws.Where(x => x.FlawText == group.LongName).OrderBy(x => x.Flaw).ThenByDescending(x => x.Value).ToList())
                    {
                        group.Add(child);
                    }
                }
                else
                {
                    foreach (var child in AttackFlaws.Where(x => x.FlawText == group.LongName).OrderBy(x => x.Flaw).ThenBy(x => x.Value).ToList())
                    {
                        group.Add(child);
                    }
                }

                AttackFlawGrouping.Add(group);
            }
        }

        private void LoadPlayerDetails()
        {
            #region playerDetails
            RaidResultAnalysisList = new ObservableCollection<RaidResultAnalysisEntry>();

            var players = CurrentRaid.Result.Select(x => x.PlayerCode).Distinct();

            foreach (var item in players)
            {
                var playerResults = CurrentRaid.Result.Where(x => x.PlayerCode == item).ToList();

                var raidAttacks = playerResults.FirstOrDefault()?.TotalRaidAttacks ?? 0;

                if(raidAttacks == 0)
                {
                    RaidResultAnalysisList.Add(new RaidResultAnalysisEntry
                    {
                        Name = playerResults.FirstOrDefault()?.PlayerName,
                        Attacks = raidAttacks,
                        Damage = 0,
                        DamagePerAttack = 0,
                        Overkill = 0,
                        IsOneOfWorstOverkills = false,
                        IsOneOfWorstParticipents = true,
                        IsBelowMinAverageDamage = true,
                    });
                }
                else
                {
                    RaidResultAnalysisList.Add(new RaidResultAnalysisEntry
                    {
                        Name = playerResults.FirstOrDefault()?.PlayerName,
                        Attacks = raidAttacks,
                        Damage = playerResults.Sum(x => x.TitanDamage),
                        DamagePerAttack = playerResults.FirstOrDefault()?.AverageDamage ?? 0,
                        Overkill = playerResults.Sum(x => x.OverkillAmount),
                        IsOneOfWorstOverkills = playerResults.Where(x => x.IsOneOfWorstOverkills).Count() > 0,
                        IsOneOfWorstParticipents = playerResults.Where(x => x.IsOneOfWorstParticipents).Count() > 0,
                        IsBelowMinAverageDamage = playerResults.Where(x => x.IsBelowMinAverageDamage).Count() > 0,
                    });
                }
            }
            #endregion
        }

        public static Color GetRandomLightColor()
        {
            var solid = Color.MistyRose;
            Random random = new Random();

            var num = random.Next(1, 37);

            switch (num)
            {
                case 1: solid = Color.Moccasin; break;
                case 2: solid = Color.NavajoWhite; break;
                case 3: solid = Color.PaleGoldenrod; break;
                case 4: solid = Color.PaleGreen; break;
                case 5: solid = Color.PaleTurquoise; break;
                case 6: solid = Color.MediumSpringGreen; break;
                case 7: solid = Color.LightSkyBlue; break;
                case 8: solid = Color.LightSteelBlue; break;
                case 9: solid = Color.LightYellow; break;
                case 10: solid = Color.MediumTurquoise; break;
                case 11: solid = Color.Linen; break;
                case 12: solid = Color.MediumAquamarine; break;
                case 13: solid = Color.PapayaWhip; break;
                case 14: solid = Color.Tan; break;
                case 15: solid = Color.Thistle; break;
                case 16: solid = Color.Wheat; break;
                case 17: solid = Color.Turquoise; break;
                case 18: solid = Color.PeachPuff; break;
                case 19: solid = Color.SkyBlue; break;
                case 20: solid = Color.Pink; break;
                case 21: solid = Color.Plum; break;
                case 22: solid = Color.PowderBlue; break;
                case 23: solid = Color.RosyBrown; break;
                case 24: solid = Color.LightSalmon; break;
                case 25: solid = Color.LightGreen; break;
                case 26: solid = Color.LightPink; break;
                case 27: solid = Color.Aqua; break;
                case 28: solid = Color.Aquamarine; break;
                case 29: solid = Color.Bisque; break;
                case 30: solid = Color.Coral; break;
                case 31: solid = Color.BurlyWood; break;
                case 32: solid = Color.CadetBlue; break;
                case 33: solid = Color.DarkTurquoise; break;
                case 34: solid = Color.Khaki; break;
                case 35: solid = Color.Lavender; break;
                case 36: solid = Color.LavenderBlush; break;
                case 37: solid = Color.LightBlue; break;
                default: solid = Color.Coral; break;
            }
            return solid;
        }
        #endregion
        #endregion

        #region E+D
        private void Drawer_OnError(object sender, CustErrorEventArgs e) => Logger.WriteToLogFile($"{sender} - {e.MyException.Message}\n{e.MyException.Data}");

        private void Drawer_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"{sender} - {e.Information}");

        private async Task Parser_OnProblemHaving(object sender, Helpers.CustErrorEventArgs e)
        {
            await _dialogService.DisplayAlertAsync(AppResources.ErrorHeader, e.MyException.Message, AppResources.OKText);
            Logger.WriteToLogFile($"ERROR: {sender} - {e.MyException.Message}");
        }

        private void Parser_OnLogMePlease(object sender, Helpers.InformationEventArgs e) => Logger.WriteToLogFile($"{sender} - {e.Information}");

        private async Task CurrentRaid_OnLevelChanged()
        {
            await LoadLevelInformationAsync();
            OnPropertyChanged(new PropertyChangedEventArgs("CurrentRaid.Level"));
        }

        private async Task CurrentRaid_OnTierChanged()
        {
            AvailableLevels = GetAvailableLevels();

            if (AvailableLevels.Contains(CurrentRaid?.Level ?? 0))
            {
                await LoadLevelInformationAsync();
            }

            if (CurrentRaid != null)
            {
                CurrentRaid.Level = CurrentRaid.Level;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentRaid.Tier"));
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }
        }
        #endregion

        #region Override
        /// <summary>
        /// When navigating to this
        /// </summary>
        /// <param name="parameters"></param>
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Analytics.TrackEvent("Module used", new Dictionary<string, string> { { "Name", "Raid Detail" } });
            RaidInfoHandler.LoadRaidInfos();

            AvailableTiers = GetAvailableTiers();
            AvailableLevels = GetAvailableLevels();
            await LoadAvailableTolerances();

            // Load configuration if passed in parameters
            if (parameters.ContainsKey("id"))
            {
                CurrentRaid = await App.DBRepo.GetClanRaidByID(JfTypeConverter.ForceInt(parameters["id"].ToString()));
                CurrentRaid.IsSaved = true;
                await LoadRaidInformation();
            }
            else
            {
                CurrentRaid = new ClanRaid();
            }
            // Abo on enemy name change to load the correct id
            CurrentRaid.OnTierChanged += async () => await CurrentRaid_OnTierChanged();
            CurrentRaid.OnLevelChanged += async () => await CurrentRaid_OnLevelChanged();

            await CurrentRaid_OnTierChanged();
            await CurrentRaid_OnLevelChanged();

            base.OnNavigatedTo(parameters);
        }
        #endregion
    }
}
