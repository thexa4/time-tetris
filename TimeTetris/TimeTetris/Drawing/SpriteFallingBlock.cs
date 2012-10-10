using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    public class SpriteFallingBlock : Sprite
    {
        /// <summary>
        /// Data Falling Block
        /// </summary>
        public Data.FallingBlock Source { get; protected set; }

        /// <summary>
        /// Position + Offset
        /// </summary>
        protected Vector2 OffsetPosition { get; set; }

        /// <summary>
        /// Creates a new sprite of a falling block
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="source">Data</param>
        public SpriteFallingBlock(Game game, Data.FallingBlock source) : base(game) 
        {
            this.Source = source;
            this.TextureName = "Graphics/blank";
            this.Size = (SpriteField.GridSize - 1) * Vector2.One;
            this.Color = Data.Block.GetColor(source.Block.Type);
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

        /// <summary>
        /// Frame draw
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            var basePosition = this.Position;

            // Start drawing
            var width = this.Source.Block.Width;
            var height = this.Source.Block.Height;

            for (Int32 x = 0; x < width; x++)
                for (Int32 y = 0; y < height; y++) 
                {
                    this.Position = this.OffsetPosition + Vector2.UnitX * SpriteField.GridSize * x +
                        Vector2.UnitY * SpriteField.GridSize * (height - y - 1);
                    if (this.Source.Block[x, y])
                        base.Draw(gameTime);
                }

            this.Position = basePosition;
        }
        
    }
}
