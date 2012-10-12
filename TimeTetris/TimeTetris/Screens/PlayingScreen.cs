using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTetris.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TimeTetris.Drawing;
using TimeTetris.Data;

namespace TimeTetris.Screens
{
    public class PlayingScreen : GameScreen
    {
        private Data.Field _field;

        private SpriteField _spriteField;
        private SpriteFallingBlock _spriteFallingBlock;
        private SpriteBlock _spriteNextBlock;
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

            _timeline = (Timeline)this.Game.Services.GetService(typeof(Timeline));

            // Create Field
            _field = new Data.Field(this.Game, _timeline, 10, 32);
            _field.Initialize();

            // Create Sprites
            _spriteField = new SpriteField(this.Game, _field);
            _spriteField.Initialize();

            _spriteFallingBlock = new SpriteFallingBlock(this.Game, _field.CurrentBlock);
            _spriteFallingBlock.Initialize();

            _spriteNextBlock = new SpriteBlock(this.Game, _field.NextBlock) { Position = Vector2.UnitX * (_field.Width * SpriteField.GridSize + 20) };
            _spriteNextBlock.Initialize();

            // Start the level
            _timeline.Start();

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

            _spriteField.Update(gameTime);
            _spriteFallingBlock.Update(gameTime);
            _spriteNextBlock.Update(gameTime);
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
            _spriteNextBlock.Draw(gameTime);

            this.ScreenManager.SpriteBatch.DrawString(this.ScreenManager.SpriteFonts["Default"], String.Format("{0} s", Math.Round(_timeline.CurrentTime / 1000, 2)), Vector2.One * 5, Color.White);
            this.ScreenManager.SpriteBatch.End();
        }
    }
}
