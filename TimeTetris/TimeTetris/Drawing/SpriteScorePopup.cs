using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTetris.Extension;
using Microsoft.Xna.Framework;

namespace TimeTetris.Drawing
{
    public class SpriteScorePopup : Sprite
    {
        private int _rows;
        private int _combo;
        private bool _tspin;
        private bool _b2b;

        protected Double _timeAlive;

        /// <summary>
        /// Creates a new sprite score popup
        /// </summary>
        /// <param name="game">Game to bind to</param>
        /// <param name="rows">Number of rows removed</param>
        /// <param name="combo">Combo count</param>
        /// <param name="tspin">was T-Spin</param>
        /// <param name="b2b">was back-2-back</param>
        public SpriteScorePopup(Game game, Int32 rows, Int32 combo, Boolean tspin, Boolean b2b)
            : base(game)
        {
            _rows = rows;
            _combo = combo;
            _tspin = tspin;
            _b2b = b2b;

            this.TextureName = "Graphics/blank";
        }

        /// <summary>
        /// Indicates if the popup has popped up and is done popping
        /// </summary>
        public bool IsFinished { get; protected set; }

        /// <summary>
        /// Loads all content
        /// </summary>
        /// <param name="manager"></param>
        public override void  LoadContent(Microsoft.Xna.Framework.Content.ContentManager manager)
        {
 	        base.LoadContent(manager);

            this.ScreenManager.SpriteFonts.LoadFont("PopFont", "Fonts/Small");
            this.Size = this.ScreenManager.SpriteFonts["PopFont"].MeasureString(RowText);
        }

        /// <summary>
        /// Draws the frame
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var alpha = 1 - (Single)(Math.Pow(Math.Abs(_timeAlive - 1f), 3));
            var offset = (Single)(Math.Pow(Math.Abs(_timeAlive - 1f), 3)) * 30;
            var pos = (this.Position - this.Size / 2 + offset * Vector2.UnitY - Vector2.One * 2);

            this.ScreenManager.SpriteBatch.Draw(
                _texture,
                new Rectangle(
                    (Int32)(Math.Round(pos.X - 5)), 
                    (Int32)(Math.Round(pos.Y - 5)), 
                    (Int32)(Math.Round(this.Size.X + 15)),  
                    (Int32)(Math.Round(this.Size.Y + 10))), 
                    Color.Black * alpha * 0.5f);

            // Draw font
            this.ScreenManager.SpriteBatch.DrawShadowedString(
                this.ScreenManager.SpriteFonts["PopFont"],
                this.RowText,
                this.Position - this.Size / 2 + offset * Vector2.UnitY,
                this.Color * this.Opacity * alpha,
                Microsoft.Xna.Framework.Color.Black * this.Opacity * alpha
                );
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _timeAlive += gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_timeAlive > 5)
                this.IsFinished = true;
        }

        /// <summary>
        /// Gets the text to show
        /// </summary>
        public String RowText {
            get
            {
                var ret = "";
                if (_tspin)
                    ret += "TSpin! ";
                switch (_rows)
                {
                    case 1:
                        ret += "Single";
                        break;
                    case 2:
                        ret += "Double";
                        break;
                    case 3:
                        ret += "Triple";
                        break;
                    case 4:
                        ret += "Tetris";
                        break;
                }
                if (_combo > 0)
                {
                    if (ret != "")
                        ret += "\n";
                    ret += "Combo: " + (_combo + 1).ToString();
                }
                if (_b2b)
                {
                    if (ret != "")
                        ret += "\n";
                    ret += "Back-2-Back!";
                }
                return ret;
            }
        }
    }
}
