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
        /// 
        /// </summary>
        public Data.FallingBlock Source { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        protected Vector2 OffsetPosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="source"></param>
        public SpriteFallingBlock(Game game, Data.FallingBlock source) : base(game) 
        {
            this.Source = source;
            this.TextureName = "Graphics/blank";
            this.Size = SpriteField.GridSize * Vector2.One;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.OffsetPosition = this.Position + Vector2.UnitX * this.Source.X * SpriteField.GridSize +
                ((this.Source.Field.Height - SpriteField.HiddenRows - this.Source.Y) * SpriteField.GridSize * Vector2.UnitY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            var basePosition = this.Position;

            // Start drawing
            var width = this.Source.Type.Values.GetLength(0);
            for (Int32 i = 0; i < this.Source.Type.Values.Length; i++)
            {
                this.Position = this.OffsetPosition + Vector2.UnitX * SpriteField.GridSize * (i % width) +
                        Vector2.UnitY * SpriteField.GridSize * (this.Source.Type.Values.Length / width - i / width);
                if (this.Source.Type.Values[i % width, i / width])
                    base.Draw(gameTime);
            }

            this.Position = basePosition;
        }
        
    }
}
