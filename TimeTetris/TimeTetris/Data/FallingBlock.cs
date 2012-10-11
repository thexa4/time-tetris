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
        public int Y { get; set; }

        /// <summary>
        /// The Grid
        /// </summary>
        public Field Field { get; set; }

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
                
                return;
            }

            // Rotation failed
            Block.Rotation -= dir;
        }
    }
}
