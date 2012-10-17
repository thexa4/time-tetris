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
        public Stack<Event> Events { get; protected set; }

        /// <summary>
        /// Current time
        /// </summary>
        public Double CurrentTime { get; protected set; }

        /// <summary>
        /// Current Rewind time left
        /// </summary>
        public Double RewindDelta { get; protected set; }

        /// <summary>
        /// Current Rewind speed
        /// <remarks>set sets base speed</remarks>
        /// </summary>
        public Double RewindSpeed { get { return _rewindSpeed; } protected set { _rewindBaseSpeed = value; } }

        /// <summary>
        /// Currently rewinding
        /// </summary>
        public Boolean IsRewindActive { get { return RewindDelta > 0 || _rewindFrameWasActive; } }

        protected Double _rewindSpeed, _rewindBaseSpeed, _rewindTime;
        protected Boolean _rewindFrameActive, _rewindFrameWasActive;

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
        /// Initializes Timeline
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Events = new Stack<Event>();
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
        /// Rewinds for certain amount of time. 
        /// This time value is real life time, not level time.
        /// </summary>
        /// <param name="time">time to rewind</param>
        public void Rewind(Double time)
        {
            RewindReset();
            this.RewindDelta = time;
        }

        /// <summary>
        /// Rewinds for this frame
        /// </summary>
        public void RewindFrame()
        {
            _rewindFrameActive = true;
            _rewindFrameWasActive = true;
        }

        /// <summary>
        /// Resets rewind
        /// </summary>
        public void RewindReset()
        {
            _rewindSpeed = _rewindBaseSpeed;
            _rewindTime = 0;
            _rewindFrameWasActive = false;
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
            this.Events.Push(action);

            return action.Time;
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!this.Game.IsActive)
                return

            var gameTimePassed = gameTime.ElapsedGameTime.TotalSeconds;
            if (this.RewindDelta <= 0 && !_rewindFrameActive)
            {
                // Forward
                this.CurrentTime += gameTimePassed;
                RewindReset();
            }
            else
            {
                // Rewind amount
                var rewind = Math.Min(Math.Min(_rewindSpeed, gameTimePassed * _rewindSpeed), this.CurrentTime);
                
                // Correct for endlevel, because we want to slow 
                // down rewinding near the start. It should be near
                // impossible to reach the beginning and the first 
                // minute, rewinding is "expensive".
                rewind = Math.Min(rewind, Math.Max(0.01, gameTimePassed * _rewindSpeed * this.CurrentTime / 60));

                // The amount of gametime will still want to rewind.
                // This won't go under 0 if _rewindFrameActive
                this.RewindDelta = Math.Min(this.CurrentTime, Math.Max(0, this.RewindDelta - gameTimePassed));
                
                // Increase the speed, so the longer you rewind, the 
                // faster it will go.
                _rewindSpeed *= 1 + (0.1f * gameTimePassed);
                
                // Pop the events and undo them
                while (this.Events.Count > 0 && this.Events.Peek().Time >= this.CurrentTime - rewind)
                    this.Events.Pop().Undo();

                this.CurrentTime -= rewind;
                _rewindFrameActive = false;
            }                
        }
    }
}
