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
        public Int32 Y { get; set; }

        /// <summary>
        /// The Grid
        /// </summary>
        public Field Field { get; set; }

        /// <summary>
        /// Timeline time last move succeeded
        /// </summary>
        public double LastMoveTime { get; set; }

        /// <summary>
        /// Timeline time last move down succeeded
        /// </summary>
        public Double LastMoveDownTime { get; set; }

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
        public Boolean MoveDown()
        {
            var res = Move(0, -1);

            if (res)
                this.LastMoveDownTime = this.LastMoveTime;
            return res;
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
            e.Undo = () =>
            {
                this.Field.CurrentBlock.X = prevx;
                this.Field.CurrentBlock.Y = prevy;
            };

            Int32 newx = X + yoff;
            Int32 newy = Y + yoff;
            e.Apply = () =>
            {
                this.Field.CurrentBlock.X = newx;
                this.Field.CurrentBlock.Y = newy;
            };

            this.LastMoveTime = this.Field.Timeline.Add(e);
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
            Int32 rot = this.Block.Rotation;
            this.Block.Rotation += dir;

            Int32[,] wallkicks = (dir > 0) ? Wallkick.WallkickData.RightMovements[new Tuple<int, int>(this.Block.Width, rot)]
                    : Wallkick.WallkickData.LeftMovements[new Tuple<int, int>(this.Block.Width, rot)];

            // Tries to wall kick until block doesn't collide
            for(Int32 i = 0; i < wallkicks.GetLength(0); i++)
            {
                if (this.Field.Collides(Block, X + wallkicks[i, 0], Y + wallkicks[i, 1]))
                    continue;
                
                Event e = new Event();
                Int32 prevx = X;
                Int32 prevy = Y;
                e.Undo = () =>
                {
                    this.Field.CurrentBlock.X = prevx;
                    this.Field.CurrentBlock.Y = prevy;
                    this.Field.CurrentBlock.Block.Rotation = rot;
                };

                Int32 newx = X + wallkicks[i, 0];
                Int32 newy = Y + wallkicks[i, 1];
                Int32 newrot = rot + dir;
                e.Apply = () =>
                {
                    this.Field.CurrentBlock.X = newx;
                    this.Field.CurrentBlock.Y = newy;
                    this.Field.CurrentBlock.Block.Rotation = newrot;
                };

                this.LastMoveTime = this.Field.Timeline.Add(e);
                
                return true;
            }

            // Rotation failed
            this.Block.Rotation -= dir;
            return false;
        }
    }
}
