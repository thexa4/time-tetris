using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public partial class Block : ICloneable
    {
        /// <summary>
        /// The block description
        /// </summary>
        public Boolean[,] Values { get; set; }

        /// <summary>
        /// The current rotation
        /// </summary>
        public Int32 Rotation
        {
            get { return _rotation; }

            // To compensate for negative numbers:
            set { _rotation = ((value % 4) + 4) % 4; }
        }
        protected Int32 _rotation = 0;

        /// <summary>
        /// Block Width
        /// </summary>
        public Int32 Width { get { return Values.GetLength(1); } }

        /// <summary>
        /// Block Height
        /// </summary>
        public Int32 Height { get { return Values.GetLength(0); } }

        /// <summary>
        /// Block Type
        /// </summary>
        public BlockType Type { get; set; }

        /// <summary>
        /// Event that runs when type is changed
        /// </summary>
        public event BlockTypeDelegate OnTypeChanged = delegate { };

        /// <summary>
        /// Creates a new block with a certain type
        /// </summary>
        /// <param name="type">The type to copy</param>
        public Block(BlockType type)
        {
            SetBlockType(type);
        }

        /// <summary>
        /// Sets the block to a certain type and copy its shape
        /// </summary>
        /// <param name="type">The type to set</param>
        public void SetBlockType(BlockType type)
        {
            Type = type;
            Values = (Boolean[,]) BlockTypes[type].Clone();
            OnTypeChanged.Invoke(type);
        }

        /// <summary>
        /// Sets the block to a certain type
        /// </summary>
        /// <param name="type">The type to set it to</param>
        public void SetBlockType(Int32 type)
        {
            SetBlockType((BlockType)type);
        }   

        /// <summary>
        /// Gets the rotated version of the block
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <returns>Wether there is a block on the current rotated position</returns>
        public Boolean this[int x, int y]
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
                return Values[y, x];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Object Clone()
        {
            return new Block(this.Type) { Rotation = this.Rotation };
        }
    }
}