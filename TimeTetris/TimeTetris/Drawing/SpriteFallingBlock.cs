using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    public class SpriteFallingBlock : SpriteBlock
    {
        /// <summary>
        /// Data Falling Block
        /// </summary>
        public Data.FallingBlock Source { get; protected set; }

        /// <summary>
        /// Creates a new sprite of a falling block
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="source">Data</param>
        public SpriteFallingBlock(Game game, Data.FallingBlock source) : base(game, source.Block) 
        {
            this.Source = source;
        }

        /// <summary>
        /// Frame renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.OffsetPosition = this.Position + Vector2.UnitX * this.Source.X * SpriteField.GridSize +
                ((this.Source.Field.Height - SpriteField.HiddenRows - 1 - this.Source.Y) * SpriteField.GridSize * Vector2.UnitY);
        }
        
    }
}
