using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Element.List;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public class TitleScreenLayer : BaseUiLayer<TitleScreenResult>, ITitleScreenLayer
    {
        private enum TitleScreenChoice
        {
            Restore,
            Generate,
            Incarnate,
            About,
            Options,
            Mods,
            Exit
        }

        private class TitleScreenCell : UiListCell<TitleScreenChoice>
        {
            private const int ITEM_HEIGHT = 35;
            
            public TitleScreenChoice Submenu;
            public IUiText UiTextSubtext;

            public TitleScreenCell(TitleScreenChoice submenu, string text, string subtext)
                : base(submenu, new UiText(FontDefOf.ListTitleScreenText, text))
            {
                this.Submenu = submenu;
                this.UiTextSubtext = new UiText(FontDefOf.ListTitleScreenSubtext, subtext);
            }

            public override void SetPosition(int x, int y)
            {
                base.SetPosition(x, y);
                if (I18N.IsFullwidth())
                {
                    this.UiTextSubtext.SetPosition(x + 40, y - 4);
                    this.UiText.SetPosition(x + 40 + this.XOffset + 4, y + 8);
                }
                else
                {
                    this.UiText.SetPosition(x + 40 + this.XOffset + 4, y + 1);
                }
            }

            public override void SetSize(int width = -1, int height = -1)
            {
                height = ITEM_HEIGHT;

                this.UiText.SetSize(width, height);
                this.UiTextSubtext.SetSize(width, height);
                base.SetSize(Math.Max(width, this.UiText.Width), height);
            }

            public override void Draw()
            {
                GraphicsEx.SetColor(Love.Color.White);
                this.AssetSelectKey.Draw(this.X, this.Y - 1);
                this.KeyNameText.Draw();
                this.UiText.Draw();
                if (I18N.IsFullwidth())
                {
                    this.UiTextSubtext.Draw();
                }
            }

            public override void Update(float dt)
            {
                this.KeyNameText.Update(dt);
                this.UiText.Update(dt);
                this.UiTextSubtext.Update(dt);
            }

            public override void Dispose()
            {
                base.Dispose();
                this.UiTextSubtext.Dispose();
            }
        }

        private FontDef FontTitleText;
        private AssetDrawable AssetTitle;
        private AssetDrawable AssetG4;

        private IUiText[] TextInfo;
        private UiWindow Window;
        private UiList<TitleScreenChoice> List;

        private string WindowTitle = "冒険の道標";

        public TitleScreenLayer()
        {
            FontTitleText = FontDefOf.TitleScreenText;
            AssetTitle = new AssetDrawable(AssetDefOf.Title);
            AssetG4 = new AssetDrawable(AssetDefOf.G4);

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
            TextInfo[2] = new UiText(FontTitleText, "OpenNefia.NET version " + Engine.Version + "  Developed by Ruin0x11");

            Window = new UiWindow(WindowTitle);

            var items = new List<TitleScreenCell>() {
                new TitleScreenCell(TitleScreenChoice.Restore, "冒険を再開する", "Restore an Adventurer"),
                new TitleScreenCell(TitleScreenChoice.Generate, "新しい冒険者を作成する", "Generate an Adventurer"),
                new TitleScreenCell(TitleScreenChoice.Incarnate, "冒険者の引継ぎ", "Incarnate an Adventurer"),
                new TitleScreenCell(TitleScreenChoice.About, "このゲームについて", "About"),
                new TitleScreenCell(TitleScreenChoice.Options, "設定の変更", "Options"),
                new TitleScreenCell(TitleScreenChoice.Mods, "MOD", "Mods"),
                new TitleScreenCell(TitleScreenChoice.Exit, "終了", "Exit"),
            };
            List = new UiList<TitleScreenChoice>(items);
            List.EventOnActivate += (_, evt) => this.RunTitleScreenAction(evt.SelectedCell.Data);
            
            this.Forwards += List;
        }

        private void RunTitleScreenAction(TitleScreenChoice selectedChoice)
        {
            if (selectedChoice != TitleScreenChoice.Generate)
            {
                Gui.PlaySound(SoundDefOf.Ok1);
            }

            switch (selectedChoice)
            {
                case TitleScreenChoice.Restore:
                    this.Finish(new TitleScreenResult(TitleScreenAction.StartGame));
                    break;
                case TitleScreenChoice.Exit:
                    this.Finish(new TitleScreenResult(TitleScreenAction.Quit));
                    break;
                default:
                    break;
            }
        }
        
        public override void GetPreferredBounds(out int x, out int y, out int width, out int height)
        {
            x = 0;
            y = 0;
            width = Love.Graphics.GetWidth();
            height = Love.Graphics.GetHeight();
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            this.Window.SetSize(320, 355);
            this.List.SetPreferredSize();
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            TextInfo[0].SetPosition(this.X + 20, this.Y + 20);
            TextInfo[1].SetPosition(this.X + 20, this.Y + 20 + (FontTitleText.GetHeight() + 5));
            TextInfo[2].SetPosition(this.X + 20, this.Y + 20 + (FontTitleText.GetHeight() + 5) * 2);
            this.Window.SetPosition(this.X + 80, (this.Height - 308) / 2);
            this.List.SetPosition(this.Window.X + 40, this.Window.Y + 48);
        }

        public override void OnQuery()
        {
            Gui.PlayMusic(MusicDefOf.Opening);
        }

        public override void Update(float dt)
        {
            foreach (var text in this.TextInfo)
                text.Update(dt);

            this.Window.Update(dt);
            this.List.Update(dt);
        }

        public override void Draw()
        {
            GraphicsEx.SetColor(Love.Color.White);
            this.AssetTitle.Draw(this.X, this.Y, this.Width, this.Height);

            foreach (var text in this.TextInfo)
                text.Draw();

            this.Window.Draw();
            this.List.Draw();

            var bgPicWidth = this.Window.Width / 5 * 4;
            var bgPicHeight = this.Window.Height - 80;
            GraphicsEx.SetColor(255, 255, 255, 50);
            this.AssetG4.Draw(this.Window.X + 160 - (bgPicWidth / 2),
                              this.Window.Y + this.Window.Height / 2 - (bgPicHeight / 2),
                              bgPicWidth,
                              bgPicHeight);
        }

        public override void Dispose()
        {
            foreach (var text in TextInfo)
            {
                text.Dispose();
            }
            Window.Dispose();
            List.Dispose();
            AssetTitle.Dispose();
            AssetG4.Dispose();
        }
    }
}
