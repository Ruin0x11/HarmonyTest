using Love;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Types;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public static class GraphicsEx
    {
        public static void DrawImage(Love.Image image, float x = 0, float y = 0, float width = 0, float height = 0, bool centered = false, float rotation = 0)
        {
            var sx = 1f;
            var sy = 1f;

            if (width > 0)
            {
                sx = width / image.GetWidth();
            }
            if (height > 0)
            {
                sx = height / image.GetHeight();
            }

            var ox = 0f;
            var oy = 0f;

            if (centered)
            {
                ox = image.GetWidth() / 2f;
                oy = image.GetHeight() / 2f;
            }

            Love.Graphics.Draw(image, x, y, rotation, sx, sy, ox, oy);
        }

        public static void DrawSpriteBatch(Love.SpriteBatch batch, float x = 0, float y = 0, float width = 0, float height = 0, float rotation = 0)
        {
            // Sprite batches will ignore the width and height of
            // love.graphics.draw; we have to manually set the scissor.
            var scissor = Love.Graphics.GetScissor();
            SetScissor((int)x, (int)y, (int)width, (int)height);

            Love.Graphics.Draw(batch, x, y, rotation);

            SetScissor(scissor);
        }

        public static void DrawImageRegion(Love.Image image, Love.Quad quad, float x = 0, float y = 0, float width = 0, float height = 0, bool centered = false, float rotation = 0)
        {
            var viewport = quad.GetViewport();

            var sx = 1f;
            var sy = 1f;

            if (width > 0)
            {
                sx = width / viewport.Width;
            }
            if (height > 0)
            {
                sy = height / viewport.Height;
            }

            var ox = 0f;
            var oy = 0f;

            if (centered)
            {
                ox = viewport.Width / 2f;
                oy = viewport.Height / 2f;
            }

            Love.Graphics.Draw(quad, image, x, y, rotation, sx, sy, ox, oy);
        }

        /// <summary>
        /// Like <see cref="Love.Graphics.SetColor"/>, but uses byte values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public static void SetColor(int r, int g, int b, int a = 255)
        {
            Love.Graphics.SetColor((float)r / 255f, (float)g / 255f, (float)b / 255f, (float)a / 255f);
        }

        public static void SetColor(Love.Color color) => Love.Graphics.SetColor(color);
        public static void SetColor(ColorAsset color) => SetColor(color.R, color.G, color.B, color.A);

        public static void SetColor(object colorBackground)
        {
            throw new NotImplementedException();
        }

        public static int GetTextWidth(string text)
        {
            return Love.Graphics.GetFont().GetWidth(text);
        }

        public static int GetTextHeight()
        {
            return Love.Graphics.GetFont().GetHeight();
        }

        public enum FontStyle
        {
            None = 0x0,
            Bold = 0x1,
            Italic = 0x2,
            Underline = 0x4,
            Strikethrough = 0x8
        }
        
        private static Dictionary<int, Love.Font> FontCache = new Dictionary<int, Love.Font>();
        private static IResourcePath FONT_PATH = new ModLocalPath(typeof(CoreMod), "Assets/MS-Gothic.ttf");

        public static void SetFont(int size, FontStyle style = FontStyle.None, IResourcePath? fontFilepath = null)
        {
            Love.Graphics.SetFont(GetFont(size, style, fontFilepath));
        }

        public static void SetFont(FontAsset spec, bool noColor = false)
        {
            SetFont(spec.Size, spec.Style, spec.FontFilepath);
            if (!noColor)
                SetColor(spec.Color);
        }

        public static void DrawFilledRect(int x, int y, int width, int height)
        {
            Love.Graphics.Rectangle(Love.DrawMode.Fill, x, y, width, height);
        }

        public static void DrawLineRect(int x, int y, int width, int height)
        {
            Love.Graphics.Rectangle(Love.DrawMode.Line, x, y, width, height);
        }

        public static Love.Text NewText(string text, int size, FontStyle style = FontStyle.None, IResourcePath? fontFilepath = null)
        {
            return Love.Graphics.NewText(GetFont(size, style, fontFilepath), text);
        }

        public static Love.Text NewText(string text, FontAsset spec)
        {
            return NewText(text, spec.Size, spec.Style, spec.FontFilepath);
        }

        public static Love.Font GetFont(int size, FontStyle style = FontStyle.None, IResourcePath? fontFilepath = null)
        {
            if (FontCache.TryGetValue(size, out Love.Font? cachedFont))
            {
                return cachedFont;
            }

            var font = Love.Graphics.NewFont(FONT_PATH.Resolve());
            FontCache[size] = font;
            return font;
        }

        public static Font GetFont(FontAsset spec)
        {
            return GetFont(spec.Size, spec.Style, spec.FontFilepath);
        }

        /// <summary>
        /// BUG: <see cref="Love.Graphics.SetScissor"/> doesn't distinguish between a zero-sized Rectangle and no scissor.
        /// This function is a temporary workaround.
        /// </summary>
        /// <param name="rectangle"></param>
        public static void SetScissor(Love.Rectangle rectangle)
        {
            SetScissor(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// BUG: <see cref="Love.Graphics.SetScissor"/> doesn't distinguish between a zero-sized scissor and no scissor.
        /// This function is a temporary workaround.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetScissor(int x = 0, int y = 0, int width = 0, int height = 0)
        {
            if (width <= 0 && height <= 0)
            {
                Love.Graphics.SetScissor();
            }
            else
            {
                Love.Graphics.SetScissor(x, y, width, height);
            }
        }

        /// <summary>
        /// Draws shadowed text.
        /// 
        /// NOTE: It's highly recommended to use <see cref="UI.Element.UiShadowedText"/> instead, for performance reasons.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="fgColor"></param>
        /// <param name="bgColor"></param>
        public static void DrawTextShadowed(string text, int x, int y, Love.Color? fgColor = null, Love.Color? bgColor = null)
        {
            if (fgColor == null)
                fgColor = Love.Color.White;
            if (bgColor == null)
                bgColor = Love.Color.Black;

            GraphicsEx.SetColor(bgColor.Value);
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    Love.Graphics.Print(text, x + dx, y + dy);

            GraphicsEx.SetColor(fgColor.Value);
            Love.Graphics.Print(text, x, y);
        }
    }
}
