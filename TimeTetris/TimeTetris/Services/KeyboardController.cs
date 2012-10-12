using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TimeTetris.Services
{
    public class KeyboardController : GameComponent, IController
    {

        public ControllerAction Action
        {
            get;
            protected set;
        }

        private Keys _down, _left, _right, _drop, _rotateLeft, _rotateRight, _time, _hold;
        private InputManager _inputManager;

        /// <summary>
        /// Creates a new Paddle Controller
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="down">Soft down button</param>
        /// <param name="drop">Drop down button</param>
        /// <param name="left">Move left button</param>
        /// <param name="right">Move right button</param>
        /// <param name="rotateLeft">Rotate Left button</param>
        /// <param name="rotateRight">Rotate Right button</param>
        /// <param name="time">Rewind time button</param>
        public KeyboardController(Game game, Keys down, Keys left, Keys right, Keys drop, Keys rotateLeft, Keys rotateRight, Keys time, Keys Hold) : base(game)
        {
            _drop = drop;
            _down = down;
            _left = left;
            _right = right;
            _rotateLeft = rotateLeft;
            _rotateRight = rotateRight;
            _time = time;
            _hold = Hold;
        }

        /// <summary>
        /// Initializes controller
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _inputManager = (InputManager)this.Game.Services.GetService(typeof(InputManager));
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Action = ControllerAction.None;

            if (!this.Enabled)
                return;

            if (_inputManager.Keyboard.IsKeyTriggerd(_left))
                Action = ControllerAction.Left;
            else if (_inputManager.Keyboard.IsKeyTriggerd(_right))
                Action = ControllerAction.Right;
            else if (_inputManager.Keyboard.IsKeyTriggerd(_down))
                Action = ControllerAction.Down;
            else if (_inputManager.Keyboard.IsKeyTriggerd(_drop))
                Action = ControllerAction.Drop;
            else if (_inputManager.Keyboard.IsKeyTriggerd(_rotateLeft))
                Action = ControllerAction.RotateCCW;
            else if (_inputManager.Keyboard.IsKeyTriggerd(_rotateRight))
                Action = ControllerAction.RotateCW;
            else if (_inputManager.Keyboard.IsKeyDown(_time))
                Action = ControllerAction.Time;
            else if (_inputManager.Keyboard.IsKeyPressed(_hold))
                Action = ControllerAction.Hold;
        }
    }
}
