using Love;
using OpenNefia.Core.Map;
using OpenNefia.Core.UI.Element;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNefia.Core.Rendering
{
    public class AsyncDrawables : BaseDrawable
    {
        private class AsyncDrawableEntry : IComparable<AsyncDrawableEntry>
        {
            public IAsyncDrawable Drawable;
            public int ZOrder;

            public AsyncDrawableEntry(IAsyncDrawable drawable, int zOrder = 0)
            {
                Drawable = drawable;
                ZOrder = zOrder;
            }

            public int CompareTo(AsyncDrawableEntry? other)
            {
                if (ZOrder == other?.ZOrder)
                {
                    return Drawable == other.Drawable ? 0 : -1;
                }
                return ZOrder.CompareTo(other?.ZOrder);
            }
        }

        private SortedSet<AsyncDrawableEntry> Active = new SortedSet<AsyncDrawableEntry>();

        public void Enqueue(IAsyncDrawable drawable, TilePos? pos, int zOrder = 0)
        {
            if (pos == null || pos.Value.Map != Current.Map)
                return;

            GraphicsEx.Coords.TileToScreen(pos.Value.X, pos.Value.Y, out var screenX, out var screenY);
            drawable.ScreenLocalX = screenX;
            drawable.ScreenLocalY = screenY;
            drawable.OnEnqueue();
            Active.Add(new AsyncDrawableEntry(drawable, zOrder));
        }

        public void Clear()
        {
            Active.Clear();
        }

        public bool HasActiveDrawables() => Active.Count > 0;

        /// <summary>
        /// Called from update code.
        /// </summary>
        public void WaitForDrawables()
        {
            while (HasActiveDrawables())
            {
                var dt = Timer.GetDelta();
                Engine.Instance.Update(dt);
                this.Update(dt);

                Engine.Instance.Draw();
                Engine.Instance.SystemStep();
            }
        }

        public override void Update(float dt)
        {
            foreach (var entry in Active)
            {
                var drawable = entry.Drawable;
                drawable.Update(dt);
                drawable.SetPosition(this.X + drawable.ScreenLocalX, this.Y + drawable.ScreenLocalY);

                if (drawable.IsFinished)
                {
                    drawable.Dispose();
                }
            }

            Active.RemoveWhere(entry => entry.Drawable.IsFinished);
        }

        public override void Draw()
        {
            foreach (var entry in Active)
            {
                if (!entry.Drawable.IsFinished)
                {
                    entry.Drawable.Draw();
                }
            }
        }
    }
}
