using CopyToLocales.Core;
using CopyToLocales.Services.Interfaces;
using CopyToLocales.ViewModel.Enums;

using System.Collections.Generic;
using System.IO;

namespace CopyToLocales.Services.Realization
{
    public class OutputsManager : IOutputsManager
    {
        #region Properties

        public OutputTypes SelectedOutputType { get; set; }

        public Dictionary<OutputTypes, IOutputManager> OutputManagers { get; }

        public List<DictionaryEntryElement> SourceDictionaryEntryElements => OutputManagers[SelectedOutputType].SourceDictionaryEntryElements;

        #endregion Properties

        #region Constuctors

        public OutputsManager(IOutputManager[] outputManagers)
        {
            OutputManagers = new Dictionary<OutputTypes, IOutputManager>();

            foreach (var outputManager in outputManagers)
                OutputManagers.Add(outputManager.OutputType, outputManager);
        }

        #endregion Constuctors

        #region Methods

        public void Save()
        {
            OutputManagers[SelectedOutputType].Save();
        }

        public void Read(FileType source, FileInfo fileInfo)
        {
            OutputManagers[SelectedOutputType].InitReaders(source, fileInfo);
        }

        #endregion Methods
    }
}
