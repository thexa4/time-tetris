using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeTetris.Extension;

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
        /// 
        /// </summary>
        public Double RewindDelta { get; protected set; }
        public Double RewindSpeed { get; protected set; }

        /// <summary>
        /// Creates the timeline
        /// </summary>
        /// <param name="game">Game to bind to</param>
        public Timeline(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.UpdateOrder = 1;
            this.RewindSpeed = 3;

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
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void Rewind(Double time)
        {
            this.RewindDelta = time;
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

            if (RewindDelta <= 0)
            {
                this.CurrentTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                var rewind = Math.Min(Math.Min(this.RewindDelta, gameTime.ElapsedGameTime.TotalSeconds * this.RewindSpeed), this.CurrentTime);
                this.RewindDelta = Math.Min(this.CurrentTime, this.RewindDelta - rewind);

                while (this.Events.Count > 0 && this.Events.Last().Time >= this.CurrentTime - rewind)
                    this.Events.Pop<Event>().Undo();

                this.CurrentTime -= rewind;
                
            }
        }
    }
}
