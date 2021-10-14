using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Data.Types.DefOf;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Rendering.TileDrawLayers;
using OpenNefia.Core.Util;
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

        internal int[] _TileInds;
        internal int[] _TileMemoryInds;
        internal ShadowMap _ShadowMap;
        internal MapObjectMemoryStore _MapObjectMemory;
        internal TileIndexMapping _TileIndexMapping;

        internal Pool _Pool;

        internal HashSet<int> _DirtyTilesThisTurn;
        internal bool _RedrawAllThisTurn;
        internal bool _NeedsRedraw { get => _DirtyTilesThisTurn.Count > 0 || _RedrawAllThisTurn; }

        public InstancedMap() : this(1, 1, TileDefOf.MapgenDefault) { }

        public InstancedMap(int width, int height) : this(width, height, TileDefOf.MapgenDefault) { }

        public InstancedMap(int width, int height, TileDef defaultTile)
        {
            _Width = width;
            _Height = height;
            _Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            _TileIndexMapping = GameWrapper.Instance.State.TileIndexMapping;
            _TileInds = new int[width * height];
            _TileMemoryInds = new int[width * height];
            _ShadowMap = new ShadowMap(this);
            _MapObjectMemory = new MapObjectMemoryStore(this);

            _DirtyTilesThisTurn = new HashSet<int>();
            _RedrawAllThisTurn = true;

            Clear(defaultTile);
            MemorizeAll();

            _Pool = new Pool(_Uid, _Width, _Height);
        }

        public void Clear(TileDef defaultTile)
        {
            for (int i = 0; i < _TileInds.Length; i++)
            {
                _TileInds[i] = _TileIndexMapping.TileDefIdToIndex[defaultTile.Id];
            }
        }

        public void MemorizeAll()
        {
            for (int i = 0; i < _TileMemoryInds.Length; i++)
            {
                _TileMemoryInds[i] = _TileInds[i];
                _MapObjectMemory.RevealObjects(i);
            }
            this._RedrawAllThisTurn = true;
        }
        
        public void RefreshVisibility()
        {
            this._ShadowMap.RefreshVisibility();
        }

        public void Redraw()
        {
            this._RedrawAllThisTurn = true;

            this.RefreshVisibility();

            foreach (var memory in this._MapObjectMemory)
            {
                memory.State = MemoryState.Added;
            }
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(Uid));
            data.ExposeValue(ref _Width, nameof(Width));
            data.ExposeValue(ref _Height, nameof(Height));

            if (data.Stage == SerialStage.LoadingDeep)
            {
                _TileInds = new int[Width * Height];
                _TileMemoryInds = new int[Width * Height];
            }

            data.ExposeDeep(ref _Pool, nameof(_Pool));

            data.ExposeCollection(ref _TileInds, nameof(_TileInds));
            data.ExposeCollection(ref _TileMemoryInds, nameof(_TileMemoryInds));
        }

        public IEnumerable<Tuple<int, int, TileDef>> Tiles
        {
            get
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    int tileIndex = _TileInds[i];
                    int x = i % Width;
                    int y = i / Height;
                    TileDef tileDef = _TileIndexMapping.IndexToTileDef[tileIndex];
                    yield return new Tuple<int, int, TileDef>(x, y, tileDef);
                }
            }
        }

        private bool CanSeeThrough(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            return !GetTile(x, y)!.IsOpaque;
        }

        public bool HasLos(int startX, int startY, int endX, int endY)
        {
            foreach (var (x, y) in PosUtils.EnumerateLine(startX, startY, endX, endY))
            {
                // In Elona, the final tile is visible even if it is solid.
                if (!this.CanSeeThrough(x, y) && !(x == endX && y == endY))
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<Tuple<int, int, TileDef>> TileMemory
        {
            get
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    int tileIndex = _TileMemoryInds[i];
                    int x = i % Width;
                    int y = i / Height;
                    TileDef tileDef = _TileIndexMapping.IndexToTileDef[tileIndex];
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
            map._TileIndexMapping = state.TileIndexMapping;

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
            _TileInds[ind] = _TileIndexMapping.TileDefIdToIndex[tile.Id]!;
        }

        public void SetTileMemory(int x, int y, TileDef tile)
        {
            if (!IsInBounds(x, y))
                return;

            var ind = y * _Width + x;
            _TileMemoryInds[ind] = _TileIndexMapping.TileDefIdToIndex[tile.Id]!;
            this._DirtyTilesThisTurn.Add(ind);
        }

        public TileDef? GetTile(int x, int y)
        {
            if (!IsInBounds(x, y))
                return null;

            return _TileIndexMapping.IndexToTileDef[_TileInds[y * _Width + x]]!;
        }

        public TileDef? GetTileMemory(int x, int y)
        {
            if (!IsInBounds(x, y))
                return null;

            return _TileIndexMapping.IndexToTileDef[_TileMemoryInds[y * _Width + x]]!;
        }

        public void MemorizeTile(int x, int y)
        {
            if (!IsInBounds(x, y))
                return;

            SetTileMemory(x, y, GetTile(x, y)!);
            _MapObjectMemory.RevealObjects(y * _Width + x);
        }

        public bool IsMemorized(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            return GetTile(x, y)! == GetTileMemory(x, y)!;
        }

        public void TakeObject(MapObject obj) => _Pool.TakeObject(obj);
        public bool HasObject(MapObject obj) => _Pool.HasObject(obj);
        public void ReleaseObject(MapObject obj) => _Pool.ReleaseObject(obj);
        public void SetPosition(MapObject mapObject, int x, int y) => _Pool.SetPosition(mapObject, x, y);
        public IEnumerable<MapObject> At(int x, int y) => _Pool.At(x, y);
        public IEnumerator<MapObject> GetEnumerator() => _Pool.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _Pool.GetEnumerator();

        public string GetUniqueIndex() => $"InstancedMap_{_Uid}";
    }
}
