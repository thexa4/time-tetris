using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Data
{
    public class DummyField : Field
    {
        public DummyField(Game game, Int32 width, Int32 height)
            : base(game, null, width, height)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }
    }
}
