using FluentResults;
using OpenNefia.Core.Data.Types;
using System;
using System.Reflection;

namespace OpenNefia.Core.Object
{
    public static class MapObjectGen
    {
        private static ConstructorInfo? FindCtor(Type mapObjectType, Type concreteDefType)
        {
            // Look for 1-argument constructor with derived MapObjectDef type.
            var ctorParamTypes = new Type[] { concreteDefType };

            try
            {
                return mapObjectType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, 
                    null, CallingConventions.HasThis, ctorParamTypes, null);
            }
            catch
            {
                return null;
            }
        }

        public static Result<MapObject> Create(MapObjectDef def)
        {
            var realType = def.MapObjectType;

            var ctor = FindCtor(realType, def.GetType());

            if (ctor == null)
                return Result.Fail($"No constructor found for {realType} with def type {def.GetType()}");

            var mapObject = (MapObject)ctor.Invoke(new object[] { def });

            return Result.Ok(mapObject);
        }

        public static Result<MapObject> Create(MapObjectDef def, IMapObjectHolder holder)
        {
            var obj = Create(def);

            if (obj.IsFailed)
                return obj;

            var taken = holder.InnerPool?.TakeObject(obj.Value) ?? false;
            if (!taken)
            {
                obj.Value.Destroy();
                return Result.Fail("Failed to take");
            }

            return obj;
        }

        public static Result<MapObject> Create(MapObjectDef def, InstancedMap map, int x, int y)
        {
            var obj = Create(def);

            if (obj.IsFailed)
                return obj;

            var spawned = MapUtils.TrySpawn(obj.Value, map, x, y);
            if (!spawned)
            {
                obj.Value.Destroy();
                return Result.Fail("Failed to spawn");
            }

            return obj;
        }
    }
}