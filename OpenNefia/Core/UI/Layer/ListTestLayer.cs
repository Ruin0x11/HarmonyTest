using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Element.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    internal class ListTestLayer : BaseUiLayer<int>
    {
        public UiWindow Window { get; }
        public UiList<string> List1 { get; }
        public UiList<string> List2 { get; }
        public UiList<string> List3 { get; }

        public ListTestLayer()
        {
            this.Window = new UiWindow("Test UiList Switching");
            this.List1 = new UiList<string>(new List<string>() { "abc", "def", "ghi" });
            this.List2 = new UiList<string>(new List<string>() { "abc", "def", "ghi" });
            this.List3 = new UiList<string>(new List<string>() { "abc", "def", "ghi" });
        }

        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            this.List1.Draw();
            this.List2.Draw();
            this.List3.Draw();
        }

        public override UiResult<int>? GetResult()
        {
            throw new NotImplementedException();
        }
    }
}
