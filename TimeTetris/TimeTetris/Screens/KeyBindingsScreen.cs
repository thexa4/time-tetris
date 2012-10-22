using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeTetris.Extension;
using TimeTetris.Services;
using System.Threading;
using System.Threading.Tasks;

namespace TimeTetris.Screens
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyBindingsScreen : GameScreen
    {
        private const String TitleString = "Time Tetris Key Bindings";

        private readonly String ExitOption = "Return";
        private readonly Dictionary<ControllerAction, String> Options = new Dictionary<ControllerAction, String> 
        { 
            { ControllerAction.Left, "Move Left" },
            { ControllerAction.Right, "Move Right" },
            { ControllerAction.Down, "Soft Drop" },
            { ControllerAction.Drop, "Hard Drop" },
            { ControllerAction.Hold, "Switch with Hold" },
            { ControllerAction.RotateCCW, "Rotate Left (CCW)" },
            { ControllerAction.RotateCW, "Rotate Right (CW)" },
            { ControllerAction.Time, "Reverse Time" },
            
        };

        protected Vector2 _positionTitle, _positionMenu;
        protected Int32 _menuIndex;

        protected Texture2D _texture;
        protected GameScreen _parent;
        protected KeyboardController _controller;
        protected Task<Keys> _bindKeyTask;
        protected CancellationTokenSource _bindKeyTaskCancel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public KeyBindingsScreen(GameScreen parent, KeyboardController controller)
            : base()
        {
            _parent = parent;
            _parent.Exiting += new EventHandler(_parent_Exiting);

            _controller = controller;

            _bindKeyTaskCancel = new CancellationTokenSource();
            _bindKeyTask = Task<Keys>.Factory.StartNew(() => { return Keys.None; }, _bindKeyTaskCancel.Token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _parent_Exiting(object sender, EventArgs e)
        {
            this.ExitScreen();
        }

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);

            _menuIndex = 0;

            this.IsPopup = true;
            this.IsCapturingInput = true;
            base.Initialize();
        }

        /// <summary>
        /// Loads all content for this screen
        /// </summary>
        /// <param name="contentManager">ContentManager to load to</param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            this.ScreenManager.SpriteFonts.LoadFont("Title", "Fonts/Title");
            this.ScreenManager.SpriteFonts.LoadFont("Menu", "Fonts/Default");
            _texture = this.ContentManager.Load<Texture2D>("Graphics/Blank");
            this.AudioManager.Load("blip", "confirm", 0.6f, .5f);
            this.AudioManager.Load("blip", "blip", 0.6f, .2f);

            var titleMeasurement = this.ScreenManager.SpriteFonts["Title"].MeasureString(TitleString);
            var menuMeasurement = Options.Values.Sum(a => this.ScreenManager.SpriteFonts["Menu"].MeasureString(a).Y + 15) - 15;
            menuMeasurement += this.ScreenManager.SpriteFonts["Menu"].MeasureString(ExitOption).Y;
            var height = titleMeasurement.Y + 10 + menuMeasurement;

            _positionTitle = Vector2.UnitX * (Int32)Math.Round((1280 - titleMeasurement.X) / 2) +
                Vector2.UnitY * (Single)Math.Round((720f - height) / 2);
            _positionMenu = Vector2.UnitX * (Int32)Math.Round(1280f / 2) +
                Vector2.UnitY * (Single)(Math.Round((720f - height) / 2) + 10 + Math.Round(titleMeasurement.Y));
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
        }

        /// <summary>
        /// Processes input
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
            var maxIndex = Options.Values.Count;

            if (_bindKeyTask != null && !(_bindKeyTask.IsCanceled || _bindKeyTask.IsCompleted || _bindKeyTask.IsFaulted))
                return;

            if (this.InputManager.Keyboard.IsKeyReleased(Keys.Enter))
            {
                if (_menuIndex == maxIndex) {
                    ExitScreen();
                    return;
                }

                var enterIndex = _menuIndex;
                var pressed = _inputManager.Keyboard.PressedKeys;

                _bindKeyTaskCancel = new CancellationTokenSource();
                _bindKeyTask = Task.Factory.StartNew<Keys>(() =>
                {
                    // Wait for a key to be pressed
                    var nowPressed = _inputManager.Keyboard.PressedKeys;
                    while (!nowPressed.Any(a => !pressed.Contains(a)) && !_bindKeyTaskCancel.IsCancellationRequested)
                    {
                        nowPressed = _inputManager.Keyboard.PressedKeys;
                        Thread.Sleep(10);
                        Thread.MemoryBarrier();
                    }

                    // Cancel if cancellation keys
                    var keyPressed = nowPressed.First(a => !pressed.Contains(a));
                    if (keyPressed == Keys.Escape)
                        _bindKeyTaskCancel.Cancel();

                    //_bindKeyTaskCancel.Token.ThrowIfCancellationRequested();
                    if (_bindKeyTaskCancel.IsCancellationRequested)
                        return Keys.None;

                    // Reset all that match
                    var actions = (ControllerAction[])System.Enum.GetValues(typeof(ControllerAction));

                    foreach (var action in actions)
                        if (_controller[action] == keyPressed)
                            _controller[action] = Keys.None;

                    Thread.Sleep(100);
                    Thread.MemoryBarrier();

                    // Set the new one
                    _controller[Options.Keys.ElementAt(enterIndex)] = keyPressed;
                    return _controller[Options.Keys.ElementAt(enterIndex)];

                }, _bindKeyTaskCancel.Token);
                
                this.AudioManager.Play("confirm");
            }
            else if (this.InputManager.Keyboard.IsKeyReleased(Keys.Escape))
            {
                this.ExitScreen();
                this.AudioManager.Play("confirm");
            }

            if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Down))
            {
                _menuIndex = (_menuIndex + 1) % (Options.Values.Count + 1);
                this.AudioManager.Play("blip");
            }
            else if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Up))
            {
                _menuIndex = (_menuIndex == 0 ? (Options.Values.Count + 1) - 1 : _menuIndex - 1);
                this.AudioManager.Play("blip");
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

            var alpha = 1 - this.TransitionPosition;

            base.Draw(gameTime);

            this.ScreenManager.SpriteBatch.Begin();
            this.ScreenManager.SpriteBatch.Draw(_texture, new Rectangle(0, 0, this.ScreenManager.ScreenWidth, this.ScreenManager.ScreenHeight), Color.Black * 0.5f * alpha);
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Title"], TitleString, _positionTitle, Color.White * alpha, Color.Black * alpha);
            var position = _positionMenu;
            var activeTask = _bindKeyTask != null && !(_bindKeyTask.IsCanceled || _bindKeyTask.IsCompleted || _bindKeyTask.IsFaulted);
            
            for (Int32 i = 0; i < Options.Values.Count; i++)
            {
                var optionString = Options.Values.ElementAt(i) + ": " + _controller[Options.Keys.ElementAt(i)].ToString();
                var measurement = this.ScreenManager.SpriteFonts["Menu"].MeasureString(optionString);
                this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Menu"], optionString, position,
                    (_controller[Options.Keys.ElementAt(i)] == Keys.None ? Color.Red : Color.White) * alpha,
                        (_menuIndex == i ? (activeTask ? Color.Green : Color.Gray) : Color.Black) * alpha, 0,
                    (Single)Math.Round(measurement.X / 2) * Vector2.UnitX + (Single)Math.Round(measurement.Y / 2) * Vector2.UnitY,
                    1, SpriteEffects.None, 0);
                position = position + Vector2.UnitY * 15;
            }

            var measurementExit = this.ScreenManager.SpriteFonts["Menu"].MeasureString(ExitOption);
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Menu"], ExitOption, position, Color.White * alpha, 
                (_menuIndex == Options.Values.Count ? Color.Gray : Color.Black) * alpha, 0,
                  (Single)Math.Round(measurementExit.X / 2) * Vector2.UnitX + (Single)Math.Round(measurementExit.Y / 2) * Vector2.UnitY,
                    1, SpriteEffects.None, 0);
            this.ScreenManager.SpriteBatch.End();
        }
    }
}
