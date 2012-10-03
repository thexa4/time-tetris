using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public partial class Block
    {
        public static Dictionary<BlockType, bool[,]> BlockTypes = new Dictionary<BlockType,bool[,]>(){
            { BlockType.IBlock, new bool[,]{
                {false, false, false, false},
                {true,  true,  true,  true },
                {false, false, false, false},
                {false, false, false, false},
            } },

            { BlockType.JBlock, new bool[,]{
                {true,  false, false},
                {true,  true,  true },
                {false, false, false},
            } },

            { BlockType.LBlock, new bool[,]{
                {false, false, true },
                {true,  true,  true },
                {false, false, false},
            } },

            { BlockType.OBlock, new bool[,]{
                {true,  true},
                {true,  true},
            } },

            { BlockType.SBlock, new bool[,]{
                {false, true,  true },
                {true,  true,  false},
                {false, false, false},
            } },

            { BlockType.TBlock, new bool[,]{
                {false, true,  false},
                {true,  true,  true },
                {false, false, false},
            } },

            { BlockType.ZBlock, new bool[,]{
                {true,  true,  false},
                {false, true,  true },
                {false, false, false},
            } },
        };
    }
}
