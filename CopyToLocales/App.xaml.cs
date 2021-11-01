using CopyToLocale.Core.Modules;

namespace CopyToLocales
{
    using CopyToLocales.View;
    using CopyToLocales.ViewModel;

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Mvvm;

    using System.Windows;
    using CopyToLocales.Model;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.Model.Outputs;

    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Prism.Unity.PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var app = new Application(containerRegistry, Container);
            app.Start();
            
            containerRegistry.RegisterSingleton<IFileManager, FileManager>();
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<ILogService, LogService>();

            containerRegistry.Register<IFormatManager, TextFormatManager>(nameof(TextFormatManager));
            containerRegistry.RegisterSingleton<IFormatsManager, FormatsManager>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<SourceFolderSelectView, SourceFolderSelectViewModel>();
            ViewModelLocationProvider.Register<TargetFolderSelectView, TargetFolderSelectViewModel>();

            ViewModelLocationProvider.Register<MainView, MainViewModel>();
            ViewModelLocationProvider.Register<SelectionView, SelectionViewModel> ();
            ViewModelLocationProvider.Register<KeysEditorView, KeysEditorViewModel> ();
            ViewModelLocationProvider.Register<SelectionOutputView, SelectionOutputViewModel> ();
            ViewModelLocationProvider.Register<SelectionInputView, SelectionInputViewModel> ();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ViewModule>();
        }
    }
}
