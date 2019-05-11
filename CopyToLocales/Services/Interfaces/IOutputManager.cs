using CopyToLocales.Core;
using CopyToLocales.ViewModel.Enums;

using System.Collections.Generic;
using System.IO;

namespace CopyToLocales.Services.Interfaces
{
    public interface IOutputManager
    {
        OutputTypes OutputType { get; }

        List<DictionaryEntryElement> SourceDictionaryEntryElements { get; }

        void Save();

        void InitReaders(FileType fileType, FileInfo fileInfo);
    }
}
