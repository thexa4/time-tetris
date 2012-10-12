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
        public Row Bottom { get; protected set; }
        public Row Top { get; protected set; }

        public Int32 Width { get; protected set; }
        public Int32 Height { get; protected set; }

        public FallingBlock CurrentBlock { get; set; }
        public Block NextBlock { get; set; }
        public Timeline Timeline { get; protected set; }

        public Int32 Level { get; set; }
        public Boolean HasEnded { get; set; }

        private Int32 _score;
        public Int32 Score { get { return _score; } protected set { _score = Math.Max(0, value); } }
        public Int32 ComboCount { get; protected set; }

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
            this.Level = 3;

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

                        for (Int32 x = 0; x < block.Width; x++)
                            for (Int32 y = 0; y < block.Height; y++)
                                if (block[x, block.Height - 1 - y])
                                {
                                    if (startY - y >= Height - SpriteField.HiddenRows)
                                    {
                                        // Game over!
                                        HasEnded = true;
                                    }
                                    this[startX + x, startY - y] = color;
                                }
                    },

                // Removes the block from the grid
                Undo = () =>
                    {
                        this.Score -= score;

                        for (Int32 x = 0; x < block.Width; x++)
                            for (Int32 y = 0; y < block.Height; y++)
                                if (block[x, block.Height - 1 - y])
                                {
                                    if (startY - y >= Height - SpriteField.HiddenRows)
                                        HasEnded = false;
                                    this[startX + x, startY - y] = 0;
                                }
                    }
            });

            if(!HasEnded)
                GenerateNextBlock();

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

            var clearScore = Points.ClearLines(rows, this.Level);
            var comboScore = this.ComboCount * Points.ClearCombo(this.Level);
            var oldComboCount = this.ComboCount;

            this.Timeline.Add(new Event()
                {
                    Apply = () => 
                    { 
                        this.ComboCount = (rows == 0) ? 0 : oldComboCount + 1;
                        this.Score += clearScore + ((rows == 0) ? 0 : comboScore);
                    },
                    Undo = () => 
                    {
                        this.ComboCount = oldComboCount;
                        this.Score -= clearScore + ((rows == 0) ? 0 : comboScore); 
                    },
                });

            

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <param name="values"></param>
        protected void CreateFullRow(int y, int[] values)
        {
            Row cur = Bottom;
            for (; y > 0; y--)
                cur = cur.Next;
            Row f = new Row(Width);
            f.Values = values;
            cur.InsertAfter(f);

            Top.Prev.Remove();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        protected void RemoveRow(int y)
        {
            Row cur = Bottom;
            for (; y >= 0; y--)
                cur = cur.Next;
            cur.Remove();

            Top.Prev.InsertAfter(new Row(Width));
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
        }
    }
}
