using OpenNefia.Core;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI;
using OpenNefia.Core.UI.Layer;
using OpenNefia.Core.Util;
using System;
using System.Linq;

namespace OpenNefia.Game
{
    internal static class Startup
    {
        internal static void RegenerateTileAtlases()
        {
            Logger.Info($"[Startup] Load chip atlas.");

            var atlasFactory = new TileAtlasFactory();
            var chipAtlas = atlasFactory
                .LoadTiles(DefStore<ChipDef>.Enumerate().Select(x => x.Image))
                .Build();

            Logger.Info($"[Startup] Load tile atlas.");

            atlasFactory = new TileAtlasFactory();
            var tileAtlas = atlasFactory
                .LoadTiles(DefStore<TileDef>.Enumerate().Select(x => x.Image))
                .LoadTiles(DefStore<TileDef>.Enumerate().Where(x => x.WallImage != null).Select(x => x.WallImage!))
                .Build();

            Atlases.Chip = chipAtlas;
            Atlases.Tile = tileAtlas;
        }

        private static void InitGraphicsDefaults()
        {
            Love.Graphics.SetLineStyle(Love.LineStyle.Rough);
            Love.Graphics.SetLineWidth(1);
        }

        internal static void Run()
        {
            InitGraphicsDefaults();

            Engine.ModLoader.Execute();

            if (VanillaAssetsDownloader.NeedsDownload())
            {
                new MinimalProgressBarLayer(new VanillaAssetsDownloader()).Query();
            }

            I18N.Env.LoadAll(I18N.Language);
            I18N.LocalizeStaticFields();
            DefLoader.LoadAll();

            RegenerateTileAtlases();

            InitTileMapping();
        }

        private static void InitTileMapping()
        {
            Logger.Info($"[Startup] Initialize tile ID -> index mapping.");

            Current.Game.TileIndexMapping.Clear();
            foreach (var tile in DefStore<TileDef>.Enumerate())
            {
                Current.Game.TileIndexMapping.AddMapping(tile);
            }
        }
    }
}
