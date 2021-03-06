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
    internal class ListTestLayer : BaseUiLayer<string>
    {
        [Localize]
        public UiWindow Window { get; }

        public UiList<string> List1 { get; }
        public UiList<string> List2 { get; }
        public UiList<string> List3 { get; }

        private int Index;

        public ListTestLayer()
        {
            this.Window = new UiWindow();
            this.List1 = new UiList<string>(new List<string>() { "abc", "def", "ghi" });
            this.List2 = new UiList<string>(new List<string>() { "hoge", "piyo", "fuga" });
            this.List3 = new UiList<string>(new List<string>() { "あいうえお", "アイウエオ", "ってコト？！" });

            this.SelectList(this.List1);

            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.UILeft] += (_) => this.NextList(-1);
            this.Keybinds[Keybind.Entries.UIRight] += (_) => this.NextList(1);

            UiListEventHandler<string> printIt = (_, evt) =>
            {
                Sounds.PlayOneShot(SoundDefOf.Ok1);
                Console.WriteLine($"Get item: {evt.SelectedCell.Data}");
            };
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

            this.MouseMoved.Callback += (evt) =>
            {
                if (this.List1.ContainsPoint(evt.X, evt.Y) && this.Index != 1)
                {
                    Sounds.PlayOneShot(SoundDefOf.Cursor1);
                    this.Index = 1;
                    this.SelectList(this.List1);
                }
                else if (this.List2.ContainsPoint(evt.X, evt.Y) && this.Index != 2)
                {
                    Sounds.PlayOneShot(SoundDefOf.Cursor1);
                    this.Index = 2;
                    this.SelectList(this.List2);
                }
                else if (this.List3.ContainsPoint(evt.X, evt.Y) && this.Index != 3)
                {
                    Sounds.PlayOneShot(SoundDefOf.Cursor1);
                    this.Index = 3;
                    this.SelectList(this.List3);
                }

                evt.Pass();
            };

            this.Index = 1;
            this.SelectList(this.List1);
        }

        public override void OnQuery()
        {
            Sounds.PlayOneShot(SoundDefOf.Pop2);
        }

        private void NextList(int delta)
        {
            Sounds.PlayOneShot(SoundDefOf.Cursor1);

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

        public override void GetPreferredBounds(out int x, out int y, out int width, out int height)
        {
            var rect = UiUtils.GetCenteredParams(400, 170);
            x = rect.X;
            y = rect.Y;
            width = rect.Width;
            height = rect.Height;
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);

            this.Window.SetSize(this.Width, this.Height);
            var listWidth = (this.Width - 40) / 3;
            this.List1.SetSize(listWidth, this.Height - 40);
            this.List2.SetSize(listWidth, this.Height - 40);
            this.List3.SetSize(listWidth, this.Height - 40);
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);

            this.Window.SetPosition(this.X, this.Y);
            this.List1.SetPosition(this.X + 20, this.Y + 40);
            this.List2.SetPosition(this.X + 20 + (int)((this.Width - 40) * 0.33), this.Y + 40);
            this.List3.SetPosition(this.X + 20 + (int)((this.Width - 40) * 0.66), this.Y + 40);
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

        public override void Dispose()
        {
            this.Window.Dispose();
            this.List1.Dispose();
            this.List2.Dispose();
            this.List3.Dispose();
        }
    }
}
