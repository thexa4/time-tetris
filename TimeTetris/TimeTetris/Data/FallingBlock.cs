﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Data
{
    public class FallingBlock : GameComponent
    {
        public const Int32 MaxDownSpeed = 60;
        public const Single MaxLockTime = 1.5f;
        public const Single MinLockTime = 0.5f;

        /// <summary>
        /// Block blueprint
        /// </summary>
        public Block Block { get; set; }

        /// <summary>
        /// X Position in the Grid
        /// </summary>
        public Int32 X { get; protected set; }

        /// <summary>
        /// Y Position in the Grid
        /// </summary>
        public Int32 Y { get; protected set; }

        /// <summary>
        /// The Grid
        /// </summary>
        public Field Field { get; set; }

        /// <summary>
        /// Timeline time last move succeeded
        /// </summary>
        public Double LastMoveTime { get; protected set; }

        /// <summary>
        /// Timeline time last move down succeeded
        /// </summary>
        public Double LastMoveDownTime { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public Boolean LastMoveIsRotation { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public Boolean LastMoveIsKick { get; protected set; }

        /// <summary>
        /// Points accumulated by block
        /// </summary>
        public Int32 BlockPoints { get; set; }

        /// <summary>
        /// Creates a new Falling block
        /// </summary>
        /// <param name="game"></param>
        public FallingBlock(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Re-Places the block on new coords
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        public void Replace(Int32 x, Int32 y, Int32 r) 
        {
            this.X = x;
            this.Y = y;
            this.Block.Rotation = r;
            this.BlockPoints = 0;

            this.LastMoveTime = this.Field.Timeline.CurrentTime;
            this.LastMoveDownTime = this.Field.Timeline.CurrentTime;
        }

        /// <summary>
        /// Creates a new Falling Block with base block
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="block">Base block</param>
        public FallingBlock(Game game, Block block) 
            : this(game)
        {
            this.Block = block;
        }

        /// <summary>
        /// Creates a new Falling Block with base block and field
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="block">Base block</param>
        /// <param name="field">Field container</param>
        public FallingBlock(Game game, Block block, Field field)
            : this(game, block)
        {
            this.Field = field;
        }

        /// <summary>
        /// Creates a new Falling Block with base block, field and position
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="block">Base block</param>
        /// <param name="field">Field container</param>
        /// <param name="x">X Grid position</param>
        /// <param name="y">Y Grid position</param>
        public FallingBlock(Game game, Block block, Field field, Int32 x, Int32 y) 
            : this(game, block, field)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Moves block left
        /// </summary>
        /// <returns>Movement succes</returns>
        public Boolean MoveLeft()
        {
            return Move(-1, 0);
        }

        /// <summary>
        /// Moves block right
        /// </summary>
        /// <returns>Movement succes</returns>
        public Boolean MoveRight()
        {
            return Move(1, 0);
        }

        /// <summary>
        /// Moves block down
        /// </summary>
        /// <returns>Movement succes</returns>
        public Boolean MoveDown(Boolean isHardDrop = false)
        {
            var result = Move(0, -1);
            double prevtime = LastMoveDownTime;
            double newtime = Field.Timeline.CurrentTime;
            Field.Timeline.Add(new Event()
            {
                Apply = () =>
                {
                    LastMoveDownTime = newtime;
                    if (result)
                        this.BlockPoints += isHardDrop ? Points.HardDrop : Points.SoftDrop;
                },
                Undo = () =>
                {
                    LastMoveDownTime = prevtime;
                    if (result)
                        this.BlockPoints -= isHardDrop? Points.HardDrop : Points.SoftDrop;
                },
            });
            this.LastMoveDownTime = this.LastMoveTime;

            return result;
        }

        /// <summary>
        /// Drops a block down (effectivly 20 G)
        /// </summary>
        /// <returns></returns>
        public Boolean Drop()
        {
            while (MoveDown(true)) {  }
            this.Field.LockFalling();

            return true;
        }

        /// <summary>
        /// Moves block over an arbitrairy vector
        /// </summary>
        /// <param name="xoff">x movement</param>
        /// <param name="yoff">y movement</param>
        /// <returns>Movement succeeded</returns>
        protected Boolean Move(Int32 xoff, Int32 yoff)
        {
            if(this.Field.Collides(this.Block, this.X + xoff, this.Y + yoff))
                return false;

            Event e = new Event();
            Int32 prevx = X;
            Int32 prevy = Y;
            Double prevtime = this.LastMoveTime;
            Double newtime = this.Field.Timeline.CurrentTime;

            var wasRot = this.LastMoveIsRotation;
            var wasKick = this.LastMoveIsKick;

            e.Undo = () =>
            {
                this.Field.CurrentBlock.X = prevx;
                this.Field.CurrentBlock.Y = prevy;
                this.LastMoveTime = prevtime;

                this.LastMoveIsRotation = wasRot;
                this.LastMoveIsKick = wasKick;
            };

            Int32 newx = X + xoff;
            Int32 newy = Y + yoff;
            e.Apply = () =>
            {
                this.Field.CurrentBlock.X = newx;
                this.Field.CurrentBlock.Y = newy;
                this.LastMoveTime = newtime;

                this.LastMoveIsRotation = false;
                this.LastMoveIsKick = false;
            };

            this.Field.Timeline.Add(e);
            return true;
        }

        /// <summary>
        /// Rotates block right
        /// </summary>
        public Boolean RotateRight()
        {
            return Rotate(1);
        }

        /// <summary>
        /// Rotates block left
        /// </summary>
        public Boolean RotateLeft()
        {
            return Rotate(-1);
        }

        /// <summary>
        /// Rotates a block an arbitrary direction
        /// </summary>
        /// <param name="dir">Direction to rotate</param>
        /// <returns>Rotation succeeded</returns>
        protected Boolean Rotate(Int32 dir)
        {
            int rot = this.Block.Rotation;
            this.Block.Rotation += dir;

            Int32[,] wallkicks = (dir > 0) ? Wallkick.WallkickData.RightMovements[new Tuple<int, int>(this.Block.Width, this.Block.Rotation)]
                    : Wallkick.WallkickData.LeftMovements[new Tuple<int, int>(this.Block.Width, this.Block.Rotation)];

            // Tries to wall kick until block doesn't collide
            for(Int32 i = 0; i < wallkicks.GetLength(0); i++)
            {
                if (this.Field.Collides(Block, X + wallkicks[i, 0], Y + wallkicks[i, 1]))
                    continue;
                
                Event e = new Event();
                Int32 prevx = X;
                Int32 prevy = Y;
                Double prevtime = this.LastMoveTime;
                Double newtime = Field.Timeline.CurrentTime;
                
                var wasRot = this.LastMoveIsRotation;
                var wasKick = this.LastMoveIsKick;

                e.Undo = () =>
                {
                    this.X = prevx;
                    this.Y = prevy;
                    this.Block.Rotation = rot;
                    this.LastMoveTime = prevtime;

                    this.LastMoveIsRotation = wasRot;
                    this.LastMoveIsKick = wasKick;
                };

                Int32 newx = X + wallkicks[i, 0];
                Int32 newy = Y + wallkicks[i, 1];
                Int32 newrot = rot + dir;
                e.Apply = () =>
                {
                    this.Field.CurrentBlock.X = newx;
                    this.Field.CurrentBlock.Y = newy;
                    this.Field.CurrentBlock.Block.Rotation = newrot;
                    this.LastMoveTime = newtime;

                    this.LastMoveIsRotation = true;
                    this.LastMoveIsKick = i > 0;
                };

                this.Field.Timeline.Add(e);
                return true;
            }

            // Rotation failed
            this.Block.Rotation -= dir;
            return false;
        }        
        
        /// <summary>
        /// Checks if the block is a T and is immobile and has at least 3 of the 4 
        /// spaces diagonal of the center are occupied.
        /// </summary>
        /// <returns></returns>
        internal Boolean IsTAndImmobileThree
        {
            get
            {
                if (this.Block.Type != BlockType.TBlock)
                    return false;

                var centerX = this.X + 1;
                var centerY = this.Y - 1;

                var count = 0;
                if (this.Field[centerX + 1, centerY + 1] != 0)
                    count++;
                if (this.Field[centerX + 1, centerY - 1] != 0)
                    count++;
                if (this.Field[centerX - 1, centerY + 1] != 0)
                    count++;
                if (this.Field[centerX - 1, centerY - 1] != 0)
                    count++;

                return count >= 3;
            }
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            if (this.Field.HasEnded || this.Field.Timeline.IsRewindActive)
                return;

            var downElapsed = this.Field.Timeline.CurrentTime - LastMoveDownTime;
            if (downElapsed > 1.0 / Math.Min(FallingBlock.MaxDownSpeed, Field.Level)) // 20 G max
                MoveDown();

            var lockElapsed = this.Field.Timeline.CurrentTime - LastMoveTime;
            if (lockElapsed > Math.Max(FallingBlock.MinLockTime, FallingBlock.MaxLockTime / Field.Level)) 
                this.Field.LockFalling();
        }


    }
}
