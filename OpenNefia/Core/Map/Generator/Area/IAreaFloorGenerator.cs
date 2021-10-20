using FluentResults;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;

namespace OpenNefia.Core.Map.Generator.Area
{
    public interface IAreaFloorGenerator : IDefDeserializable
    {
        Result<InstancedMap> GenerateFloor(InstancedArea area, int floor);
    }
}