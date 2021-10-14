﻿using OpenNefia.Core;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI;
using System;
using System.Linq;

namespace OpenNefia.Game
{
    internal static class Startup
    {
        internal static void Run()
        {
            GameWrapper.Instance.ModLoader.Execute();
            DefLoader.LoadAll();
            DefLoader.PopulateStaticEntries();

            Logger.Info($"[Startup] Load chip atlas.");

            var atlasFactory = new TileAtlasFactory();
            var chipAtlas = atlasFactory
                .LoadTiles(DefStore<ChipDef>.Enumerate().Select(x => x.Tile))
                .Build();

            Logger.Info($"[Startup] Load tile atlas.");

            atlasFactory = new TileAtlasFactory();
            var tileAtlas = atlasFactory
                .LoadTiles(DefStore<TileDef>.Enumerate().Select(x => x.Tile))
                .LoadTiles(DefStore<TileDef>.Enumerate().Where(x => x.Wall != null).Select(x => x.Wall!))
                .Build();

            Atlases.Chip = chipAtlas;
            Atlases.Tile = tileAtlas;

            InitTileMapping();
            
            // Lazy
            var _ = RawKey.AllKeys.Value;
        }

        private static void InitTileMapping()
        {
            Logger.Info($"[Startup] Initialize tile ID -> index mapping.");

            GameWrapper.Instance.State.TileIndexMapping.Clear();
            foreach (var tile in DefStore<TileDef>.Enumerate())
            {
                GameWrapper.Instance.State.TileIndexMapping.AddMapping(tile);
            }
        }
    }
}
