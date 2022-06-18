using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Interfaces;
using Xamarin.Forms;

namespace TT2Master.Model.Drawing
{
    public class ProfileDrawer
    {
        private ArtifactDrawingInfo _artifactDrawingInfo;
        private BranchDrawingInfo _branchDrawingInfo;
        private PetsDrawingInfo _petsDrawingInfo;
        private SetDrawingInfo _setDrawingInfo;
        private SKBitmap _bitmap;
        private SKCanvas _canvas;

        private int _bitmapWidth = 1600;
        private int _bitmapHeight = 2400;

        private SKPaint _textPaint = new SKPaint
        {
            IsStroke = false,
            Color = SKColors.White,
            TextSize = 50,
            TextAlign = SKTextAlign.Left,
        };

    public ProfileDrawer()
        {
            _bitmap = new SKBitmap(_bitmapWidth, _bitmapHeight);
            _canvas = new SKCanvas(_bitmap);
        }

        private void Initialize()
        {
            _branchDrawingInfo = new BranchDrawingInfo(400, 400, _canvas);
            _artifactDrawingInfo = new ArtifactDrawingInfo(800, 1500, _canvas);
            _petsDrawingInfo = new PetsDrawingInfo(800, 800, _canvas);
            _setDrawingInfo = new SetDrawingInfo(800, 1500, _canvas);
        }

        private void DrawPlayerInfo()
        {
            string profileStr1 = App.Save.ThisPlayer.PlayerName == "__DUMMYPLAYERNAME__"
                ? "Player Data"
                : $"{App.Save.ThisPlayer.PlayerName} - {App.Save.ThisClan.Name}";
            string profileStr2 = $"SP {App.Save.ThisPlayer.TotalSkillPoints} MS {App.Save.ThisPlayer.StageMax}";

            _canvas.DrawText(profileStr1, 10, 50, _textPaint);
            _canvas.DrawText(profileStr2, 800, 50, _textPaint);
        }

        private void DrawSkillTree()
        {
            // Create tree
            SkillInfoHandler.LoadSkills();
            SkillInfoHandler.FillSkills(App.Save);
            SkillInfoHandler.PopulateOptSkills(App.Save);

            var finalTree = SkillInfoHandler.OptSkills;

            // draw skills
            _branchDrawingInfo.SetCoordinates(0f, 100f);
            _branchDrawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchRed").ToList());

            _branchDrawingInfo.SetCoordinates(400f, 100f);
            _branchDrawingInfo.SetSkillColorFromBranchName("BranchYellow");
            _branchDrawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchYellow").ToList());

            _branchDrawingInfo.SetCoordinates(0f, 500f);
            _branchDrawingInfo.SetSkillColorFromBranchName("BranchBlue");
            _branchDrawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchBlue").ToList());

            _branchDrawingInfo.SetCoordinates(400f, 500f);
            _branchDrawingInfo.SetSkillColorFromBranchName("BranchGreen");
            _branchDrawingInfo.DrawBranch(finalTree.Where(x => x.Branch == "BranchGreen").ToList());
        }

        private void DrawArtifacts()
        {
            _artifactDrawingInfo.SetCoordinates(0f, 900f);
            _artifactDrawingInfo.Draw();
        }

        private void DrawPets()
        {
            _petsDrawingInfo.SetCoordinates(800f, 100f);
            _petsDrawingInfo.Draw();
        }

        private void DrawSetInfo()
        {
            _setDrawingInfo.SetCoordinates(800f, 1600f);
            _setDrawingInfo.Draw();
        }

        public void Draw()
        {
            Initialize();

            DrawPlayerInfo();

            DrawSkillTree();

            DrawArtifacts();

            DrawPets();

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

                var result = await DependencyService.Get<IPhotoLibrary>().SavePhotoAsync(data, "TT2Master", $"profile{DateTime.Now.ToString("yyyy_MM_dd_hh_ss")}.jpg");

                // Check return value for success!
                return result;
            }
        }
    }
}
