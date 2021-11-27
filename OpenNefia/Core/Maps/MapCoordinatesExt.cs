﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNefia.Core.GameObjects;
using OpenNefia.Core.IoC;

namespace OpenNefia.Core.Maps
{
    public static class MapCoordinatesExt
    {
        public static IEnumerable<IEntity> GetEntities(this MapCoordinates coords)
        {
            return IoCManager.Resolve<IMapManager>()
                .GetMap(coords.MapId)
                .Entities
                .Where(e => e.Pos == coords.Position);
        }
    }
}