﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Why.Core.ContentPack;
using Why.Core.GameObjects;
using Why.Core.IoC;
using Why.Core.Maps;
using Why.Core.Reflection;
using Why.Core.Utility;

namespace Why.Tests
{
    [Parallelizable]
    public abstract partial class WhyUnitTest
    {
        [OneTimeSetUp]
        public void BaseSetup()
        {
            // Clear state across tests.
            IoCManager.Clear();

            RegisterIoC();

            var assemblies = new List<Assembly>(4);

            assemblies.Add(AppDomain.CurrentDomain.GetAssemblyByName("Why"));
            assemblies.Add(Assembly.GetExecutingAssembly());

            var contentAssemblies = GetContentAssemblies();

            // Required systems
            var systems = IoCManager.Resolve<IEntitySystemManager>();
            systems.Initialize();

            var entMan = IoCManager.Resolve<IEntityManager>();

            if (entMan.EventBus == null)
            {
                entMan.Initialize();
                entMan.Startup();
            }

            var mapMan = IoCManager.Resolve<IMapManager>();

            IoCManager.Resolve<IReflectionManager>().LoadAssemblies(assemblies);

            var modLoader = IoCManager.Resolve<TestingModLoader>();
            modLoader.Assemblies = contentAssemblies;
            modLoader.TryLoadModulesFrom(ResourcePath.Root, "");

            // Required components for the engine to work
            var compFactory = IoCManager.Resolve<IComponentFactory>();

            if (!compFactory.AllRegisteredTypes.Contains(typeof(MetaDataComponent)))
            {
                compFactory.RegisterClass<MetaDataComponent>();
            }

            if (entMan.EventBus == null)
            {
                entMan.Startup();
            }
        }

        [OneTimeTearDown]
        public void BaseTearDown()
        {
            IoCManager.Clear();
        }

        /// <summary>
        /// Called after all IoC registration has been done, but before the graph has been built.
        /// This allows one to add new IoC types or overwrite existing ones if needed.
        /// </summary>
        protected virtual void OverrideIoC()
        {
        }

        protected virtual Assembly[] GetContentAssemblies()
        {
            return Array.Empty<Assembly>();
        }
    }
}