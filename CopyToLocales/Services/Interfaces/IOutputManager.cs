using CopyToLocales.ViewModel.Enums;

using System.Collections.Generic;

namespace CopyToLocales.Services.Interfaces
{
    using ViewModel;

    public interface IOutputManager
    {
        OutputTypes OutputType { get; }

        void Save(Dictionary<string, SelectFileViewModel> sourceDictionaryEntryElements,
                  Dictionary<string, SelectFileViewModel> targetDictionaryEntryElements);

        string InitReaders(FileType fileType, SelectFileViewModel selectFileViewModel);
    }
}
