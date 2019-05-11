using CopyToLocales.Core;
using CopyToLocales.ViewModel.Enums;

using System.Collections.Generic;
using System.IO;

namespace CopyToLocales.Services.Interfaces
{
    public interface IOutputsManager
    {
        Dictionary<OutputTypes, IOutputManager> OutputManagers { get; }

        OutputTypes SelectedOutputType { get; set; }

        List<DictionaryEntryElement> SourceDictionaryEntryElements { get; }

        void Save();

        void Read(FileType source, FileInfo fileInfo);
    }
}
