using Love;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public static class Drawing
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
                sx = height / viewport.Height;
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
            if (x == 0 && y == 0 && width == 0 && height == 0)
            {
                Love.Graphics.SetScissor();
            }
            else
            {
                Love.Graphics.SetScissor(x, y, width, height);
            }
        }
    }
}
