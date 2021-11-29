﻿using OpenNefia.Core.Game;
using OpenNefia.Core.GameObjects;
using OpenNefia.Core.Maths;
using OpenNefia.Core.Prototypes;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Utility;

namespace OpenNefia.Core.Maps
{
    public enum TileFlag : int
    {
        None = 0b00000000,

        IsSolid = 0b00000001,
        IsOpaque = 0b00000010,
    }

    public sealed class Map : IMap
    {
        public MapId Id { get; set; }

        public int Width { get; }
        public int Height { get; }
        public Vector2i Size => new Vector2i(Width, Height);

        public Tile[,] Tiles { get; }
        public Tile[,] TileMemory { get; }
        public TileFlag[,] TileFlags { get; }
        internal int[,] _InSight;
        internal int _LastSightId;
        public ShadowMap ShadowMap { get; }
        public MapObjectMemoryStore MapObjectMemory { get; }

        public HashSet<MapCoordinates> DirtyTilesThisTurn { get; } = new();
        public bool RedrawAllThisTurn { get; set; }
        public bool NeedsRedraw { get => DirtyTilesThisTurn.Count > 0 || RedrawAllThisTurn; }

        private List<IEntity> _entities { get; } = new List<IEntity>();

        public IEnumerable<IEntity> Entities => _entities.ToList();

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            TileMemory = new Tile[width, height];
            TileFlags = new TileFlag[width, height];
            MapObjectMemory = new MapObjectMemoryStore(this);
            _InSight = new int[width, height];
            ShadowMap = new ShadowMap(this);
        }

        public void Clear(PrototypeId<TilePrototype> tile)
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.SetTile(new Vector2i(x, y), tile);
                }
            }
        }

        public void ClearMemory(PrototypeId<TilePrototype> tile)
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.SetTileMemory(new Vector2i(x, y), tile);
                }
            }
            this.RedrawAllThisTurn = true;
        }

        public void MemorizeAll()
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    TileMemory[x, y] = Tiles[x, y];
                    _InSight[x, y] = _LastSightId;
                }
            }
            this.RedrawAllThisTurn = true;
        }

        public void SetTile(Vector2i pos, PrototypeId<TilePrototype> tileId)
        {
            Tiles[pos.X, pos.Y] = new Tile(tileId.ResolvePrototype().TileIndex);
        }

        public void SetTileMemory(Vector2i pos, PrototypeId<TilePrototype> tileId)
        {
            TileMemory[pos.X, pos.Y] = new Tile(tileId.ResolvePrototype().TileIndex);
        }

        public IEnumerable<MapCoordinates> AllTiles
        {
            get
            {
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        yield return new MapCoordinates(this, x, y);
                    }
                }
            }
        }

        public MapCoordinates AtPos(Vector2i pos)
        {
            return new MapCoordinates(this, pos);
        }

        public MapCoordinates AtPos(int x, int y)
        {
            return new MapCoordinates(this, x, y);
        }

        public void RefreshVisibility()
        {
            _LastSightId += 1;
            this.ShadowMap.RefreshVisibility();

            MapObjectMemory.AllMemory.Values
                .Where(memory => !IsInWindowFov(memory.Coords) && !ShouldShowMemory(memory))
                .Select(memory => memory.Coords)
                .Distinct()
                .ForEach(coords => MapObjectMemory.ForgetObjects(coords.Position));
        }

        private static bool ShouldShowMemory(MapObjectMemory memory)
        {
            // TODO
            return true;
        }

        public bool IsInWindowFov(MapCoordinates coords)
        {
            if (GameSession.ActiveMap != coords.Map)
                return false;

            if (!coords.IsInBounds())
                return false;

            return _InSight[coords.X, coords.Y] == _LastSightId;
        }

        public void AddEntity(IEntity newEntity)
        {
            if (newEntity.Map != null)
            {
                throw new ArgumentException($"Entity is already in map {newEntity.Map.Id}", nameof(newEntity));
            }
            ((Entity)newEntity).ChangeMap(this);
            _entities.Add(newEntity);
        }
    }
}
