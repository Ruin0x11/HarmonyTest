using OpenNefia.Core.Data.Types;
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

        private int Index;

        public ListTestLayer()
        {
            this.Window = new UiWindow("Test UiList Switching");
            this.List1 = new UiList<string>(new List<string>() { "abc", "def", "ghi" });
            this.List2 = new UiList<string>(new List<string>() { "hoge", "piyo", "fuga" });
            this.List3 = new UiList<string>(new List<string>() { "あいうえお", "アイウエオ", "ってコト？！" });

            this.SelectList(this.List1);

            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.UILeft] += (_) => this.NextList(-1);
            this.Keybinds[Keybind.Entries.UIRight] += (_) => this.NextList(1);

            UiListEventHandler<string> printIt = (_, evt) => Console.WriteLine($"Get item: {evt.SelectedChoice.Data}");
            this.List1.EventOnActivate += printIt;
            this.List2.EventOnActivate += printIt;
            this.List3.EventOnActivate += printIt;

            UiListEventHandler<string> selectIt = (_, evt) =>
            {
                this.List1.SelectedIndex = evt.SelectedIndex;
                this.List2.SelectedIndex = evt.SelectedIndex;
                this.List3.SelectedIndex = evt.SelectedIndex;
            };
            this.List1.EventOnSelect += selectIt;
            this.List2.EventOnSelect += selectIt;
            this.List3.EventOnSelect += selectIt;

            this.List2.Keybinds[Keybind.Entries.Mode] += (_) => Console.WriteLine("Dood!");

            this.Index = 1;
            this.SelectList(this.List1);
        }

        private void NextList(int delta)
        {
            this.Index += delta;
            if (this.Index > 3)
                this.Index = 1;
            else if (this.Index < 1)
                this.Index = 3;
            
            switch (this.Index)
            {
                default:
                case 1:
                    this.SelectList(this.List1);
                    break;  
                case 2:
                    this.SelectList(this.List2);
                    break;
                case 3:
                    this.SelectList(this.List3);
                    break;
            }
        }

        private void SelectList(IUiList<string> list)
        {
            this.ClearAllForwards();
            this.List1.HighlightSelected = false;
            this.List2.HighlightSelected = false;
            this.List3.HighlightSelected = false;

            list.HighlightSelected = true;
            this.Forwards += list;
        }

        public override void Relayout(int x = -1, int y = -1, int width = -1, int height = -1, RelayoutMode mode = RelayoutMode.Layout)
        {
            width = 400;
            height = 170;

            var rect = UiUtils.GetCenteredParams(width, height);
            
            base.Relayout(rect.X, rect.Y, rect.Width, rect.Height);

            this.Window.Relayout(this.X, this.Y, this.Width, this.Height);
            this.List1.Relayout(this.X + 20, this.Y + 40, this.Width - 40, this.Height - 40);
            this.List2.Relayout(this.X + 20 + (int)((this.Width - 40) * 0.33), this.Y + 40, this.Width - 40, this.Height - 40);
            this.List3.Relayout(this.X + 20 + (int)((this.Width - 40) * 0.66), this.Y + 40, this.Width - 40, this.Height - 40);
        }

        public override void Update(float dt)
        {
            this.Window.Update(dt);
            this.List1.Update(dt);
            this.List2.Update(dt);
            this.List3.Update(dt);
        }

        public override void Draw()
        {
            this.Window.Draw();
            this.List1.Draw();
            this.List2.Draw();
            this.List3.Draw();
        }
    }
}
