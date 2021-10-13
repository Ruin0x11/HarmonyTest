using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI;
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
            
            // Lazy
            var _ = RawKey.AllKeys.Value;
        }
    }
}
