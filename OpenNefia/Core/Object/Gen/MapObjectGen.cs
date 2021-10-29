using FluentResults;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object.Aspect;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenNefia.Core.Object
{
    public static class MapObjectGen
    {
        private static ConstructorInfo? FindCtor(Type mapObjectType, Type concreteDefType)
        {
            // Look for a 1-argument constructor with derived MapObjectDef type.
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

        private static MapObjectAspect CreateAspectFromProps(MapObject mapObject, AspectProperties props)
        {
            var aspectClassAttrib = props.GetType().GetCustomAttribute<AspectClassAttribute>();
            if (aspectClassAttrib == null)
            {
                throw new Exception($"AspectProperties class {props.GetType()} must have an AspectClass attribute");
            }
            if (!typeof(MapObjectAspect).IsAssignableFrom(aspectClassAttrib.AspectType))
            {
                throw new Exception($"{aspectClassAttrib.AspectType} is not convertable to a MapObjectAspect");
            }

            var aspect = (MapObjectAspect)Activator.CreateInstance(aspectClassAttrib.AspectType, mapObject)!;

            return aspect;
        }

        private static void InitializeAspects(MapObject mapObject)
        {
            var aspectProps = mapObject.BaseDef.Aspects;
            var aspects = new List<MapObjectAspect>();

            foreach (var props in aspectProps)
            {
                var aspect = CreateAspectFromProps(mapObject, props);
                aspect.Initialize(props);
                aspects.Add(aspect);
            }

            mapObject._Aspects.AddRange(aspects);
        }

        public static Result<MapObject> Create(MapObjectDef def)
        {
            var realType = def.MapObjectType;

            var ctor = FindCtor(realType, def.GetType());

            if (ctor == null)
                return Result.Fail($"No constructor found for {realType} with def type {def.GetType()}");

            var mapObject = (MapObject)ctor.Invoke(new object[] { def });

            InitializeAspects(mapObject);

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

        public static Result<MapObject> Create(MapObjectDef def, TilePos pos)
        {
            var obj = Create(def);

            if (obj.IsFailed)
                return obj;

            var spawned = MapUtils.TrySpawn(obj.Value, pos);
            if (!spawned)
            {
                obj.Value.Destroy();
                return Result.Fail("Failed to spawn");
            }

            return obj;
        }
    }
}