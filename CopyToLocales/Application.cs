using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using MefPrototype.Core.Modules;
using MefPrototype.Core.Services;
using System.Composition.Hosting;
using MefPrototype;
using NLog;
using Prism.Ioc;

namespace CopyToLocales
{
    internal sealed class Application : IApplication
    {
        private readonly IContainerRegistry _containerRegistry;
        private readonly IContainerProvider _containerProvider;

        #region Constants

        private const string MODULES_DIRECTORY = "Modules\\net5.0";

        private const string SEARCH_MODULE_PATTERN = "CopyToLocales.*.dll";

        #endregion Constants

        #region Fields

        private readonly ILogger _logger;
        private AssemblyLoadContext? _modulesLoadContext;

        #endregion Fields

        #region Constructors

        public Application(IContainerRegistry containerRegistry, IContainerProvider containerProvider)
        {
            _containerRegistry = containerRegistry;
            _containerProvider = containerProvider;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            IReadOnlyList<Assembly> moduleAssemblies = LoadModuleAssemblies();

            if (moduleAssemblies.Count == 0)
            {
                _logger.Warn("No modules loaded.");
                return;
            }

            using CompositionHost host = new ContainerConfiguration()
                .WithAssemblies(moduleAssemblies)
                .CreateContainer();

            var serviceCollection = new ServiceCollection(_containerRegistry, _containerProvider);

            _logger.Info("Configuring services of modules...");

            foreach (IModule module in host.GetExports<IModule>())
            {
                try
                {
                    module.ConfigureServices(serviceCollection);

                    _logger.Info(() => $"Services of module `{module.GetType().Name}` configured.");
                }
                catch (Exception ex)
                {
                    _logger.Error(() => $"Failed to configure services of module `{module.GetType().Name}`. {ex}");
                }
            }
        }

        public void Stop()
        {
            _modulesLoadContext?.Unload();
            _modulesLoadContext = null;
        }

        private IReadOnlyList<Assembly> LoadModuleAssemblies()
        {
            _logger.Info("Loading assemblies of modules...");

            string entryAssemblyLocation = Assembly.GetEntryAssembly()!.Location;
            string entryAssemblyDirectoryName = Path.GetDirectoryName(entryAssemblyLocation)!;
            string modulesLocation = Path.Combine(entryAssemblyDirectoryName, MODULES_DIRECTORY);

            if (!Directory.Exists(modulesLocation))
            {
                _logger.Warn(() => $"Modules directory `{modulesLocation}` not found.");
                return Array.Empty<Assembly>();
            }

            _modulesLoadContext = new AssemblyLoadContext("Modules", true);

            var assemblies = new List<Assembly>();

            foreach (var filePath in Directory.EnumerateFiles(modulesLocation, SEARCH_MODULE_PATTERN, SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(filePath);

                try
                {
                    Assembly assembly = _modulesLoadContext.LoadFromAssemblyPath(filePath);
                    assemblies.Add(assembly);

                    _logger.Info(() => $"Module assembly `{fileName}` loaded.");
                }
                catch (Exception ex)
                {
                    _logger.Error(() => $"Failed to load module assembly `{fileName}`. {ex}");
                }
            }

            return assemblies;
        }

        #endregion Methods
    }
}