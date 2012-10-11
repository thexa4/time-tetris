using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Data
{
    public class Timeline : GameComponent
    {
        public List<Event> Events { get; protected set; }
        public double CurrentTime { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public Timeline(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Game.Services.AddService(typeof(Timeline), this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            this.CurrentTime = 0;
            this.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            this.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Resume()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.CurrentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
