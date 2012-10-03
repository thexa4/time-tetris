using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeTetris.Services;

namespace TimeTetris.Drawing
{
	public class Sprite : DrawableGameComponent
	{
		/// <summary>
		/// Contentmanager
		/// </summary>
		protected ContentManager ContentManager { get; set; }

		/// <summary>
		/// Sprite Position
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// Sprite Size
		/// </summary>
		public Vector2 Size { get; set; }

		/// <summary>
		/// Scale
		/// </summary>
		public Single Scale { get; set; }

		/// <summary>
		/// Base Color
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Sprite Origin
		/// </summary>
		public Vector2 Origin { get; set; }

		/// <summary>
		/// Texture source rectangle
		/// </summary>
		public Rectangle SourceRectangle { get; set; }

		/// <summary>
		/// Rotation
		/// </summary>
		public Single Rotation { get; set; }

        public ScreenManager ScreenManager { get; set; }

        public String TextureName { get; set; }


        protected Texture2D _texture;

		/// <summary>
		/// Creates a new sprite
		/// </summary>
		/// <param name="layer"></param>
		public Sprite(Game game) : base(game)
		{
			this.Rotation = 0;
			this.Scale = 1;
			this.Position = Vector2.Zero;
			this.Color = Color.White;
			this.Origin = Vector2.Zero;
		}

		/// <summary>
		/// Initializes Sprite
		/// </summary>
		public virtual void Initialize() 
		{
			this.Enabled = true;
			this.Visible = true;

            this.ScreenManager = (ScreenManager)this.Game.Services.GetService(typeof(ScreenManager));
		}

		/// <summary>
		/// Draws Sprite
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		/// <param name="offset">Camera draw offset</param>
		public override void Draw(GameTime gameTime)
		{
            base.Draw(gameTime);

			if (this.Visible == false)
				return;

			this.ScreenManager.SpriteBatch.Draw(
				_texture, Position, 
				this.SourceRectangle, 
				this.Color, 
				this.Rotation, 
				this.Origin,
				this.Scale,
				SpriteEffects.None, 
				0);
		}

		/// <summary>
		/// Loads all content
		/// </summary>
		/// <param name="manager"></param>
		public virtual void LoadContent(ContentManager manager)
		{
            base.LoadContent();

			this.ContentManager = manager;

			if (_texture == null || _texture.IsDisposed)
			{
				if (!String.IsNullOrWhiteSpace(TextureName))
					_texture = manager.Load<Texture2D>(TextureName);

				if (_texture != null)
				{
					this.Size = new Vector2(_texture.Bounds.Width, _texture.Bounds.Height);
					this.SourceRectangle = new Rectangle(0, 0, _texture.Bounds.Width, _texture.Bounds.Height);
				}
			}
		}
		/// <summary>
		/// Calculates if a given ISprite is visible
		/// </summary>
		/// <param name="sprite">The sprite</param>
		/// <param name="offset">The camera offset</param>
		/// <returns>Wether the sprite is visible</returns>
		public static Boolean IsVisible(ScreenManager screenManager, Sprite sprite)
		{
			if (!sprite.Visible)
				return false;

			Vector2 screenPosition = sprite.Position;
			
			if (screenPosition.X < -sprite.Size.X - 100)
				return false;
			if (screenPosition.X > screenManager.ScreenWidth + sprite.Size.X + 100)
				return false;

			if (screenPosition.Y < -sprite.Size.Y - 100)
				return false;
            if (screenPosition.Y > screenManager.ScreenHeight + sprite.Size.Y + 100)
				return false;

			return true;
		}
    }
}
