namespace CopyToLocales.Services.Realization
{
    using CopyToLocales.Services.Interfaces;
    using CopyToLocales.ViewModel.Enums;

    using System;
    using System.Collections.Generic;

    using ViewModel;

    public class OutputsManager : IOutputsManager
    {
        #region Properties

        public OutputTypes SelectedOutputType { get; set; }

        public OutputTypes SelectedInputType { get; set; }

        public Dictionary<OutputTypes, IOutputManager> OutputManagers { get; }

        public Dictionary<string, SelectFileViewModel> SourceDictionaryEntryElements { get ;}
        public Dictionary<string, SelectFileViewModel> TargetDictionaryEntryElements { get; }

        #endregion Properties

        #region Constuctors

        public OutputsManager(IOutputManager[] outputManagers)
        {
            OutputManagers = new Dictionary<OutputTypes, IOutputManager>();

            SourceDictionaryEntryElements = new Dictionary<string, SelectFileViewModel>();
            TargetDictionaryEntryElements = new Dictionary<string, SelectFileViewModel>();

            foreach (var outputManager in outputManagers)
                OutputManagers.Add(outputManager.OutputType, outputManager);
        }

        #endregion Constuctors

        #region Methods

        public void Save()
        {
            OutputManagers[SelectedOutputType].Save(SourceDictionaryEntryElements, TargetDictionaryEntryElements);
        }

        public void Read(FileType source, SelectFileViewModel selectFileViewModel)
        {
            string tmp;
            if (selectFileViewModel.FullPath.EndsWith(".resx"))
                tmp = OutputManagers[OutputTypes.Resx].InitReaders(source, selectFileViewModel);
            else if (selectFileViewModel.FullPath.EndsWith(".xaml"))
                tmp = OutputManagers[OutputTypes.Xaml].InitReaders(source, selectFileViewModel);
            else
                tmp = OutputManagers[OutputTypes.Text].InitReaders(source, selectFileViewModel);

            switch (source)
            {
                case FileType.Source:
                    SourceDictionaryEntryElements.Add(tmp, selectFileViewModel);
                    break;
                case FileType.Target:
                    TargetDictionaryEntryElements.Add(tmp, selectFileViewModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public void Clear(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Source:
                    SourceDictionaryEntryElements.Clear();
                    break;
                case FileType.Target:
                    TargetDictionaryEntryElements.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }

        #endregion Methods
    }
}
