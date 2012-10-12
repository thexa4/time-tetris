﻿using System;
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
        /// Setups the randomizer
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
            this.CurrentBlock.X = this.CurrentBlock.Field.Width / 2 - this.CurrentBlock.Block.Width / 2 - 1;
            this.CurrentBlock.Y = this.CurrentBlock.Field.Height - 1;
            this.CurrentBlock.Block.Rotation = Block.GetStartRotation(this.CurrentBlock.Block.Type);

            // Set next block
            this.NextBlock = new Block(_blockTypeQueue.Dequeue());
            this.NextBlock.Rotation = Block.GetStartRotation(this.NextBlock.Type);
        }

        /// <summary>
        /// Generates the next block
        /// </summary>
        protected void GenerateNextBlock() 
        {
            var oldType = this.CurrentBlock.Block.Type;
            var oldX = this.CurrentBlock.X;
            var oldY = this.CurrentBlock.Y;
            var oldR = this.CurrentBlock.Block.Rotation;

            var newType = this.NextBlock.Type;
            var newR = Block.GetStartRotation(newType);
            var nextType = _blockTypeQueue.Dequeue();

            this.Timeline.Add(new Event()
            {
                Apply = () =>
                    {
                        // Sets currentblock
                        this.CurrentBlock.Block.SetBlockType(newType);
                        this.CurrentBlock.X = this.CurrentBlock.Field.Width / 2 - this.CurrentBlock.Block.Width / 2 - 1;
                        this.CurrentBlock.Y = this.CurrentBlock.Field.Height - 1;

                        // Sets the next block
                        this.NextBlock.SetBlockType(nextType);
                        this.NextBlock.Rotation = Block.GetStartRotation(this.NextBlock.Type);
                        if (_blockTypeQueue.Count == 0)
                            GenerateNextPermutation();
                    },

                Undo = () =>
                    {
                        // Restore next type
                        var oldQueue = new Queue<BlockType>();
                        while (_blockTypeQueue.Count != 0)
                            oldQueue.Enqueue(_blockTypeQueue.Dequeue());
                        _blockTypeQueue.Enqueue(nextType);
                        while (oldQueue.Count != 0)
                            _blockTypeQueue.Enqueue(oldQueue.Dequeue());

                        // Retore new block
                        this.NextBlock.SetBlockType(newType);
                        this.NextBlock.Rotation = Block.GetStartRotation(this.NextBlock.Type);

                        // Restore current block
                        this.CurrentBlock.Block.SetBlockType(oldType);
                        this.CurrentBlock.X = oldX;
                        this.CurrentBlock.Y = oldY;
                        this.CurrentBlock.Block.Rotation = oldR;
                    },
            });

        }

        /// <summary>
        /// Generates a new permutation of choices
        /// </summary>
        protected void GenerateNextPermutation()
        {
            // Get all the options
            var options = ((BlockType[])System.Enum.GetValues(typeof(BlockType))).ToList();
            while (options.Count > 0)
            {
                // While there are options pick one
                var index = Field.Randomizer.Next(options.Count);
                var chosen = options[index];

                // And enqueue it
                _blockTypeQueue.Enqueue(chosen);
                options.Remove(chosen);
            }
        }
    }
}