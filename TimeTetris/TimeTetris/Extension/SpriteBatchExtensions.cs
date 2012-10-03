using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TimeTetris.Extension
{
    public static class SpriteBatchExtensions
    {
        /// <summary>
        ///  Adds a string to a batch of sprites for rendering using the specified font,text, position, and color.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont">A font for diplaying text</param>
        /// <param name="text"> A text string</param>
        /// <param name="position"> The location (in screen coordinates) to draw the sprite</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting</param>
        /// <param name="shadow">The color of the shadow</param>
        public static void DrawShadowedString(this SpriteBatch spriteBatch, SpriteFont spriteFont, String text, Vector2 position, Color color, Color shadow)
        {
            spriteBatch.DrawString(spriteFont, text, position + Vector2.One, shadow);
            spriteBatch.DrawString(spriteFont, text, position, color);
        }


        /// <summary>
        ///  Adds a string to a batch of sprites for rendering using the specified font,text, position, and color.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont">A font for diplaying text</param>
        /// <param name="text"> A text string</param>
        /// <param name="position"> The location (in screen coordinates) to draw the sprite</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting</param>
        /// <param name="shadow">The color of the shadow</param>
        public static void DrawShadowedString(this SpriteBatch spriteBatch, SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, Color shadow)
        {
            spriteBatch.DrawString(spriteFont, text, position + Vector2.One, shadow);
            spriteBatch.DrawString(spriteFont, text, position, color);
        }

        /// <summary>
        ///  Adds a string to a batch of sprites for rendering using the specified font, text, position, 
        ///  color, rotation, origin, scale, effects and layer.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont">A font for diplaying text</param>
        /// <param name="text"> A text string</param>
        /// <param name="position"> The location (in screen coordinates) to draw the sprite</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting</param>
        /// <param name="shadow">The color of the shadow</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        /// <param name="layerDepth">The depth of a layer. By default, 0 represents the front layer and 1 represents
        ///     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        ///     drawing.</param>
        /// 
        public static void DrawShadowedString(this SpriteBatch spriteBatch, SpriteFont spriteFont, String text, Vector2 position, Color color, Color shadow,
            Single rotation, Vector2 origin, Single scale, SpriteEffects effects, Single layerDepth)
        {
            spriteBatch.DrawString(spriteFont, text, position + Vector2.One, shadow, rotation, origin,scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        ///  Adds a string to a batch of sprites for rendering using the specified font, text, position, 
        ///  color, rotation, origin, scale, effects and layer.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont">A font for diplaying text</param>
        /// <param name="text"> A text string</param>
        /// <param name="position"> The location (in screen coordinates) to draw the sprite</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting</param>
        /// <param name="shadow">The color of the shadow</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        /// <param name="layerDepth">The depth of a layer. By default, 0 represents the front layer and 1 represents
        ///     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        ///     drawing.</param>
        /// 
        public static void DrawShadowedString(this SpriteBatch spriteBatch, SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, Color shadow,
            Single rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, Single layerDepth)
        {
            spriteBatch.DrawString(spriteFont, text, position + Vector2.One, shadow, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }
        /// <summary>
        ///  Adds a string to a batch of sprites for rendering using the specified font, text, position, 
        ///  color, rotation, origin, scale, effects and layer.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont">A font for diplaying text</param>
        /// <param name="text"> A text string</param>
        /// <param name="position"> The location (in screen coordinates) to draw the sprite</param>
        /// <param name="color">The color to tint a sprite. Use Color.White for full color with no tinting</param>
        /// <param name="shadow">The color of the shadow</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        /// <param name="layerDepth">The depth of a layer. By default, 0 represents the front layer and 1 represents
        ///     a back layer. Use SpriteSortMode if you want sprites to be sorted during
        ///     drawing.</param>
        /// 
        public static void DrawShadowedString(this SpriteBatch spriteBatch, SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, Color shadow,
            Single rotation, Vector2 origin, Single scale, SpriteEffects effects, Single layerDepth)
        {
            spriteBatch.DrawString(spriteFont, text, position + Vector2.One, shadow, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
