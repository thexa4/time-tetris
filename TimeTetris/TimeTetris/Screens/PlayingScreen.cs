using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTetris.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TimeTetris.Drawing;
using TimeTetris.Data;
using Microsoft.Xna.Framework.Input;
using TimeTetris.Extension;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TimeTetris.Screens
{
    public class PlayingScreen : GameScreen
    {
        private Data.Field _field;

        private SpriteField _spriteField;
        private Sprite _spriteNextBlockBoundary, _spriteHoldBlockBoundary;
        private SpriteBlock _spriteNextBlock;
        private SpriteGhostBlock _spriteGhostBlock;
        private SpriteNullableBlock _spriteHoldBlock;
        private SpriteFallingBlock _spriteFallingBlock;
        private KeyboardController _controller;

        private RenderTarget2D _intermediateTarget;
        private RenderTarget2D _distortTarget;
        protected Effect _distortEffect;
        protected Texture2D _retroTv;

        protected Timeline _timeline;
        protected Boolean _isRewinding;

        protected GameScreen _popupScreen;
        protected Double _displayScore;
        protected Double _scoreIncrement;
        protected List<SpriteScorePopup> _spriteScorePopups;

        protected SoundEffectInstance _rotateSound;
        protected SoundEffectInstance _lockSound;

       
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
            DummyField dummyField = new Data.DummyField(this.Game, 5, 5 + SpriteField.HiddenRows);

            // Create Sprites
            _spriteField = new SpriteField(this.Game, _field) { Position = Vector2.One * SpriteField.GridCellSize * 3 };
            _spriteGhostBlock = new SpriteGhostBlock(this.Game, _field.CurrentBlock) { Position = _spriteField.Position };
            _spriteFallingBlock = new SpriteFallingBlock(this.Game, _field.CurrentBlock) { Position = _spriteField.Position };

            // Next Block and HoldBlock Boundaries
            _spriteNextBlockBoundary = new SpriteField(this.Game, dummyField) { Position = _spriteField.Position + (Vector2.UnitX * (_field.Width + 2)) * SpriteField.GridCellSize, };
            _spriteHoldBlockBoundary = new SpriteField(this.Game, dummyField) { Position = _spriteField.Position + (Vector2.UnitX * (_field.Width + 2) + Vector2.UnitY * (6 + 1)) * SpriteField.GridCellSize, };
           
            // Next BLock
            _spriteNextBlock = new SpriteBlock(this.Game, _field.NextBlock) { Position = _spriteNextBlockBoundary.Position + Vector2.One * SpriteField.GridCellSize, };
            _field.NextBlock.OnTypeChanged += new BlockTypeDelegate(NextBlock_OnTypeChanged);
            _spriteHoldBlock = new SpriteNullableBlock(this.Game) { Position = _spriteHoldBlockBoundary.Position + Vector2.One * SpriteField.GridCellSize, };
            
            _spriteField.Initialize();
            _spriteGhostBlock.Initialize();
            _spriteFallingBlock.Initialize();
            _spriteNextBlockBoundary.Initialize();
            _spriteHoldBlockBoundary.Initialize();
            _spriteNextBlock.Initialize();
            _spriteHoldBlock.Initialize();

            _spriteScorePopups = new List<SpriteScorePopup>();

            // Player controller
            _controller = new KeyboardController(this.Game, Keys.S, Keys.A, Keys.D, Keys.W, Keys.Q, Keys.E, Keys.Space, Keys.Enter);
            _controller.Initialize();

            _field.OnGameEnded += new EventHandler(_field_OnGameEnded);
            _field.OnRowsCleared += new RowsDelegate(_field_OnRowsCleared);
            _field.OnPointsEarned += new PointsDelegate(_field_OnPointsEarned);

            // Start the level
            _timeline.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        protected void _field_OnPointsEarned(Double points)
        {
            _scoreIncrement += points;
        }

        /// <summary>
        /// Yields when a row is cleared (also on rows = 0)
        /// </summary>
        /// <param name="rows">number of rows</param>
        /// <param name="combo">combo</param>
        /// <param name="tspin"></param>
        /// <param name="b2b"></param>
        protected void _field_OnRowsCleared(Int32 rows, Int32 combo, Boolean tspin, Boolean b2b)
        {
            _lockSound.Play();

            if (rows == 0 && !tspin) // nothing to report
                return;

            var sprite = new SpriteScorePopup(this.Game, rows, combo, tspin, b2b)
                {
                    Position = _spriteField.Position + (Vector2.UnitX * _field.Width * SpriteField.GridCellSize +
                        Vector2.UnitY * (_field.Height - SpriteField.HiddenRows) * SpriteField.GridCellSize) / 2,

                    Opacity = 0.9f,
                };

            sprite.Initialize();
            sprite.LoadContent(this.ContentManager);

            _spriteScorePopups.Add(sprite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _field_OnGameEnded(object sender, EventArgs e)
        {
            _popupScreen = new ReplayScreen(this);
            _popupScreen.Exited += new EventHandler(_replayScreen_Exited);
            this.ScreenManager.AddScreen(_popupScreen);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _replayScreen_Exited(object sender, EventArgs e)
        {
            if (this.IsExiting)
                return;

            _isRewinding = true;
            _timeline.Enabled = true;
            _popupScreen = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        private void NextBlock_OnTypeChanged(BlockType block)
        {
            //_spriteNextBlockBoundary.Size = Vector2.One * SpriteField.GridCellSize * _field.NextBlock.Width;
            //_spriteNextBlockBoundary.Scale = _spriteNextBlockBoundary.Size.X / _spriteNextBlockBoundary.SourceRectangle.Width * Vector2.UnitX + 
                //_spriteNextBlockBoundary.Size.Y / _spriteNextBlockBoundary.SourceRectangle.Height * Vector2.UnitY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        private void HoldBlock_OnTypeChanged(BlockType block)
        {
            //_spriteHoldBlockBoundary.Size = Vector2.One * SpriteField.GridCellSize * _field.HoldBlock.Width;
            //_spriteHoldBlockBoundary.Scale = _spriteHoldBlockBoundary.Size.X / _spriteHoldBlockBoundary.SourceRectangle.Width * Vector2.UnitX +
                //_spriteHoldBlockBoundary.Size.Y / _spriteHoldBlockBoundary.SourceRectangle.Height * Vector2.UnitY;
        }

        /// <summary>
        /// Loads all content
        /// </summary>
        /// <param name="contentManager"></param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            this._intermediateTarget = new RenderTarget2D(Game.GraphicsDevice, 800, 600);
            this._distortTarget = new RenderTarget2D(Game.GraphicsDevice, 800, 600);
            this._distortEffect = contentManager.Load<Effect>("Shaders\\Distort");

            //Default projection
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, this._intermediateTarget.Width, this._intermediateTarget.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            this._distortEffect.Parameters["Projection"].SetValue(halfPixelOffset * projection);

            this._retroTv = contentManager.Load<Texture2D>("Graphics\\retrotv");
            
            _spriteField.LoadContent(contentManager);
            _spriteGhostBlock.LoadContent(contentManager);
            _spriteFallingBlock.LoadContent(contentManager);
            _spriteNextBlock.LoadContent(contentManager);
            _spriteHoldBlock.LoadContent(contentManager);
            _spriteNextBlockBoundary.LoadContent(contentManager);
            _spriteHoldBlockBoundary.LoadContent(contentManager);

            _rotateSound = this.AudioManager.Load("blip", "rotate", 0.6f, 0);
            _lockSound = this.AudioManager.Load("blip", "lock", 0.6f, -.5f);

            this.ScreenManager.SpriteFonts.LoadFont("SmallInfo", "Fonts/Default");
            this.ScreenManager.SpriteFonts.LoadFont("Small", "Fonts/Small");
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

            if ((_popupScreen != null && (_popupScreen.ScreenState == Services.ScreenState.Active || _popupScreen.IsTransitioning))
                || otherScreenHasFocus)
                return;

            if (_isRewinding)
            {
                _timeline.RewindFrame();
                if (_timeline.CurrentTime <= 0.01)
                {
                    _timeline.Start();
                    _isRewinding = false;

                    _field.ResetBlockGenerator();
                }
            }

            _field.Update(gameTime);
            _controller.Update(gameTime);

            _spriteField.Update(gameTime);
            _spriteFallingBlock.Update(gameTime);            
            _spriteGhostBlock.Update(gameTime);
            _spriteNextBlock.Update(gameTime);
            _spriteNextBlockBoundary.Update(gameTime);
            _spriteHoldBlock.Update(gameTime);
            _spriteHoldBlockBoundary.Update(gameTime);

            var removablePopups = new List<SpriteScorePopup>();
            _spriteScorePopups.ForEach(a => { a.Update(gameTime); if (a.IsFinished) removablePopups.Add(a); });
            removablePopups.ForEach(a => _spriteScorePopups.Remove(a));

            var deltaScore = Math.Min((_field.Score - _displayScore), 
                Math.Max((Single)(100 * gameTime.ElapsedGameTime.TotalSeconds), 
                    (Single)((_field.Score - _displayScore) * gameTime.ElapsedGameTime.TotalSeconds)));
            _displayScore = _displayScore + deltaScore;
            _scoreIncrement = Math.Min(_scoreIncrement, (_field.Score - _displayScore));
        }

        /// <summary>
        /// Handle input
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            if (InputManager.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape) && _popupScreen == null)
            {
                _timeline.Stop();
                _popupScreen = new PauseScreen(this);
                this.ScreenManager.AddScreen(_popupScreen);

                _popupScreen.Exited += new EventHandler(_pauseScreen_Exited);
                return;
            }

            // Don't process game if ended
            if (_field.HasEnded)
                return;

            if (_controller.Action == ControllerAction.Time && _field.Score > 0)
                _timeline.RewindFrame();

            // Don't process if rewinding
            if (_timeline.IsRewindActive)
                return;

            // Action from the controller
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
                case ControllerAction.Drop:
                    _field.CurrentBlock.Drop();
                    break;
                case ControllerAction.RotateCW:
                    if (_field.CurrentBlock.RotateRight())
                        _rotateSound.Play();
                    break;
                case ControllerAction.RotateCCW:
                    if (_field.CurrentBlock.RotateLeft())
                        _rotateSound.Play();
                    break;
                case ControllerAction.Hold:
                    var oldHoldBlock = _field.HoldBlock;
                    if (_field.SwitchHoldingBlock()) {
                        var holdBlock = _field.HoldBlock;
                        _timeline.Add(new Event() {
                        
                            Apply = () => {
                                _spriteHoldBlock.SetBlock(holdBlock);
                                HoldBlock_OnTypeChanged(holdBlock.Type);
                            },

                            Undo = () => {
                                _spriteHoldBlock.SetBlock(oldHoldBlock);

                                if (oldHoldBlock != null)
                                    HoldBlock_OnTypeChanged(oldHoldBlock.Type);
                            } 
                        });
                            
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pauseScreen_Exited(object sender, EventArgs e)
        {
            _popupScreen = null;
            _timeline.Resume();
        }

        /// <summary>
        /// Draw frame
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            if (!this.IsTransitioning && this.ScreenState != Services.ScreenState.Active)
                return;

            base.Draw(gameTime);

            this.Game.GraphicsDevice.SetRenderTarget(_intermediateTarget);

            // Clear the graphics device, making the background somewhat translucent
            this.Game.GraphicsDevice.Clear(new Color(0.0f, 0.0f, 0.0f, 0.6f));

            this.ScreenManager.SpriteBatch.Begin();

            // Draw the blocks and field and so on
            _spriteField.Draw(gameTime);
            _spriteGhostBlock.Draw(gameTime);
            _spriteFallingBlock.Draw(gameTime);
            _spriteNextBlockBoundary.Draw(gameTime);
            _spriteNextBlock.Draw(gameTime);
            _spriteHoldBlockBoundary.Draw(gameTime);
            _spriteHoldBlock.Draw(gameTime);

            // Popups
            _spriteScorePopups.ForEach(a => a.Draw(gameTime));

            // Strings to do
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["SmallInfo"],
                "Next block", _spriteNextBlockBoundary.Position - Vector2.One * 5, Color.White, Color.Black);
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["SmallInfo"],
                "Hold block", _spriteHoldBlockBoundary.Position - Vector2.One * 5, Color.White, Color.Black);
            this.ScreenManager.SpriteBatch.DrawString(this.ScreenManager.SpriteFonts["SmallInfo"], 
                String.Format("{0:0.00}ls   {3:0.00}ls/s   {1:####0} points   {2} combo   level {5} / {4} lines   BTB is {6}",
                    Math.Round(_timeline.CurrentTime, 2), _displayScore, _field.CurrentCombo, _timeline.RewindSpeed, 
                    _field.LinesCleared, _field.Level, _field.IsBackToBackEnabled ? "enabled" : "not enabled"), Vector2.One * 5, Color.White);
            
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Small"],
                String.Format("{0:####0} points", _displayScore), _spriteHoldBlockBoundary.Position +
                    7 * SpriteField.GridCellSize * Vector2.UnitY, Color.White, Color.Black);

            if (_scoreIncrement > 0)
                this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Small"],
                    String.Format("+{0:####0}", _scoreIncrement), _spriteHoldBlockBoundary.Position +
                        8 * SpriteField.GridCellSize * Vector2.UnitY, Color.White, Color.Black);

            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Small"],
                    String.Format("Level {0:####0}", _field.Level), _spriteHoldBlockBoundary.Position +
                        10 * SpriteField.GridCellSize * Vector2.UnitY, Color.White, Color.Black);
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Small"],
                    String.Format("{0:####0} lines left", _field.Level * 10 - _field.LinesCleared), _spriteHoldBlockBoundary.Position +
                        11 * SpriteField.GridCellSize * Vector2.UnitY, Color.White, Color.Black);
            
                    

            this.ScreenManager.SpriteBatch.End();

            // Distort effect
            this.Game.GraphicsDevice.SetRenderTarget(_distortTarget);

            this.Game.GraphicsDevice.Clear(new Color(0.0f, 0.0f, 0.0f, 0.0f));
            this.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.AnisotropicClamp, null, null, this._distortEffect);
            this.ScreenManager.SpriteBatch.Draw(this._intermediateTarget, Vector2.Zero, Color.White);
            this.ScreenManager.SpriteBatch.End();

            // Draw distort scene in the tv
            this.Game.GraphicsDevice.SetRenderTarget(null);

            this._distortEffect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds / 20 + 5000);
            this.ScreenManager.SpriteBatch.Begin();
            this.ScreenManager.SpriteBatch.Draw(this._retroTv, Vector2.Zero, Color.White);
            this.ScreenManager.SpriteBatch.Draw(this._distortTarget, new Rectangle(114, 95, 680, 510), Color.White);
            this.ScreenManager.SpriteBatch.End();

            this.ScreenManager.FadeBackBufferToBlack((Byte)(255 - this.TransitionAlpha));
        }
    }
}
