using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeTetris.Data;
using TimeTetris.Data.Wallkick;
using Microsoft.Xna.Framework.Content;

namespace TimeTetris.Drawing
{
    public class SpritesetWallkick : DrawableGameComponent
    {
        protected List<Sprite> _spritesLeft, _spritesRight;
        protected Data.Field _field;
        protected Int32 _rotation = 0;

        /// <summary>
        /// Is currently drawing right rotations
        /// </summary>
        public Boolean IsDrawingRight { get; set; }

        /// <summary>
        /// Block Type of the spriteset
        /// </summary>
        public BlockType Type { get; protected set; }

        /// <summary>
        /// ContentManager Reference
        /// </summary>
        public ContentManager ContentManager { get; set; }

        /// <summary>
        /// Current Position
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The current rotation
        /// </summary>
        public Int32 Rotation
        {
            get { return _rotation; }

            // To compensate for negative numbers:
            set { _rotation = ((value % 4) + 4) % 4; }
        }

        /// <summary>
        /// Creates the spriteset for a wallkick of a blocktyp
        /// </summary>
        /// <param name="game">Game reference</param>
        /// <param name="type">Block Type</param>
        public SpritesetWallkick(Game game, BlockType type) : base(game)
        {
            this.Type = type;
        }

        /// <summary>
        /// Initializes spriteset
        /// </summary>
        public override void Initialize()
        {
            _spritesLeft = new List<Sprite>();
            _spritesRight = new List<Sprite>();

            var block = new Block(this.Type);
            _field = new DummyField(this.Game, block.Width + 4, block.Height + 4 + SpriteField.HiddenRows);

            // Add for each rotation left
            for (Int32 i = 0; i < 4; i++)
            {
                // Get kick values and make sprites
                _spritesLeft.AddRange(GetMovementSprites(block, WallkickData.LeftMovements[new Tuple<int, int>(block.Width, block.Rotation)], block.Rotation + 1, i));
                block.Rotation--;
            }

            // Add for each rotation right
            for (Int32 i = 0; i < 4; i++)
            {
                // Get kick values and make sprites
                _spritesRight.AddRange(GetMovementSprites(block, WallkickData.RightMovements[new Tuple<int, int>(block.Width, block.Rotation)], block.Rotation - 1, i));
                block.Rotation++;
            }

            foreach (var sprite in _spritesLeft)
                sprite.Initialize();
            foreach (var sprite in _spritesRight)
                sprite.Initialize();
        }

        /// <summary>
        /// Gets a serie of sprites according to movement offsets
        /// </summary>
        /// <param name="block">base block</param>
        /// <param name="movements">wall kicks</param>
        /// <param name="baseRotation">original rotation</param>
        /// <param name="iteration">display iteration</param>
        protected virtual List<Sprite> GetMovementSprites(Data.Block block, Int32[,] movements, Int32 baseRotation, Int32 iteration)
        {
            var sprites = new List<Sprite>();

            for (Int32 j = 0; j < movements.GetLength(0); j++)
            {
                // Grid
                sprites.Add(
                    new SpriteField(this.Game, _field)
                    {
                        Position = this.Position + SpriteField.GridCellSize * ((_field.Width + 1) * j) * Vector2.UnitX +
                            SpriteField.GridCellSize * ((_field.Height - SpriteField.HiddenRows + 1) * iteration) * Vector2.UnitY,
                    }
                );

                // Block boundary
                sprites.Add(
                    new Sprite(this.Game)
                    {
                        TextureName = "Graphics/blank",
                        Size = Vector2.One * SpriteField.GridCellSize * block.Width,
                        Position = this.Position + SpriteField.GridCellSize * (2 + (_field.Height - SpriteField.HiddenRows + 1) * iteration) * Vector2.UnitY +
                            SpriteField.GridCellSize * (2 + (_field.Width + 1) * j) * Vector2.UnitX,
                        Opacity = 0.1f,
                        Color = Color.White,
                    }
                );

                // Add base sprite
                var baseBlock = (Data.Block)block.Clone();
                baseBlock.Rotation = baseRotation;

                sprites.Add(
                    new SpriteFallingBlock(this.Game,
                        new Data.FallingBlock(this.Game, baseBlock, _field, 2, block.Height + 1)
                    )
                    {
                        Position = this.Position + SpriteField.GridCellSize * ((_field.Height - SpriteField.HiddenRows + 1) * iteration) * Vector2.UnitY +
                            SpriteField.GridCellSize * ((_field.Width + 1) * j) * Vector2.UnitX,
                        Opacity = 0.2f,
                    }
                );

                // Add kicked sprite
                sprites.Add(
                    new SpriteFallingBlock(this.Game,
                        new Data.FallingBlock(this.Game, (Data.Block)block.Clone(), _field,  2 + movements[j, 0], block.Height + 1 + movements[j, 1])
                    )
                    {
                        Position = this.Position + SpriteField.GridCellSize * ((_field.Height - SpriteField.HiddenRows + 1) * iteration) * Vector2.UnitY +
                            SpriteField.GridCellSize * ((_field.Width + 1) * j) * Vector2.UnitX
                    }
                );
            }

            return sprites;
        }

        /// <summary>
        /// Loads all content
        /// </summary>
        /// <param name="manager"></param>
        public virtual void LoadContent(ContentManager manager)
        {
            base.LoadContent();

            this.ContentManager = manager;
            foreach (var sprite in _spritesLeft)
                sprite.LoadContent(this.ContentManager);
            foreach (var sprite in _spritesRight)
                sprite.LoadContent(this.ContentManager);
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var sprite in _spritesLeft)
                sprite.Update(gameTime);
            foreach (var sprite in _spritesRight)
                sprite.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!IsDrawingRight)
                foreach (var sprite in _spritesLeft)
                    sprite.Draw(gameTime);
            else
                foreach (var sprite in _spritesRight)
                    sprite.Draw(gameTime);
        }
    }
}
