using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public class FallingBlock
    {
        /// <summary>
        /// Block blueprint
        /// </summary>
        public Block Block { get; set; }

        /// <summary>
        /// X Position in the Grid
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Y Position in the Grid
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The Grid
        /// </summary>
        public Field Field { get; set; }

        /// <summary>
        /// Rotates block right
        /// </summary>
        public void RotateRight()
        {
            Block.Rotation++;
        }
    }
}
