using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Interfaces;
using TT2Master.Model.Raid;
using TT2Master.Resources;
using TT2Master.ViewModels.Raid;
using Xamarin.Forms;

namespace TT2Master.Model.Drawing
{
    public class RaidTacticsDrawer
    {
        #region private fields
        private SKBitmap _bitmap;
        private SKCanvas _canvas;

        private int _bitmapWidth = 1600;
        private int _bitmapHeight = 1200;

        private ClanRaid _raid;

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
            int startY = y + 20, nextTextY = 30;

            _canvas.DrawText($"{AppResources.RaidTactics} {_raid.Tier}-{_raid.Level}", x, startY, _textHeaderPaint);

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
                    _canvas.DrawText($"{desc[i]}", x + 10, startY + (nextTextY * (i+1)), _textPaint);
                }
            }
        }

        /// <summary>
        /// Draws tolerances header info
        /// </summary>
        private void DrawTolerances(int x, int y, int w, int h)
        {
            _canvas.DrawRect(x, y, w, h, _borderPaint);
            int startY = y + 30, nextTextY = 30;

            _canvas.DrawText($"{AppResources.NameHeader}: {_raid.Tolerance.Name}"                        , x + 10, startY, _textPaint);

            switch (_raid.Tolerance.OverkillType)
            {
                case OverkillCalculationType.Absolute:
                    _canvas.DrawText($"{AppResources.MaxOverkillAmount}: {_raid.Tolerance.OverkillTolerance:N0}", x + 10, startY + nextTextY, _textPaint);
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

            switch (_raid.Tolerance.AmountType)
            {
                case AttackAmountCalculationType.AbsoluteInAttacks:
                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {_raid.Tolerance.AmountTolerance:N0}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                case AttackAmountCalculationType.AbsoluteInWavesIncludingLastWave:
                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {string.Format(AppResources.MaxWavesMinusIncluding, _raid.Tolerance.AmountTolerance.ToString("N0"))}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                case AttackAmountCalculationType.AbsoluteInWavesExcludingLastWave:
                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {string.Format(AppResources.MaxWavesExcluding, _raid.Tolerance.AmountTolerance.ToString("N0"))}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                case AttackAmountCalculationType.RelativeFromAllAttacksSum:
                    _canvas.DrawText($"{AppResources.MinAttackAmount}: {string.Format(AppResources.PercentageOfAllAttacks, _raid.Tolerance.AmountTolerance.ToString("N2"))}", x + 10, startY + nextTextY * 2, _textPaint);
                    break;
                default:
                    break;
            }

            _canvas.DrawText($"{AppResources.MinAverageDamage}: {_raid.Tolerance.AverageTolerance:N0}", x + 10, startY + nextTextY * 3, _textPaint);

            if (string.IsNullOrWhiteSpace(_raid.Tolerance.Description))
            {
                return;
            }

            if (!_raid.Tolerance.Description.Contains("\n"))
            {
                _canvas.DrawText($"{_raid.Tolerance.Description}"                                            , x + 10, startY + nextTextY * 4, _textPaint);
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
            _canvas.DrawText($"{AppResources.Strategies}", x, y + 24, _textPaint);

            for (int i = 0; i < _raid.Strategies.Count; i++)
            {
                DrawStrategy(_raid.Strategies[i], x, y + 50, i);
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

        private void CalculateMeasures()
        {
            _bitmapHeight = 500;

            _bitmapWidth = Math.Max(875, 10 + ((_raid.Strategies.Count() + 1) * 25) + (200 * (_raid.Strategies.Count() + 1)));
        }

        private void DrawBackground()
        {
            _canvas.DrawRect(0, 0, _bitmapWidth, _bitmapHeight, _backgroundPaint);
        }
        #endregion

        #region Ctor
        public RaidTacticsDrawer(ClanRaid raid)
        {
            _raid = raid;

            CalculateMeasures();

            _bitmap = new SKBitmap(_bitmapWidth, _bitmapHeight);
            _canvas = new SKCanvas(_bitmap);

            DrawBackground();
        } 
        #endregion

        #region public functions
        /// <summary>
        /// Draws the bitmap
        /// </summary>
        public void Draw()
        {
            DrawHeader(10, 10); // h = 200
            DrawTolerances(350, 10, 525, 200); // h = 200

            DrawStrategies(10, 220); // h = 250
        }

        /// <summary>
        /// Saves image to bitmap
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> SaveImage()
        {
            using (var memStream = new MemoryStream())
            using (var wstream = new SKManagedWStream(memStream))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                _bitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
#pragma warning restore CS0618 // Type or member is obsolete
                byte[] data = memStream.ToArray();

                // Check the data array for content!

                var result = await DependencyService.Get<IPhotoLibrary>().SavePhotoAsync(data, "TT2Master", $"raid_tactics_{_raid.Tier}-{_raid.Level}_{DateTime.Now.ToString("yyyy_MM_dd_hh_ss")}.png");

                // Check return value for success!
                return result;
            }
        } 
        #endregion
    }
}
