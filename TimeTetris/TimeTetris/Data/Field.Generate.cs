﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeTetris.Extension;

namespace TimeTetris.Data
{
    public partial class Field : GameComponent
    {
        protected static Random Randomizer = new Random();
        protected List<BlockType> _blockTypeQueue; // Queue is one way

        /// <summary>
        /// Setups the randomizer
        /// </summary>
        protected void SetupBlockGenerator(Boolean level_reset = false) 
        {
            // Create empty Queue
            if (level_reset) 
                _blockTypeQueue.Clear();
            else
                _blockTypeQueue = new List<BlockType>();

            GenerateNextPermutation();

            // First block of first bag is always I J L or T
            var firstAllowed = new BlockType[] { BlockType.IBlock, BlockType.JBlock, BlockType.LBlock, BlockType.TBlock };
            while (!firstAllowed.Contains(_blockTypeQueue.First()))
                _blockTypeQueue.Push(_blockTypeQueue.UnShift<BlockType>());

            // Generate as a block
            if (level_reset)
                this.CurrentBlock.Block.SetBlockType(_blockTypeQueue.UnShift<BlockType>());
            else
                this.CurrentBlock = new FallingBlock(this.Game, new Block(_blockTypeQueue.UnShift<BlockType>()), this);

            this.CurrentBlock.Replace(
                (this.Width - this.CurrentBlock.Block.Width) / 2 + Block.GetBaseXPosition(this.CurrentBlock.Block.Type),
                this.CurrentBlock.Field.Height - 1,
                Block.GetStartRotation(this.CurrentBlock.Block.Type)
                );

            // Set next block
            if (level_reset)
                this.NextBlock.SetBlockType(_blockTypeQueue.UnShift<BlockType>());
            else
                this.NextBlock = new Block(_blockTypeQueue.UnShift<BlockType>());

            this.NextBlock.Rotation = Block.GetStartRotation(this.NextBlock.Type);
        }

        /// <summary>
        /// Resets block generator
        /// </summary>
        public void ResetBlockGenerator()
        {
            SetupBlockGenerator(true);
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
            var oldPoints = this.CurrentBlock.BlockPoints;

            var newType = this.NextBlock.Type;
            var newR = Block.GetStartRotation(newType);
            var nextType = _blockTypeQueue.UnShift<BlockType>();
            var newY = this.CurrentBlock.Field.Height - 1;
            var newX = (this.Width - this.NextBlock.Width) / 2 + Block.GetBaseXPosition(newType);

            this.Timeline.Add(new Event()
            {
                Apply = () =>
                    {
                        // Sets currentblock
                        this.CurrentBlock.Block.SetBlockType(newType);
                        this.CurrentBlock.Replace(newX, newY, newR);

                        // Sets the next block
                        this.NextBlock.SetBlockType(nextType);
                        this.NextBlock.Rotation = Block.GetStartRotation(nextType);
                        if (_blockTypeQueue.Count == 0)
                            GenerateNextPermutation();
                    },

                Undo = () =>
                    {
                        // Restore next type
                        _blockTypeQueue.Shift(nextType);

                        // Retore new block
                        this.NextBlock.SetBlockType(newType);
                        this.NextBlock.Rotation = Block.GetStartRotation(newType);

                        // Restore current block
                        this.CurrentBlock.Block.SetBlockType(oldType);
                        this.CurrentBlock.Replace(oldX, oldY, oldR);
                        this.CurrentBlock.BlockPoints = oldPoints;
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
                _blockTypeQueue.Push(chosen);
                options.Remove(chosen);
            }
        }
    }
}
