using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    class FallingBlock
    {
        public Block Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Field Field { get; set; }
    }
}
