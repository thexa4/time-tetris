using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Services
{
    public enum ControllerAction
    {
        None = 0,

        Left = 1,
        Up = 2,
        Right = 3,
        Down = 4,
        Drop = 5,
        RotateCW = 6,
        RotateCCW = 7,
        Time = 8,
    }
}
