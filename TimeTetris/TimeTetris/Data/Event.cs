using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class Event
    {
        public Double Time { get; set; }
        public Action Apply { get; set; }
        public Action Undo { get; set; }
    }
}
