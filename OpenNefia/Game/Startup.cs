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

            var atlasFactory = new TileAtlasFactory();
            var chipAtlas = atlasFactory
                .LoadTiles(DefStore<ChipDef>.Enumerate().Select(x => x.Tile))
                .Build();

            atlasFactory = new TileAtlasFactory();
            var tileAtlas = atlasFactory
                .LoadTiles(DefStore<TileDef>.Enumerate().Select(x => x.Tile))
                .Build();

            Atlases.Chip = chipAtlas;
            Atlases.Tile = tileAtlas;

            InitTileMapping();
            
            // Lazy
            var _ = RawKey.AllKeys.Value;
        }

        private static void InitTileMapping()
        {
            GameWrapper.Instance.State.TileIndexMapping.Clear();
            foreach (var tile in DefStore<TileDef>.Enumerate())
            {
                GameWrapper.Instance.State.TileIndexMapping.AddMapping(tile);
            }
        }
    }
}
