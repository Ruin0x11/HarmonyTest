using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Util;
using OpenNefia.Game;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using OpenNefia.Core.Extensions;
using OpenNefia.Serial;
using FluentResults;

namespace OpenNefia.Core
{
    public sealed class InstancedMap : IDataExposable, ILocation
    {
        internal enum TileFlags : int
        {
            None     = 0b00000000,

            IsSolid  = 0b00000001,
            IsOpaque = 0b00000010,
        }

        int _Width;
        int _Height;
        ulong _Uid;
        MapDef _Def;

        public int Width { get => _Width; }
        public int Height { get => _Height; }
        public ulong Uid { get => _Uid; }
        public MapDef Def { get => _Def; }

        internal int[] _TileInds;
        internal int[] _TileMemoryInds;
        internal TileFlags[] _TileFlags;
        internal int[] _InSight;
        internal int _LastSightId;
        internal ShadowMap _ShadowMap;
        internal MapObjectMemoryStore _MapObjectMemory;
        internal TileIndexMapping _TileIndexMapping;

        internal Pool _Pool;

        internal HashSet<int> _DirtyTilesThisTurn;
        internal bool _RedrawAllThisTurn;
        internal bool _NeedsRedraw { get => _DirtyTilesThisTurn.Count > 0 || _RedrawAllThisTurn; }

        public ILocation? ParentLocation => null;

        internal InstancedMap() : this (1, 1, MapDefOf.Default, TileDefOf.Grass) { }

        public InstancedMap(int width, int height, MapDef def, TileDef? defaultTile = null)
        {
            if (defaultTile == null)
                defaultTile = TileDefOf.Grass;

            _Width = width;
            _Height = height;
            _Uid = Current.Game.Uids.GetNextAndIncrement();
            _Def = def;
            _TileIndexMapping = Current.Game.TileIndexMapping;
            _TileInds = new int[width * height];
            _TileMemoryInds = new int[width * height];
            _TileFlags = new TileFlags[width * height];
            _InSight = new int[width * height];
            _LastSightId = 0;
            _ShadowMap = new ShadowMap(this);
            _MapObjectMemory = new MapObjectMemoryStore(this);
            _Pool = new Pool(this, _Width, _Height);

            _DirtyTilesThisTurn = new HashSet<int>();
            _RedrawAllThisTurn = true;

            Clear(defaultTile);
            ClearMemory(defaultTile);
        }

        public static Result<InstancedMap> Generate(MapDef def)
        {
            return def.Generator.Generate(def);
        }

        public void RefreshTile(int x, int y)
        {
            this._DirtyTilesThisTurn.Add(x + y * Height);
        }

        public void Clear(TileDef tile)
        {
            for (int i = 0; i < _TileInds.Length; i++)
            {
                _TileInds[i] = _TileIndexMapping.TileDefIdToIndex[tile.Id];
            }
        }

        public void ClearMemory(TileDef tile)
        {
            for (int i = 0; i < _TileInds.Length; i++)
            {
                _TileMemoryInds[i] = _TileIndexMapping.TileDefIdToIndex[tile.Id];
            }
            this._RedrawAllThisTurn = true;
        }

        public void MemorizeAll()
        {
            for (int i = 0; i < _TileMemoryInds.Length; i++)
            {
                _TileMemoryInds[i] = _TileInds[i];
                _InSight[i] = _LastSightId;
            }
            this._RedrawAllThisTurn = true;
        }
        
        public void RefreshVisibility()
        {
            _LastSightId += 1;
            this._ShadowMap.RefreshVisibility();

            _MapObjectMemory.AllMemory.Values
                .Where(memory => !IsInWindowFov(memory.TileX, memory.TileY) && !ShouldShowMemory(memory))
                .Select(memory => memory.TileX + memory.TileY * Width)
                .Distinct()
                .ForEach(index => _MapObjectMemory.ForgetObjects(index));
        }

        private bool ShouldShowMemory(MapObjectMemory memory)
        {
            // TODO
            return memory.ObjectType != typeof(Chara);
        }

        public void Redraw()
        {
            this._RedrawAllThisTurn = true;

            this.RefreshVisibility();

            this._MapObjectMemory.RedrawAll();
        }

        public IEnumerable<Tuple<int, int, TileDef>> Tiles
        {
            get
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    int tileIndex = _TileInds[i];
                    int x = i % Width;
                    int y = i / Width;
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

        public bool IsInWindowFov(int tileX, int tileY)
        {
            if (!IsInBounds(tileX, tileY))
                return false;

            return _InSight[tileY * Width + tileX] == _LastSightId;
        }

        public IEnumerable<Tuple<int, int, TileDef>> TileMemory
        {
            get
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    int tileIndex = _TileMemoryInds[i];
                    int x = i % Width;
                    int y = i / Width;
                    TileDef tileDef = _TileIndexMapping.IndexToTileDef[tileIndex];
                    yield return new Tuple<int, int, TileDef>(x, y, tileDef);
                }
            }
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(_Uid));
            data.ExposeValue(ref _Width, nameof(_Width));
            data.ExposeValue(ref _Height, nameof(_Height));
            data.ExposeDef(ref _Def!, nameof(_Def));

            if (data.Stage == SerialStage.LoadingDeep)
            {
                _TileInds = new int[Width * Height];
                _TileMemoryInds = new int[Width * Height];
            }

            data.ExposeDeep(ref _Pool, nameof(_Pool));

            data.ExposeCollection(ref _TileInds, nameof(_TileInds));
            data.ExposeCollection(ref _TileMemoryInds, nameof(_TileMemoryInds));
            data.ExposeCollection(ref _InSight, nameof(_InSight));
            data.ExposeValue(ref _LastSightId, nameof(_LastSightId));
            data.ExposeDeep(ref _MapObjectMemory, nameof(_MapObjectMemory), this);

            if (data.Stage == SerialStage.ResolvingRefs)
            {
                this._ShadowMap = new ShadowMap(this);
                foreach (var obj in this._Pool)
                {
                    if (obj.Uid == Chara.Player?.Uid)
                    {
                        Chara.Player = (Chara)obj;
                    }
                }
                this.Redraw();
            }
        }

        public static void Save(InstancedMap map, string filepath)
        {
            SerializationUtils.Serialize(filepath, map, nameof(InstancedMap));
        }

        public static InstancedMap Load(string filepath, GameState state)
        {
            return SerializationUtils.Deserialize(filepath, new InstancedMap(), nameof(InstancedMap));
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

            var ind = y * _Width + x;

            SetTileMemory(x, y, GetTile(x, y)!);
            _MapObjectMemory.RevealObjects(ind);
            _InSight[ind] = _LastSightId;
            this._DirtyTilesThisTurn.Add(ind);
        }

        public bool IsMemorized(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            return GetTile(x, y)! == GetTileMemory(x, y)!;
        }

        public bool TakeObject(MapObject obj, int x, int y) => _Pool.TakeObject(obj, x, y);
        public bool HasObject(MapObject obj) => _Pool.HasObject(obj);
        public void ReleaseObject(MapObject obj) => _Pool.ReleaseObject(obj);
        public bool CanReceiveObject(MapObject obj, int x, int y) => _Pool.CanReceiveObject(obj, x, y);
        public void SetPosition(MapObject mapObject, int x, int y) => _Pool.SetPosition(mapObject, x, y);
        public IEnumerable<MapObject> At(int x, int y) => _Pool.At(x, y);
        public IEnumerable<T> At<T>(int x, int y) where T : MapObject => _Pool.At<T>(x, y);
        public IEnumerable<T> EnumerateType<T>() where T : MapObject => _Pool.EnumerateType<T>();
        public IEnumerator<MapObject> GetEnumerator() => _Pool.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _Pool.GetEnumerator();

        public string GetUniqueIndex() => $"{nameof(InstancedMap)}_{_Uid}";
    }
}
