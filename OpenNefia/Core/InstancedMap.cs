using OpenNefia.Game.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia
{
    public class InstancedMap : IDataExposable
    {
        int Width;
        int Height;
        List<int> tiles;

        public InstancedMap() : this(1, 1) { }

        public InstancedMap(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = new List<int>(Width * Height);
            for (int i = 0; i < Width * Height; i++)
            {
                tiles.Add(i % 5);
            }
        }

        public class InnerObject : IDataExposable, IDataReferenceable
        {
            public string Name;
            public InnerObject? Ref1;
            public InnerObject? Ref2;

            public InnerObject() : this(string.Empty) { }

            public InnerObject(string name)
            {
                Name = name;
            }

            public void Expose(DataExposer data)
            {
                data.ExposeValue(ref Name, "Name");
                data.ExposeWeak(ref Ref1, "Ref1");
                data.ExposeWeak(ref Ref2, "Ref2");
            }

            public string GetUniqueIndex() => Name;

            public override string ToString() => $"obj {Name}";
        }

        public InnerObject? obj1;
        public InnerObject? obj2;
        public InnerObject? obj3;

        public void Expose(DataExposer data)
        {
            int a = 5;
            int b = 6;
            int c = 7;
            data.ExposeValue(ref a, "A");
            data.ExposeValue(ref b, "B");
            data.ExposeValue(ref c, "C");
            Console.WriteLine($"{data.Stage}! {a}, {b}, {c}");

            data.ExposeDeep(ref obj1, "obj1");
            data.ExposeDeep(ref obj2, "obj2");
            data.ExposeDeep(ref obj3, "obj3");
            Console.WriteLine($"{data.Stage}! {obj1}, {obj2}, {obj3}");
            Console.WriteLine($"{obj1?.Ref1}, {obj1?.Ref2}");
            Console.WriteLine($"{obj2?.Ref1}, {obj2?.Ref2}");
            Console.WriteLine($"{obj3?.Ref1}, {obj3?.Ref2}");
        }

        public static void Save(InstancedMap map, string filepath)
        {
            map.obj1 = new InnerObject("One");
            map.obj2 = new InnerObject("Two");
            map.obj3 = new InnerObject("Three");

            map.obj1.Ref1 = map.obj2;
            map.obj2.Ref1 = map.obj1;
            map.obj2.Ref2 = map.obj3;

            var exposer = new DataExposer(filepath, SerialStage.Saving);
            exposer.ExposeDeep(ref map!, "Map");
            exposer.Save();

            map.obj1 = null;
            map.obj2 = null;
            map.obj3 = null;
        }

        public static InstancedMap Load(string filepath)
        {
            var map = new InstancedMap(1, 1);
            var exposer = new DataExposer(filepath, SerialStage.LoadingDeep);
            exposer.ExposeDeep(ref map, "Map");

            exposer.Stage = SerialStage.ResolvingRefs;
            exposer.ExposeDeep(ref map, "Map");

            return map!;
        }

        public void Set(int x, int y, int tile)
        {
            tiles[y * Width + x] = tile;
        }

        public int Get(int x, int y)
        {
            return tiles[y * Width + x];
        }

        public void Draw(TileBatch batch, int sx, int sy)
        {
            batch.Clear();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int tile = Get(x, y);
                    batch.Add(tile, x * 48, y * 48);
                }
            }

            batch.Draw(sx, sy);
        }
    }
}
