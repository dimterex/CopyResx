using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Composition;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using MefPrototype.Core.Modules;
using MefPrototype.Core.Services;
using System.Composition.Hosting;
using CopyToLocale.Core.Modules;
using MefPrototype;
using Microsoft.Extensions.DependencyModel;
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
        
        [ImportMany]
        public IFormatManager[] IpCheckerList { get; set; }

        private CompositionContainer _container;

        public void Start()
        {
            string entryAssemblyLocation = Assembly.GetEntryAssembly()!.Location;
            string entryAssemblyDirectoryName = Path.GetDirectoryName(entryAssemblyLocation)!;
            string modulesLocation = Path.Combine(entryAssemblyDirectoryName, MODULES_DIRECTORY);
            // An aggregate catalog that combines multiple catalogs.
            
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new DirectoryCatalog(modulesLocation));
            _container = new CompositionContainer(catalog);
            

            _container.ComposeParts(this);
            
            return;
            
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

            _modulesLoadContext = new CustomAssemblyLoadContext();

            var assemblies = new List<Assembly>();

            var envValue = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            var remainingLibraries = new Queue<string>(Directory
                .GetFiles(modulesLocation, SEARCH_MODULE_PATTERN));
            
            while (remainingLibraries.Count > 0)
            {
                var current = remainingLibraries.Dequeue();
                try
                {
                     if (File.Exists(current))
                     {
                         // var asm = Assembly.LoadFrom(current);
                         var asm = _modulesLoadContext.LoadFromAssemblyPath(current);
                         assemblies.Add(asm);
     
                         _logger.Info(() => $"Module assembly `{current}` loaded.");
                         // if (asm != null)
                         // {
                         //     var depcontext = DependencyContext.Load(asm);
                         //     if (depcontext != null)
                         //     {
                         //         foreach (var runtimeLibrary in depcontext.RuntimeLibraries)
                         //         {
                         //             // we only care about nuget packages, our main dlls are local anyway
                         //             if (runtimeLibrary.Type == "package")
                         //             {
                         //                 foreach (var runtimeAssemblyGroup in runtimeLibrary.RuntimeAssemblyGroups)
                         //                 {
                         //                     foreach (var assetPath in runtimeAssemblyGroup.AssetPaths)
                         //                     {
                         //                         string packageSource =
                         //                             $".nuget/packages/{runtimeLibrary.Name}/{runtimeLibrary.Version}";
                         //                         var fullPath = Path.Combine(envValue, packageSource, assetPath)
                         //                             .Replace("\\", "/");
                         //                         remainingLibraries.Enqueue(fullPath);
                         //                     }
                         //                 }
                         //             }
                         //         }
                         //     }
                         // }
                     }
                }
                catch (Exception ex)
                {
                    _logger.Error(() => $"Failed to load module assembly `{current}`. {ex}");
                }
            }

            return assemblies;
        }

        #endregion Methods
    }
}