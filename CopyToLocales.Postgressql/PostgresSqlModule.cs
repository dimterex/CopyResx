using System.Composition;
using CopyToLocale.Core.Modules;
using CopyToLocales.Postgressql._Interfaces_;
using MefPrototype.Core.Modules;
using MefPrototype.Core.Services;

namespace CopyToLocales.Postgressql
{
    [Export(typeof(IModule))]
    internal sealed class PostgresSqlModule : IModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IFormatManager, PostgresSqlFormatManager>(nameof(PostgresSqlFormatManager));
            // .AddSingleton<ILocalizationDataBase, LocalizationDataBase>();
        }
    }
}