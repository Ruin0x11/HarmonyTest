using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Effect;
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
            var coords = GraphicsEx.Coords;
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
        public static FieldLayer? Instance = null;

        public InstancedMap Map { get; private set; }

        private UiScroller Scroller;
        private Camera Camera;
        private MapRenderer MapRenderer;
        public AsyncDrawables AsyncDrawables { get; }
        private UiFpsCounter FpsCounter;

        private FontDef FontText;

        public string Message { get; private set; }
        private string MouseText;
        private TileDef? PlacingTile = null;

        public List<Thing> Things;

        internal FieldLayer()
        {
            Map = InstancedMap.Generate(MapDefOf.Vernis).Value;
            //Map = new InstancedMap(50, 50, TileDefOf.Carpet5);
            //this.InitMap();
            
            var player = new Chara(ChipDefOf.CharaChicken);
            Map.TakeObject(player, 2, 2);
            Chara.Player = player;

            Map.ClearMemory(TileDefOf.WallForestFog);
            Map.RefreshVisibility();

            Scroller = new UiScroller();
            Camera = new Camera(this.Map, this);
            Things = new List<Thing>();
            FontText = FontDefOf.WindowTitle;

            int x = 0;
            int y = 0;
            foreach (var pair in ThingRepo.Instance.Iter())
            {
                var thingData = pair.Value;
                Things.Add(new Thing(thingData, x, y));
                x += 1;
            }

            var result = PrintMessage("dood");
            Console.WriteLine($"Got back: {result}");
            Message = result;
            this.MouseText = "";

            FpsCounter = new UiFpsCounter();
            MapRenderer = new MapRenderer(this.Map);
            AsyncDrawables = new AsyncDrawables();

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.Identify] += (state) => this.QueryLayer();
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keys.Ctrl | Keys.S] += (_) => this.Save();
            this.Keybinds[Keys.Ctrl | Keys.O] += (_) => this.Load();
            this.Keybinds[Keys.Ctrl | Keys.T] += (_) => new PicViewLayer(Atlases.Tile.Image).Query();
            this.Keybinds[Keybind.Entries.North] += (_) => this.MovePlayer(0, -1);
            this.Keybinds[Keybind.Entries.South] += (_) => this.MovePlayer(0, 1);
            this.Keybinds[Keybind.Entries.West] += (_) => this.MovePlayer(-1, 0);
            this.Keybinds[Keybind.Entries.East] += (_) => this.MovePlayer(1, 0);
            this.Keybinds[Keys.G] += (_) => this.GetItem();
            this.Keybinds[Keys.D] += (_) => this.DropItem();
            this.Keybinds[Keys.C] += (_) => this.CastSpell();
            this.Keybinds[Keys.Ctrl | Keys.B] += (_) => this.ActivateBeautify();
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
            var player = Chara.Player;

            if (player != null)
            {
                player.SetPosition(player.X + dx, player.Y + dy);
                Camera.CenterOn(player);
                Map.RefreshVisibility();
            }
        }

        private void GetItem()
        {
            var player = Chara.Player;

            if (player != null)
            {
                var item = Map.At<Item>(player.X, player.Y).FirstOrDefault();

                if (item != null && player.TakeItem(item))
                {
                    Gui.PlaySound(SoundDefOf.Get1);

                    if (item.StackAll())
                    {
                        Gui.PlaySound(SoundDefOf.Heal1);
                    }
                }
            }

            var drawable = new BasicAnimAsyncDrawable(BasicAnimDefOf.AnimSmoke);
            drawable.SetPosition(Random.Rnd(Love.Graphics.GetWidth()), Random.Rnd(Love.Graphics.GetHeight()));
            AsyncDrawables.Enqueue(drawable);
        }

        private void DropItem()
        {
            var player = Chara.Player;

            if (player != null)
            {
                var item = player.Inventory.EnumerateType<Item>().FirstOrDefault();

                if (item != null && player.DropItem(item))
                {
                    Gui.PlaySound(SoundDefOf.Drop1);

                    if (item.StackAll())
                    {
                        Gui.PlaySound(SoundDefOf.AtkChaos);
                    }
                }
            }
        }

        private void CastSpell()
        {
            var prompt = new Prompt<SpellDef>(DefStore<SpellDef>.Enumerate());
            var result = prompt.Query();
            if (result.HasValue)
            {
                Spell.CastSpell(result.Value.ChoiceData, Chara.Player!);
            }
        }

        bool IsBeautify = false;

        private void ActivateBeautify()
        {
            if (IsBeautify)
                return;

            Console.WriteLine($"Applying beautify!");

            DefLoader.ApplyActiveThemes(new List<ThemeDef>() { ThemeDefOf.Beautify });
            Startup.RegenerateTileAtlases();
            MapRenderer.OnThemeSwitched();
            Map.Redraw();
            IsBeautify = true;
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

        public void Save()
        {
            Console.WriteLine("Saving...");
            InstancedMap.Save(Map, "TestMap.nbt");
        }

        public void Load()
        {
            Console.WriteLine("Loading...");
            Map = InstancedMap.Load("TestMap.nbt", Current.Game);
            MapRenderer.SetMap(Map);
            Camera.CenterOn(Chara.Player!);
            Map.RefreshVisibility();
        }

        public override void GetPreferredBounds(out int x, out int y, out int width, out int height)
        {
            x = 0;
            y = 0;
            width = Love.Graphics.GetWidth();
            height = Love.Graphics.GetHeight();
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            base.SetSize(width, height);
            MapRenderer.SetSize(width, height);
            FpsCounter.SetSize(400, 500);

            var player = Chara.Player;
            if (player != null)
            {
                Camera.CenterOn(player);
            }
        }

        public override void SetPosition(int x = 0, int y = 0)
        {
            base.SetPosition(x, y);
            MapRenderer.SetPosition(x, y);
            FpsCounter.SetPosition(Width - FpsCounter.Text.Width - 5, 5);
        }

        public override void OnQuery()
        {
            // Gui.PlayMusic(MusicDefOf.Field1);
        }

        private void QueryLayer()
        {
            using (var layer = new TestLayer())
            {
                Console.WriteLine("Query layer!");
                var result = layer.Query();
                Console.WriteLine($"Get result: {result.Value}");
            }
        }

        public override void Update(float dt)
        {
            if (this.Map._NeedsRedraw)
            {
                Map.RefreshVisibility();
                this.MapRenderer.RefreshAllLayers();
            }

            this.Scroller.GetPositionDiff(dt, out var dx, out var dy);

            this.SetPosition(Camera.ScreenX, Camera.ScreenY);

            if (PlacingTile != null)
            {
                var mouse = Love.Mouse.GetPosition();
                var coords = GraphicsEx.Coords;
                coords.ScreenToTile((int)mouse.X - this.X, (int)mouse.Y - this.Y, out var tileX, out var tileY);

                if (Map.GetTile(tileX, tileY) != PlacingTile)
                {
                    if (PlacingTile.IsSolid)
                    {
                        Gui.PlaySound(SoundDefOf.Offer1);
                    }
                    Map.SetTile(tileX, tileY, PlacingTile);
                    Map.MemorizeTile(tileX, tileY);
                }
            }

            this.MapRenderer.Update(dt);
            this.AsyncDrawables.Update(dt);
            this.FpsCounter.Update(dt);
        }

        public override void Draw()
        {
            Love.Graphics.SetColor(255, 255, 255);

            this.MapRenderer.Draw();

            Love.Graphics.SetColor(255, 0, 0);

            var player = Chara.Player!;
            player.GetScreenPos(out var sx, out var sy);
            GraphicsEx.LineRect(X + sx, Y + sy, OrthographicCoords.TILE_SIZE, OrthographicCoords.TILE_SIZE);

            GraphicsEx.SetFont(this.FontText);
            Love.Graphics.Print(Message, 5, 5);
            Love.Graphics.Print(MouseText, 5, 20);
            Love.Graphics.Print($"Player: ({player.X}, {player.Y})", 5, 35);

            this.AsyncDrawables.Draw();

            this.FpsCounter.Draw();
        }
    }
}
