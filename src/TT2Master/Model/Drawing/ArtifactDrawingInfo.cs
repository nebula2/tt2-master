using SkiaSharp;
using System;
using System.Collections.Generic;
using TT2Master.Model.Arti;

namespace TT2Master.Model.Drawing
{
    public class ArtifactDrawingInfo
    {
        #region Properties
        /// <summary>
        /// Total width
        /// </summary>
        public int TotalWidth { get; private set; }

        /// <summary>
        /// Total height
        /// </summary>
        public int TotalHeight { get; private set; }

        /// <summary>
        /// Start Coordinate X
        /// </summary>
        public float StartX { get; private set; }

        /// <summary>
        /// Start Coordinate Y
        /// </summary>
        public float StartY { get; private set; }

        /// <summary>
        /// Amount of columns
        /// </summary>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Amount of rows
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Height of row
        /// </summary>
        public int SlotHeight { get; private set; }

        /// <summary>
        /// Width of a Slot
        /// </summary>
        public int SlotWidth { get; private set; }

        /// <summary>
        /// Size of an Image (use this for width and height)
        /// </summary>
        public int SkillSize { get; private set; }

        public int SlotFreeWidth { get; private set; }

        public int SlotFreeHeight { get; private set; }

        private readonly float _textFactor = 0.35f;

        /// <summary>
        /// Paint for Level
        /// </summary>
        public SKPaint LevelPaint { get; private set; }
        public SKPaint EnchantmentPaint { get; private set; }
        public SKPaint ItemPaint { get; private set; }

        public SKCanvas Canvas { get; private set; }
        #endregion

        #region private methods
        private void InitializeColors()
        {
            LevelPaint = new SKPaint
            {
                IsStroke = false,
                Color = SKColors.White,
                TextSize = SkillSize * _textFactor,
                TextAlign = SKTextAlign.Center,
            };

            EnchantmentPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Purple,
                StrokeWidth = 4,
            };

            ItemPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White,
                StrokeWidth = 4,
            };
        }

        private float GetSlotXCoordinate(int row) => (row * SlotWidth) + StartX + SlotFreeWidth;

        private float GetSlotYCoordinate(int column) => (column * SlotHeight) + StartY + SlotFreeHeight;
        #endregion

        #region Ctor
        public ArtifactDrawingInfo(int width, int height, SKCanvas canvas)
        {
            TotalWidth = width;
            TotalHeight = height;
            Canvas = canvas;

            StartX = 0;
            StartY = 0;

            SkillSize = 90;
            SlotFreeWidth = 5;
            SlotFreeHeight = 5;

            ColumnCount = TotalWidth / ArtifactHandler.Artifacts.Count;
            int correctionVal = ArtifactHandler.Artifacts.Count % ColumnCount != 0 ? 1 : 0;
            RowCount = (ArtifactHandler.Artifacts.Count / ColumnCount) + correctionVal;

            SlotWidth = SkillSize + SlotFreeWidth;
            SlotHeight = SkillSize + SlotFreeHeight;

            InitializeColors();
        }
        #endregion

        #region Public Functions
        public void SetCoordinates(float startX, float startY)
        {
            StartX = startX;
            StartY = startY;
        } 

        public void Draw()
        {
            if (ArtifactHandler.Artifacts == null)
            {
                return;
            }
            if (ArtifactHandler.Artifacts.Count == 0)
            {
                return;
            }

            int artifactIdCounter = 0;

            // draw grid
            for (int i = 0; i < RowCount; i++)
            {
                for (int k = 0; k < ColumnCount; k++)
                {
                    //check if we are somehow out of bounds
                    if(artifactIdCounter == ArtifactHandler.Artifacts.Count)
                    {
                        return;
                    }

                    // get artifact 
                    var itemToPaint = ArtifactHandler.Artifacts[artifactIdCounter];

                    // get image
                    var imgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource(itemToPaint.GetImageSourceId());

                    float coordX = GetSlotXCoordinate(k);
                    float coordY = GetSlotYCoordinate(i);

                    var destRect = new SKRect(
                          left: coordX
                        , top: coordY
                        , right: coordX + SkillSize
                        , bottom: coordY + SkillSize);

                    var backgroundRect = new SKRect(
                          left: coordX - SlotFreeWidth * 0.5f
                        , top: coordY - SlotFreeHeight * 0.5f
                        , right: coordX + SkillSize + (SlotFreeWidth * 0.5f)
                        , bottom: coordY + SlotHeight + (SlotFreeHeight * 0.5f));

                    // draw background
                    Canvas.DrawRect(backgroundRect, itemToPaint.EnchantmentLevel > 0 ? EnchantmentPaint : ItemPaint);

                    // draw bitmap
                    Canvas.DrawBitmap(imgSrc, destRect);

                    // draw level
                    string levelStr = GetLevelString(itemToPaint.Level);

                    Canvas.DrawText(levelStr
                            , coordX + SkillSize * 0.5f
                            , coordY + SkillSize * 0.8f
                            , LevelPaint);

                    artifactIdCounter++;
                }
            }
        }

        private static string GetLevelString(double level) => level < 100 ? level.ToString() : string.Format("{0:#.#e00}", level);
        #endregion
    }
}