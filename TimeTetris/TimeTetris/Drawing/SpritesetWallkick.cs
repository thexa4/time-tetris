using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeTetris.Data;
using TimeTetris.Data.Wallkick;

namespace TimeTetris.Drawing
{
    public class SpritesetWallkick
    {
        /// <summary>
        /// 
        /// </summary>
        public Game Game { get; set; }
        public BlockType Type { get; protected set; }

        private List<SpriteBlock> _sprites;
        private Data.Field _field;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public SpritesetWallkick(Game game, BlockType type)
        {
            this.Game = game;
            this.Type = type;
        }

        public virtual void Initialize() {
            _sprites = new List<SpriteBlock>();

            var block = new Block(this.Type);
            _field = new Field(block.Width + 4, block.Height + 4);

            _sprites.Add(
                new SpriteFallingBlock(this.Game,
                    new Data.FallingBlock()
                    {
                        Block = (Data.Block)block.Clone(),
                        Field = _field,
                        X = 2,
                        Y = 2,
                    }
                )
            );

            for (Int32 i = 0; i < 4; i++)
            {
                var movements = WallkickData.LeftMovements[new Tuple<int, int>(block.Width, block.Rotation)];

                for (Int32 j = 0; j < movements.GetLength(0); j++ )
                    _sprites.Add(
                        new SpriteFallingBlock(this.Game,
                            new Data.FallingBlock()
                            {
                                Block = (Data.Block)block.Clone(),
                                Field = null,
                                X = 2 + movements[j, 0],
                                Y = 2 + movements[j, 1],
                            }
                        )
                    );

                block.Rotation++;
            }
        }
    }
}
