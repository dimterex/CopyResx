namespace MefPrototype.Core.Services
{
    using System;

    public interface IServiceCollection
    {
        IServiceCollection AddSingleton<TService, TImplementation>()
            where TImplementation : class, TService;
        
        IServiceCollection AddSingleton<TService, TImplementation>(string name)
            where TImplementation : class, TService;
        
        IServiceCollection AddSingleton<TService, TImplementation>(Action<TService>? initializer)
            where TImplementation : class, TService;
        
        IServiceCollection AddSingleton<TService, TImplementation>(string name, Action<TService>? initializer)
            where TImplementation : class, TService;
        
    }
}