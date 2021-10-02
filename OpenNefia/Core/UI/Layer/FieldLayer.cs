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
        }

        public string PrintMessage(string dood)
        {
            Console.WriteLine($"Hi, I'm {dood}.");
            return dood + "?";
        }

        public override void Update(float dt)
        {
            if (!this.IsQuerying())
            {
                return;
            }

            var delta = 1000f;
            var amount = (int)(dt * delta);
            if (Love.Keyboard.IsDown(Love.KeyConstant.Up))
            {
                DrawY += amount;
            }
            if (Love.Keyboard.IsDown(Love.KeyConstant.Down))
            {
                DrawY -= amount;
            }
            if (Love.Keyboard.IsDown(Love.KeyConstant.Left))
            {
                DrawX += amount;
            }
            if (Love.Keyboard.IsDown(Love.KeyConstant.Right))
            {
                DrawX -= amount;
            }

            if (Love.Keyboard.IsPressed(Love.KeyConstant.K))
            {
                Console.WriteLine("Query layer!");
                var result = new TestLayer().Query();
                Console.WriteLine($"Get result: {result.Result}");
            }
        }

        public override UiResult<int>? GetResult()
        {
            if (Love.Keyboard.IsPressed(Love.KeyConstant.Escape))
            {
                return UiResult<int>.Finished(42);
            }

            return null;
        }

        public override void Draw()
        {
            Map.Draw(Batch, DrawX, DrawY);

            foreach (var thing in Things)
            {
                Love.Graphics.Draw(thing.Texture, thing.PosX * 48 + DrawX, thing.PosY * 48 + DrawY);
            }

            Love.Graphics.Print(Message, 5, 5);
        }
    }
}
