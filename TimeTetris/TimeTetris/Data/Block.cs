using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public partial class Block
    {
        /// <summary>
        /// The block description
        /// </summary>
        public bool[,] Values { get; set; }

        /// <summary>
        /// The current rotation
        /// </summary>
        public int Rotation
        {
            get { return _rotation; }
            // To compensate for negative numbers:
            set { _rotation = ((value % 4) + 4) % 4; }
        }
        protected int _rotation = 0;

        public int Width { get { return Values.GetLength(1); } }
        public int Height { get { return Values.GetLength(0); } }

        /// <summary>
        /// Sets the block to a certain type
        /// </summary>
        /// <param name="type">The type to set it to</param>
        public void SetBlockType(BlockType type)
        {
            Values = BlockTypes[type];
        }

        /// <summary>
        /// Sets the block to a certain type
        /// </summary>
        /// <param name="type">The type to set it to</param>
        public void SetBlockType(int type)
        {
            Values = BlockTypes[(BlockType)type];
        }   

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
                        x = Height - 1 - y;
                        y = tmp;
                        break;
                    case 2:
                        x = Width - 1 - x;
                        y = Height - 1 - y;
                        break;
                    case 3:
                        x = y;
                        y = Width - 1 - tmp;
                        break;
                }
                return Values[x,y];
            }
        }
    }
}