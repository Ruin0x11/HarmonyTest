using FluentResults;
using OpenNefia.Core.Data.Types;

namespace OpenNefia.Core.Map.Generator
{
    public class EmptyMapGenerator : BaseMapGenerator
    {
        public int MapSize = 25;

        public TileDef? Tile = null;

        public override Result<InstancedMap> Generate(MapDef mapDef, InstancedArea area, int floor)
        {
            return Result.Ok(new InstancedMap(MapSize, MapSize, mapDef, Tile ?? TileDefOf.Grass));
        }
    }
}
