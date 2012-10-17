using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TimeTetris.Services
{
    /// <summary>
    /// The screen manager is a component which manages one or more <see cref="GameScreen"/>
    /// instances. It maintains a stack of _screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes _input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields (Blank Texture, GraphicsDeviceService, Screens, SpriteFonts)

        private static Texture2D _blankTexture;
        private static IGraphicsDeviceService _graphicsDeviceService;
        private static List<GameScreen> _screens = new List<GameScreen>();
        private static List<GameScreen> _screensToUpdate = new List<GameScreen>();
        private static FontCache _spriteFonts;
        private Int32 _screenWidth, _screenHeight;

        #endregion

        private readonly String BlankTextureAsset = "Graphics/Blank";

        /// <summary>
        /// Number of Screens
        /// </summary>
        public Int32 Count { get { return _screens == null ? 0 : _screens.Count; } }

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        /// <param name="game">Game Reference</param>
        /// <exception cref="InvalidOperationException">No graphics device service.</exception>
        public ScreenManager(Game game)
            : base(game)
        {
            // Load Content Manager and set component values
            this.ContentManager = new ContentManager(game.Services);
            this.ContentManager.RootDirectory = "Content";
            this.UpdateOrder = 50;
            this.DrawOrder = 50;

            // Add the spriteFonts Component
            _spriteFonts = new FontCache(this.Game);
            this.Game.Components.Add(_spriteFonts);

            // Add Exiting Handler
            this.Game.Exiting += new EventHandler<EventArgs>(GameExiting);

            // Add as Service
            this.Game.Services.AddService(this.GetType(), this);
        }

        #region Properties
        /// <summary>
        /// A content manager used to load data that is shared between multiple
        /// screens. This is never unloaded, so if a screen requires a large amount
        /// of temporary data, it should create a local content manager instead.
        /// </summary>
        public ContentManager ContentManager { get; private set; }

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public InputManager InputManager { get; private set; }

        /// <summary>
        /// Property that returns the FontCollector Object
        /// </summary>
        public FontCache SpriteFonts
        {
            get { return _spriteFonts; }
        }

        /// <summary>
        /// Expose access to our Game instance (this is protected in the
        /// default <see cref="GameComponent"/>, but we want to make it public).
        /// </summary>
        public new Game Game
        {
            get { return base.Game; }
        }

        /// <summary>
        /// Expose access to our graphics device (this is protected in the
        /// default <see cref="DrawableGameComponent"/>, but we want to make it public).
        /// </summary>
        public new GraphicsDevice GraphicsDevice
        {
            get { return base.GraphicsDevice; }
        }

        /// <summary>
        /// Get Screen Width (viewport)
        /// </summary>
        public Int32 ScreenWidth
        {
            get
            {
                return _screenWidth;
            }
            private set
            {
                _screenWidth = value;
            }
        }

        /// <summary>
        /// Get Screen Height (viewport)
        /// </summary>
        public Int32 ScreenHeight
        {
            get
            {
                return _screenHeight;
            }
            private set
            {
                _screenHeight = value;
            }
        }

        /// <summary>
        /// Returns the Center of the Screen
        /// </summary>
        public Vector2 ScreenCenter
        {
            get
            {
                return new Vector2(this.ScreenWidth / 2f,
                                   this.ScreenHeight / 2f);
            }
        }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String NextScreen
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NoMoreScreens;
        #endregion

        /// <summary>
        /// Event Handler for Game Exiting
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arguments</param>
        private void GameExiting(object sender, EventArgs e)
        {
            //Make sure to dispose ALL screens when the game is forcefully closed
            //We do this to ensure that open resources and threads created by screens are closed.
            foreach (GameScreen screen in _screens)
            {
                screen.Dispose();
            }
            // Clear the Arrays
            _screens.Clear();
            _screensToUpdate.Clear();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Load Graphics Device Service
            _graphicsDeviceService = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));
            if (_graphicsDeviceService == null)
                throw new InvalidOperationException("No graphics device service.");

            // Load Input Device Service
            this.InputManager = (InputManager)this.Game.Services.GetService(typeof(InputManager));
            if (this.InputManager == null)
                throw new InvalidOperationException("No input device manager.");

            // Initialize component
            base.Initialize();

            // Set Viewport
            this.ScreenWidth = this.GraphicsDevice.Viewport.Width;
            this.ScreenHeight = this.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            this.SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Try to load this Texture
            try
            {
                _blankTexture = this.ContentManager.Load<Texture2D>(BlankTextureAsset);
            }
            // If texture was not found...
            catch (ContentLoadException)
            {
                // ...Notify and mark a method as not usable
                //Logger.Warning("Blank texture was not loaded. FadeBackBuffertoBlack can not be used");
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            ContentManager.Unload();

            // Tell each of the _screens to unload their content.
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }

            SpriteBatch.Dispose();
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();
            lock (_screens)
                for (Int32 i = 0; i < _screens.Count; i++)
                    _screensToUpdate.Add(_screens[i]);

            // Exit Game if there is nothing more to show or update
            if (_screensToUpdate.Count == 0)
            {
                if (NoMoreScreens != null)
                {
                    NoMoreScreens.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Game.Exit();
                }
            }

            // Get Game State and Screen State
            Boolean i_otherScreenHasFocus = !Game.IsActive;
            Boolean i_coveredByOtherScreen = false;

            // Loop as long as there are _screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen i_screen = _screensToUpdate[_screensToUpdate.Count - 1];
                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                if (i_screen.IsEnabled)
                {
                    i_screen.Update(gameTime, i_otherScreenHasFocus, i_coveredByOtherScreen);

                    if (i_screen.ScreenState == ScreenState.TransitionOn ||
                        i_screen.ScreenState == ScreenState.Active)
                    {
                        // If this is the first active screen we came across,
                        // give it a chance to handle _input.
                        if (i_screen.IsCapturingInput)
                        {
                            i_screen.HandleInput(gameTime);
                            i_otherScreenHasFocus = true;
                        }
                        // If this is some active popup, handle _input
                        else if (i_screen.IsPopup)
                            i_screen.HandleInput(gameTime);

                        // If this is an active non-popup, inform any subsequent
                        // _screens that they are covered by it.
                        if (!i_screen.IsPopup)
                            i_coveredByOtherScreen = true;
                    }
                }
            }

            // Print debug trace?
            if (TraceEnabled)
                TraceScreens();
        }

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary> 
        private void TraceScreens()
        {
            // Make a list of screenNames in memory
            List<String> i_screenNames = new List<String>();
            // Add all ScreenNames to this list          
            foreach (GameScreen i_screen in _screens)
                i_screenNames.Add(i_screen.GetType().Name);
            // Write the List to the Trace
            Trace.WriteLine(String.Join(", ", i_screenNames.ToArray()));
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // For every screen
            for (Int32 i = 0; i < _screens.Count; i++)
            {
                // If not hidden ScreenState
                if (_screens[i].ScreenState == ScreenState.Hidden || _screens[i].IsVisible == false)
                    continue;

                // Draw this screen
                _screens[i].Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.Game = this.Game;

            // Initialize this Screen
            screen.Initialize();

            // If we have a graphics device, tell the screen to load content.
            if ((_graphicsDeviceService != null) &&
                (_graphicsDeviceService.GraphicsDevice != null))
            {
                // Load the Content
                screen.LoadContent(new ContentManager(Game.Services, "Content"));
            }
            // Actually Add the screen to the list
            lock (_screens)
                _screens.Add(screen);
            // Process post actions
            screen.PostProcessing();

        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use <see cref="GameScreen"/>.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if ((_graphicsDeviceService != null) &&
                (_graphicsDeviceService.GraphicsDevice != null))
            {
                screen.UnloadContent();
            }
            // Remove the screen from the arrays
            lock (_screens)
                _screens.Remove(screen);

            lock (_screensToUpdate)
                _screensToUpdate.Remove(screen);
            // Dispose the Screen (Release it's Content)
            screen.Dispose();
        }

        /// <summary>
        /// Removes all screens but argument screen
        /// </summary>
        /// <param name="excluded_screen"></param>
        public void RemoveAllBut(GameScreen excludedScreen)
        {
            List<GameScreen> i_screensToRemove = new List<GameScreen>();

            lock (_screens)
                foreach (GameScreen i_screen in _screens)
                    if (i_screen != excludedScreen)
                        i_screensToRemove.Add(i_screen);

            foreach (GameScreen i_screen in i_screensToRemove)
                RemoveScreen(i_screen);

            i_screensToRemove.Clear();
        }

        /// <summary>
        /// Exits all screens
        /// </summary>
        public void ExitAll()
        {
            GameScreen[] to_remove = null;
            lock (_screens)
            {
                to_remove = new GameScreen[_screens.Count];
                _screens.CopyTo(to_remove);
            }

            foreach (GameScreen i_screen in to_remove)
                i_screen.ExitScreen();
        }

        /// <summary>
        /// Helper draws a translucent black full screen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(Byte alpha)
        {
            // If texture was not loaded
            if (_blankTexture == null)
                // Kill this call
                throw (new InvalidOperationException("FadeBackBufferToBlack not available",
                    new ContentLoadException("Blank texture was not loaded. FadeBackBuffertoBlack can not be used")));

            // Start the SpriteBatch
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            // Stretch Texture to Viewport
            SpriteBatch.Draw(_blankTexture,
                             new Rectangle(0, 0, this.ScreenWidth, this.ScreenHeight),
                             new Color(0, 0, 0, alpha));
            // Flush the SpriteBatch
            SpriteBatch.End();
        }
    }
}
