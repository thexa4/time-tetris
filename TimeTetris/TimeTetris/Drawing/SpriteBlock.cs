using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    public class SpriteBlock : Sprite
    {
        protected Data.Block _source; 

        /// <summary>
        /// Creates a new sprite of a falling block
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="source">Data</param>
        public SpriteBlock(Game game, Data.Block source) : base(game) 
        {
            _source = source;
            this.TextureName = "Graphics/blank";
            this.Size = (SpriteField.GridCellSize - 1) * Vector2.One;

            if (_source != null)
            {
                this.Color = Data.Block.GetColor(source.Type);
                _source.OnTypeChanged += new Data.BlockTypeDelegate(_source_OnTypeChanged);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        protected void _source_OnTypeChanged(Data.BlockType b)
        {
            this.Color = Data.Block.GetColor(b);
        }

        /// <summary>
        /// Frame draw
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            var basePosition = this.Position;

            // Start drawing
            var width = _source.Width;
            var height = _source.Height;

            for (Int32 x = 0; x < width; x++)
                for (Int32 y = 0; y < height; y++) 
                {
                    this.Position = basePosition + Vector2.UnitX * SpriteField.GridCellSize * x +
                        Vector2.UnitY * SpriteField.GridCellSize * (height - 1 - y);
                    if (_source[x, y])
                        base.Draw(gameTime);
                }

            this.Position = basePosition;
        }

    }
}
