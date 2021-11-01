namespace MefPrototype.Core.Modules
{
    using Services;

    /// <summary>
    /// Интерфейс модуля.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Настраивает сервисы модуля.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        void ConfigureServices(IServiceCollection services);
    }
}