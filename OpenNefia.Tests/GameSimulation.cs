﻿using JetBrains.Annotations;
using Moq;
using OpenNefia.Core.ContentPack;
using OpenNefia.Core.Exceptions;
using OpenNefia.Core.Game;
using OpenNefia.Core.GameObjects;
using OpenNefia.Core.IoC;
using OpenNefia.Core.Locale;
using OpenNefia.Core.Log;
using OpenNefia.Core.Maps;
using OpenNefia.Core.Prototypes;
using OpenNefia.Core.Reflection;
using OpenNefia.Core.Serialization.Manager;
using OpenNefia.Core.Serialization.Manager.Attributes;
using OpenNefia.Core.UI;
using OpenNefia.Core.UI.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Tests
{
    [PublicAPI]
    internal interface ISimulationFactory
    {
        ISimulationFactory RegisterComponents(CompRegistrationDelegate factory);
        ISimulationFactory RegisterDependencies(DiContainerDelegate factory);
        ISimulationFactory RegisterEntitySystems(EntitySystemRegistrationDelegate factory);
        ISimulationFactory RegisterPrototypes(PrototypeRegistrationDelegate factory);
        ISimulation InitializeInstance();
    }

    [PublicAPI]
    internal interface ISimulation
    {
        IDependencyCollection Collection { get; }

        /// <summary>
        /// Resolves a dependency directly out of IoC collection.
        /// </summary>
        T Resolve<T>();

        /// <summary>
        /// Registers and sets a map as active.
        /// </summary>
        void SetActiveMap(IMap map);
        IEntity SpawnEntity(string? protoId, MapCoordinates coordinates);

        IMap? ActiveMap { get; }
    }

    internal delegate void DiContainerDelegate(IDependencyCollection diContainer);

    internal delegate void CompRegistrationDelegate(IComponentFactory factory);

    internal delegate void EntitySystemRegistrationDelegate(IEntitySystemManager systemMan);

    internal delegate void PrototypeRegistrationDelegate(IPrototypeManager protoMan);

    internal class GameSimulation : ISimulation, ISimulationFactory
    {
        private DiContainerDelegate? _diFactory;
        private CompRegistrationDelegate? _regDelegate;
        private EntitySystemRegistrationDelegate? _systemDelegate;
        private PrototypeRegistrationDelegate? _protoDelegate;

        public IDependencyCollection Collection { get; private set; } = default!;

        public IMap? ActiveMap => Resolve<IMapManager>().ActiveMap;

        public T Resolve<T>()
        {
            return Collection.Resolve<T>();
        }

        public void SetActiveMap(IMap map)
        {
            var mapMan = Collection.Resolve<IMapManager>();
            mapMan.RegisterMap(map);
            mapMan.ChangeActiveMap(map.Id);
        }

        public IEntity SpawnEntity(string? protoId, MapCoordinates coordinates)
        {
            var entMan = Collection.Resolve<IEntityManager>();
            return entMan.SpawnEntity(protoId, coordinates);
        }

        private GameSimulation() { }

        public ISimulationFactory RegisterDependencies(DiContainerDelegate factory)
        {
            _diFactory += factory;
            return this;
        }

        public ISimulationFactory RegisterComponents(CompRegistrationDelegate factory)
        {
            _regDelegate += factory;
            return this;
        }

        public ISimulationFactory RegisterEntitySystems(EntitySystemRegistrationDelegate factory)
        {
            _systemDelegate += factory;
            return this;
        }

        public ISimulationFactory RegisterPrototypes(PrototypeRegistrationDelegate factory)
        {
            _protoDelegate += factory;
            return this;
        }

        public ISimulation InitializeInstance()
        {
            var container = new DependencyCollection();
            Collection = container;

            IoCManager.InitThread(container, true);

            //Tier 1: System
            container.Register<ILogManager, LogManager>();
            container.Register<IRuntimeLog, RuntimeLog>();
            container.Register<IDynamicTypeFactory, DynamicTypeFactory>();
            container.Register<IDynamicTypeFactoryInternal, DynamicTypeFactory>();
            container.Register<ILocalizationManager, LocalizationManager>();
            container.Register<IModLoader, TestingModLoader>();
            container.Register<IModLoaderInternal, TestingModLoader>();

            var uiLayerManager = new Mock<IUiLayerManager>();
            container.RegisterInstance<IUiLayerManager>(uiLayerManager.Object);

            var realReflection = new ReflectionManager();
            realReflection.LoadAssemblies(new List<Assembly>(1)
            {
                AppDomain.CurrentDomain.GetAssemblyByName("OpenNefia.Core"),
            });

            var reflectionManager = new Mock<IReflectionManager>();
            reflectionManager
                .Setup(x => x.FindTypesWithAttribute<MeansDataDefinitionAttribute>())
                .Returns(() => new[]
                {
                    typeof(DataDefinitionAttribute)
                });

            reflectionManager
                .Setup(x => x.FindTypesWithAttribute(typeof(DataDefinitionAttribute)))
                .Returns(() => new[]
                {
                    typeof(EntityPrototype),
                    typeof(SpatialComponent),
                    typeof(MetaDataComponent)
                });

            reflectionManager
                .Setup(x => x.FindTypesWithAttribute<TypeSerializerAttribute>())
                .Returns(() => realReflection.FindTypesWithAttribute<TypeSerializerAttribute>());

            reflectionManager
                .Setup(x => x.FindAllTypes())
                .Returns(() => realReflection.FindAllTypes());

            container.RegisterInstance<IReflectionManager>(reflectionManager.Object); // tests should not be searching for types
            container.RegisterInstance<IResourceManager>(new Mock<IResourceManager>().Object); // no disk access for tests

            //Tier 2: Simulation
            container.Register<IEntityManager, EntityManager>();
            container.Register<IMapManager, MapManager>();
            container.Register<ISerializationManager, SerializationManager>();
            container.Register<IPrototypeManager, PrototypeManager>();
            container.Register<IComponentFactory, ComponentFactory>();
            container.Register<IComponentDependencyManager, ComponentDependencyManager>();
            container.Register<IEntitySystemManager, EntitySystemManager>();
            container.Register<IGameSessionManager, GameSessionManager>();

            _diFactory?.Invoke(container);
            container.BuildGraph();

            var logMan = container.Resolve<ILogManager>();
            logMan.RootSawmill.AddHandler(new TestLogHandler("SIM"));

            var compFactory = container.Resolve<IComponentFactory>();
            compFactory.DoDefaultRegistrations();

            _regDelegate?.Invoke(compFactory);

            compFactory.FinishRegistration();

            var entityMan = container.Resolve<IEntityManager>();
            entityMan.Initialize();

            var entitySystemMan = container.Resolve<IEntitySystemManager>();
            _systemDelegate?.Invoke(entitySystemMan);

            var mapManager = container.Resolve<IMapManager>();

            entityMan.Startup();

            container.Resolve<ISerializationManager>().Initialize();

            var protoMan = container.Resolve<IPrototypeManager>();
            protoMan.RegisterType(typeof(EntityPrototype));
            _protoDelegate?.Invoke(protoMan);
            protoMan.Resync();

            return this;
        }

        public static ISimulationFactory NewSimulation()
        {
            return new GameSimulation();
        }
    }
}
