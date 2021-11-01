using System;
using System.Composition;
using CopyToLocale.Core.Modules;
using MefPrototype.Core.Modules;
using MefPrototype.Core.Services;

namespace CopyToLocales.Text
{
    [Export(typeof(IModule))]
    internal sealed class TextModule : IModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IFormatManager, TextFormatManager>(nameof(TextFormatManager));
        }
    }
}