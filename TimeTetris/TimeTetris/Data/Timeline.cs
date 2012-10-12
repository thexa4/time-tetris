using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeTetris.Data
{
    public class Timeline : GameComponent
    {
        /// <summary>
        /// Events that occured
        /// </summary>
        public List<Event> Events { get; protected set; }

        /// <summary>
        /// Current time
        /// </summary>
        public Double CurrentTime { get; protected set; }

        /// <summary>
        /// Creates the timeline
        /// </summary>
        /// <param name="game">Game to bind to</param>
        public Timeline(Game game)
            : base(game)
        {
            this.Events = new List<Event>();
            this.Enabled = false;
            this.UpdateOrder = 1;

            this.Game.Services.AddService(typeof(Timeline), this);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Events = new List<Event>();
        }

        /// <summary>
        /// Starts the timeline
        /// </summary>
        public void Start()
        {
            this.CurrentTime = 0;
            this.Enabled = true;
        }

        /// <summary>
        /// Stops the timeline
        /// </summary>
        public void Stop()
        {
            this.Enabled = false;
        }

        /// <summary>
        /// Resumes the timeline froma stopped state
        /// </summary>
        public void Resume()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Adds an event
        /// </summary>
        /// <param name="action">event to register</param>
        /// <returns>Event time</returns>
        public Double Add(Event action)
        {
            action.Time = this.CurrentTime;
            action.Apply();
            this.Events.Add(action);

            return action.Time;
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.CurrentTime += gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
