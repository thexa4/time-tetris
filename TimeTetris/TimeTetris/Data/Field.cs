using System;
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

        public Field(Int32 width, Int32 height)
        {
            Width = width;
            Height = height;

            Bottom = new Row(width);
            Top = new Row(width);
            Bottom.Next = Top;
            Top.Prev = Bottom;

            for (int i = 0; i < height; i++)
                Bottom.InsertAfter(new Row(width));
        }
    }
}
