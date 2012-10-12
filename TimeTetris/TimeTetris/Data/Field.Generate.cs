using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Data
{
    public partial class Field
    {
        protected static Random Randomizer = new Random();
        protected Queue<BlockType> _blockTypeQueue;

        /// <summary>
        /// 
        /// </summary>
        protected void SetupBlockGenerator() 
        {
            _blockTypeQueue = new Queue<BlockType>();
            GenerateNextPermutation();

            // First block of first bag is always I J L or T
            var firstAllowed = new BlockType[] { BlockType.IBlock, BlockType.JBlock, BlockType.LBlock, BlockType.TBlock };
            while (!firstAllowed.Contains(_blockTypeQueue.Peek()))
                _blockTypeQueue.Enqueue(_blockTypeQueue.Dequeue());

            // Generate as a block
            this.CurrentBlock = new FallingBlock() { Block = new Block(_blockTypeQueue.Dequeue()), Field = this };
            this.CurrentBlock.X = this.CurrentBlock.Field.Width / 2 - this.CurrentBlock.Block.Width / 2;
            this.CurrentBlock.Y = this.CurrentBlock.Field.Height - 1;

            // Set next block
            this.NextBlock = new Block(_blockTypeQueue.Dequeue());
        }

        /// <summary>
        /// 
        /// </summary>
        protected void GenerateNextBlock() 
        {
            // Sets currentblock
            this.CurrentBlock.Block.SetBlockType(this.NextBlock.Type);
            this.CurrentBlock.X = this.CurrentBlock.Field.Width / 2 - this.CurrentBlock.Block.Width / 2;
            this.CurrentBlock.Y = this.CurrentBlock.Field.Height - 1;

            // Sets the next block
            this.NextBlock.SetBlockType(_blockTypeQueue.Dequeue());
            if (_blockTypeQueue.Count == 0)
                GenerateNextPermutation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected void GenerateNextPermutation()
        {
            var options = ((BlockType[])System.Enum.GetValues(typeof(BlockType))).ToList();
            while (options.Count > 0)
            {
                var index = Field.Randomizer.Next(options.Count);
                var chosen = options[index];
                _blockTypeQueue.Enqueue(chosen);
                options.Remove(chosen);
            }
        }
    }
}
