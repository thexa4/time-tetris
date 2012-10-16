using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Data
{
    public partial class Block
    {
        public static Dictionary<BlockType, bool[,]> BlockTypes = new Dictionary<BlockType, bool[,]>(){
            { BlockType.IBlock, new bool[,]{
                {false, false, false, false},
                {false, false, false, false},
                {true,  true,  true,  true },
                {false, false, false, false},
            } },

            { BlockType.JBlock, new bool[,]{
                {false, false, false},
                {true,  true,  true },
                {true,  false, false},
            } },

            { BlockType.LBlock, new bool[,]{
                {false, false, false},
                {true,  true,  true },
                {false, false, true },
            } },

            { BlockType.OBlock, new bool[,]{
                {true,  true},
                {true,  true},
            } },

            { BlockType.SBlock, new bool[,]{
                {false, false, false},
                {true,  true,  false},
                {false, true,  true },
            } },

            { BlockType.TBlock, new bool[,]{
                {false, false, false},
                {true,  true,  true },
                {false, true,  false},
            } },

            { BlockType.ZBlock, new bool[,]{
                {false, false, false},
                {false, true,  true },
                {true,  true,  false},
            } },
        };

        /// <summary>
        /// Gets the color by the blocktype
        /// </summary>
        /// <param name="blockType">Blocktype requested</param>
        /// <returns>Corresponding Color</returns>
        internal static Color GetColor(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.IBlock:
                    return Color.Cyan;
                case BlockType.JBlock:
                    return Color.Blue;
                case BlockType.LBlock:
                    return Color.Orange;
                case BlockType.OBlock:
                    return Color.Yellow;
                case BlockType.SBlock:
                    return Color.Green;
                case BlockType.TBlock:
                    return Color.Purple;
                case BlockType.ZBlock:
                    return Color.Red;
                default:
                    return Color.Transparent;
            }
        }

        /// <summary>
        /// Gets the color by blocktype in the grid
        /// </summary>
        /// <param name="gridValue">Value in the grid</param>
        /// <returns>Corresponding Color</returns>
        internal static Color GetColor(Int32 gridValue)
        {
            if (gridValue <= 0)
                return Color.Transparent;

            // Since 0 is used as empty color, we need to convert
            return GetColor(FromGridValue(gridValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockType"></param>
        /// <returns></returns>
        internal static Int32 ToGridValue(BlockType blockType)
        {
            return ((Int32)blockType) + 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridValue"></param>
        /// <returns></returns>
        internal static BlockType FromGridValue(Int32 gridValue)
        {
            return (BlockType)(gridValue - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockType"></param>
        /// <returns></returns>
        internal static Int32 GetStartRotation(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.IBlock:
                case BlockType.LBlock:
                
                    return 1;
                case BlockType.SBlock:
                    return 2;
                case BlockType.JBlock:
                    return 3;
                case BlockType.TBlock:
                case BlockType.ZBlock:
                case BlockType.OBlock:
                    return 0;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockType"></param>
        /// <returns></returns>
        internal static Int32 GetBaseXPosition(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.IBlock:
                    return -1;
                case BlockType.LBlock:
                    return 0;
                case BlockType.SBlock:
                    return 0;
                case BlockType.JBlock:
                    return 1;
                case BlockType.TBlock:
                    return 0;
                case BlockType.ZBlock:
                    return 0;
                case BlockType.OBlock:
                    return 0;
                default:
                    return 0;
            }
        }
    }
}
