using System;
using System.Composition;
using CopyToLocale.Core.Modules;
using MefPrototype.Core.Modules;
using MefPrototype.Core.Services;

namespace CopyToLocates.Resx
{
    [Export(typeof(IModule))]
    internal sealed class ResxModule : IModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IFormatManager, ResxFormatManager>(nameof(ResxFormatManager));
        }
    }
}