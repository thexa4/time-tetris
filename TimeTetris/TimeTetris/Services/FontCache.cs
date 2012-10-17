using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TimeTetris.Services
{
    /// <summary>
    /// This Component loads and keeps track of all the fonts. It allows us to load a spritefont once instead of 
    /// many times (each screen, each instance). The global contentmanager holds the loaded fonts, so individual
    /// screen-contentmanagers can easily be disposed without the risk of triggering an ContentDisposedException.
    /// </summary>
    public class FontCache : DrawableGameComponent
    {
        #region Private fields

        private ContentManager _contentManager;
        private Dictionary<String, SpriteFont> _spritefontDictionairy;

        #endregion

        private readonly String DefaultFontAsset = @"Fonts\Default";

        /// <summary>
        /// Constructor for the FontCollector
        /// </summary>
        /// <param name="game"></param>
        public FontCache(Game game)
            : base(game)
        {
            _contentManager = game.Content;

            // Create the Dictionairy
            _spritefontDictionairy = new Dictionary<String, SpriteFont>();

            // Add this as service to the services container
            game.Services.AddService(typeof(FontCache), this);
        }

        /// <summary>
        /// Load a Font
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// </summary>
        public void LoadFont(String key, String path)
        {
            if (_spritefontDictionairy.ContainsKey(key))
                return;

            // Load Default font, and assign them
            _spritefontDictionairy[key] = _contentManager.Load<SpriteFont>(path);
        }

        /// <summary>
        /// Load All Content
        /// </summary>
        protected override void LoadContent()
        {
            // Load DefaultSprite
            _spritefontDictionairy["Default"] = _contentManager.Load<SpriteFont>(DefaultFontAsset);

            // Base Loading
            base.LoadContent();
        }

        /// <summary>
        /// Get a Spritefont
        /// </summary>
        /// <param name="fontName">key</param>
        /// <returns>Font or Default Font</returns>
        public SpriteFont this[String fontName]
        {
            get
            {
                SpriteFont font;
                if (_spritefontDictionairy.TryGetValue(fontName, out font))
                    return font;
                return _spritefontDictionairy["Default"];
            }
        }
    }
}
