using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class Row
    {
        public Int32 Width { get; protected set; }
        public Int32[] Values { get; set; }

        public Row Next { get; set; }
        public Row Prev { get; set; }

        public Boolean IsFull { get { return Values.All(val => val != 0); } }

        public Row(Int32 width)
        {
            Values = new Int32[width];
            Width = width;
        }

        public Int32 this[int x]
        {
            get
            {
                if (x < 0 || x >= Width)
                    return -1;
                return Values[x];
            }
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
