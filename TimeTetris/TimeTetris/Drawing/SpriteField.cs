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
        public const Int32 GridCellSize = 24;

        /// <summary>
        /// Data Field
        /// </summary>
        public Data.Field Source { get; protected set; }

        /// <summary>
        /// Creates a new sprite of a data gield
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="source">Field data</param>
        public SpriteField(Game game, Data.Field source)
            : base(game) 
        {
            this.Source = source;
            this.TextureName = "Graphics/blank";
            this.Size = (SpriteField.GridCellSize - 1) * Vector2.One;
        }

        /// <summary>
        /// Draw Frame
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            var basePosition = this.Position;

            for (Int32 x = 0; x < this.Source.Width; x++)
                for (Int32 y = 0; y < this.Source.Height - HiddenRows; y++)
                {
                    this.Position = basePosition + (x * GridCellSize * Vector2.UnitX) + 
                        ((this.Source.Height - HiddenRows - 1 - y) * GridCellSize * Vector2.UnitY);

                    this.Color = Color.White * 0.1f;
                    this.Size += Vector2.One;
                    base.Draw(gameTime);

                    this.Color = Color.Transparent;
                    this.Size -= Vector2.One;
                    base.Draw(gameTime);

                    if (this.Source[x, y] == 0)
                        continue;

                    this.Color = Data.Block.GetColor(this.Source[x, y]);
                    base.Draw(gameTime);

                    //this.ScreenManager.SpriteBatch.DrawString(this.ScreenManager.SpriteFonts["Default"], String.Format("[{0}:{1}]", x, y), this.Position, Color.White);
                }

            this.Position = basePosition;
        }

    }
}
