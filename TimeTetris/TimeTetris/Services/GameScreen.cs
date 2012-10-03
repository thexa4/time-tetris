using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TimeTetris.Services
{
    /// <summary>
    /// The GameScreen provides basic functionality to be used with the ScreenManager.
    /// </summary>
    public class GameScreen
    {
        #region Fields

        private Boolean _isExiting;
        private Boolean _otherScreenHasFocus;
        private ScreenState _screenState;

        protected Game _gameReference;
        protected Boolean _hasCalledExited = false;
        protected Boolean _noGarbageCollection = false;

        protected ScreenManager _screenManager;
        protected InputManager _inputManager;
        protected AudioManager _audioManager;
        protected GameScreen _nextScreen;

        private TimeSpan _transitionOffTime = TimeSpan.Zero;
        private TimeSpan _transitionOnTime = TimeSpan.Zero;
        private Single _transitionPosition = 1;
        #endregion

        #region Properties
        /// <summary>
        /// The next screen will be called if this screen is exited
        /// </summary>
        public GameScreen Next
        {
            set
            {
                // Exit Previous NextScreen
                if (_nextScreen != null && _nextScreen.ScreenState != Services.ScreenState.Hidden) 
                    _nextScreen.ExitScreen();
                else
                    Exited += new EventHandler(GameScreen_Exited);

                _nextScreen = value;
            }
            get
            {
                return _nextScreen;
            }
        }

        /// <summary>
        /// Eventhandler on exit
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        private void GameScreen_Exited(object sender, EventArgs e)
        {
            if (_hasCalledExited)
            {
                this.ScreenManager.AddScreen(this.Next);
                _hasCalledExited = false;
            }
        }

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            set { _screenManager = value; }
            get { return _screenManager; }
        }

        /// <summary>
        /// Exposes InputService
        /// </summary>
        public InputManager InputManager
        {
            set { _inputManager = value; }
            get { return _inputManager; }
        }

        /// <summary>
        /// Exposes AudioService
        /// </summary>
        public AudioManager AudioManager
        {
            get { return _audioManager; }
            set { _audioManager = value; }
        }

        /// <summary>
        /// Game reference
        /// </summary>
        public Game Game
        {
            get { return _gameReference; }
            set { _gameReference = value; }
        }

        /// <summary>
        /// Expose ContentManager
        /// </summary>
        protected ContentManager ContentManager { get; set; }

        /// <summary>
        /// Normally when one screen is brought up over the top of another, the first screen will 
        /// transition off to make room for the new one. This property indicates whether the 
        /// screen is only a small popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public Boolean IsPopup { get; protected set; }

        /// <summary>
        /// When true, this screen will capture the inputmanager and screens with lower priority 
        /// will not have their HandleInput() function executed.
        /// </summary>
        public Boolean IsCapturingInput { get; protected set; }

        /// <summary>
        /// There are two possible reasons why a screen might be transitioning off. It could be 
        /// temporarily going away to make room for another screen that is on top of it, or it 
        /// could be going away for good. This property indicates whether the screen is exiting 
        /// for real: if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public Boolean IsExiting
        {
            get { return _isExiting; }
            protected set { _isExiting = value; }
        }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public Boolean IsActive
        {
            get
            {
                return !_otherScreenHasFocus &&
                       (_screenState == ScreenState.TransitionOn ||
                        _screenState == ScreenState.Active);
            }
        }

        /// <summary>
        /// Check whether this screen is active and is transitioning
        /// </summary>
        public Boolean IsTransitioning
        {
            get
            {
                return (_screenState == ScreenState.TransitionOn ||
                        _screenState == ScreenState.TransitionOff);
            }
        }

        /// <summary>
        /// Indicates how long the screen takes to transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return _transitionOnTime; }
            protected set { _transitionOnTime = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return _transitionOffTime; }
            protected set { _transitionOffTime = value; }
        }

        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public Single TransitionPosition
        {
            get { return _transitionPosition; }
            protected set { _transitionPosition = value; }
        }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging from 255 (fully active, 
        /// no transition) to 0 (transitioned fully off to nothing).
        /// </summary>
        public Byte TransitionAlpha
        {
            get { return (Byte)(255 - TransitionPosition * 255); }
        }

        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return _screenState; }
            protected set { _screenState = value; }
        }

        /// <summary>
        /// Currently visible (calls draw)
        /// </summary>
        public Boolean IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Currently enabled (calls update)in
        /// </summary>
        public Boolean IsEnabled
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Initializes Screen, sets Variables
        /// </summary>
        public virtual void Initialize()
        {
            if (this.IsPopup == false)
                this.IsCapturingInput = true;

            if (this.InputManager == null)
                this.InputManager = (InputManager)this.Game.Services.GetService(typeof(InputManager));
            if (this.InputManager == null)
                throw new InvalidOperationException("No Input service found.");
            if (this.AudioManager == null)
                this.AudioManager = (AudioManager)this.Game.Services.GetService(typeof(AudioManager));

            if (_noGarbageCollection == false)
                GC.Collect(Int32.MaxValue, GCCollectionMode.Forced);

            this.IsVisible = true;
            this.IsEnabled = true;
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            this.ContentManager = contentManager;

            if (this.ContentManager.RootDirectory == String.Empty)
                this.ContentManager.RootDirectory = "Content";
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent()
        {
            if (this.ContentManager != null)
            {
                this.ContentManager.Unload();
                this.ContentManager.Dispose();

                this.ContentManager = null;
            }
        }

        /// <summary>
        /// Post process after adding screen to the list
        /// </summary>
        public virtual void PostProcessing()
        {

        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike <see cref="HandleInput"/>, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing values</param>
        /// <param name="otherScreenHasFocus">Game is blurred</param>
        /// <param name="coveredByOtherScreen">Other GameScreen is active</param>
        public virtual void Update(GameTime gameTime, Boolean otherScreenHasFocus, Boolean coveredByOtherScreen)
        {
            _otherScreenHasFocus = otherScreenHasFocus;

            if (_isExiting)
            {
                // If the screen is going away to die, it should transition off.
                _screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, _transitionOffTime, 1) && _screenManager != null)
                {
                    // When the transition finishes, remove the screen.
                    _screenManager.RemoveScreen(this);
                    // Reset the variable
                    _isExiting = false;
                    // Call the event
                    if (Exited != null)
                        Exited(this, EventArgs.Empty);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, _transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    _screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    _screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Update delay
                if (_screenState == ScreenState.Hidden)
                {
                    // Set on transition
                    _screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Otherwise the screen should transition on and become active.
                    if (UpdateTransition(gameTime, _transitionOnTime, -1))
                    {
                        // Still busy transitioning.
                        _screenState = ScreenState.TransitionOn;
                    }
                    else
                    {
                        // Transition finished!
                        _screenState = ScreenState.Active;
                    }
                }
            }
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        /// <param name="gameTime">Snapshot of timing Values</param>
        /// <param name="direction">Direction of the transition</param>
        /// <param name="time">Transition Time</param>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, Int32 direction)
        {
            // How much should we move by?
            Single transitionDelta;

            // Update delay

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (Single)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                           time.TotalMilliseconds);

            // Update the transition position.
            _transitionPosition += transitionDelta * direction;


            // Did we reach the end of the transition?
            if ((_transitionPosition <= 0) || (_transitionPosition >= 1))
            {
                _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
                return false;
            }
            // Otherwise we are still busy transitioning.
            return true;
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        /// <param name="gameTime">Snapshot of timing Values</param>
        public virtual void HandleInput(GameTime gameTime)
        {

        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime">Snapshot of timing Values</param>
        public virtual void Draw(GameTime gameTime)
        {

        }

        /// <summary>
        /// Tells the screen to go away. Unlike <see cref="ScreenManager">RemoveScreen</see>, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if ((TransitionOffTime == TimeSpan.Zero || this.IsEnabled == false) && _screenManager != null)
            {
                if (!_isExiting && Exiting != null)
                    Exiting(this, EventArgs.Empty);

                // If the screen has a zero transition time, remove it immediately.
                _screenManager.RemoveScreen(this);
                // Call Exited if needed
                if (Exited != null)
                    Exited(this, EventArgs.Empty);
            }
            else
            {
                if (!_isExiting && Exiting != null)
                    Exiting(this, EventArgs.Empty);

                // Otherwise flag that it should transition off and then exit.
                _isExiting = true;
            }
        }

        /// <summary>
        /// Exits the screen and calls all post exiting.
        /// </summary>
        public void ExitScreenAnd()
        {
            _hasCalledExited = true;
            ExitScreen();
        }

        /// <summary>
        /// OnExited Event
        /// </summary>
        public event EventHandler Exited;

        /// <summary>
        /// While exiting
        /// </summary>
        public event EventHandler Exiting;

        /// <summary>
        /// IDisposable Members Unloading
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
