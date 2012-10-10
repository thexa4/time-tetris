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
        /// 
        /// </summary>
        /// <param name="blockType"></param>
        /// <returns></returns>
        internal static Color GetColor(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.IBlock:
                    return Color.Red;
                case BlockType.JBlock:
                    return Color.Azure;
                case BlockType.LBlock:
                    return Color.Beige;
                case BlockType.OBlock:
                    return Color.Brown;
                case BlockType.SBlock:
                    return Color.DarkGoldenrod;
                case BlockType.TBlock:
                    return Color.DarkKhaki;
                case BlockType.ZBlock:
                    return Color.Gainsboro;
                default:
                    return Color.White * 0.2f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockTypeValue"></param>
        /// <returns></returns>
        internal static Color GetColor(Int32 blockTypeValue)
        {
            if (blockTypeValue <= 0)
                return Color.Transparent;

            // Since 0 is used as empty color, we need to convert
            return GetColor((BlockType)(blockTypeValue - 1));
        }
    }
}
