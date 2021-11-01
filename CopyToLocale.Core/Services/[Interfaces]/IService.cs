namespace MefPrototype.Core.Services
{
    /// <summary>
    /// Интерфейс сервиса.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Запускает работу сервиса.
        /// </summary>
        void Start();

        /// <summary>
        /// Останавливает работу сервиса.
        /// </summary>
        void Stop();
    }
}