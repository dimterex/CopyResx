namespace CopyToLocales.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.ViewModel;
    using CopyToLocales.ViewModel.Common;
    using CopyToLocales.ViewModel.Enums;

    public class FormatsManager : IFormatsManager
    {
        private IList<SelectFileViewModel> _sourceFileList;
        private IList<SelectFileViewModel> _targetFileList;
        private string _defaultSourceKey;
        private readonly Dictionary<FormatTypes, IFormatManager> _outputManagers;

        #region Properties

        public FormatTypes SelectedFormatType { get; set; }

        public FormatTypes SelectedInputType { get; set; }

        public Dictionary<string, ResourceModel> SourceDictionaryEntryElements { get; private set; }
        public Dictionary<string, ResourceModel> SelectedSourceElements { get; }
        public Dictionary<string, ResourceModel> TargetDictionaryEntryElements { get; private set; }
        public List<FormatTypes> SupportsFormat { get; set; }

        #endregion Properties

        #region Constuctors

        public FormatsManager(IFormatManager[] outputManagers)
        {
            _outputManagers = new Dictionary<FormatTypes, IFormatManager>();
            SupportsFormat = new List<FormatTypes>();

            SourceDictionaryEntryElements = new Dictionary<string, ResourceModel>();
            SelectedSourceElements = new Dictionary<string, ResourceModel>();
            
            TargetDictionaryEntryElements = new Dictionary<string, ResourceModel>();

            foreach (var outputManager in outputManagers)
            {
                _outputManagers.Add(outputManager.FormatType, outputManager);
                SupportsFormat.Add(outputManager.FormatType);
            }
        }

        #endregion Constuctors

        #region Methods

        public void Save(ObservableCollection<ChangedResourceViewModel> dictionaryEntryElements)
        {
            foreach (var dictionaryElementViewModel in dictionaryEntryElements)
                dictionaryElementViewModel.Clear(_defaultSourceKey);

            _outputManagers[SelectedFormatType].Update(dictionaryEntryElements.ToList(), _targetFileList);
        }

        public void Load(DirectionType source, IList<SelectFileViewModel> selectFileViewModel)
        {
            switch (source)
            {
                case DirectionType.Source:
                    _sourceFileList = selectFileViewModel;
                    SourceDictionaryEntryElements =  _outputManagers[SelectedInputType].InitReaders(selectFileViewModel);;
                    break;
                case DirectionType.Target:
                    _targetFileList = selectFileViewModel;
                    TargetDictionaryEntryElements = _outputManagers[SelectedFormatType].InitReaders(selectFileViewModel);;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public List<string> GetSourceKeys()
        {
            return _sourceFileList?.Select(x => x.SourceKey).ToList();
        }

        public void SetDefaultKey(string sourceKey)
        {
            _defaultSourceKey = sourceKey;
        }

        public List<string> GetAnotherLanguages(string path, DirectionType directionType)
        {
            switch (directionType)
            {
                case DirectionType.Source:
                    return _outputManagers[SelectedInputType].GetAnotherLanguages(path);
                case DirectionType.Target:
                    return _outputManagers[SelectedFormatType].GetAnotherLanguages(path);
                default:
                    throw new ArgumentOutOfRangeException(nameof(directionType), directionType, null);
            }
        }

        #endregion Methods
    }
}
