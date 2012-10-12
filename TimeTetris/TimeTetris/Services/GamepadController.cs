using System;
using Microsoft.Xna.Framework;

namespace TimeTetris.Services
{
    public class GamepadController : GameComponent, IController
    {
        public ControllerAction Action
        {
            get;
            protected set;
        }


        private InputManager _inputManager;
        private PlayerIndex _gamePadIndex;

        /// <summary>
        /// Creates a new GamepadPaddleController
        /// </summary>
        /// <param name="game"></param>
        /// <param name="gamePadIndex"></param>
        public GamepadController(Game game, PlayerIndex gamePadIndex)
            : base(game)
        {
            _gamePadIndex = gamePadIndex;
        }

        /// <summary>
        /// Initializes the controller
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _inputManager = (InputManager)this.Game.Services.GetService(typeof(InputManager));
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // TODO write logic
            //var stickDirection = GetThumbY();
            //this.Direction = stickDirection < 0 ? ControllerAction.Up : (stickDirection > 0 ? ControllerAction.Down : ControllerAction.None);
        }

        /// <summary>
        /// Get Thumb Y value
        /// </summary>
        /// <returns></returns>
        public Single GetThumbY()
        {
            return _inputManager.GamePad.GamePadPlayerCurrentState(_gamePadIndex).ThumbSticks.Right.Y;
        }
    }
}
