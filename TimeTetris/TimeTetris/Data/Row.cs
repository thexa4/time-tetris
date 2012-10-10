using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class Row
    {
        public Int32 Width { get; protected set; }
        public Boolean[] Values { get; protected set; }

        public Row Next { get; set; }
        public Row Prev { get; set; }

        public Boolean IsFull { get { return Values.All(val => val); } }

        public Row(Int32 width)
        {
            Values = new Boolean[width];
            Width = width;
        }

        public bool this[int x]
        {
            get
            {
                if (x < 0 || x >= Width)
                    return true;
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
