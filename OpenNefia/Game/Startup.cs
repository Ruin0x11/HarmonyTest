using OpenNefia.Core.Data;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .LoadTiles(DefStore<TileSpec>.Enumerate())
                .Build();
            
            // Lazy
            var _ = RawKey.AllKeys.Value;
        }
    }
}
