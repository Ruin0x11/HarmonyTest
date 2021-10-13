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
        int _Width;
        int _Height;
        ulong _Uid;

        public int Width { get => _Width; }
        public int Height { get => _Height; }
        public ulong Uid { get => _Uid; }

        internal List<int> Tiles;
        internal List<int> DirtyTilesThisTurn;
        TileIndexMapping TileIndexMapping;

        internal Pool Pool;

        public InstancedMap() : this(1, 1) { }

        public InstancedMap(int width, int height)
        {
            _Width = width;
            _Height = height;
            _Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            TileIndexMapping = GameWrapper.Instance.State.TileIndexMapping;
            Tiles = new List<int>();

            DirtyTilesThisTurn = new List<int>();

            for (int i = 0; i < _Width * _Height; i++)
            {
                Tiles.Add(i % 5);
            }

            Pool = new Pool(_Uid, _Width, _Height);
            Pool.TakeObject(new CharaObject(0, 0));
            Pool.TakeObject(new CharaObject(0, 0));
            Pool.TakeObject(new CharaObject(1, 1));
            Pool.TakeObject(new CharaObject(2, 2));
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(Uid));
            data.ExposeValue(ref _Width, nameof(Width));
            data.ExposeValue(ref _Height, nameof(Height));
            data.ExposeDeep(ref Pool, nameof(Pool));

            data.ExposeCollection(ref Tiles, nameof(Tiles));
        }

        public static void Save(InstancedMap map, string filepath)
        {
            var exposer = new DataExposer(filepath, SerialStage.Saving);
            exposer.ExposeDeep(ref map!, "Map");
            exposer.Save();
        }

        public static InstancedMap Load(string filepath, GameState state)
        {
            var map = new InstancedMap(1, 1);
            map.TileIndexMapping = state.TileIndexMapping;

            var exposer = new DataExposer(filepath, SerialStage.LoadingDeep);
            exposer.ExposeDeep(ref map, "Map");

            exposer.Stage = SerialStage.ResolvingRefs;
            exposer.ExposeDeep(ref map, "Map");

            return map!;
        }

        public void Set(int x, int y, int tile)
        {
            Tiles[y * _Width + x] = tile;
        }

        public int Get(int x, int y)
        {
            return Tiles[y * _Width + x];
        }

        public void TakeObject(MapObject obj) => Pool.TakeObject(obj);
        public bool HasObject(MapObject obj) => Pool.HasObject(obj);
        public void ReleaseObject(MapObject obj) => Pool.ReleaseObject(obj);
        public void SetPosition(MapObject mapObject, int x, int y) => Pool.SetPosition(mapObject, x, y);
        public IEnumerable<MapObject> At(int x, int y) => Pool.At(x, y);
        public IEnumerator<MapObject> GetEnumerator() => Pool.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Pool.GetEnumerator();

        public string GetUniqueIndex() => $"InstancedMap_{_Uid}";
    }
}
