using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace TimeTetris.Services
{
    public class AudioManager : GameComponent
    {
        private Dictionary<String, SoundEffectInstance> _sounds;
        private ContentManager _contentManager;
        private Queue<SoundEffectInstance> _queue;
        private SoundEffectInstance _queuePlaying;


        public delegate void SoundQueueDelegate(SoundEffectInstance sfx);

        /// <summary>
        /// 
        /// </summary>
        public event SoundQueueDelegate OnQueuedFinished = delegate { };

        /// <summary>
        /// Creates a new CollisionManager, detects collisions and moves actors around based on velocity
        /// </summary>
        /// <param name="game"></param>
        public AudioManager(Game game)
            : base(game)
        {
            // Add this as a service
            this.Game.Services.AddService(typeof(AudioManager), this);
            _contentManager = new ContentManager(game.Services);
            _contentManager.RootDirectory = "Content/Audio";
        }

        /// <summary>
        /// Initializes Manager
        /// </summary>
        public override void Initialize()
        {
            _sounds = new Dictionary<string, SoundEffectInstance>();
            _queue = new Queue<SoundEffectInstance>();
            _queuePlaying = null;

            base.Initialize();
        }

        /// <summary>
        /// Loads and Caches a sound and creates an instance
        /// </summary>
        /// <param name="soundAsset">Asset path</param>
        /// <param name="instanceName">Instance name</param>
        /// <param name="volume">Volume</param>
        /// <param name="pitch">Pitch</param>
        /// <returns></returns>
        public SoundEffectInstance Load(String soundAsset, String instanceName, Single volume = 1f, Single pitch = 0f)
        {
            SoundEffectInstance instance;
            if (!_sounds.TryGetValue(instanceName, out instance))
            {
                instance = _contentManager.Load<SoundEffect>(soundAsset).CreateInstance();
                instance.Pitch = pitch;
                instance.Volume = volume * (GameSettings.Instance.SoundVolume / 100f);
                // TODO dynamic volume
                _sounds[instanceName] = instance;
            }

            return instance;
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="instanceName">Instance Name</param>
        public void Play(String instanceName)
        {
            SoundEffectInstance instance;
            if (_sounds.TryGetValue(instanceName, out instance))
                instance.Play();
        }

        /// <summary>
        /// Stops a sound
        /// </summary>
        /// <param name="instanceName">Instance name</param>
        public void Stop(String instanceName)
        {
            SoundEffectInstance instance;
            if (_sounds.TryGetValue(instanceName, out instance))
                instance.Stop();
        }

        /// <summary>
        /// Unloads all resources
        /// </summary>
        public void Unload()
        {
            _contentManager.Unload();
            foreach (var sound in _sounds)
                if (!sound.Value.IsDisposed)
                    sound.Value.Dispose();
            _sounds.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public void PlayLoop(String instanceName)
        {
            SoundEffectInstance instance;
            if (_sounds.TryGetValue(instanceName, out instance))
                instance.IsLooped = true;

            Play(instanceName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public void QueuePlay(String instanceName)
        {
            SoundEffectInstance instance;
            if (_sounds.TryGetValue(instanceName, out instance))
                QueuePlay(instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        public void QueuePlay(SoundEffectInstance instance)
        {
            _queue.Enqueue(instance);
        }

        /// <summary>
        /// 
        /// </summary>
        public void QueuePlayNext()
        {
            if (_queuePlaying != null)
                this.OnQueuedFinished.Invoke(_queuePlaying);

            if (_queue.Count > 0)
            {
                _queuePlaying = _queue.Dequeue();
                _queuePlaying.Play();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_queue.Count > 0 && (_queuePlaying == null || _queuePlaying.State == SoundState.Stopped))
                QueuePlayNext();
        }
    }
}
