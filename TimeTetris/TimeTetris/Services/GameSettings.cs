using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TimeTetris.Services
{
    public class GameSettings
    {
        // define singleton
        private static GameSettings _instance = new GameSettings();

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GameSettings Instance
        {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GameSettings()
        {
            SoundVolume = 100;
        }
    
        public  float SoundVolume { get; set; }
    }
}
