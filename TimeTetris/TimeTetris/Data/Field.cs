using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public partial class Field
    {
        public Row Bottom { get; protected set; }
        public Row Top { get; protected set; }

        public Int32 Width { get; protected set; }
        public Int32 Height { get; protected set; }

        public FallingBlock CurrentBlock { get; set; }
        public Block NextBlock { get; set; }
        public Timeline Timeline { get; protected set; }

        /// <summary>
        /// Creates a new field
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Field(Timeline timeline, Int32 width, Int32 height)
        {
            this.Timeline = timeline;

            this.Width = width;
            this.Height = height;

            this.Bottom = new Row(width);
            this.Top = new Row(width);
            this.Bottom.Next = Top;
            this.Top.Prev = Bottom;

            for (int i = 0; i < height; i++)
                this.Bottom.InsertAfter(new Row(width));

            SetupBlockGenerator();
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

            this.Timeline.Add(new Event()
            {
                // Puts the block in the grid
                Apply = () =>
                    {
                        for (Int32 x = 0; x < block.Width; x++)
                            for (Int32 y = 0; y < block.Height; y++)
                                if (block[x, y])
                                    this[startX + x, startY + y] = color;
                    },

                // Removes the block from the grid
                Undo = () =>
                    {
                        for (Int32 x = 0; x < block.Width; x++)
                            for (Int32 y = 0; y < block.Height; y++)
                                if (block[x, y])
                                    this[startX + x, startY + y] = 0;
                    }
            });

            GenerateNextBlock();

            // TODO CHECK ROW FULL
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
                    if (block[a, b] && this[x + a, y + b] != 0)
                        return true;
            return false;
        }
    }
}
