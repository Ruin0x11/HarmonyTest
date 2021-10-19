using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Element.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public class TitleScreenLayer : BaseUiLayer<TitleScreenResult>, ITitleScreenLayer
    {
        private class TitleScreenCell : UiListCell<TitleScreenAction>
        {
            public TitleScreenAction Action;
            public IUiText UiTextSubtext;

            public TitleScreenCell(TitleScreenAction action, string text, string subtext)
                : base(action, new UiText(FontDefOf.ListTitleScreenText, text))
            {
                this.Action = action;
                this.UiTextSubtext = new UiText(FontDefOf.ListTitleScreenSubtext, subtext);
            }

            public override void SetPosition(int x, int y)
            {
                base.SetPosition(x, y);
                if (I18N.IsFullwidth())
                {
                    this.UiTextSubtext.SetPosition(x + 40 + this.XOffset, y - 4);
                    this.UiText.SetPosition(x + 40 + this.XOffset, y + 8);
                }
                else
                {
                    this.UiText.SetPosition(x + 40 + this.XOffset, y + 1);
                }
            }

            public override void SetSize(int width = -1, int height = -1)
            {
                this.UiText.SetSize(width, height);
                this.UiTextSubtext.SetSize(width, height);
                base.SetSize(Math.Max(width, this.UiText.Width), height);
            }

            public override void Draw()
            {
                this.UiText.Draw();
                this.UiTextSubtext.Draw();
            }

            public override void Update(float dt)
            {
                this.UiText.Update(dt);
                this.UiTextSubtext.Update(dt);
            }

            public override void Dispose()
            {
                this.UiText.Dispose();
                this.UiTextSubtext.Dispose();
            }
        }

        private FontDef FontTitleText;
        private IUiText[] TextInfo;
        private UiWindow Window;
        private UiList<TitleScreenCell> List;
        private AssetDrawable AssetG4;

        private string WindowTitle = "Starting Menu";

        public TitleScreenLayer()
        {
            FontTitleText = FontDefOf.TitleScreenText;

            var version = "1.22";
            TextInfo = new IUiText[3];

            TextInfo[0] = new UiText(FontTitleText, $"Elona version {version}  Developed by Noa");
            if (I18N.Language == "ja")
            {
                TextInfo[1] = new UiText(FontTitleText, "Contributor MSL / View the credits for more");
            }
            else
            {
                TextInfo[1] = new UiText(FontTitleText, "Contributor f1r3fly, Sunstrike, Schmidt, Elvenspirit / View the credits for more");
            }
            TextInfo[2] = new UiText(FontTitleText, "OpenNefia.NET version " + Env.Version + "  Developed by Ruin0x11");

            Window = new UiWindow(WindowTitle);

            var items = new List<TitleScreenCell>() {

            };
            List = new UiList<TitleScreenCell>(items);

            AssetG4 = new AssetDrawable(AssetDefOf.G4);
        }

        public override void SetDefaultSize()
        {
            TextInfo[0].SetPosition(20, 20);
            TextInfo[1].SetPosition(20, 20 + (FontTitleText.GetHeight() + 5));
            TextInfo[2].SetPosition(20, 20 + (FontTitleText.GetHeight() + 5) * 2);
        }

        public override void OnQuery()
        {
            Gui.PlayMusic(MusicDefOf.Opening);
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
        }

        public override void Dispose()
        {
            foreach (var text in TextInfo)
            {
                text.Dispose();
            }
            Window.Dispose();
            List.Dispose();
            AssetG4.Dispose();
        }
    }
}
