﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class Field
    {
        public Row Bottom { get; protected set; }
        public Row Top { get; protected set; }

        public Int32 Width { get; protected set; }
        public Int32 Height { get; protected set; }

        public FallingBlock CurrentBlock { get; set; }
        public Block NextBlock { get; set; }
        public Timeline Timeline { get; protected set; }

        public Field(Int32 width, Int32 height)
        {
            Timeline = new Data.Timeline();

            Width = width;
            Height = height;

            Bottom = new Row(width);
            Top = new Row(width);
            Bottom.Next = Top;
            Top.Prev = Bottom;

            for (int i = 0; i < height; i++)
                Bottom.InsertAfter(new Row(width));
        }

        public void LockFalling()
        {
            for(int x = 0; x < CurrentBlock.Block.Width; x++)
                for (int y = 0; y < CurrentBlock.Block.Height; y++)
                    this[CurrentBlock.X + x, CurrentBlock.Y + y] = CurrentBlock.Color;
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
