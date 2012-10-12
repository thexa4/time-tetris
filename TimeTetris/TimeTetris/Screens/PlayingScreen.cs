﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTetris.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TimeTetris.Drawing;
using TimeTetris.Data;
using Microsoft.Xna.Framework.Input;

namespace TimeTetris.Screens
{
    public class PlayingScreen : GameScreen
    {
        private Data.Field _field;


        private SpriteField _spriteField;
        private Sprite _spriteNextBlockBoundary;
        private SpriteFallingBlock _spriteFallingBlock;
        private SpriteBlock _spriteNextBlock;
        private KeyboardController _controller;
        // TODO spriteset level ?

        protected Timeline _timeline;

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);
            this.IsPopup = false;

            base.Initialize();

            // Get service
            _timeline = (Timeline)this.Game.Services.GetService(typeof(Timeline));

            // Create Field
            _field = new Data.Field(this.Game, _timeline, 10, 24);
            _field.Initialize();

            // Create Sprites
            _spriteField = new SpriteField(this.Game, _field) { Position = Vector2.One * SpriteField.GridCellSize * 5 };
            _spriteField.Initialize();

            _spriteFallingBlock = new SpriteFallingBlock(this.Game, _field.CurrentBlock) { Position = _spriteField.Position };
            _spriteFallingBlock.Initialize();

            // Next Block Boundary Background
            _spriteNextBlockBoundary = new Sprite(this.Game)
                {
                    TextureName = "Graphics/blank",
                    Size = Vector2.One * SpriteField.GridCellSize * _field.NextBlock.Width,
                    Position = _spriteField.Position + Vector2.UnitX * (_field.Width + 2) * SpriteField.GridCellSize,
                    Opacity = 0.2f,
                    Color = Color.White,
                };
            _spriteNextBlockBoundary.Initialize();

            // Next BLock
            _spriteNextBlock = new SpriteBlock(this.Game, _field.NextBlock) { Position = _spriteNextBlockBoundary.Position, };
            _spriteNextBlock.Initialize();
            _field.NextBlock.OnTypeChanged += new BlockTypeDelegate(NextBlock_OnTypeChanged);

            // Player controller
            _controller = new KeyboardController(this.Game, Keys.S, Keys.A, Keys.D, Keys.W, Keys.Q, Keys.E, Keys.Space);
            _controller.Initialize();

            // Start the level
            _timeline.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        private void NextBlock_OnTypeChanged(BlockType block)
        {
            _spriteNextBlockBoundary.Size = Vector2.One * SpriteField.GridCellSize * _field.NextBlock.Width;
        }

        /// <summary>
        /// Loads all content
        /// </summary>
        /// <param name="contentManager"></param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            _spriteField.LoadContent(contentManager);
            _spriteFallingBlock.LoadContent(contentManager);
            _spriteNextBlock.LoadContent(contentManager);
            _spriteNextBlockBoundary.LoadContent(contentManager);
        }

        /// <summary>
        /// Frame renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing values</param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            _field.Update(gameTime);
            _controller.Update(gameTime);

            _spriteField.Update(gameTime);
            _spriteFallingBlock.Update(gameTime);
            _spriteNextBlock.Update(gameTime);
            _spriteNextBlockBoundary.Update(gameTime);
        }

        /// <summary>
        /// Handle input
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            if (InputManager.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                _timeline.Stop();
            else if (InputManager.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
                _timeline.Resume();

            switch (_controller.Action)
            {
                case ControllerAction.Down:
                    _field.CurrentBlock.MoveDown();
                    break;
                case ControllerAction.Left:
                    _field.CurrentBlock.MoveLeft();
                    break;
                case ControllerAction.Right:
                    _field.CurrentBlock.MoveRight();
                    break;
                //case ControllerAction.Up:
                //    _field.CurrentBlock.DropDown();
                case ControllerAction.RotateCW:
                    _field.CurrentBlock.RotateLeft();
                    break;
                case ControllerAction.RotateCCW:
                    _field.CurrentBlock.RotateRight();
                    break;
                case ControllerAction.Time:
                    break;
            }
        }

        /// <summary>
        /// Draw frame
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.ScreenManager.SpriteBatch.Begin();

            _spriteField.Draw(gameTime);
            _spriteFallingBlock.Draw(gameTime);
            _spriteNextBlockBoundary.Draw(gameTime);
            _spriteNextBlock.Draw(gameTime);

            this.ScreenManager.SpriteBatch.DrawString(this.ScreenManager.SpriteFonts["Default"], String.Format("{0} s", Math.Round(_timeline.CurrentTime, 2)), Vector2.One * 5, Color.White);
            this.ScreenManager.SpriteBatch.End();
        }
    }
}
