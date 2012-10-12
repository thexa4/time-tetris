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
        /// 
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
        /// 
        /// </summary>
        public void LockFalling()
        {
            // TODO timeline event

            var color = Block.ToGridValue(CurrentBlock.Block.Type);
            for (int x = 0; x < CurrentBlock.Block.Width; x++)
                for (int y = 0; y < CurrentBlock.Block.Height; y++)
                    this[CurrentBlock.X + x, CurrentBlock.Y + y] = color;

            GenerateNextBlock();
        }

        public Int32 this[int x, int y]
        {
            get {
                if(x < 0 || x >= Width)
                    return -1;
                if (y < 0 || y >= Height)
                    return -1;
                Row cur = Bottom;
                for (int i = 0; i <= y; i++)
                    cur = cur.Next;
                return cur.Values[x];
            }
            set
            {
                Row cur = Bottom;
                for (int i = 0; i <= y; i++)
                    cur = cur.Next;
                cur.Values[x] = value;
            }
        }

        public bool Collides(Block block, int x, int y)
        {
            for (int a = 0; a < block.Width; a++)
                for (int b = 0; b < block.Height; b++)
                    if (block[a, b] && this[x + a, y + b] != 0)
                        return true;
            return false;
        }
    }
}
