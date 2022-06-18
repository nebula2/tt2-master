using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Interfaces;
using Xamarin.Forms;

namespace TT2Master.Model.Drawing
{
    public class RaidDrawer
    {
        private RaidcardDrawingInfo _raidcardDrawingInfo;
        private RaidcardSetDrawingInfo _raidcardSetDrawingInfo;
        private SKBitmap _bitmap;
        private SKCanvas _canvas;

        private int _bitmapWidth = 1600;
        private int _bitmapHeight = 1200;

        private SKPaint _textPaint = new SKPaint
        {
            IsStroke = false,
            Color = SKColors.White,
            TextSize = 50,
            TextAlign = SKTextAlign.Left,
        };

        public RaidDrawer()
        {
            _bitmap = new SKBitmap(_bitmapWidth, _bitmapHeight);
            _canvas = new SKCanvas(_bitmap);
        }

        private void Initialize()
        {
            _raidcardDrawingInfo = new RaidcardDrawingInfo(700, 1050, _canvas);
            _raidcardSetDrawingInfo = new RaidcardSetDrawingInfo(700, 1050, _canvas);
        }

        private void DrawPlayerInfo()
        {
            string profileStr1 = $"{App.Save.ThisPlayer.PlayerName} - {App.Save.ThisClan.Name}";
            _canvas.DrawText(profileStr1, 10, 50, _textPaint);

            string profileStr2 = $"SP {App.Save.ThisPlayer.TotalSkillPoints} MS {App.Save.ThisPlayer.StageMax}";
            _canvas.DrawText(profileStr2, 800, 50, _textPaint);

            string infoStr1 = $"CARDS";
            _canvas.DrawText(infoStr1, 10, 100, _textPaint);

            string infoStr2 = $"DECKS";
            _canvas.DrawText(infoStr2, 800, 100, _textPaint);
        }

        private void DrawCards()
        {
            _raidcardDrawingInfo.SetCoordinates(0f, 150f);
            _raidcardDrawingInfo.Draw();
        }

        private void DrawSetInfo()
        {
            _raidcardSetDrawingInfo.SetCoordinates(800f, 150f);
            _raidcardSetDrawingInfo.Draw();
        }

        public void Draw()
        {
            Initialize();

            DrawPlayerInfo();

            DrawCards();

            DrawSetInfo();
        }

        public async Task<(bool, string)> SaveImage()
        {
            using (var memStream = new MemoryStream())
            using (var wstream = new SKManagedWStream(memStream))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                _bitmap.Encode(wstream, SKEncodedImageFormat.Jpeg, 90);
#pragma warning restore CS0618 // Type or member is obsolete
                byte[] data = memStream.ToArray();

                // Check the data array for content!

                var result = await DependencyService.Get<IPhotoLibrary>().SavePhotoAsync(data, "TT2Master", $"raid{DateTime.Now.ToString("yyyy_MM_dd_hh_ss")}.jpg");

                // Check return value for success!
                return result;
            }
        }
    }
}
