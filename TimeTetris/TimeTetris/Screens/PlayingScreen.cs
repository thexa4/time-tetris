using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTetris.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TimeTetris.Drawing;

namespace TimeTetris.Screens
{
    public class PlayingScreen : GameScreen
    {
        private Data.Field _field;
        // TODO Data level ?

        private SpriteField _spriteField;
        // TODO spriteset level ?

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);
            this.IsPopup = false;

            base.Initialize();

            _field = new Data.Field(10, 32);

            _spriteField = new SpriteField(this.Game, _field);
            _spriteField.Initialize();
        }

        /// <summary>
        /// Loads all content
        /// </summary>
        /// <param name="contentManager"></param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            _spriteField.LoadContent(contentManager);
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
            _spriteField.Update(gameTime);
        }

        /// <summary>
        /// Handle input
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
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
            this.ScreenManager.SpriteBatch.End();
        }
    }
}
