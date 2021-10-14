using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Data.Types.DefOf;
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

        internal int[] TileInds;
        internal int[] TileMemoryInds;
        internal TileIndexMapping TileIndexMapping;

        internal Pool Pool;

        internal HashSet<int> DirtyTilesThisTurn;
        internal bool RedrawAllThisTurn;
        internal bool NeedsRedraw { get => DirtyTilesThisTurn.Count > 0 || RedrawAllThisTurn; }

        public InstancedMap() : this(1, 1, TileDefOf.MapgenDefault) { }

        public InstancedMap(int width, int height) : this(width, height, TileDefOf.MapgenDefault) { }

        public InstancedMap(int width, int height, TileDef defaultTile)
        {
            _Width = width;
            _Height = height;
            _Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            TileIndexMapping = GameWrapper.Instance.State.TileIndexMapping;
            TileInds = new int[width * height];
            TileMemoryInds = new int[width * height];

            DirtyTilesThisTurn = new HashSet<int>();
            RedrawAllThisTurn = true;

            Clear(defaultTile);
            MemorizeAll();

            Pool = new Pool(_Uid, _Width, _Height);
            Pool.TakeObject(new CharaObject(0, 0));
            Pool.TakeObject(new CharaObject(0, 0));
            Pool.TakeObject(new CharaObject(1, 1));
            Pool.TakeObject(new CharaObject(2, 2));
        }

        public void Clear(TileDef defaultTile)
        {
            for (int i = 0; i < TileInds.Length; i++)
            {
                TileInds[i] = TileIndexMapping.TileDefIdToIndex[defaultTile.Id];
            }
        }

        public void MemorizeAll()
        {
            for (int i = 0; i < TileMemoryInds.Length; i++)
            {
                TileMemoryInds[i] = TileInds[i];
            }
            this.RedrawAllThisTurn = true;
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(Uid));
            data.ExposeValue(ref _Width, nameof(Width));
            data.ExposeValue(ref _Height, nameof(Height));

            if (data.Stage == SerialStage.LoadingDeep)
            {
                TileInds = new int[Width * Height];
                TileMemoryInds = new int[Width * Height];
            }

            data.ExposeDeep(ref Pool, nameof(Pool));

            data.ExposeCollection(ref TileInds, nameof(TileInds));
            data.ExposeCollection(ref TileMemoryInds, nameof(TileMemoryInds));
        }

        public IEnumerable<Tuple<int, int, TileDef>> Tiles
        {
            get
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    int tileIndex = TileInds[i];
                    int x = i % Width;
                    int y = i / Height;
                    TileDef tileDef = TileIndexMapping.IndexToTileDef[tileIndex];
                    yield return new Tuple<int, int, TileDef>(x, y, tileDef);
                }
            }
        }

        public IEnumerable<Tuple<int, int, TileDef>> TileMemory
        {
            get
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    int tileIndex = TileMemoryInds[i];
                    int x = i % Width;
                    int y = i / Height;
                    TileDef tileDef = TileIndexMapping.IndexToTileDef[tileIndex];
                    yield return new Tuple<int, int, TileDef>(x, y, tileDef);
                }
            }
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

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public void SetTile(int x, int y, TileDef tile)
        {
            if (!IsInBounds(x, y))
                return;

            var ind = y * _Width + x;
            TileInds[ind] = TileIndexMapping.TileDefIdToIndex[tile.Id]!;
        }

        public void SetTileMemory(int x, int y, TileDef tile)
        {
            if (!IsInBounds(x, y))
                return;

            var ind = y * _Width + x;
            TileMemoryInds[ind] = TileIndexMapping.TileDefIdToIndex[tile.Id]!;
            this.DirtyTilesThisTurn.Add(ind);
        }

        public TileDef? GetTile(int x, int y)
        {
            if (!IsInBounds(x, y))
                return null;

            return TileIndexMapping.IndexToTileDef[TileInds[y * _Width + x]]!;
        }

        public TileDef? GetTileMemory(int x, int y)
        {
            if (!IsInBounds(x, y))
                return null;

            return TileIndexMapping.IndexToTileDef[TileMemoryInds[y * _Width + x]]!;
        }

        public void MemorizeTile(int x, int y)
        {
            if (!IsInBounds(x, y))
                return;

            SetTileMemory(x, y, GetTile(x, y)!);
        }

        public bool IsMemorized(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            return GetTile(x, y)! == GetTileMemory(x, y)!;
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
