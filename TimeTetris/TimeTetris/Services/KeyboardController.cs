using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TimeTetris.Services
{
    public class KeyboardController : GameComponent, IController
    {

        public ControllerAction Direction
        {
            get;
            protected set;
        }

        private Keys _up, _down, _left, _right, _drop, _rotateLeft, _rotateRight;
        private InputManager _inputManager;

        /// <summary>
        /// Creates a new Paddle Controller
        /// </summary>
        /// <param name="game"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public KeyboardController(Game game, Keys up, Keys down, Keys left, Keys right, Keys drop, Keys rotateLeft, Keys rotateRight) : base(game)
        {
            _up = up;
            _down = down;
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
        }
    }
}
