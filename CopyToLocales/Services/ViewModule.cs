using CopyToLocales.Core;
using CopyToLocales.View;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace CopyToLocales.Services
{
    public class ViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.AddToRegion(Constants.ContentRegion, containerProvider.Resolve<SourceFolderSelectView>());
            //regionManager.AddToRegion(Constants.ContentRegion, containerProvider.Resolve<SelectionView>());
            //regionManager.AddToRegion(Constants.ContentRegion, containerProvider.Resolve<KeysEditorView>());
            regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionInputView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SelectionInputView>();
            containerRegistry.RegisterForNavigation<SourceFolderSelectView>();
            containerRegistry.RegisterForNavigation<SelectionView>();
            containerRegistry.RegisterForNavigation<KeysEditorView>();
            containerRegistry.RegisterForNavigation<SelectionOutputView>();
            containerRegistry.RegisterForNavigation<TargetFolderSelectView>();
        }
    }
}
