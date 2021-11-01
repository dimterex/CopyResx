using System.ComponentModel;
using Prism.Ioc;
using Unity;

namespace MefPrototype.Core.Services
{
    using System;

    /// <inheritdoc />
    public sealed class ServiceCollection : IServiceCollection
    {
        #region Fields

        private readonly IContainerRegistry _container;
        private readonly IContainerProvider _containerProvider;

        #endregion Fields

        #region Constructors

        public ServiceCollection(IContainerRegistry container, IContainerProvider containerProvider)
        {
            _container = container;
            _containerProvider = containerProvider;
        }

        #endregion Constructors

        #region Methods

        public IServiceCollection AddSingleton<TService, TImplementation>()
            where TImplementation : class, TService
        {
            return Add<TService, TImplementation>(null,null);
        }

        public IServiceCollection AddSingleton<TService, TImplementation>(string name) where TImplementation : class, TService
        {
            return Add<TService, TImplementation>(name, null);
        }

        public IServiceCollection AddSingleton<TService, TImplementation>(Action<TService>? initializer) where TImplementation : class, TService
        {
            return Add<TService, TImplementation>(null, initializer);
        }

        public IServiceCollection AddSingleton<TService, TImplementation>(string name, Action<TService>? initializer) where TImplementation : class, TService
        {
            return Add<TService, TImplementation>(name, initializer);
        }

        private IServiceCollection Add<TService, TImplementation>(string name, Action<TService>? initializer)
            where TImplementation : class, TService
        {
            if (string.IsNullOrEmpty(name))
                _container.RegisterSingleton<TService, TImplementation>();
            else
                _container.Register<TService, TImplementation>(name);

            if (initializer != null)
            {
                var impl = _containerProvider.Resolve<TService>();
                initializer(impl);
            }

            return this;
        }
        

        #endregion Methods
    }
}