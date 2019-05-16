using CopyToLocales.ViewModel.Enums;

using System.Collections.Generic;

namespace CopyToLocales.Services.Interfaces
{
    using ViewModel;

    public interface IOutputsManager
    {
        Dictionary<OutputTypes, IOutputManager> OutputManagers { get; }

        OutputTypes SelectedOutputType { get; set; }
        OutputTypes SelectedInputType { get; set; }

        Dictionary<string, SelectFileViewModel> SourceDictionaryEntryElements { get; }

        Dictionary<string, SelectFileViewModel> TargetDictionaryEntryElements { get; }

        void Save();

        void Read(FileType source, SelectFileViewModel selectFileViewModel);

        void Clear(FileType fileType);
    }
}
