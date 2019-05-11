using CopyToLocales.Services;
using CopyToLocales.Services.Interfaces;
using CopyToLocales.Services.Realization;
using CopyToLocales.View;
using CopyToLocales.ViewModel;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using System.Windows;

namespace CopyToLocales
{
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
            containerRegistry.RegisterSingleton<IFileManager, FileManager>();
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<ILogService, LogService>();

            containerRegistry.Register<IOutputManager, ResxOutputManager>(nameof(ResxOutputManager));

            containerRegistry.RegisterSingleton<IOutputsManager, OutputsManager>();
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
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ViewModule>();
        }
    }
}
