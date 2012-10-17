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
        public const Int32 Rewind = 100;
        public const Int32 RewindLevelTax = 5;

        // T-Spin system used = 3-corner-T
        // Wallkick/Rotation system: SRS

        public const Int32 TSpinNoLineKick = 100;
        public const Int32 TSpinNoLineNoKick = 0;
        public const Int32 TSpinSingleKick = 200;
        public const Int32 TSpinSingleNoKick = Points.Single;
        public const Int32 TSpinDoubleKick = 1200;
        public const Int32 TSpinDoubleNoKick = Points.Double;
        public const Int32 TSpinTripleKick = 1600;
        public const Int32 TSpinTripleNoKick = Points.Triple;
        
        /// <summary>
        /// Gets the number of points earned for clearing lines
        /// </summary>
        /// <param name="lines">Number of lines</param>
        /// <param name="level">Current level</param>
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
        /// Gets the number of points earned for making a combo
        /// </summary>
        /// <param name="level">Current level</param>
        /// <returns></returns>
        internal static Int32 ClearCombo(Int32 level)
        {
            return Points.Combo * level;
        }

        /// <summary>
        /// Determines T-Spin score
        /// </summary>
        /// <param name="rows">Number of rows cleared</param>
        /// <param name="level">Current level</param>
        /// <param name="kick">Was the block kicked</param>
        /// <returns>Points earned</returns>
        internal static Int32 TSpin(Int32 rows, Int32 level, Boolean kick)
        {
            if (kick)
                switch (rows)
                {
                    case 0:
                        return TSpinNoLineKick * level;
                    case 1:
                        return TSpinSingleKick * level;
                    case 2:
                        return TSpinDoubleKick * level;
                    case 3:
                        return TSpinTripleKick * level;
                }
            
            switch (rows)
            {
                case 0:
                    return TSpinNoLineNoKick * level;
                case 1:
                    return TSpinSingleNoKick * level;
                case 2:
                    return TSpinDoubleNoKick * level;
                case 3:
                    return TSpinTripleNoKick * level;
            }

            return 0;
        }
    }
}
