using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Model.SP;

namespace TT2Master.Model.Drawing
{
    public class BranchDrawingInfo
    {
        #region Properties
        /// <summary>
        /// Total width
        /// </summary>
        public int TreeWidth { get; private set; }

        /// <summary>
        /// Total height
        /// </summary>
        public int TreeHeight { get; private set; }

        /// <summary>
        /// Width of a Slot
        /// </summary>
        public int SlotWidth { get; private set; }

        /// <summary>
        /// Width of Tier
        /// </summary>
        public int TierHeight { get; private set; }

        /// <summary>
        /// Size of a Skill (use this for width and height)
        /// </summary>
        public int SkillSize { get; private set; }

        public int SlotFreeWidth { get; private set; }

        public int SlotFreeHeight { get; private set; }

        /// <summary>
        /// Start Coordinate X
        /// </summary>
        public float StartX { get; private set; }

        /// <summary>
        /// Start Coordinate Y
        /// </summary>
        public float StartY { get; private set; }

        /// <summary>
        /// Color for Skill Branch
        /// </summary>
        public SKColor SkillColor { get; private set; }

        /// <summary>
        /// Paint for Skill Line
        /// </summary>
        public SKPaint SkillLinePaint { get; private set; }

        public SKPaint ZeroSkillLinePaint { get; private set; }

        /// <summary>
        /// Paint for Skill Rectangle
        /// </summary>
        public SKPaint SkillPaint { get; private set; }

        /// <summary>
        /// Paint for Level
        /// </summary>
        public SKPaint LevelPaint { get; private set; }

        public SKCanvas Canvas { get; private set; }
        #endregion

        #region private methods
        private void InitializeColors()
        {
            SkillColor = SKColors.Red;

            SkillLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SkillColor,
                StrokeWidth = 10
            };

            ZeroSkillLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Gray,
                StrokeWidth = 10
            };

            SkillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SkillColor,
                StrokeWidth = 4,
            };

            LevelPaint = new SKPaint
            {
                IsStroke = false,
                Color = SKColors.White,
                TextSize = (SkillSize * 0.8f),
                TextAlign = SKTextAlign.Center,
            };
        }
        #endregion

        #region Ctor
        public BranchDrawingInfo(int treeWidth, int treeHeight, SKCanvas canvas)
        {
            TreeWidth = treeWidth;
            TreeHeight = treeHeight;
            
            SlotWidth = TreeWidth / 3;
            TierHeight = TreeHeight / 5;

            SkillSize = Math.Min(90, (SlotWidth < TierHeight ? SlotWidth : TierHeight)); // should be 80 if it fits. Else the smallest it can get in a square

            SlotFreeWidth = SlotWidth - SkillSize;
            SlotFreeHeight = TierHeight - SkillSize;

            StartX = 0;
            StartY = 0;

            Canvas = canvas;

            InitializeColors();
        }
        #endregion

        #region Public Functions
        public void SetCoordinates(float startX, float startY)
        {
            StartX = startX;
            StartY = startY;
        } 

        public void SetSkillColorFromBranchName(string branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName))
            {
                return;
            }

            switch (branchName)
            {
                case "BranchRed":
                    SkillColor = SKColors.Red;
                    break;
                case "BranchYellow":
                    SkillColor = SKColors.Orange;
                    break;
                case "BranchBlue":
                    SkillColor = SKColors.Blue;
                    break;
                case "BranchGreen":
                    SkillColor = SKColors.Green;
                    break;
                default:
                    break;
            }

            SkillLinePaint.Color = SkillColor;
            SkillPaint.Color = SkillColor;
        }

        public void DrawBranch(List<SPOptSkill> skills)
        {
            if (skills == null)
            {
                return;
            }
            if (skills.Count == 0)
            {
                return;
            }

            // Draw line and skill for each slot in tree
            foreach (var item in skills)
            {
                switch (item.Slot)
                {
                    case 0:
                        // first skill in branch. TOP MIDDLE
                        Canvas.DrawRoundRect(StartX + SlotWidth + (SlotFreeWidth / 2)
                            , StartY + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);

                        break;
                    case 1:
                        // Tier II LEFT skill in branch. Line to top and then right
                        Canvas.DrawLine(
                              StartX + (SkillSize / 2) + SlotFreeWidth / 2
                            , StartY + (SlotFreeHeight / 2) + SkillSize / 2
                            , StartX + (SkillSize / 2) + SlotFreeWidth / 2
                            , StartY + TierHeight + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawLine(
                              StartX + (SkillSize / 2) + SlotFreeWidth / 2
                            , StartY + (SlotFreeHeight / 2) + SkillSize / 2
                            , StartX + SlotWidth + SlotFreeWidth / 2
                            , StartY + (SlotFreeHeight / 2) + (SkillSize / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + (SlotFreeWidth / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);

                        break;
                    case 2:

                        // Tier II MIDDLE skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (SlotFreeHeight / 2) + SkillSize
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + SlotWidth + (SlotFreeWidth / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);

                        break;
                    case 3:

                        // Tier II RIGHT skill in branch. Line to top and then to left
                        Canvas.DrawLine(
                              StartX + SlotWidth + (SlotFreeWidth / 2) + SkillSize
                            , StartY + (SlotFreeHeight / 2) + (SkillSize / 2)
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (SlotFreeHeight / 2) + (SkillSize / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawLine(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (SlotFreeHeight / 2) + (SkillSize / 2)
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2) + (SkillSize / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 4:
                        // Tier III LEFT skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2) + SkillSize
                            , StartX + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + (SlotFreeWidth / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 5:

                        // Tier III MIDDLE skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2) + SkillSize
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + SlotWidth + (SlotFreeWidth / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 6:

                        // Tier III RIGHT skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + TierHeight + (SlotFreeHeight / 2) + SkillSize
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 7:
                        // Tier IV LEFT skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2) + SkillSize
                            , StartX + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + (SlotFreeWidth / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 8:
                        // Tier IV MIDDLE skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2) + SkillSize
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + SlotWidth + (SlotFreeWidth / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 9:
                        // Tier IV RIGHT skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (2 * TierHeight) + (SlotFreeHeight / 2) + SkillSize
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 11:
                        // Tier V MIDDLE skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2) + SkillSize
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (4 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + SlotWidth + (SlotFreeWidth / 2)
                            , StartY + (4 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + SlotWidth + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (4 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    case 12:
                        // Tier V RIGHT skill in branch. Line to top
                        Canvas.DrawLine(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (3 * TierHeight) + (SlotFreeHeight / 2) + SkillSize
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (4 * TierHeight) + (SlotFreeHeight / 2)
                            , item.CurrentLevel == 0 ? ZeroSkillLinePaint : SkillLinePaint
                            );

                        Canvas.DrawRoundRect(
                              StartX + (2 * SlotWidth) + (SlotFreeWidth / 2)
                            , StartY + (4 * TierHeight) + (SlotFreeHeight / 2)
                            , SkillSize
                            , SkillSize
                            , 10
                            , 10
                            , SkillPaint);

                        Canvas.DrawText(item.CurrentLevel.ToString()
                            , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                            , StartY + (4 * TierHeight) + (SlotFreeHeight / 2) + SkillSize * 0.8f
                            , LevelPaint);
                        break;
                    default:
                        break;
                }
            }

            Canvas.DrawText(
                  skills.Sum(x => x.GetSpSpentAmount()).ToString()
                , StartX + (2 * SlotWidth) + (SlotFreeWidth / 2) + (SkillSize / 2)
                , StartY + (SlotFreeHeight / 2) + SkillSize * 0.8f
                , LevelPaint);
        }
        #endregion
    }
}