using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Data.Types.DefOf;
using OpenNefia.Core.Map;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Element;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNefia.Core.UI.Layer
{
    public class FieldLayer : BaseUiLayer<string>
    {
        public InstancedMap Map { get; private set; }

        private UiScroller Scroller;
        private MapRenderer Renderer;
        private UiFpsCounter FpsCounter;

        private FontAsset FontText;

        public string Message { get; private set; }
        private string MouseText;
        private TileDef? PlacingTile = null;

        public List<Thing> Things;

        private Love.Image Chip;

        public FieldLayer()
        {
            Map = new InstancedMap(500, 500, TileDefOf.Carpet5);
            Scroller = new UiScroller();
            Things = new List<Thing>();
            FontText = FontAsset.Entries.WindowTitle;

            int x = 0;
            int y = 0;
            foreach (var pair in ThingRepo.Instance.Iter())
            {
                var thingData = pair.Value;
                Things.Add(new Thing(thingData, x, y));
                x += 1;
            }

            var player = new CharaObject(2, 2);
            Map.TakeObject(player);
            GameWrapper.Instance.State.Player = player;

            MapgenUtils.SprayTile(Map, TileDefOf.Brick1, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.Carpet4, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.Cobble9, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.LightGrass1, 100);
            Map.MemorizeAll();

            var result = PrintMessage("dood");
            Console.WriteLine($"Got back: {result}");
            Message = result;
            this.MouseText = "";

            Chip = ImageLoader.NewImage("Assets/Graphic/chara_1.bmp");

            FpsCounter = new UiFpsCounter();
            Renderer = new MapRenderer(this.Map);

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.Identify] += (state) => this.QueryLayer();
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keys.Ctrl | Keys.S] += (_) => this.SaveLoad();
            this.Keybinds[Keys.Ctrl | Keys.T] += (_) => new PicViewLayer(Atlases.Tile.Image).Query();
            this.Keybinds[Keys.W] += (_) => this.MovePlayer(0, -1);
            this.Keybinds[Keys.A] += (_) => this.MovePlayer(-1, 0);
            this.Keybinds[Keys.S] += (_) => this.MovePlayer(0, 1);
            this.Keybinds[Keys.D] += (_) => this.MovePlayer(1, 0);

            this.Scroller.BindKeys(this);

            this.MouseMoved.Callback += (evt) =>
            {
                this.MouseText = $"{evt.X}, {evt.Y}";
            };

            this.MouseButtons[UI.MouseButtons.Mouse1].Bind((evt) => PlaceTile(evt), trackReleased: true);
            this.MouseButtons[UI.MouseButtons.Mouse2].Bind((evt) => PlaceTile(evt), trackReleased: true);
            this.MouseButtons[UI.MouseButtons.Mouse3].Bind((evt) => PlaceTile(evt), trackReleased: true);
        }

        private void MovePlayer(int dx, int dy)
        {
            var player = GameWrapper.Instance.State.Player;

            player?.SetPosition(player.X + dx, player.Y + dy);
            Map.RefreshVisibility();
        }

        private void PlaceTile(MouseButtonEvent evt)
        {
            if (evt.State == KeyPressState.Pressed)
            {
                if (evt.Button == UI.MouseButtons.Mouse1)
                {
                    PlacingTile = TileDefOf.Dirt;
                }
                else if (evt.Button == UI.MouseButtons.Mouse2)
                {
                    PlacingTile = TileDefOf.WallBrick;
                }
                else
                {
                    PlacingTile = TileDefOf.Flooring1;
                }
            }
            else
            {
                PlacingTile = null;
            }
        }

        public string PrintMessage(string dood)
        {
            Console.WriteLine($"Hi, I'm {dood}.");
            return dood + "?";
        }

        public void SaveLoad()
        {
            InstancedMap.Save(Map, "TestMap.nbt");
            Map = InstancedMap.Load("TestMap.nbt", GameWrapper.Instance.State);
            Renderer.SetMap(Map);
        }

        public override void SetDefaultSize()
        {
            this.SetSize(Love.Graphics.GetWidth(), Love.Graphics.GetHeight());
            this.SetPosition(0, 0);
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            base.SetSize(width, height);
            Renderer.SetSize(width, height);
            FpsCounter.SetSize(400, 500);
        }

        public override void SetPosition(int x = 0, int y = 0)
        {
            base.SetPosition(x, y);
            Renderer.SetPosition(x, y);
            FpsCounter.SetPosition(Width - FpsCounter.Text.Width - 5, 5);
        }

        public override void OnQuery()
        {
            // Gui.PlayMusic(MusicDefOf.Field1);
        }

        private KeyActionResult? QueryLayer()
        {
            Console.WriteLine("Query layer!");
            var result = new TestLayer().Query();
            Console.WriteLine($"Get result: {result.Result}");

            return null;
        }

        public override void Update(float dt)
        {
            if (this.Map._NeedsRedraw)
            {
                Map.RefreshVisibility();
                this.Renderer.RefreshAllLayers();
            }

            this.Scroller.UpdateParentPosition(this, dt);

            if (PlacingTile != null)
            {
                var mouse = Love.Mouse.GetPosition();
                var coords = GraphicsEx.GetCoords();
                coords.ScreenToTile((int)mouse.X - this.X, (int)mouse.Y - this.Y, out var tileX, out var tileY);
                Map.SetTile(tileX, tileY, PlacingTile);
                Map.MemorizeTile(tileX, tileY);
            }

            this.Renderer.Update(dt);
            this.FpsCounter.Update(dt);
        }

        public override void Draw()
        {
            Love.Graphics.SetColor(255, 255, 255);

            this.Renderer.Draw();

            Love.Graphics.SetColor(255, 255, 255);

            var player = GameWrapper.Instance.State.Player!;
            player.GetScreenPos(out var sx, out var sy);
            Love.Graphics.Draw(this.Chip, X + sx, Y + sy);

            GraphicsEx.SetFont(this.FontText);
            Love.Graphics.Print(Message, 5, 5);
            Love.Graphics.Print(MouseText, 5, 20);

            this.FpsCounter.Draw();
        }
    }
}
