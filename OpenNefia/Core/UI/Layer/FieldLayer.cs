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
    internal class Camera
    {
        private InstancedMap Map;
        private IDrawable Parent;
        private int _ScreenX;
        private int _ScreenY;
        public int ScreenX { get => _ScreenX; }
        public int ScreenY { get => _ScreenY; }

        public Camera(InstancedMap map, IDrawable parent)
        {
            Map = map;
            Parent = parent;
            _ScreenX = 0;
            _ScreenY = 0;
        }

        public void CenterOn(int sx, int sy)
        {
            var coords = GraphicsEx.GetCoords();
            coords.BoundDrawPosition(sx, sy, this.Map.Width, this.Map.Height, this.Parent.Width, this.Parent.Height, out _ScreenX, out _ScreenY);
        }

        public void CenterOn(MapObject obj)
        {
            obj.GetScreenPos(out var sx, out var sy);
            CenterOn(sx, sy);
        }
    }

    public class FieldLayer : BaseUiLayer<string>
    {
        public InstancedMap Map { get; private set; }

        private UiScroller Scroller;
        private Camera Camera;
        private MapRenderer Renderer;
        private UiFpsCounter FpsCounter;

        private FontAsset FontText;

        public string Message { get; private set; }
        private string MouseText;
        private TileDef? PlacingTile = null;

        public List<Thing> Things;

        public FieldLayer()
        {
            Map = new InstancedMap(50, 50, TileDefOf.Carpet5);
            Scroller = new UiScroller();
            Camera = new Camera(this.Map, this);
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

            var player = new Chara(2, 2, ChipDefOf.CharaChicken);
            Map.TakeObject(player);
            GameWrapper.Instance.State.Player = player;

            MapgenUtils.SprayTile(Map, TileDefOf.Brick1, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.Carpet4, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.Cobble9, 100);
            MapgenUtils.SprayTile(Map, TileDefOf.LightGrass1, 100);

            for (int i = 0; i < 10; i++)
                Map.TakeObject(new Item(5 + i, 5, ChipDefOf.ItemComputer));
            for (int i = 0; i < 10; i++)
                Map.TakeObject(new Chara(5 + i, 7, ChipDefOf.CharaCat));

            Map.ClearMemory(TileDefOf.WallForestFog);
            Map.RefreshVisibility();

            var result = PrintMessage("dood");
            Console.WriteLine($"Got back: {result}");
            Message = result;
            this.MouseText = "";

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
            this.Keybinds[Keys.Period] += (_) => this.MovePlayer(0, 0);

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

            if (player != null)
            {
                player.SetPosition(player.X + dx, player.Y + dy);
                Camera.CenterOn(player);
                Map.RefreshVisibility();
            }
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

            var player = GameWrapper.Instance.State.Player;
            if (player != null)
            {
                Camera.CenterOn(player);
            }
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

            this.Scroller.GetPositionDiff(dt, out var dx, out var dy);

            this.SetPosition(Camera.ScreenX, Camera.ScreenY);

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

            Love.Graphics.SetColor(255, 0, 0);

            var player = GameWrapper.Instance.State.Player!;
            player.GetScreenPos(out var sx, out var sy);
            GraphicsEx.LineRect(X + sx, Y + sy, Constants.TILE_SIZE, Constants.TILE_SIZE);

            GraphicsEx.SetFont(this.FontText);
            Love.Graphics.Print(Message, 5, 5);
            Love.Graphics.Print(MouseText, 5, 20);
            Love.Graphics.Print($"Player: ({player.X}, {player.Y})", 5, 35);

            this.FpsCounter.Draw();
        }
    }
}
