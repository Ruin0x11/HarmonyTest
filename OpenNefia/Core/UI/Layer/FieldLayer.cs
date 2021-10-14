using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Data.Types.DefOf;
using OpenNefia.Core.Map;
using OpenNefia.Core.Rendering;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNefia.Core.UI.Layer
{
    public class FieldLayer : BaseUiLayer<string>
    {
        public TileAtlas Atlas { get; private set; }
        public TileBatch Batch { get; private set; }

        public InstancedMap Map { get; private set; }

        private UiScroller Scroller;
        private MapRenderer Renderer;

        private FontAsset FontText;

        public string Message { get; private set; }
        private string MouseText;
        private bool PlacingTile = false;

        public List<Thing> Things;

        public FieldLayer()
        {
            Atlas = new TileAtlas();
            Batch = new TileBatch(Atlas);
            Map = new InstancedMap(100, 100, TileDefOf.Carpet5);
            Scroller = new UiScroller();
            Things = new List<Thing>();
            FontText = FontAsset.Entries.WindowTitle;

            Renderer = new MapRenderer(this.Map);

            int x = 0;
            int y = 0;
            foreach (var pair in ThingRepo.Instance.Iter())
            {
                var thingData = pair.Value;
                Things.Add(new Thing(thingData, x, y));
                x += 1;
            }

            MapgenUtils.SprayTile(Map, TileDefOf.Brick1, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.Carpet4, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.Cobble9, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.LightGrass1, 100);
            Map.MemorizeAll();

            var result = PrintMessage("dood");
            Console.WriteLine($"Got back: {result}");
            Message = result;
            this.MouseText = "";

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.Identify] += (state) => this.QueryLayer();
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keys.Ctrl | Keys.S] += (_) => this.SaveLoad();
            this.Keybinds[Keys.Ctrl | Keys.T] += (_) => new PicViewLayer(Atlases.Tile.Image).Query();

            this.Scroller.BindKeys(this);

            this.MouseMoved.Callback += (evt) =>
            {
                this.MouseText = $"{evt.X}, {evt.Y}";
            };

            this.MouseButtons[UI.MouseButtons.Mouse1].Bind((evt) => PlacingTile = evt.State == KeyPressState.Pressed, trackReleased: true);
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
        }

        public override void SetPosition(int x = 0, int y = 0)
        {
            base.SetPosition(x, y);
            Renderer.SetPosition(x, y);
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
            if (this.Map.NeedsRedraw)
            {
                this.Renderer.RefreshAllLayers();
            }

            this.Scroller.UpdateParentPosition(this, dt);

            if (PlacingTile)
            {
                var mouse = Love.Mouse.GetPosition();
                var coords = GraphicsEx.GetCoords();
                coords.ScreenToTile((int)mouse.X - this.X, (int)mouse.Y - this.Y, out var tileX, out var tileY);
                Map.SetTile(tileX, tileY, TileDefOf.WallBrick);
                Map.MemorizeTile(tileX, tileY);
            }

            this.Renderer.Update(dt);
        }

        public override void Draw()
        {
            Love.Graphics.SetColor(255, 255, 255);

            this.Renderer.Draw();

            GraphicsEx.SetFont(this.FontText);
            Love.Graphics.Print(Message, 5, 5);
            Love.Graphics.Print(MouseText, 5, 20);
        }
    }
}
