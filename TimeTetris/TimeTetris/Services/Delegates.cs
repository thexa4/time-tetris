using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTetris.Data;

namespace TimeTetris.Services
{
    public delegate void BlockTypeDelegate(BlockType type);
    public delegate void RowsDelegate(Int32 rows, Int32 combo, Boolean tspin, Boolean b2b);
}
