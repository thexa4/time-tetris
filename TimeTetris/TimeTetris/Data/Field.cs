using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    class Field
    {
        Row Bottom { get; protected set; }
        Row Top { get; protected set; }

        int Width { get; protected set; }
        int Height { get; protected set; }

        public Field(int width, int height)
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
