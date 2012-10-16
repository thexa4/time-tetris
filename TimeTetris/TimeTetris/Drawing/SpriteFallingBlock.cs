using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    /// <summary>
    /// 
    /// <remarks>Not an override of SpriteBlock because draw logic is different and source is too</remarks>
    /// </summary>
    public class SpriteFallingBlock : Sprite
    {
        /// <summary>
        /// Data Falling Block
        /// </summary>
        public Data.FallingBlock Source { get; protected set; }

        /// <summary>
        /// Offset position by the source
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
            this.Size = (SpriteField.GridCellSize - 1) * Vector2.One;
            this.Color = Data.Block.GetColor(source.Block.Type);

            source.Block.OnTypeChanged += new Data.BlockTypeDelegate(source_Block_OnTypeChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        private void source_Block_OnTypeChanged(Data.BlockType b)
        {
            this.Color = Data.Block.GetColor(this.Source.Block.Type);
        }

        /// <summary>
        /// Frame renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.OffsetPosition = this.Position + Vector2.UnitX * this.Source.X * SpriteField.GridCellSize +
                ((this.Source.Field.Height - SpriteField.HiddenRows - 1 - this.Source.Y) * SpriteField.GridCellSize * Vector2.UnitY);
        }

        /// <summary>
        /// Frame draw (need override since some parts should not be drawn)
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
                    this.Position = this.OffsetPosition + Vector2.UnitX * SpriteField.GridCellSize * x +
                        Vector2.UnitY * SpriteField.GridCellSize * (height - 1 - y);

                    if (this.Source.Block[x, y] && IsInView(y))
                        base.Draw(gameTime);
                }

            this.Position = basePosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Boolean IsInView(Int32 y) {
            return (this.Source.Y - this.Source.Block.Height + y + 1) < this.Source.Field.Height - SpriteField.HiddenRows;
        }
        
    }
}
