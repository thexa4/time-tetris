using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class Timeline
    {
        public List<Event> Events { get; protected set; }
        public double CurrentTime { get; protected set; }
    }
}
