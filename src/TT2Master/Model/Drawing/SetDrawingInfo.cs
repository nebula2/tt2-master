using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Loggers;
using TT2Master.Model.Pets;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Drawing
{
    public class SetDrawingInfo
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

        private readonly float _textFactor = 1f;

        /// <summary>
        /// Paint for Level
        /// </summary>
        public SKPaint UnfinishedPaint { get; private set; }

        public SKPaint FinishedPaint { get; private set; }

        public SKCanvas Canvas { get; private set; }

        private List<EquipmentSet> _sets;
        #endregion

        #region private methods
        private void InitializeColors()
        {
            UnfinishedPaint = new SKPaint
            {
                IsStroke = false,
                Color = SKColors.Red,
                TextSize = SkillSize * _textFactor,
                TextAlign = SKTextAlign.Left,
            };

            FinishedPaint = new SKPaint
            {
                IsStroke = false,
                Color = SKColors.Green,
                TextSize = SkillSize * _textFactor,
                TextAlign = SKTextAlign.Left,
            };

        }

        private float GetSlotXCoordinate(int column) => (column * SlotWidth) + StartX + SlotFreeWidth;

        private float GetSlotYCoordinate(int row) => (row * SlotHeight) + StartY + SlotFreeHeight;
        #endregion

        #region Ctor
        public SetDrawingInfo(int width, int height, SKCanvas canvas)
        {
            TotalWidth = width;
            TotalHeight = height;
            Canvas = canvas;

            StartX = 0;
            StartY = 0;

            SkillSize = 50;
            SlotFreeWidth = 5;
            SlotFreeHeight = 5;

            ColumnCount = 1;

            SlotWidth = TotalWidth / ColumnCount;
            SlotHeight = SkillSize + SlotFreeHeight;

            InitializeColors();
        }
        #endregion

        #region Private methods
        private void Init()
        {
            EquipmentHandler.OnLogMePlease += PetHandler_OnLogMePlease;
            EquipmentHandler.OnProblemHaving += PetHandler_OnProblemHaving;

            bool loaded = EquipmentHandler.Load();
            bool setloaded = EquipmentHandler.LoadSetInformation(App.Save);
            bool filled = false;
            if (loaded && setloaded)
            {
                EquipmentHandler.FillEquipment(App.Save);
                filled = true;
            }
            else
            {
                filled = false;
            }

            EquipmentHandler.OnLogMePlease -= PetHandler_OnLogMePlease;
            EquipmentHandler.OnProblemHaving -= PetHandler_OnProblemHaving;

            if (!filled)
            {
                return;
            }

            _sets = EquipmentHandler.EquipmentSets.Where(x => x.SetType == "Mythic").ToList();

            int correctionVal = _sets.Count % ColumnCount != 0 ? 1 : 0;
            RowCount = (_sets.Count / ColumnCount) + correctionVal;
        }

        private static string GetLevelString(EquipmentSet item) => item.Completed ? $"{item.Set} completed" : $"{item.Set} not completed";
        #endregion

        #region Public Functions
        public void SetCoordinates(float startX, float startY)
        {
            StartX = startX;
            StartY = startY;
        }

        public void Draw()
        {
            Init();

            if (_sets == null)
            {
                return;
            }
            if (_sets.Count == 0)
            {
                return;
            }

            int idCounter = 0;

            // draw grid
            for (int i = 0; i < RowCount; i++)
            {
                for (int k = 0; k < ColumnCount; k++)
                {
                    //check if we are somehow out of bounds
                    if (idCounter == _sets.Count)
                    {
                        return;
                    }

                    // get artifact 
                    var itemToPaint = _sets[idCounter];

                    //// get image
                    //var imgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource(PetHandler.GetImagePathForDrawerId(itemToPaint.PetId));

                    float coordX = GetSlotXCoordinate(k);
                    float coordY = GetSlotYCoordinate(i);

                    //var destRect = new SKRect(
                    //      left: coordX
                    //    , top: coordY
                    //    , right: coordX + SkillSize
                    //    , bottom: coordY + SkillSize);

                    //// draw bitmap
                    //Canvas.DrawBitmap(imgSrc, destRect);

                    // draw level
                    string levelStr = GetLevelString(itemToPaint);

                    Canvas.DrawText(levelStr
                            , coordX + SlotFreeWidth
                            , coordY + SkillSize * 0.5f
                            , itemToPaint.Completed ? FinishedPaint : UnfinishedPaint);

                    idCounter++;
                }
            }
        }
        #endregion

        #region E + D
        private void PetHandler_OnProblemHaving(Exception e) => Logger.WriteToLogFile($"PetHandler Error: {e.Message}");
        private void PetHandler_OnLogMePlease(string message) => Logger.WriteToLogFile($"PetHandler Info: {message}");
        #endregion
    }
}