using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class Points
    {
        public const Int32 SoftDrop = 1;
        public const Int32 HardDrop = 2;

        public const Int32 Single = 100;
        public const Int32 Double = 300;
        public const Int32 Triple = 500;
        public const Int32 Tetris = 800;

        public const Int32 Combo = 50;
        
        // TODO T-Spin
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Int32 ClearLines(Int32 lines, Int32 level) 
        {
            switch (lines)
            {
                case 1:
                    return Points.Single * level;
                case 2:
                    return Points.Double * level;
                case 3:
                    return Points.Triple * level;
                case 4:
                    return Points.Tetris * level;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static Int32 ClearCombo(Int32 level)
        {
            return Points.Combo * level;
        }
    }
}
