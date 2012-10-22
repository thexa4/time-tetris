using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Services
{
    public enum ControllerAction : int
    {
        None = 0,

        Left = 1,
        Right = 2,
        Down = 3,
        Drop = 4,
        RotateCW = 5,
        RotateCCW = 6,
        Time = 7,
        Hold = 8,
    }
}
