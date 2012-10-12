using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    public class SpriteNullableBlock : SpriteBlock
    {
        public SpriteNullableBlock(Game game)
            : base(game, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetBlock(Data.Block value) {
            if (_source != null)
                _source.OnTypeChanged -= _source_OnTypeChanged;

            _source = value;

            if (_source == null)
                return;

            _source.OnTypeChanged += new Data.BlockTypeDelegate(_source_OnTypeChanged);

            this.Color = Data.Block.GetColor(_source.Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (_source != null)
                base.Draw(gameTime);
        }

        
    }
}
