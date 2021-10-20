using FluentResults;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;

namespace OpenNefia.Core.Map.Generator
{
    public interface IMapGenerator : IDefDeserializable
    {
        Result<InstancedMap> Generate(MapDef mapDef, InstancedArea area, int floor);
    }
}