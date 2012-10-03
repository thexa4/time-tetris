using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public struct Block
    {
        public bool[,] Values;
        public Block RightRotation;
        public Block LeftRotation;

        public static Block BlockI0 = new Block
        {
            Values = new bool[4, 4]{ {false, false, false, false},
                                     {true, true, true, true},
                                     {false, false, false, false},
                                     {false, false, false, false} },
            LeftRotation = BlockI1
        };

        public static Block BlockI1 = new Block
        {
            Values = new bool[4, 4] { { false, false, true, false},
                {false, false, true, false},
                {false, false, true, false},
                {false, false, true, false}},
            LeftRotation = BlockI0,
        };
    }
}