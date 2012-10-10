using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TimeTetris.Extension;
using TimeTetris.Services;

namespace TimeTetris.Screens
{
    /// <summary>
    /// 
    /// </summary>
    public class TitleScreen : GameScreen
    {
        private const String TitleString = "Time Tetris";
        private const String HelpString = "Press enter to start.";

        protected Vector2 _positionTitle, _positionHelp;
        protected Color _shadowColor;
        protected Single _sinusAlpha;
        protected SoundEffect _menuSound;
        protected SoundEffectInstance _menuSoundInstance;

        public Drawing.SpriteFallingBlock block;
        public Data.Field field;

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);
            this.IsPopup = false;
            base.Initialize();

            field = new Data.Field(8, 14);
            block = new Drawing.SpriteFallingBlock(this.Game, new Data.FallingBlock()
            {
                Field = field,
                Type = new Data.Block() { Values = Data.Block.BlockTypes[Data.BlockType.JBlock] }
            })
            {
                Position = Vector2.One * 100
            };
            block.Initialize();
        }

        /// <summary>
        /// Loads all content for this screen
        /// </summary>
        /// <param name="contentManager">ContentManager to load to</param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            this.ScreenManager.SpriteFonts.LoadFont("Title", "Fonts/Title");
            this.ScreenManager.SpriteFonts.LoadFont("Help", "Fonts/Default");
            this.AudioManager.Load("blip", "confirm", 1f, .5f);

            var titleMeasurement = this.ScreenManager.SpriteFonts["Title"].MeasureString(TitleString);
            var helpMeasurement = this.ScreenManager.SpriteFonts["Help"].MeasureString(HelpString);
            var height = titleMeasurement.Y + helpMeasurement.Y;

            _positionTitle = Vector2.UnitX * (Int32)Math.Round((1280 - titleMeasurement.X) / 2) +
               Vector2.UnitY * (Single)Math.Round((720f - height) / 2);
            _positionHelp = Vector2.UnitX * (Int32)Math.Round((1280f - helpMeasurement.X) / 2) +
                Vector2.UnitY * (Single)(Math.Round((720f - height) / 2) + Math.Round(titleMeasurement.Y));
            block.LoadContent(contentManager);
        }

        /// <summary>
        /// Update logic
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        /// <param name="otherScreenHasFocus">Game is blurred</param>
        /// <param name="coveredByOtherScreen">Other GameScreen is active</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            _shadowColor = Color.Black;
            _sinusAlpha = (Single)((Math.Sin((Single)gameTime.TotalGameTime.TotalSeconds * 4)) + 1) / 2;
            block.Update(gameTime);
        }

        /// <summary>
        /// Processes input
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            if (this.InputManager.Keyboard.IsKeyReleased(Keys.Enter))
            {
                this.AudioManager.Play("confirm");
                //this.Next = new MenuScreen();
                this.ExitScreenAnd();
            }
            else if (this.InputManager.Keyboard.IsKeyReleased(Keys.Escape))
            {
                this.AudioManager.Play("confirm");
                this.ExitScreenAnd();
            }

        }

        /// <summary>
        /// Draws frame
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            if (!this.IsTransitioning && this.ScreenState != Services.ScreenState.Active)
                return;

            base.Draw(gameTime);

            this.ScreenManager.SpriteBatch.Begin();
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Title"], TitleString, _positionTitle, Color.White, _shadowColor);
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Help"], HelpString, _positionHelp, Color.White * _sinusAlpha, _shadowColor * _sinusAlpha);
            block.Draw(gameTime);
            this.ScreenManager.SpriteBatch.End();

            this.ScreenManager.FadeBackBufferToBlack((Byte)(255 - this.TransitionAlpha));
        }
    }
}
