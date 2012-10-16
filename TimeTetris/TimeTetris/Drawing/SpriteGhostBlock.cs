using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    public class SpriteGhostBlock : SpriteFallingBlock 
    {
        private Int32 _oldSourceX, _oldSourceR, _displayY;

        /// <summary>
        /// Creates a new sprite of a falling block
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="source">Data</param>
        public SpriteGhostBlock(Game game, Data.FallingBlock source)
            : base(game, source)
        {
            this.Opacity = 0.2f;
            this.Source.Block.OnTypeChanged += new Data.BlockTypeDelegate(Block_OnTypeChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        void Block_OnTypeChanged(Data.BlockType b)
        {
            _displayY = this.Source.Field.Height + 1;
        }

        /// <summary>
        /// Frame renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {

            if (_oldSourceX != this.Source.X || _oldSourceR != this.Source.Block.Rotation || _displayY > this.Source.Y)
            {
                _oldSourceX = this.Source.X;
                _oldSourceR = this.Source.Block.Rotation;
                _displayY = this.Source.Y;
                while (!this.Source.Field.Collides(this.Source.Block, this.Source.X, _displayY)) { _displayY--; }
            }

            this.OffsetPosition = this.Position + Vector2.UnitX * this.Source.X * SpriteField.GridCellSize +
                ((this.Source.Field.Height - SpriteField.HiddenRows - 1 - (_displayY + 1)) * SpriteField.GridCellSize * Vector2.UnitY);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected override bool IsInView(Int32 y)
        {
            return (this.Source.Y - this.Source.Block.Height + y + 1) < this.Source.Field.Height;
        }
    }
}
