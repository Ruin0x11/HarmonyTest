using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element
{
    public abstract class BaseUiElement : IUiElement
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public virtual void Relayout(int x = -1, int y = -1, int width = -1, int height = -1)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public abstract void Update(float dt);
        public abstract void Draw();
    }
}
