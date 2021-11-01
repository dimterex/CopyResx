using System;
using System.Composition;
using CopyToLocale.Core.Modules;
using MefPrototype.Core.Modules;
using MefPrototype.Core.Services;

namespace CopyToLocales.Xaml
{
    [Export(typeof(IModule))]
    internal sealed class XamlModule : IModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IFormatManager, XamlFormatManager>(nameof(XamlFormatManager));
        }
    }
}