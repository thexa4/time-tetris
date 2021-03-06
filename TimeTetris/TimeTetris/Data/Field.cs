﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeTetris.Drawing;
using TimeTetris.Services;

namespace TimeTetris.Data
{
    public partial class Field : GameComponent
    {
        /// <summary>
        /// Bewlo lowest row in the field
        /// </summary>
        public Row Bottom { get; protected set; }

        /// <summary>
        /// Above highest row in the field
        /// </summary>
        public Row Top { get; protected set; }

        /// <summary>
        /// Grid Width
        /// </summary>
        public Int32 Width { get; protected set; }

        /// <summary>
        /// Grid Height
        /// </summary>
        public Int32 Height { get; protected set; }
        
        /// <summary>
        /// Active falling block
        /// </summary>
        public FallingBlock CurrentBlock { get; set; }

        /// <summary>
        /// Next block
        /// </summary>
        public Block NextBlock { get; set; }

        /// <summary>
        /// Next block
        /// </summary>
        public Block HoldBlock { get; set; }
        private Boolean _holdLocked;

        /// <summary>
        /// Timeline
        /// </summary>
        public Timeline Timeline { get; protected set; }

        /// <summary>
        /// Current Level
        /// </summary>
        public Int32 Level { get { return this.LinesCleared / 10 + 1; } }

        /// <summary>
        /// Number of lines cleared
        /// </summary>
        public Int32 LinesCleared { get; set; }

        /// <summary>
        /// Game has ended (died) flag
        /// </summary>
        public Boolean HasEnded { get; set; }

        /// <summary>
        /// On Game End (game over)
        /// </summary>
        public event EventHandler OnGameEnded = delegate { };

        /// <summary>
        /// On Rows cleared
        /// </summary>
        public event RowsDelegate OnRowsCleared = delegate { };

        /// <summary>
        /// On Points earned
        /// </summary>
        public event PointsDelegate OnPointsEarned = delegate { };

        /// <summary>
        /// Current score (double so we can subtract partial points)
        /// </summary>
        public Double Score { get { return _score; } protected set { _score = Math.Max(0, value); } }
        private Double _score;

        /// <summary>
        /// Current Combo count
        /// </summary>
        public Int32 CurrentCombo { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsBackToBackEnabled { get; protected set; }

        /// <summary>
        /// Creates a new field
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="timeline">Timeline service</param>
        /// <param name="width">Field width</param>
        /// <param name="height">Field height</param>
        public Field(Game game, Timeline timeline, Int32 width, Int32 height)
            : base(game)
        {
            this.Timeline = timeline;
            this.LinesCleared = 0;

            this.Width = width;
            this.Height = height;
            
            this.Bottom = new Row(width);
            this.Top = new Row(width);
            this.Bottom.Next = Top;
            this.Top.Prev = Bottom;

            for (int i = 0; i < height; i++)
                this.Bottom.InsertAfter(new Row(width));
        }

        /// <summary>
        /// Intializes the field
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            SetupBlockGenerator();
            this.CurrentBlock.Initialize();
        }

        /// <summary>
        /// Locks a falling block
        /// </summary>
        public void LockFalling()
        {
            _holdLocked = false;

            var color = Block.ToGridValue(this.CurrentBlock.Block.Type);
            var startX = this.CurrentBlock.X;
            var startY = this.CurrentBlock.Y;
            var block = this.CurrentBlock.Block;
            var score = this.CurrentBlock.BlockPoints;

            this.Timeline.Add(new Event()
            {
                // Puts the block in the grid
                Apply = () =>
                    {
                        this.Score += score;
                        this.OnPointsEarned(score);

                        var dead = false;

                        for (Int32 x = 0; x < block.Width; x++)
                            for (Int32 y = 0; y < block.Height; y++)
                                if (block[x, block.Height - 1 - y])
                                {
                                    if (startY - y >= Height - SpriteField.HiddenRows)
                                        dead = true;
                                    this[startX + x, startY - y] = color;
                                }

                        if (dead)
                        {
                            // Game over!
                            HasEnded = true;
                            this.Timeline.Stop();

                            this.OnGameEnded.Invoke(this, EventArgs.Empty);
                        }
                    },

                // Removes the block from the grid
                Undo = () =>
                    {
                        this.Score -= score;

                        var died = false;

                        for (Int32 x = 0; x < block.Width; x++)
                            for (Int32 y = 0; y < block.Height; y++)
                                if (block[x, block.Height - 1 - y])
                                {
                                    if (startY - y >= Height - SpriteField.HiddenRows)
                                        died = true;
                                    this[startX + x, startY - y] = 0;
                                }

                        // Game un-over
                        if (died)
                        {
                            HasEnded = false;
                            this.Timeline.Resume();
                        }
                    }
            });

            // TSpin check before rows are gone
            var isTSpin = this.CurrentBlock.LastMoveIsRotation && this.CurrentBlock.IsTAndImmobileThree;

            // Remove full rows
            Row cur = Bottom;
            Int32 futurey = 0;
            Int32 rows = 0;
            for (Int32 y = 0; y < Height; y++)
            {
                cur = cur.Next;
                futurey++;
                if (cur.IsFull)
                {
                    rows++;

                    futurey--;
                    Int32 storedy = futurey;
                    Int32[] values = (Int32[])cur.Values.Clone();

                    this.Timeline.Add(new Event()
                    {
                        Undo = () => CreateFullRow(storedy, values),
                        Apply = () => RemoveRow(storedy),
                    });
                }
            }

            // Score process

            var isTetris = rows == 4;
            var isB2b = this.IsBackToBackEnabled && (isTetris || isTSpin);

            var clearScore = Points.ClearLines(rows, this.Level);
            var comboScore = rows == 0 ? 0 : this.CurrentCombo * Points.ClearCombo(this.Level);
            var tspinScore = isTSpin ? Points.TSpin(rows, this.Level, this.CurrentBlock.LastMoveIsKick) : 0;
            var actionScore = clearScore + comboScore + tspinScore;
            if (isB2b)
                actionScore = (Int32)Math.Round((3 / 2d) * actionScore);

            var oldB2B = this.IsBackToBackEnabled;
            var oldComboCount = this.CurrentCombo;

            this.Timeline.Add(new Event()
                {
                    Apply = () => 
                    { 
                        this.CurrentCombo = (rows == 0) ? 0 : oldComboCount + 1;
                        this.Score += actionScore;
                        this.LinesCleared += rows;

                        this.IsBackToBackEnabled = (rows == 0) ? oldB2B : (isTetris || isTSpin);
                        this.OnRowsCleared(rows, (rows == 0) ? 0 : oldComboCount, isTSpin, isB2b);
                        this.OnPointsEarned(actionScore);
                    },
                    Undo = () => 
                    {
                        this.CurrentCombo = oldComboCount;
                        this.Score -= actionScore;
                        this.LinesCleared -= rows;
                        this.IsBackToBackEnabled = oldB2B;
                    },
                });

            if (!HasEnded)
                GenerateNextBlock();
        }

        /// <summary>
        /// Creates (introduces) a (full) row by set values
        /// </summary>
        /// <param name="y">Insert at</param>
        /// <param name="values">Values to insert</param>
        protected void CreateFullRow(int y, int[] values)
        {
            Row cur = this.Bottom;
            for (; y > 0; y--)
                cur = cur.Next;
            Row f = new Row(this.Width);
            f.Values = values;
            cur.InsertAfter(f);

            this.Top.Prev.Remove();
        }

        /// <summary>
        /// Removes a (full) row
        /// </summary>
        /// <param name="y">Remove at</param>
        protected void RemoveRow(int y)
        {
            Row cur = this.Bottom;
            for (; y >= 0; y--)
                cur = cur.Next;
            cur.Remove();

            this.Top.Prev.InsertAfter(new Row(this.Width));
        }

        /// <summary>
        /// Returns the value on a position
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <returns>Grid color value</returns>
        public Int32 this[Int32 x, Int32 y]
        {
            get {
                if(x < 0 || x >= Width)
                    return -1;
                if (y < 0 || y >= Height)
                    return -1;
                Row cur = Bottom;
                for (Int32 i = 0; i <= y; i++)
                    cur = cur.Next;
                return cur.Values[x];
            }

            set
            {
                Row cur = Bottom;
                for (Int32 i = 0; i <= y; i++)
                    cur = cur.Next;
                cur.Values[x] = value;
            }
        }

        /// <summary>
        /// Checks if a block collides if placed on a position
        /// </summary>
        /// <param name="block">block to check</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <returns>Collision flag</returns>
        public Boolean Collides(Block block, Int32 x, Int32 y)
        {
            for (Int32 a = 0; a < block.Width; a++)
                for (Int32 b = 0; b < block.Height; b++)
                    if (block[a, block.Height - 1 - b] && this[x + a, y - b] != 0)
                        return true;
            return false;
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.CurrentBlock.Update(gameTime);

            if (this.Timeline.IsRewindActive)
                this.Score -= Points.Rewind * (this.Level / Points.RewindLevelTax + 1) * gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Switches holding block with current
        /// </summary>
        public Boolean SwitchHoldingBlock()
        {
            if (_holdLocked)
                return false;
            _holdLocked = true;

            // Always drop hold blocks from the top
            var baseY = this.CurrentBlock.Field.Height - 1;

            var oldType = this.CurrentBlock.Block.Type;
            var oldR = this.CurrentBlock.Block.Rotation;
            var oldPoints = this.CurrentBlock.BlockPoints;
            var oldX = (this.Width - this.CurrentBlock.Block.Width) / 2 + Block.GetBaseXPosition(oldType);

            // If this is the first too hold
            if (this.HoldBlock == null)
            {
                // We can't replace the current block, so we generate a new one
                GenerateNextBlock();

                this.Timeline.Add(new Event()
                {
                    Apply = () =>
                        {
                            this.HoldBlock = new Block(oldType);
                        },

                    Undo = () =>
                        {
                            this.HoldBlock = null;
                            this.CurrentBlock.Replace(oldX, baseY, oldR);
                        },

                });

                return true;
            }

            // Time to switch a block
            var newType = this.HoldBlock.Type;
            var newR = Block.GetStartRotation(newType);
            var newX = (this.Width - Block.BlockTypes[newType].GetLength(0)) / 2 + Block.GetBaseXPosition(newType);


            // Switch the hold block with the current block
            this.Timeline.Add(new Event()
            {
                Apply = () =>
                {
                    this.HoldBlock.SetBlockType(oldType);
                    this.CurrentBlock.Block.SetBlockType(newType);
                    this.CurrentBlock.Replace(newX, baseY, newR);
                },

                Undo = () =>
                {
                    this.HoldBlock.SetBlockType(newType);
                    this.CurrentBlock.Block.SetBlockType(oldType);
                    this.CurrentBlock.Replace(oldX, baseY, oldR);
                },
            });

            return true;
        }
    }
}
