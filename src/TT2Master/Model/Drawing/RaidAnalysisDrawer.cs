using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Interfaces;
using TT2Master.Model.Raid;
using TT2Master.Resources;
using TT2Master.ViewModels.Raid;
using Xamarin.Forms;

namespace TT2Master.Model.Drawing
{
    public class RaidAnalysisDrawer
    {
        #region private fields
        private SKBitmap _bitmap;
        private SKCanvas _canvas;

        private int _bitmapWidth = 1600;
        private int _bitmapHeight = 1200;

        private ClanRaid _raid;
        private RaidAnalysisHeaderData _analysisHeaderData;
        private List<GroupedClanRaidAttackFlaw> _groupedFlaws;
        private List<RaidResultAnalysisEntry> _resultDetailList;

        private List<RaidStrategy> _strategies = new List<RaidStrategy>();

        private SKBitmap _yesImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("yes");
        private SKBitmap _noImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("no");
        private SKBitmap _vMImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("FinisherAttack");
        private SKBitmap _toPImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("TotemFairySkill");
        private SKBitmap _oneImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("One");
        private SKBitmap _twoImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("Two");
        private SKBitmap _threeImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("Three");
        private SKBitmap _fourImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("Four");
        private SKBitmap _fiveImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("Five");
        private SKBitmap _sixImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("Six");
        private SKBitmap _sevenImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("Seven");
        private SKBitmap _eightImgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource("Eight");


        private SKPaint _textPaint = new SKPaint
        {
            IsStroke = false,
            Color = SKColors.White,
            TextSize = 24,
            TextAlign = SKTextAlign.Left,
            TextEncoding = SKTextEncoding.Utf8,
        };
        
        private SKPaint _contentPaint = new SKPaint
        {
            IsStroke = false,
            Color = SKColors.White,
            TextSize = 16,
            TextAlign = SKTextAlign.Left,
            TextEncoding = SKTextEncoding.Utf8,
        };
        
        private SKPaint _textHeaderPaint = new SKPaint
        {
            IsStroke = false,
            Color = SKColors.White,
            TextSize = 26,
            TextAlign = SKTextAlign.Left,
            TextEncoding = SKTextEncoding.Utf8,
        };

        private SKPaint _borderPaint = new SKPaint
        {
            Color = SKColors.White,
            IsStroke = true,
        };

        private SKPaint _flawPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.IndianRed,
        };

        private SKPaint _backgroundPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black,
        };
        #endregion

        #region private methods
        /// <summary>
        /// Header line
        /// </summary>
        private void DrawHeader(int x, int y)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawHeader() called"));
            int startY = y + 20, nextTextY = 30;

            _canvas.DrawText($"{AppResources.RaidResult} {_raid.Tier}-{_raid.Level}", x, startY, _textHeaderPaint);

            if (string.IsNullOrWhiteSpace(_raid.Description))
            {
                return;
            }

            if (!_raid.Description.Contains("\n"))
            {
                _canvas.DrawText($"{_raid.Description}", x, startY + nextTextY, _textPaint);
            }
            else
            {
                var desc = _raid.Description.Split('\n');

                for (int i = 0; i < Math.Min(desc.Length, 6); i++)
                {
                    _canvas.DrawText($"{desc[i]}", x + 10, startY + (nextTextY * (i + 1)), _textPaint);
                }
            }
        }

        /// <summary>
        /// Draws analysis header info
        /// </summary>
        private void DrawAnalysisHeader(int x, int y, int w, int h)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawAnalysisHeader() called"));
            _canvas.DrawRect(x, y, w, h, _borderPaint);
            int startY = y + 30, nextTextY = 30;

            _canvas.DrawText($"{AppResources.AttacksInTotal}: {_analysisHeaderData.TotalAttacks}"                  , x + 10, startY, _textPaint);
            _canvas.DrawText($"{AppResources.Waves}: {_analysisHeaderData.AmountOfWaves}"                          , x + 10, startY + nextTextY, _textPaint);
            _canvas.DrawText($"{AppResources.TotalDamage}: {_analysisHeaderData.TotalDamage:N0}"                   , x + 10, startY + nextTextY * 2, _textPaint);
            _canvas.DrawText($"{AppResources.DamagePerAttack}: {_analysisHeaderData.DamagePerAttack:N0}"           , x + 10, startY + nextTextY * 3, _textPaint);
            _canvas.DrawText($"{AppResources.OverkillAmount}: {_analysisHeaderData.TotalOverkillAmount:N0}"        , x + 10, startY + nextTextY * 4, _textPaint);
            _canvas.DrawText($"{AppResources.OverkillPercentage}: {_analysisHeaderData.TotalOverkillPercentage:N2}", x + 10, startY + nextTextY * 5, _textPaint);
        }

        /// <summary>
        /// Draws tolerances header info
        /// </summary>
        private void DrawTolerances(int x, int y, int w, int h)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawTolerances() called"));

            _canvas.DrawRect(x, y, w, h, _borderPaint);
            int startY = y + 30, nextTextY = 30;

            _canvas.DrawText($"{AppResources.NameHeader}: {_raid.Tolerance.Name}", x + 10, startY, _textPaint);

            switch (_raid.Tolerance.OverkillType)
            {
                case OverkillCalculationType.Absolute:
                    _canvas.DrawText($"{AppResources.MaxOverkillAmount}: {_raid.Tolerance.OverkillTolerance:N0}" , x + 10, startY + nextTextY, _textPaint);
                    break;
                case OverkillCalculationType.RelativeFromAllPlayerDamage:
                    _canvas.DrawText($"{AppResources.MaxOverkillAmount}: {_raid.Tolerance.OverkillTolerance:N2} % ({AppResources.AllPlayers})", x + 10, startY + nextTextY, _textPaint);
                    break;
                case OverkillCalculationType.RelativeFromCurrentPlayerDamage:
                    _canvas.DrawText($"{AppResources.MaxOverkillAmount}: {_raid.Tolerance.OverkillTolerance:N2} % ({AppResources.Player})", x + 10, startY + nextTextY, _textPaint);
                    break;
                default:
                    break;
            }

            int wavesNeeded, atkNeeded;

            switch (_raid.Tolerance.AmountType)
            {
                case AttackAmountCalculationType.AbsoluteInAttacks:
                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {_raid.Tolerance.AmountTolerance:N0}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                case AttackAmountCalculationType.AbsoluteInWavesIncludingLastWave:

                    wavesNeeded = Math.Max(0, _analysisHeaderData.AmountOfWaves - (int)_raid.Tolerance.AmountTolerance);
                    atkNeeded = wavesNeeded * 4;

                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {string.Format(AppResources.AmountInWaves, wavesNeeded.ToString("N0"), atkNeeded.ToString("N0"))}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                case AttackAmountCalculationType.AbsoluteInWavesExcludingLastWave:
                    wavesNeeded = Math.Max(0, _analysisHeaderData.AmountOfWaves - (int)_raid.Tolerance.AmountTolerance - 1);
                    atkNeeded = wavesNeeded * 4;

                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {string.Format(AppResources.XofYWaves, wavesNeeded.ToString("N0"), _analysisHeaderData.AmountOfWaves, atkNeeded.ToString("N0"), (_analysisHeaderData.AmountOfWaves * 4))}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                case AttackAmountCalculationType.RelativeFromAllAttacksSum:
                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {string.Format(AppResources.PercentageOfAllAttacks,_raid.Tolerance.AmountTolerance.ToString("N2"))}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                default:
                    break;
            }

            _canvas.DrawText($"{AppResources.MinAverageDamage}: {_raid.Tolerance.AverageTolerance:N0}"   , x + 10, startY + nextTextY * 3, _textPaint);

            if (string.IsNullOrWhiteSpace(_raid.Tolerance.Description))
            {
                return;
            }

            if (!_raid.Tolerance.Description.Contains("\n"))
            {
                _canvas.DrawText($"{_raid.Tolerance.Description}", x + 10, startY + nextTextY * 4, _textPaint);
            }
            else
            {
                var desc = _raid.Tolerance.Description.Split('\n');

                for (int i = 0; i < Math.Min(desc.Length, 2); i++)
                {
                    _canvas.DrawText($"{desc[i]}", x + 10, startY + nextTextY * (4 + i), _textPaint);
                }
            }
        }

        /// <summary>
        /// Titan strategies
        /// </summary>
        private void DrawStrategies(int x, int y)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawStrategies() called"));

            _canvas.DrawText($"{AppResources.Strategies}", x, y + 24, _textPaint);

            for (int i = 0; i < _strategies.Count; i++)
            {
                DrawStrategy(_strategies[i], x, y + 50, i);
            }
        }

        private void DrawStrategy(RaidStrategy strategy, int x, int y, int titanNumber)
        {
            var imgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource(strategy?.EnemyName);

            float coordX = x + (titanNumber * 25) + (200 * titanNumber);
            float coordY = y;

            var destRect = new SKRect(
                  left: coordX
                , top: coordY
                , right: coordX + 200
                , bottom: coordY + 200);

            // draw enemy
            _canvas.DrawBitmap(imgSrc, destRect);

            // left shoulder
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.LeftShoulder)
                , new SKRect 
                ( 
                    left: coordX + 10
                    , top: coordY + 25
                    , right: coordX + 60
                    , bottom: coordY + 75
                ));

            // head
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.Head)
                , new SKRect
                (
                    left: coordX + 75
                    , top: coordY + 25
                    , right: coordX + 125
                    , bottom: coordY + 75
                ));

            // right shoulder
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.RightShoulder)
                , new SKRect
                (
                    left: coordX + 150
                    , top: coordY + 25
                    , right: coordX + 200
                    , bottom: coordY + 75
                ));

            // left arm
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.LeftHand)
                , new SKRect
                (
                    left: coordX + 10
                    , top: coordY + 75
                    , right: coordX + 60
                    , bottom: coordY + 125
                ));

            // torso
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.Torso)
                , new SKRect
                (
                    left: coordX + 75
                    , top: coordY + 75
                    , right: coordX + 125
                    , bottom: coordY + 125
                ));

            // right arm
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.RightHand)
                , new SKRect
                (
                    left: coordX + 150
                    , top: coordY + 75
                    , right: coordX + 200
                    , bottom: coordY + 125
                ));

            // left leg
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.LeftLeg)
                , new SKRect
                (
                    left: coordX + 50
                    , top: coordY + 125
                    , right: coordX + 100
                    , bottom: coordY + 175
                ));

            // right leg
            _canvas.DrawBitmap(GetImageSrcForAttackType(strategy.RightLeg)
                , new SKRect
                (
                    left: coordX + 100
                    , top: coordY + 125
                    , right: coordX + 150
                    , bottom: coordY + 175
                ));
        }

        private SKBitmap GetImageSrcForAttackType(EnemyAttackType type)
        {
            switch (type)
            {
                case EnemyAttackType.No:
                    return _noImgSrc;
                case EnemyAttackType.VM:
                    return _vMImgSrc;
                case EnemyAttackType.ToP:
                    return _toPImgSrc;
                case EnemyAttackType.One:
                    return _oneImgSrc;
                case EnemyAttackType.Two:
                    return _twoImgSrc;
                case EnemyAttackType.Three:
                    return _threeImgSrc;
                case EnemyAttackType.Four:
                    return _fourImgSrc;
                case EnemyAttackType.Five:
                    return _fiveImgSrc;
                case EnemyAttackType.Six:
                    return _sixImgSrc;
                case EnemyAttackType.Seven:
                    return _sevenImgSrc;
                case EnemyAttackType.Eight:
                    return _eightImgSrc;
                case EnemyAttackType.Yes:    
                default:
                    return _yesImgSrc;
            }
        }

        private RaidStrategy GetStrategy(int titanId)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - GetStrategy() for {titanId} called"));
            // get titan full name from result
            var titanName = _raid.Result.Where(x => x.TitanNumber == titanId).First().TitanName;

            // get strategy
            return _raid.Strategies.Where(x => titanName.ToLower().Contains(x.EnemyName.ToLower())).FirstOrDefault();
        }

        /// <summary>
        /// Titan attack flaw heatmaps
        /// </summary>
        private void DrawHeatmaps(int x, int y)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawHeatmaps() called"));

            _canvas.DrawText($"{AppResources.OverkillParts}", x, y + 24, _textPaint);

            for (int i = 0; i < _strategies.Count; i++)
            {
                DrawOverkill(_strategies[i], x, y + 50, i);
            }
        }

        private void DrawOverkill(RaidStrategy strategy, int x, int y, int titanNumber)
        {
            var imgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource(strategy?.EnemyName);

            float coordX = x + (titanNumber * 25) + (200 * titanNumber);
            float coordY = y;

            var destRect = new SKRect(
                  left: coordX
                , top: coordY
                , right: coordX + 200
                , bottom: coordY + 200);

            // draw enemy
            _canvas.DrawBitmap(imgSrc, destRect);

            var worstOverkills = _raid.Result.Where(x => x.IsOneOfWorstOverkills && x.TitanNumber == titanNumber).ToList();
            if(worstOverkills == null || worstOverkills.Count == 0)
            {
                return;
            }

            // left arm
            if(strategy.LeftShoulder == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillRightArm > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                    , new SKRect
                    (
                        left: coordX + 10
                        , top: coordY + 25
                        , right: coordX + 60
                        , bottom: coordY + 75
                    ));
            }

            // head
            if (strategy.Head == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillHead > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                        , new SKRect
                        (
                            left: coordX + 75
                            , top: coordY + 25
                            , right: coordX + 125
                            , bottom: coordY + 75
                        )); 
            }

            // right arm
            if (strategy.RightShoulder == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillLeftArm > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                        , new SKRect
                        (
                            left: coordX + 150
                            , top: coordY + 25
                            , right: coordX + 200
                            , bottom: coordY + 75
                        )); 
            }

            // left hand
            if (strategy.LeftHand == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillRightHand > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                        , new SKRect
                        (
                            left: coordX + 10
                            , top: coordY + 75
                            , right: coordX + 60
                            , bottom: coordY + 125
                        )); 
            }

            // torso
            if (strategy.Torso == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillTorso > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                        , new SKRect
                        (
                            left: coordX + 75
                            , top: coordY + 75
                            , right: coordX + 125
                            , bottom: coordY + 125
                        )); 
            }

            // right hand
            if (strategy.RightHand == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillLeftHand > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                        , new SKRect
                        (
                            left: coordX + 150
                            , top: coordY + 75
                            , right: coordX + 200
                            , bottom: coordY + 125
                        )); 
            }

            // left leg
            if (strategy.LeftLeg == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillRightLeg > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                        , new SKRect
                        (
                            left: coordX + 50
                            , top: coordY + 125
                            , right: coordX + 100
                            , bottom: coordY + 175
                        )); 
            }

            // right leg
            if (strategy.RightLeg == EnemyAttackType.No && worstOverkills.Where(x => x.OverkillLeftLeg > x.OverkillAmount * 0.01).Any())
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.No)
                        , new SKRect
                        (
                            left: coordX + 100
                            , top: coordY + 125
                            , right: coordX + 150
                            , bottom: coordY + 175
                        )); 
            }
        }

        /// <summary>
        /// Titan specific flaw tables
        /// </summary>
        private void DrawFlawTables(int x, int y)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawFlawTables() called"));
            _canvas.DrawText($"{AppResources.WorstOverkillAmounts}", x, y + 24, _textPaint);

            for (int i = 0; i < _strategies.Count; i++)
            {
                DrawFlawTable(_strategies[i], x, y + 50, i);
            }
        }

        private void DrawFlawTable(RaidStrategy strategy, int x, int y, int titanNumber)
        {
            float coordX = x + (titanNumber * 25) + (200 * titanNumber);
            float coordY = y;

            _canvas.DrawRect(coordX, coordY, 200, 200, _borderPaint);
            float startY = coordY + 20, nextTextY = 30;

            var worstOverkills = _raid.Result.Where(x => x.IsOneOfWorstOverkills && x.TitanNumber == titanNumber).ToList();
            if (worstOverkills == null || worstOverkills.Count == 0)
            {
                _canvas.DrawBitmap(GetImageSrcForAttackType(EnemyAttackType.Yes)
                    , new SKRect
                    (
                        left: coordX + 75
                        , top: coordY + 75
                        , right: coordX + 125
                        , bottom: coordY + 125
                    ));
                return;
            }

            worstOverkills = worstOverkills.OrderByDescending(x => x.OverkillAmount).Take(6).ToList();

            // print 6 worst dudes
            for (int i = 0; i < worstOverkills.Count; i++)
            {
                _canvas.DrawText($"{worstOverkills[i].PlayerName}: {worstOverkills[i].OverkillAmount:N0}", coordX + 10, startY + (nextTextY * i), _contentPaint);
            }
        }

        /// <summary>
        /// Player details table
        /// </summary>
        private void DrawDetailsTable(float x, float y)
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawDetailsTable() called"));
            float lineHeight = 30;

            float height = (_resultDetailList.Count + 1) * lineHeight;

            _canvas.DrawRect(x, y, 800, height, _borderPaint);

            #region header line

            // name
            _canvas.DrawRect(x, y, 200, lineHeight, _borderPaint);
            _canvas.DrawText($"{AppResources.NameHeader}", x + 5, y + lineHeight, _contentPaint);

            // attacks
            _canvas.DrawRect(x + 200, y, 150, lineHeight, _borderPaint);
            _canvas.DrawText($"{AppResources.Attacks}", x + 205, y + lineHeight, _contentPaint);

            // damage
            _canvas.DrawRect(x + 350, y, 150, lineHeight, _borderPaint);
            _canvas.DrawText($"{AppResources.Damage}", x + 355, y + lineHeight, _contentPaint);

            // avg. damage
            _canvas.DrawRect(x + 500, y, 150, lineHeight, _borderPaint);
            _canvas.DrawText($"{AppResources.DamagePerAttack}", x + 505, y + lineHeight, _contentPaint);

            // overkill
            _canvas.DrawRect(x + 650, y, 150, lineHeight, _borderPaint);
            _canvas.DrawText($"{AppResources.Overkill}", x + 655, y + lineHeight, _contentPaint);

            #endregion

            #region draw content
            for (int i = 0; i < _resultDetailList.Count; i++)
            { 
                // name
                _canvas.DrawRect(x, y + 30 + (i * lineHeight), 200, lineHeight, _borderPaint);
                _canvas.DrawText($"{_resultDetailList[i].Name}"              , x +  5, y + 55 + (i * lineHeight), _contentPaint);

                // attacks
                if (_resultDetailList[i].IsOneOfWorstParticipents) _canvas.DrawRect(x + 200, y + 30 + (i * lineHeight), 150, lineHeight, _flawPaint);
                _canvas.DrawRect(x + 200, y + 30 + (i * lineHeight), 150, lineHeight, _borderPaint);
                _canvas.DrawText($"{_resultDetailList[i].Attacks}"           , x + 205, y + 55 + (i * lineHeight), _contentPaint);

                // damage
                _canvas.DrawRect(x + 350, y + 30 + (i * lineHeight), 150, lineHeight, _borderPaint);
                _canvas.DrawText($"{_resultDetailList[i].Damage:N0}"         , x + 355, y + 55 + (i * lineHeight), _contentPaint);

                // avg. damage
                if (_resultDetailList[i].IsBelowMinAverageDamage) _canvas.DrawRect(x + 500, y + 30 + (i * lineHeight), 150, lineHeight, _flawPaint);
                _canvas.DrawRect(x + 500, y + 30 + (i * lineHeight), 150, lineHeight, _borderPaint);
                _canvas.DrawText($"{_resultDetailList[i].DamagePerAttack:N0}", x + 505, y + 55 + (i * lineHeight), _contentPaint);

                // overkill
                if (_resultDetailList[i].IsOneOfWorstOverkills) _canvas.DrawRect(x + 650, y + 30 + (i * lineHeight), 150, lineHeight, _flawPaint);
                _canvas.DrawRect(x + 650, y + 30 + (i * lineHeight), 150, lineHeight, _borderPaint);
                _canvas.DrawText($"{_resultDetailList[i].Overkill:N0}"       , x + 655, y + 55 + (i * lineHeight), _contentPaint);
            }
            #endregion
        }

        private void CalculateMeasures()
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - CalculateMeasures() called"));
            _bitmapHeight = 990 + (_resultDetailList.Count + 1) * 30;

            _bitmapWidth = Math.Max(1300, 10 + ((_strategies.Count() + 1) * 25) + (200 * (_strategies.Count() + 1)));
        }

        private void DrawBackground()
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - DrawBackground() called"));
            _canvas.DrawRect(0, 0, _bitmapWidth, _bitmapHeight, _backgroundPaint);
        }

        private void LoadStrategies()
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - LoadStrategies() called"));
            // get titans in order
            int titanAmount = _raid.Result.Max(x => x.TitanNumber) + 1;
            _strategies = new List<RaidStrategy>();

            for (int i = 0; i < titanAmount; i++)
            {
                _strategies.Add(GetStrategy(i));
            }
        }
        #endregion

        #region Ctor
        public RaidAnalysisDrawer(ClanRaid raid, RaidAnalysisHeaderData analysisHeaderData, List<GroupedClanRaidAttackFlaw> raidAttackFlaws, List<RaidResultAnalysisEntry> resultAnalysisEntries)
        {
            _raid = raid;
            _analysisHeaderData = analysisHeaderData;
            _groupedFlaws = raidAttackFlaws;
            _resultDetailList = resultAnalysisEntries;
        } 
        #endregion

        #region public functions
        /// <summary>
        /// Draws the bitmap
        /// </summary>
        public void Draw()
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - Draw() called"));
            LoadStrategies();
            CalculateMeasures();

            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - Setting bitmap with width of {_bitmapWidth} and height of {_bitmapHeight}"));
            _bitmap = new SKBitmap(_bitmapWidth, _bitmapHeight);
            _canvas = new SKCanvas(_bitmap);

            DrawBackground();

            DrawHeader(10, 10); // h = 200
            DrawAnalysisHeader(350, 10, 350, 200); // h = 200
            DrawTolerances(700, 10, 525, 200); // h = 200

            DrawStrategies(10, 220); // h = 250
            DrawHeatmaps(10, 470); // h = 250
            DrawFlawTables(10, 720); // h = 200

            DrawDetailsTable(10, 980); // h = ?
        }

        /// <summary>
        /// Saves image to bitmap
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> SaveImage()
        {
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"{nameof(RaidAnalysisDrawer)} - SaveImage() called"));
            using (var memStream = new MemoryStream())
            using (var wstream = new SKManagedWStream(memStream))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                _bitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
#pragma warning restore CS0618 // Type or member is obsolete
                byte[] data = memStream.ToArray();

                // Check the data array for content!

                var result = await DependencyService.Get<IPhotoLibrary>().SavePhotoAsync(data, "TT2Master", $"raid_analysis_{_raid.Tier}-{_raid.Level}_{DateTime.Now.ToString("yyyy_MM_dd_hh_ss")}.png");

                // Check return value for success!
                return result;
            }
        }
        #endregion

        #region E + D
        /// <summary>
        /// Delegate for <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public event ProgressCarrier OnLogMePlease;
        #endregion
    }
}
