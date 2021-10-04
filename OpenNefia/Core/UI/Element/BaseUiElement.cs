using Love;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element
{
    public abstract class BaseUiElement : IUiElement
    {
        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;
        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;

        public void SetSizeAndPosition(Rectangle rect)
        {
            this.SetSize(rect.Width, rect.Height);
            this.SetPosition(rect.X, rect.Y);
        }

        public virtual void SetSize(int width = 0, int height = 0)
        {
            this.Width = width;
            this.Height = height;
        }

        public virtual void SetPosition(int x = 0, int y = 0)
        {
            this.X = x;
            this.Y = y;
        }

        public abstract void Update(float dt);
        public abstract void Draw();
    }
}
