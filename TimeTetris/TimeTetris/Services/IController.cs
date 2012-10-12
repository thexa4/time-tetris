using Microsoft.Xna.Framework;
using TimeTetris.Services;

namespace TimeTetris.Services
{
    public interface IController
    {
        ControllerAction Action { get; }

        void Initialize();
        void Update(GameTime gameTime);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAIController : IController
    {
        //Level.Level Level { set; }
    }
}
