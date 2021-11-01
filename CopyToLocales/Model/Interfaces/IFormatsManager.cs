using CopyToLocale.Core;
using CopyToLocale.Core.Models;

namespace CopyToLocales.Model.Interfaces
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CopyToLocales.ViewModel;
    using CopyToLocales.ViewModel.Common;

    public interface IFormatsManager
    {
        string SelectedFormatType { set; }
        string SelectedInputType { set; }

        Dictionary<string, ResourceModel> SourceDictionaryEntryElements { get; }
        Dictionary<string, ResourceModel> SelectedSourceElements { get; }

        Dictionary<string, ResourceModel> TargetDictionaryEntryElements { get; }
        List<string> SupportsFormat { get; }

        void Save(ObservableCollection<ChangedResourceViewModel> dictionaryEntryElements);

        void Load(DirectionType source, IList<SelectFileViewModel> selectFileViewModel);

        List<SupportedLocalization> GetSourceKeys();
        void SetDefaultKey(SupportedLocalization sourceKey);
        List<string> GetAnotherLanguages(string path, DirectionType directionType);

        SupportedLocalization GetKey(string path, DirectionType directionType);
    }
}
