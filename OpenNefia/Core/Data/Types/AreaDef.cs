using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Map.Generator.Area;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Types
{
    public class AreaDef : Def
    {
        public AreaDef(string id) : base(id)
        {
        }

        [DefRequired]
        public IAreaFloorGenerator FloorGenerator = null!;

        public HashSet<MapType> Types = new HashSet<MapType>();

        [DefRequired]
        public ChipDef Chip = null!;

        public AreaEntranceSpec? Entrance;
    }

    public class AreaEntranceSpec : IDefDeserializable
    {
        [DefRequired]
        public AreaDef ParentArea = null!;

        [DefRequired]
        public int ParentFloor = 0;

        [DefRequired]
        public int X = 0;

        [DefRequired]
        public int Y = 0;

        [DefRequired]
        public int StartingFloor = 0;

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }
}
