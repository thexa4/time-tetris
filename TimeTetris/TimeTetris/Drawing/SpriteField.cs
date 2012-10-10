using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    public class SpriteField : Sprite
    {
        public const Int32 HiddenRows = 4;
        public const Int32 GridSize = 32;

        /// <summary>
        /// 
        /// </summary>
        public Data.Field Source { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="source"></param>
        public SpriteField(Game game, Data.Field source)
            : base(game) 
        {
            this.Source = source;
            this.TextureName = "Graphics/blank";
            this.Size = SpriteField.GridSize * Vector2.One;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            var basePosition = this.Position;

            for (Int32 x = 0; x < this.Source.Width; x++)
                for (Int32 y = 0; y < this.Source.Height - HiddenRows; y++)
                {
                    this.Position = basePosition + (x * GridSize * Vector2.UnitX) + 
                        ((this.Source.Height - HiddenRows - 1 - y) * GridSize * Vector2.UnitY);
                    if (this.Source[x, y] != 0)
                        base.Draw(gameTime);

#if DEBUG
                    this.ScreenManager.SpriteBatch.DrawString(
                        this.ScreenManager.SpriteFonts["Default"], 
                        String.Format("[{0},{1}]", x, y), this.Position, Color.White);
#endif
                }

            this.Position = basePosition;
        }

    }
}
