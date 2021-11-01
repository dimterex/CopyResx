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
using Unity;

namespace CopyToLocales
{
    internal sealed class Application : IApplication
    {
        private readonly IContainerRegistry _containerRegistry;
        private readonly IContainerProvider _containerProvider;

        #region Constants

        /// <summary>
        /// Директория, в которой находятся загружаемые модули.
        /// </summary>
        private const string MODULES_DIRECTORY = "Modules\\net5.0";

        /// <summary>
        /// Шаблон поиска файлов сборок модулей.
        /// </summary>
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

        /// <inheritdoc />
        public void Start()
        {
            // Загружаем сборки модулей.
            IReadOnlyList<Assembly> moduleAssemblies = LoadModuleAssemblies();

            if (moduleAssemblies.Count == 0)
            {
                _logger.Warn("No modules loaded.");
                return;
            }

            // На основе загруженных сборок создаём контейнер композиции.
            // Через него будет происходить импорт реализаций `IModule` из загруженных ранее сборок.
            using CompositionHost host = new ContainerConfiguration()
                .WithAssemblies(moduleAssemblies)
                .CreateContainer();



            // Создаём обёртку над контейнером, при помощи которой модули будут регистрировать свои зависимости.
            var serviceCollection = new ServiceCollection(_containerRegistry, _containerProvider);

            _logger.Info("Configuring services of modules...");

            // Получаем реализации интерфейса `IModule`.
            foreach (IModule module in host.GetExports<IModule>())
            {
                try
                {
                    // Вызываем метод настройки/регистрации сервисов модуля.
                    module.ConfigureServices(serviceCollection);

                    _logger.Info(() => $"Services of module `{module.GetType().Name}` configured.");
                }
                catch (Exception ex)
                {
                    _logger.Error(() => $"Failed to configure services of module `{module.GetType().Name}`. {ex}");
                }
            }
        }

        /// <inheritdoc />
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

            // Загрузка сборок происходит в изолированный контекст с возможностью выгрузить его в дальнейшем.
            // Это не обязательное условие. Можно загружать сборки в `AssemblyLoadContext.Default`,
            // но выгрузить их в дальнейшем не представится возможным.
            // Выгрузка может пригодиться, если понадобится перезагружать модули.
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