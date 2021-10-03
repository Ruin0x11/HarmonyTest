using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNefia.Core.UI.Layer
{
    public class FieldLayer : BaseUiLayer<int>
    {
        public TileAtlas Atlas { get; private set; }
        public TileBatch Batch { get; private set; }

        public InstancedMap Map { get; private set; }
        public int DrawX { get; private set; }
        public int DrawY { get; private set; }

        private bool Up;
        private bool Down;
        private bool Left;
        private bool Right;
        private bool Finished;

        public string Message { get; private set; }

        public List<Thing> Things;

        public FieldLayer()
        {
            Atlas = new TileAtlas();
            Batch = new TileBatch(Atlas);
            Map = new InstancedMap(100, 100);
            DrawX = 0;
            DrawY = 0;
            Things = new List<Thing>();

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

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.UIUp].BindKey((state) => this.MoveUp(state), trackReleased: true);
            this.Keybinds[Keybind.Entries.UIDown].BindKey((state) => this.MoveDown(state), trackReleased: true);
            this.Keybinds[Keybind.Entries.UILeft].BindKey((state) => this.MoveLeft(state), trackReleased: true);
            this.Keybinds[Keybind.Entries.UIRight].BindKey((state) => this.MoveRight(state), trackReleased: true);
            this.Keybinds[Keybind.Entries.Identify] += (state) => this.QueryLayer();
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
        }

        public string PrintMessage(string dood)
        {
            Console.WriteLine($"Hi, I'm {dood}.");
            return dood + "?";
        }

        private void MoveUp(KeyInputEvent evt)
        {
            this.Up = (evt.State != KeyPressState.Released);
        }

        private void MoveDown(KeyInputEvent evt)
        {
            this.Down = (evt.State != KeyPressState.Released);
        }

        private void MoveLeft(KeyInputEvent evt)
        {
            this.Left = (evt.State != KeyPressState.Released);
        }

        private void MoveRight(KeyInputEvent evt)
        {
            this.Right = (evt.State != KeyPressState.Released) ;
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
            var dx = 0;
            var dy = 0;

            if (this.Up) dy += 1;
            if (this.Down) dy -= 1;
            if (this.Left) dx += 1;
            if (this.Right) dx -= 1;

            var delta = 1000f;
            var amount = (int)(dt * delta);
            DrawX += amount * dx;
            DrawY += amount * dy;
        }

        public override UiResult<int>? GetResult()
        {
            if (this.Finished)
            {
                this.Finished = false;
                return UiResult<int>.Finished(42);
            }

            return null;
        }

        public override void Draw()
        {
            Love.Graphics.SetColor(255, 255, 255);

            Map.Draw(Batch, DrawX, DrawY);

            foreach (var thing in Things)
            {
                Love.Graphics.Draw(thing.Texture, thing.PosX * 48 + DrawX, thing.PosY * 48 + DrawY);
            }

            Love.Graphics.Print(Message, 5, 5);
        }
    }
}
