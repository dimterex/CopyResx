namespace CopyToLocales.Model.Interfaces
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CopyToLocales.ViewModel;
    using CopyToLocales.ViewModel.Common;
    using CopyToLocales.ViewModel.Enums;

    public interface IFormatsManager
    {
        FormatTypes SelectedFormatType { set; }
        FormatTypes SelectedInputType { set; }

        Dictionary<string, ResourceModel> SourceDictionaryEntryElements { get; }
        Dictionary<string, ResourceModel> SelectedSourceElements { get; }

        Dictionary<string, ResourceModel> TargetDictionaryEntryElements { get; }
        List<FormatTypes> SupportsFormat { get; }

        void Save(ObservableCollection<ChangedResourceViewModel> dictionaryEntryElements);

        void Load(DirectionType source, IList<SelectFileViewModel> selectFileViewModel);

        List<string> GetSourceKeys();
        void SetDefaultKey(string sourceKey);
        List<string> GetAnotherLanguages(string path, DirectionType directionType);
    }
}
