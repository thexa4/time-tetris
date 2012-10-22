using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.Remoting.Contexts;

namespace TimeTetris.Services
{
    /// <summary>
    /// The InputManager Class provides functions for serveral different types of input. 
    /// Easily checkup on Keyboard or Mouse input and also use more advanced functions 
    /// like IsTriggerd button state management.
    /// </summary>
    public partial class InputManager : GameComponent
    {
        /// <summary>
        /// Keyboard Input Reference
        /// </summary>
        public KeyboardInputComponent Keyboard
        {
            get;
            private set;
        }

        public GamepadInputComponent GamePad
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor for the Input Component
        /// </summary>
        /// <param name="game">Game to bind to</param>
        public InputManager(Game game)
            : base(game)
        {
            // Set this to Update before everything
            this.UpdateOrder = 0;
            // Create a Keyboard Input Reference
            this.Keyboard = new KeyboardInputComponent(game);

            this.GamePad = new GamepadInputComponent(game);

            // Add as Service
            this.Game.Services.AddService(this.GetType(), this);
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            // Update Input
            this.Keyboard.Update(gameTime);

            this.GamePad.Update(gameTime);

            // Update the Component
            base.Update(gameTime);
        }
    }

    /// <summary>
    /// The KeyboardComponent is a component that handles Keyboard input. It handles
    /// the actual keyboard states, whereas the manager only references. 
    /// </summary>
    public partial class KeyboardInputComponent : GameComponent
    {
        #region Fields
        private KeyboardState _currentState;
        private KeyboardState _previousState;
        private Boolean _isWatching;
        private Dictionary<Keys, TriggerKey> _triggers;

        private readonly Keys[] _DEFAULTWATCHING = new Keys[] { Keys.Enter, Keys.Up, Keys.Down, Keys.Left, 
            Keys.Right, Keys.W, Keys.S, Keys.A, Keys.D, Keys.Escape };
        private readonly Keys[] _LETTERS = new Keys[] { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, 
            Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, 
            Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z };
        private readonly Keys[] _DIGITS = new Keys[] { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, 
            Keys.D5, Keys.D6, Keys.D7,Keys.D8, Keys.D9 };

        private readonly Dictionary<Keys, String> _DIGITStoSTRING = new Dictionary<Keys, String>();
        private readonly Dictionary<Keys, String> _DIGITStoCAPITALSTRING = new Dictionary<Keys, String>();

        internal Keys[] DEFAULTWATCHING { get { return _DEFAULTWATCHING; } }
        internal Keys[] LETTERS { get { return _LETTERS; } }
        internal Keys[] DIGITS { get { return _DIGITS; } }
        internal Dictionary<Keys, String> DIGITStoSTRING { get { return _DIGITStoSTRING; } }
        internal Dictionary<Keys, String> DIGITStoCAPITALSTRING { get { return _DIGITStoCAPITALSTRING; } }
        #endregion

        /// <summary>
        /// Gets/Sets the Watching Flag, which enables or 
        /// disables watching keystrokes on trigger.
        /// </summary>
        internal Boolean IsWatching
        {
            get { return _isWatching; }
            set { _isWatching = value; }
        }
        /// <summary>
        /// Gets/Sets the current keyboard state
        /// </summary>
        protected KeyboardState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
        /// <summary>
        /// Gets/Sets the previous keyboard state
        /// </summary>
        protected KeyboardState PreviousState
        {
            get { return _previousState; }
            set { _previousState = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Keys[] PressedKeys
        {
            get {
                
                // If we are not in the correct context, we need to schedule updating the
                // get pressed key on the other context. We can only call void targets so
                // a private function will update the array. If we update on the wrong context
                // the array will simply be empty.

                if (Thread.CurrentContext != _executingThread)
                    _executingThread.DoCallBack(new CrossContextDelegate(UpdatePressedKeys));
                else
                    UpdatePressedKeys();

                return _pressedKeys; }
            set {
                _pressedKeys = value;
            }
        }

        private Keys[] _pressedKeys;
        private void UpdatePressedKeys()
        {
            PressedKeys = _currentState.GetPressedKeys();
        }

        /// <summary>
        /// KeyboardComponent Constructor
        /// </summary>
        /// <param name="game">Game to bind to</param>
        internal KeyboardInputComponent(Game game)
            : base(game)
        {
            // Key Mapping
            _DIGITStoSTRING.Add(Keys.D0, "0"); _DIGITStoCAPITALSTRING.Add(Keys.D1, "!");
            _DIGITStoSTRING.Add(Keys.D1, "1"); _DIGITStoCAPITALSTRING.Add(Keys.D2, "@");
            _DIGITStoSTRING.Add(Keys.D2, "2"); _DIGITStoCAPITALSTRING.Add(Keys.D3, "#");
            _DIGITStoSTRING.Add(Keys.D3, "3"); _DIGITStoCAPITALSTRING.Add(Keys.D4, "$");
            _DIGITStoSTRING.Add(Keys.D4, "4"); _DIGITStoCAPITALSTRING.Add(Keys.D5, "%");
            _DIGITStoSTRING.Add(Keys.D5, "5"); _DIGITStoCAPITALSTRING.Add(Keys.D6, "^");
            _DIGITStoSTRING.Add(Keys.D6, "6"); _DIGITStoCAPITALSTRING.Add(Keys.D7, "&");
            _DIGITStoSTRING.Add(Keys.D7, "7"); _DIGITStoCAPITALSTRING.Add(Keys.D8, "*");
            _DIGITStoSTRING.Add(Keys.D8, "8"); _DIGITStoCAPITALSTRING.Add(Keys.D9, "(");
            _DIGITStoSTRING.Add(Keys.D9, "9"); _DIGITStoCAPITALSTRING.Add(Keys.D0, ")");

            // Fill the states
            this.PreviousState = Keyboard.GetState();
            this.CurrentState = Keyboard.GetState();

            // Enable watching
            this.IsWatching = true;
            
            // Holds the data of the watched keys
            _triggers = new Dictionary<Keys, TriggerKey>();
            foreach (Keys key in this.DEFAULTWATCHING)
                Watch(key);

            _executingThread = Thread.CurrentContext;
        }

        private Context _executingThread;

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            // Move the old new state to the old slot
            // Set the new new state to the new slot
            this.PreviousState = this.CurrentState;
            this.CurrentState = Keyboard.GetState();

            // If Watching is Enabled
            if (IsWatching)
            {
                // Count Optimisation
                Int32 count = _triggers.Keys.Count;

                // Iterate and Update
                foreach (Keys key in _triggers.Keys)
                {
                    // Start Exception block
                    try
                    {
                        // Fetch the object
                        TriggerKey curobj;
                        _triggers.TryGetValue(key, out curobj);
                        // Update using GameTime
                        curobj.Update(gameTime, CurrentState.IsKeyDown(key));

                        if (CurrentState.IsKeyDown(key))
                        {

                        }
                    }
                    // Catch the Exception
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("Exception on fetching value from dictionary", "Error");
                    }
                }
            }
            // Update the Component
            base.Update(gameTime);
        }

        /// <summary>
        /// Returns whether a specific key is currently pressed down.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True on pressed down</returns>
        public Boolean IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns whether a specific key is currently released up.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True on released up</returns>
        public Boolean IsKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns whether a specific key was just pressed down
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if newly pressed</returns>
        public Boolean IsKeyPressed(Keys key)
        {
            return IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns whether a specific key is just released
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if newly released</returns>
        public Boolean IsKeyReleased(Keys key)
        {
            return IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns whether a specific key is triggerd.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if triggerd</returns>
        public Boolean IsKeyTriggerd(Keys key)
        {
            TriggerKey curobj;
            if (_triggers.TryGetValue(key, out curobj))
                return curobj.IsTriggerd;
            else
            {
                Watch(key);
                return IsKeyTriggerd(key);
            }
        }

        /// <summary>
        /// Start watching a specific key.
        /// </summary>
        /// <param name="key">Key to Watch</param>
        public void Watch(Keys key)
        {
            if (_triggers.ContainsKey(key) == false)
                _triggers.Add(key, new TriggerKey());
        }

        /// <summary>
        /// Stop Watching a specific key.
        /// </summary>
        /// <param name="key">Key not to Watch</param>
        public void Unwatch(Keys key)
        {
            if (_triggers.ContainsKey(key) == true)
                _triggers.Remove(key);
        }

        /// <summary>
        /// Start Watching all alphanumeric keys
        /// </summary>
        public void WatchAlphaNumeric()
        {
            // Add Backspace
            Watch(Keys.Back);

            // Watch all Letters
            foreach (Keys key in LETTERS)
                Watch(key);

            // Watch all Digits
            foreach (Keys key in DIGITS)
                Watch(key);
        }

        /// <summary>
        /// Stop watching all alphanumeric keys
        /// </summary>
        public void UnwatchAlphaNumeric()
        {
            // UnWatch all Letters
            foreach (Keys key in LETTERS)
            {
                // If this is not a default key
                Boolean isDefault = false;
                foreach (Keys dkey in DEFAULTWATCHING)
                    if (dkey == key)
                    {
                        isDefault = true;
                        break;
                    }

                // If not default
                if (isDefault == false)
                    // Unwatch the key
                    Unwatch(key);
            }

            // Unwatch all Digits
            foreach (Keys key in DIGITS)
            {
                // If this is not a default key
                Boolean isDefault = false;
                foreach (Keys dkey in DEFAULTWATCHING)
                    if (dkey == key)
                    {
                        isDefault = true;
                        break;
                    }

                // If not default
                if (isDefault == false)
                    // Unwatch the key
                    Unwatch(key);
            }

            // If Backkey is included in defaults
            foreach (Keys dkey in DEFAULTWATCHING)
                if (dkey == Keys.Back)
                    // Stop method
                    return;

            // Or unwatch key and then stop method
            Unwatch(Keys.Back);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class GamepadInputComponent : GameComponent
    {
        protected Dictionary<PlayerIndex, GamePadState> _currentState, _previousState;
        protected Dictionary<PlayerIndex, Boolean> _connected;
        private Object _connectLock;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connected"></param>
        public delegate void ConnectDelegate(PlayerIndex? connected);

        /// <summary>
        /// 
        /// </summary>
        public event ConnectDelegate OnDisconnect = delegate { };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public GamePadState GamePadPlayerCurrentState(PlayerIndex player)
        {
            return _currentState[player];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public GamePadState GamePadPlayerPreviousState(PlayerIndex player)
        {
            return _previousState[player];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public GamepadInputComponent(Game game)
            : base(game)
        {
            _currentState = new Dictionary<PlayerIndex, GamePadState>();
            _previousState = new Dictionary<PlayerIndex, GamePadState>();
            _connected = new Dictionary<PlayerIndex, Boolean>();
            _connectLock = new Object();

            // Fill the states
            foreach (var playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
            {
                _currentState.Add(playerIndex, GamePad.GetState(playerIndex));
                _previousState.Add(playerIndex, GamePad.GetState(playerIndex));
                _connected.Add(playerIndex, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var playerIndex in _connected.Keys)
            {
                if (_connected[playerIndex])
                {
                    _previousState[playerIndex] = _currentState[playerIndex];
                    _currentState[playerIndex] = GamePad.GetState(playerIndex);

                    if (!_currentState[playerIndex].IsConnected)
                        Disconnect(playerIndex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public Task<PlayerIndex?> Connect(ConnectDelegate onConnect)
        {
            return Task<PlayerIndex?>.Factory.StartNew(WaitForConnect)
                .ContinueWith(
                    (r) =>
                    {
                        onConnect.Invoke(r.Result);
                        return r.Result;
                    }
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerIndex"></param>
        public void Disconnect(PlayerIndex playerIndex)
        {
            Task.Factory.StartNew(() => WaitForDisconnect(playerIndex));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PlayerIndex? WaitForConnect()
        {
            DateTime time = DateTime.Now;

            while (true)
            {
                if (Monitor.TryEnter(_connectLock, 100))
                {
                    try
                    {
                        foreach (var playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
                        {
                            var state = GamePad.GetState(playerIndex);
                            if (state.IsConnected && !_connected[playerIndex])
                            {
                                if (GamePad.GetCapabilities(playerIndex).GamePadType != GamePadType.GamePad)
                                    continue;

                                if (state.IsButtonDown(Buttons.Start))
                                {
                                    _connected[playerIndex] = true;
                                    return playerIndex;
                                }
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(_connectLock);
                    }
                }

                if (DateTime.Now - time > TimeSpan.FromSeconds(10))
                    break;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerIndex"></param>
        public void WaitForDisconnect(PlayerIndex playerIndex)
        {
            while (true)
            {
                if (Monitor.TryEnter(_connectLock, 100))
                {
                    try
                    {
                        var state = GamePad.GetState(playerIndex);
                        _connected[playerIndex] = state.IsConnected;
                        if (!_connected[playerIndex])
                            OnDisconnect.Invoke(playerIndex);

                        return;
                    }
                    finally
                    {
                        Monitor.Exit(_connectLock);
                    }
                }
            }
        }


    }

    /// <summary>
    /// The Triggerkey Helper class holds all the trigger data for a specific key.
    /// </summary>
    public class TriggerKey
    {
        #region Fields
        private Double _triggerTime;
        private TriggerState _triggerState;
        #endregion

        /// <summary>
        /// Gets Triggerd State
        /// </summary>
        internal Boolean IsTriggerd
        {
            get { return _triggerState == TriggerState.Active || _triggerState == TriggerState.PressedActive; }
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        /// <param name="state">Current key state</param>
        internal void Update(GameTime gameTime, Boolean state)
        {
            // Adding to triggerTime
            _triggerTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            // React on current states
            switch (_triggerState)
            {
                // Inactive State (no candidate for triggering)
                case TriggerState.Inactive:
                    // If pressed down
                    if (state)
                    {
                        // Reset the triggertime
                        _triggerTime = 0;
                        // Set the triggerstate
                        _triggerState = TriggerState.Active;
                    }
                    break;
                // Pressed State (candidate for triggering)
                case TriggerState.Pressed:
                    // If released up
                    if (!state)
                    {
                        // This key was not triggerd
                        _triggerState = TriggerState.Inactive;
                    }
                    // If Repeatingly holding
                    else if (_triggerTime > 300)
                        // Set the triggerstate
                        _triggerState = TriggerState.PressedActive;
                    break;
                // Pressed Active State (triggerd, still pressed down)
                case TriggerState.PressedActive:
                    // Reset the count
                    _triggerTime = 0;
                    // If released
                    if (!state)
                        // Change the state
                        _triggerState = TriggerState.Inactive;
                    else
                        // Keep as a candidate
                        _triggerState = TriggerState.PressedInactive;
                    break;
                // Pressed InActive State (candidate for triggering, still pressed down)
                case TriggerState.PressedInactive:
                    // If fraction of a second passed
                    if (_triggerTime > 15)
                        // (Re)-trigger this key
                        _triggerState = TriggerState.PressedActive;
                    break;
                // Active State (triggerd)
                case TriggerState.Active:
                    if (state)
                        _triggerState = TriggerState.Pressed;
                    else
                        _triggerState = TriggerState.Inactive;
                    break;
            }
        }

        /// <summary>
        /// Triggerstate Enumeration
        /// </summary>
        private enum TriggerState : byte
        {
            Inactive = 0,
            JustPressed = 1,
            Pressed = 2,
            Active = 3,
            PressedActive = 4,
            PressedInactive = 5,
        };
    }
}
