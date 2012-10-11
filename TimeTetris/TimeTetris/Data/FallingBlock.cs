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
        /// The color id of the block
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// X Position in the Grid
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Y Position in the Grid
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The Grid
        /// </summary>
        public Field Field { get; set; }

        public double LastMoveTime { get; set; }
        public double LastMoveDownTime { get; set; }

        public void MoveLeft()
        {
            Move(-1, 0);
        }

        public void MoveRight()
        {
            Move(1, 0);
        }

        public void MoveDown()
        {
            Move(0, -1);
            LastMoveDownTime = Field.Timeline.CurrentTime;
        }

        protected bool Move(int xoff, int yoff)
        {
            if(Field.Collides(Block, X + xoff, Y + yoff))
                return false;

            int prevx = X;
            int prevy = Y;
            int newx = X + yoff;
            int newy = Y + yoff;

            Event e = new Event();
            e.Undo = () =>
            {
                Field.CurrentBlock.X = prevx;
                Field.CurrentBlock.Y = prevy;
            };
            e.Apply = () =>
            {
                Field.CurrentBlock.X = newx;
                Field.CurrentBlock.Y = newy;
            };
            e.Time = Field.Timeline.CurrentTime;
            e.Apply();

            LastMoveTime = Field.Timeline.CurrentTime;
            return true;
        }

        /// <summary>
        /// Rotates block right
        /// </summary>
        public void RotateRight()
        {
            Rotate(1);
        }

        /// <summary>
        /// Rotates block left
        /// </summary>
        public void RotateLeft()
        {
            Rotate(-1);
        }

        protected void Rotate(int dir)
        {
            int rot = Block.Rotation;
            Block.Rotation += dir;
            int[,] wallkicks;
            if(dir > 0)
                wallkicks = Wallkick.WallkickData.RightMovements[new Tuple<int, int>(Block.Width, rot)];
            else
                wallkicks = Wallkick.WallkickData.LeftMovements[new Tuple<int, int>(Block.Width, rot)];

            for(int i = 0; i < wallkicks.GetLength(0); i++)
            {
                if (Field.Collides(Block, X + wallkicks[i,0], Y + wallkicks[i,1]))
                    continue;
                
                int prevx = X;
                int prevy = Y;
                Event e = new Event();
                e.Undo = () =>
                {
                    Field.CurrentBlock.X = prevx;
                    Field.CurrentBlock.Y = prevy;
                    Field.CurrentBlock.Block.Rotation = rot;
                };
                int newx = X + wallkicks[i, 0];
                int newy = Y + wallkicks[i, 1];
                int newrot = rot + dir;
                e.Apply = () =>
                {
                    Field.CurrentBlock.X = newx;
                    Field.CurrentBlock.Y = newy;
                    Field.CurrentBlock.Block.Rotation = newrot;
                };
                e.Apply();
                e.Time = Field.Timeline.CurrentTime;
                Field.Timeline.Events.Add(e);
                LastMoveTime = Field.Timeline.CurrentTime;
                
                return;
            }

            // Rotation failed
            Block.Rotation -= dir;
        }
    }
}
