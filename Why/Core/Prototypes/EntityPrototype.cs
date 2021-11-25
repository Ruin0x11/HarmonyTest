using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Why.Core.GameObjects;
using Why.Core.IoC;
using Why.Core.Serialization;
using Why.Core.Serialization.Manager;
using Why.Core.Serialization.Manager.Attributes;

namespace Why.Core.Prototypes
{
    /// <summary>
    /// Prototype that represents game entities.
    /// </summary>
    [Prototype("entity", -1)]
    public class EntityPrototype : IPrototype, IInheritingPrototype, ISerializationHooks
    {
        private static readonly Dictionary<string, string> LocPropertiesDefault = new();

        // LOCALIZATION NOTE:
        // Localization-related properties in here are manually localized in LocalizationManager.
        // As such, they should NOT be inherited to avoid confusing the system.

        private const int DEFAULT_RANGE = 200;

        [NeverPushInheritance]
        [DataField("loc")]
        private Dictionary<string, string>? _locPropertiesSet;

        /// <summary>
        /// The "in code name" of the object. Must be unique.
        /// </summary>
        [DataField("id")]
        public string ID { get; private set; } = default!;

        /// <summary>
        ///     If true, this object should not show up in the entity spawn panel.
        /// </summary>
        [NeverPushInheritance]
        [DataField("abstract")]
        public bool Abstract { get; private set; }

        /// <summary>
        /// The prototype we inherit from.
        /// </summary>
        [DataField("parent", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string? Parent { get; private set; }

        /// <summary>
        /// A dictionary mapping the component type list to the YAML mapping containing their settings.
        /// </summary>
        [DataField("components")]
        [AlwaysPushInheritance]
        public ComponentRegistry Components { get; } = new();

        public EntityPrototype()
        {
            // All entities comes with a metadata component.
            Components.Add("MetaData", new MetaDataComponent());
        }

        public bool TryGetComponent<T>(string name, [NotNullWhen(true)] out T? component) where T : IComponent
        {
            if (!Components.TryGetValue(name, out var componentUnCast))
            {
                component = default;
                return false;
            }

            // There are no duplicate component names
            // TODO Sanity check with names being in an attribute of the type instead
            component = (T) componentUnCast;
            return true;
        }

        public void UpdateEntity(Entity entity)
        {
            if (ID != entity.Prototype?.ID)
            {
                Logger.Error(
                    $"Reloaded prototype used to update entity did not match entity's existing prototype: Expected '{ID}', got '{entity.Prototype?.ID}'");
                return;
            }

            var factory = IoCManager.Resolve<IComponentFactory>();
            var entityManager = IoCManager.Resolve<IEntityManager>();
            var oldPrototype = entity.Prototype;

            var oldPrototypeComponents = oldPrototype.Components.Keys
                .Where(n => n != "Transform" && n != "MetaData")
                .Select(name => (name, factory.GetRegistration(name).Type))
                .ToList();
            var newPrototypeComponents = Components.Keys
                .Where(n => n != "Transform" && n != "MetaData")
                .Select(name => (name, factory.GetRegistration(name).Type))
                .ToList();

            var ignoredComponents = new List<string>();

            // Find components to be removed, and remove them
            foreach (var (name, type) in oldPrototypeComponents.Except(newPrototypeComponents))
            {
                if (Components.Keys.Contains(name))
                {
                    ignoredComponents.Add(name);
                    continue;
                }

                entityManager.RemoveComponent(entity.Uid, type);
            }

            entityManager.CullRemovedComponents();

            var componentDependencyManager = IoCManager.Resolve<IComponentDependencyManager>();

            // Add new components
            foreach (var (name, type) in newPrototypeComponents.Where(t => !ignoredComponents.Contains(t.name))
                .Except(oldPrototypeComponents))
            {
                var data = Components[name];
                var component = (Component) factory.GetComponent(name);
                component.Owner = entity;
                componentDependencyManager.OnComponentAdd(entity.Uid, component);
                entity.AddComponent(component);
            }

            // Update entity metadata
            entity.MetaData.EntityPrototype = this;
        }

        internal static void LoadEntity(EntityPrototype? prototype, Entity entity, IComponentFactory factory,
            IEntityLoadContext? context) //yeah officer this method right here
        {
            /*YamlObjectSerializer.Context? defaultContext = null;
            if (context == null)
            {
                defaultContext = new PrototypeSerializationContext(prototype);
            }*/

            if (prototype != null)
            {
                foreach (var (name, data) in prototype.Components)
                {
                    var fullData = data;
                    if (context != null)
                    {
                        fullData = context.GetComponentData(name, data);
                    }

                    EnsureCompExistsAndDeserialize(entity, factory, name, fullData, context as ISerializationContext);
                }
            }

            if (context != null)
            {
                foreach (var name in context.GetExtraComponentTypes())
                {
                    if (prototype != null && prototype.Components.ContainsKey(name))
                    {
                        // This component also exists in the prototype.
                        // This means that the previous step already caught both the prototype data AND map data.
                        // Meaning that re-running EnsureCompExistsAndDeserialize would wipe prototype data.
                        continue;
                    }

                    var ser = context.GetComponentData(name, null);

                    EnsureCompExistsAndDeserialize(entity, factory, name, ser, context as ISerializationContext);
                }
            }
        }

        private static void EnsureCompExistsAndDeserialize(Entity entity, IComponentFactory factory, string compName,
            IComponent data, ISerializationContext? context)
        {
            var compType = factory.GetRegistration(compName).Type;

            if (!entity.TryGetComponent(compType, out var component))
            {
                var newComponent = (Component) factory.GetComponent(compName);
                newComponent.Owner = entity;
                entity.AddComponent(newComponent);
                component = newComponent;
            }

            // TODO use this value to support struct components
            _ = IoCManager.Resolve<ISerializationManager>().Copy(data, component, context);
        }

        public override string ToString()
        {
            return $"EntityPrototype({ID})";
        }

        public class ComponentRegistry : Dictionary<string, IComponent>
        {
            public ComponentRegistry()
            {
            }

            public ComponentRegistry(Dictionary<string, IComponent> components) : base(components)
            {
            }
        }

        [DataDefinition]
        public class EntityPlacementProperties
        {
            public bool PlacementOverriden { get; private set; }
            public bool SnapOverriden { get; private set; }
            private string _placementMode = "PlaceFree";
            private Vector2i _placementOffset;

            [DataField("mode")]
            public string PlacementMode
            {
                get => _placementMode;
                set
                {
                    PlacementOverriden = true;
                    _placementMode = value;
                }
            }

            [DataField("offset")]
            public Vector2i PlacementOffset
            {
                get => _placementOffset;
                set
                {
                    PlacementOverriden = true;
                    _placementOffset = value;
                }
            }

            [DataField("nodes")] public List<int>? MountingPoints;

            [DataField("range")] public int PlacementRange = DEFAULT_RANGE;
            private HashSet<string> _snapFlags = new();

            [DataField("snap")]
            public HashSet<string> SnapFlags
            {
                get => _snapFlags;
                set
                {
                    SnapOverriden = true;
                    _snapFlags = value;
                }
            }
        }
    }
}
