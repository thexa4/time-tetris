using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data.Wallkick
{
    public class WallkickData
    {
        public static Dictionary<Tuple<int, int>, int[,]> RightMovements = new Dictionary<Tuple<int, int>, int[,]>()
        {
            {new Tuple<int, int>(4, 0), new int[,]{
                {0,0},
                {-2,0},
                {1,0},
                {-2,1},
                {1,-2},
            }},
            {new Tuple<int, int>(4, 1), new int[,]{
                {0,0},
                {-2,0},
                {1,0},
                {1,2},
                {-2,-1},
            }},
            {new Tuple<int, int>(4, 2), new int[,]{
                {0,0},
                {-1,0},
                {2,0},
                {-1,2},
                {2,-1},
            }},
            {new Tuple<int, int>(4, 3), new int[,]{
                {0,0},
                {2,0},
                {-1,0},
                {2,1},
                {-1,-1},
            }},

            {new Tuple<int, int>(3, 0), new int[,]{
                {0,0},
                {-1,0},
                {-1,-1},
                {0,2},
                {-1,2},
            }},
            {new Tuple<int, int>(3, 1), new int[,]{
                {0,0},
                {-1,0},
                {-1,1},
                {0,-2},
                {-1,-2},
            }},
            {new Tuple<int, int>(3, 2), new int[,]{
                {0,0},
                {1,0},
                {1,-1},
                {0,2},
                {1,2},
            }},
            {new Tuple<int, int>(3, 3), new int[,]{
                {0,0},
                {1,0},
                {1,1},
                {0,-2},
                {1,-2},
            }},

            // No wallkick for O block
            {new Tuple<int, int>(2, 0), new int[,]{{0,0},}},
            {new Tuple<int, int>(2, 1), new int[,]{{0,0},}},
            {new Tuple<int, int>(2, 2), new int[,]{{0,0},}},
            {new Tuple<int, int>(2, 3), new int[,]{{0,0},}},
        };

        public static Dictionary<Tuple<int, int>, int[,]> LeftMovements = new Dictionary<Tuple<int, int>, int[,]>()
        {
            {new Tuple<int, int>(4, 0), new int[,]{
                {0,0},
                {2,0},
                {-1,0},
                {2,1},
                {-1,-2},
            }},
            {new Tuple<int, int>(4, 1), new int[,]{
                {0,0},
                {-2,0},
                {1,0},
                {-2,1},
                {1,-1},
            }},
            {new Tuple<int, int>(4, 2), new int[,]{
                {0,0},
                {1,0},
                {-2,0},
                {1,2},
                {-2,-1},
            }},
            {new Tuple<int, int>(4, 3), new int[,]{
                {0,0},
                {2,0},
                {-1,0},
                {-1,2},
                {2,-1},
            }},

            {new Tuple<int, int>(3, 0), new int[,]{
                {0,0},
                {1,0},
                {1,-1},
                {0,2},
                {1,2},
            }},
            {new Tuple<int, int>(3, 1), new int[,]{
                {0,0},
                {-1,0},
                {-1,1},
                {0,-2},
                {-1,-2},
            }},
            {new Tuple<int, int>(3, 2), new int[,]{
                {0,0},
                {-1,0},
                {-1,-1},
                {0,2},
                {-1,2},
            }},
            {new Tuple<int, int>(3, 3), new int[,]{
                {0,0},
                {1,0},
                {1,1},
                {0,-2},
                {1,-2},
            }},

            // No wallkick for O block
            {new Tuple<int, int>(2, 0), new int[,]{{0,0},}},
            {new Tuple<int, int>(2, 1), new int[,]{{0,0},}},
            {new Tuple<int, int>(2, 2), new int[,]{{0,0},}},
            {new Tuple<int, int>(2, 3), new int[,]{{0,0},}},
        };
    }
}
