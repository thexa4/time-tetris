using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class Block
    {
        public bool[,] Values { get; set; }
        public int Rotation
        {
            get { return _rotation; }
            // To compensate for negative numbers:
            set { _rotation = ((value % 4) + 4) % 4; }
        }
        protected int _rotation = 0;

        /// <summary>
        /// Gets the rotated version of the block
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <returns>Wether there is a block on the current rotated position</returns>
        public bool this[int x, int y]
        {
            get
            {
                int tmp = x;
                switch (_rotation)
                {
                    case 1:
                        x = Values.GetLength(0) - 1 - y;
                        y = tmp;
                        break;
                    case 2:
                        x = Values.GetLength(0) - 1 - x;
                        y = Values.GetLength(1) - 1 - y;
                        break;
                    case 3:
                        x = y;
                        y = Values.GetLength(1) - 1 - tmp;
                        break;
                }
                return Values[x,y];
            }
        }

        public static Block BlockI0 = new Block
        {
            Values = new bool[4, 4]{ {false, false, false, false},
                                     {true, true, true, true},
                                     {false, false, false, false},
                                     {false, false, false, false} },
        };

        public static Block BlockI1 = new Block
        {
            Values = new bool[4, 4] { { false, false, true, false},
                {false, false, true, false},
                {false, false, true, false},
                {false, false, true, false}},
        };
    }
}