using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    class Row
    {
        public int Width { get; protected set; }
        public bool[] Values { get; protected set; }

        public Row Next { get; set; }
        public Row Prev { get; set; }

        public bool IsFull { get { return Values.All(val => val); } }

        public Row(int width)
        {
            Values = new bool[width];
            Width = width;
        }

        public void Remove()
        {
            Prev.Next = Next;
            Next.Prev = Prev;
        }

        public void InsertAfter(Row r)
        {
            Next.Prev = r;
            r.Prev = this;
            r.Next = Next;
            Next = r;
        }
    }
}
