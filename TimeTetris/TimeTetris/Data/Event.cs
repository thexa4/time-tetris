using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    class Event
    {
        public double Time { get; set; }
        public Action Apply { get; set; }
        public Action Undo { get; set; }
    }
}
