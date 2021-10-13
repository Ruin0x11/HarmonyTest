using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Game;
using OpenNefia.Game.Serial;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    public class InstancedMap : IDataExposable, ILocation
    {
        int Width;
        int Height;
        ulong Uid;
        List<int> Tiles;

        Love.Image chip;

        internal Pool Pool;

        public InstancedMap() : this(1, 1) { }

        public InstancedMap(int width, int height)
        {
            Width = width;
            Height = height;
            Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            Tiles = new List<int>();
            for (int i = 0; i < Width * Height; i++)
            {
                Tiles.Add(i % 5);
            }

            Pool = new Pool(Uid, Width, Height);
            Pool.TakeObject(new CharaObject(0, 0));
            Pool.TakeObject(new CharaObject(0, 0));
            Pool.TakeObject(new CharaObject(1, 1));
            Pool.TakeObject(new CharaObject(2, 2));
            chip = ImageLoader.NewImage("Assets/Graphic/chara_1.bmp");
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref Uid, nameof(Uid));
            data.ExposeValue(ref Width, nameof(Width));
            data.ExposeValue(ref Height, nameof(Height));
            data.ExposeDeep(ref Pool, nameof(Pool));

            data.ExposeCollection(ref Tiles, nameof(Tiles));
            Console.WriteLine($"{Tiles.Count}");
        }

        public static void Save(InstancedMap map, string filepath)
        {
            var exposer = new DataExposer(filepath, SerialStage.Saving);
            exposer.ExposeDeep(ref map!, "Map");
            exposer.Save();
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
            Tiles[y * Width + x] = tile;
        }

        public int Get(int x, int y)
        {
            return Tiles[y * Width + x];
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

            foreach (var obj in Pool)
            {
                Love.Graphics.Draw(chip, sx + obj.X * 48, sy + obj.Y * 48);
            }
        }

        public void TakeObject(MapObject obj) => Pool.TakeObject(obj);
        public bool HasObject(MapObject obj) => Pool.HasObject(obj);
        public void ReleaseObject(MapObject obj) => Pool.ReleaseObject(obj);
        public void SetPosition(MapObject mapObject, int x, int y) => Pool.SetPosition(mapObject, x, y);
        public IEnumerable<MapObject> At(int x, int y) => Pool.At(x, y);
        public IEnumerator<MapObject> GetEnumerator() => Pool.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Pool.GetEnumerator();

        public string GetUniqueIndex() => $"InstancedMap_{Uid}";
    }
}
