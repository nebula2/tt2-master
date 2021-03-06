using SkiaSharp;
using System;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Clan;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Drawing
{
    public class RaidcardSetDrawingInfo
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
                TextAlign = SKTextAlign.Left,
            };
        }

        private float GetSlotXCoordinate(int column) => (column * SlotWidth) + StartX + SlotFreeWidth;

        private float GetSlotYCoordinate(int row) => (row * SlotHeight) + StartY + SlotFreeHeight;
        #endregion

        #region Ctor
        public RaidcardSetDrawingInfo(int width, int height, SKCanvas canvas)
        {
            TotalWidth = width;
            TotalHeight = height;
            Canvas = canvas;

            StartX = 0;
            StartY = 0;

            SkillSize = 90;
            SlotFreeWidth = 5;
            SlotFreeHeight = 5;

            ColumnCount = 3;

            SlotWidth = TotalWidth / ColumnCount;
            SlotHeight = SkillSize + SlotFreeHeight;

            InitializeColors();
        }
        #endregion

        #region Private methods
        private void Init()
        {
            RaidCardHandler.OnLogMePlease += PetHandler_OnLogMePlease;
            RaidCardHandler.OnProblemHaving += PetHandler_OnProblemHaving;

            bool loaded = RaidCardHandler.LoadItemsFromInfofile();
            bool filled = false;
            if (loaded)
            {
                filled = RaidCardHandler.FillItems();
            }

            RaidCardHandler.OnLogMePlease -= PetHandler_OnLogMePlease;
            RaidCardHandler.OnProblemHaving -= PetHandler_OnProblemHaving;

            if (!filled)
            {
                return;
            }

            RowCount = RaidCardHandler.RaidCardSets.Count;
        }

        private static string GetLevelString(RaidCard item) => $"Lv. {item.Level}";

        
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

            if (RaidCardHandler.RaidCards == null)
            {
                return;
            }
            if (RaidCardHandler.RaidCards.Count == 0)
            {
                return;
            }

            if (RaidCardHandler.RaidCardSets == null)
            {
                return;
            }
            if (RaidCardHandler.RaidCardSets.Count == 0)
            {
                return;
            }

            int idCounter = 0;

            // draw grid
            for (int i = 0; i < RowCount; i++)
            {
                //check if we are somehow out of bounds
                if (idCounter == RaidCardHandler.RaidCardSets.Count)
                {
                    return;
                }

                // get artifact 
                var setToPaint = RaidCardHandler.RaidCardSets[idCounter];


                for (int k = 0; k < RaidCardSet.MaxSlotCount; k++)
                {
                    // get item
                    var itemToPaint = setToPaint.GetRaidCard(k);

                    if(itemToPaint == null)
                    {
                        continue;
                    }

                    // get image
                    var imgSrc = Xamarin.Forms.DependencyService.Get<IGetBitmapResources>().GetDecodedResource(RaidCardHandler.GetImagePathForDrawerId(itemToPaint.CardId));

                    float coordX = GetSlotXCoordinate(k);
                    float coordY = GetSlotYCoordinate(i);

                    var destRect = new SKRect(
                          left: coordX
                        , top: coordY
                        , right: coordX + SkillSize
                        , bottom: coordY + SkillSize);

                    // draw bitmap
                    Canvas.DrawBitmap(imgSrc, destRect);

                    // draw level
                    string levelStr = GetLevelString(itemToPaint);

                    Canvas.DrawText(levelStr
                            , coordX + SkillSize + SlotFreeWidth
                            , coordY + SkillSize * 0.8f
                            , LevelPaint);

                    
                }

                idCounter++;
            }
        }
        #endregion

        #region E + D
        private void PetHandler_OnProblemHaving(object sender, CustErrorEventArgs e) => Logger.WriteToLogFile($"ItemHandler Error at {sender.ToString()}: {e.MyException.Message}\n{e.MyException.Data}");
        private void PetHandler_OnLogMePlease(object sender, InformationEventArgs e) => Logger.WriteToLogFile($"ItemHandler Info at {sender.ToString()}: {e.Information}");
        #endregion
    }
}