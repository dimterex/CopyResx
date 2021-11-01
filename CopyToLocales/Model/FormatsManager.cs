using System.Windows.Documents;
using CopyToLocale.Core;
using CopyToLocale.Core.Models;
using CopyToLocale.Core.Modules;

namespace CopyToLocales.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.ViewModel;
    using CopyToLocales.ViewModel.Common;

    public class FormatsManager : IFormatsManager
    {
        private readonly ILogService _logService;
        private IList<SelectFileViewModel> _sourceFileList;
        private IList<SelectFileViewModel> _targetFileList;
        private SupportedLocalization _defaultSourceKey;
        private readonly Dictionary<string, IFormatManager> _outputManagers;

        #region Properties

        public string SelectedFormatType { get; set; }

        public string SelectedInputType { get; set; }

        public Dictionary<string, ResourceModel> SourceDictionaryEntryElements { get; private set; }
        public Dictionary<string, ResourceModel> SelectedSourceElements { get; }
        public Dictionary<string, ResourceModel> TargetDictionaryEntryElements { get; private set; }
        public List<string> SupportsFormat { get; set; }

        #endregion Properties

        #region Constuctors

        public FormatsManager(IFormatManager[] outputManagers, ILogService logService)
        {
            _logService = logService;
            _outputManagers = new Dictionary<string, IFormatManager>();
            SupportsFormat = new List<string>();

            SourceDictionaryEntryElements = new Dictionary<string, ResourceModel>();
            SelectedSourceElements = new Dictionary<string, ResourceModel>();
            
            TargetDictionaryEntryElements = new Dictionary<string, ResourceModel>();

            foreach (var outputManager in outputManagers)
            {
                _outputManagers.Add(outputManager.FormatType, outputManager);
                outputManager.StateChangeEvent += OutputManagerOnStateChangeEvent;
                SupportsFormat.Add(outputManager.FormatType);
            }
        }
       

        #endregion Constuctors

        #region Methods
        
        private void OutputManagerOnStateChangeEvent(object? sender, string e)
        {
            _logService.AddMessage(e);
        }

        public void Save(ObservableCollection<ChangedResourceViewModel> dictionaryEntryElements)
        {
            var forUpdate = new List<ChangedResourceModel>();
            foreach (var dictionaryElementViewModel in dictionaryEntryElements)
            {
                dictionaryElementViewModel.Clear(_defaultSourceKey);
                var model = new ChangedResourceModel()
                {
                    NewKey = dictionaryElementViewModel.NewKey,
                    Key = dictionaryElementViewModel.Key,
                    KeyValueList = dictionaryElementViewModel.KeyValueList,
                };

                forUpdate.Add(model);
            }

            _outputManagers[SelectedFormatType].Update(forUpdate, _targetFileList.Select(x => new SelectFileModel()
            {
                FullPath = x.FullPath,
                SourceKey = x.SourceKey,
                SourceKeys = x.SourceKeys,
                IsDefault = x.IsDefault
            }).ToList());
        }

        public void Load(DirectionType source, IList<SelectFileViewModel> selectFileViewModel)
        {
            switch (source)
            {
                case DirectionType.Source:
                    _sourceFileList = selectFileViewModel;
                    SourceDictionaryEntryElements =  _outputManagers[SelectedInputType].InitReaders(selectFileViewModel.Select(x => new SelectFileModel()
                    {
                        FullPath = x.FullPath,
                        SourceKey = x.SourceKey,
                        SourceKeys = x.SourceKeys,
                        IsDefault = x.IsDefault
                    }).ToList());;
                    break;
                case DirectionType.Target:
                    _targetFileList = selectFileViewModel;
                    TargetDictionaryEntryElements = _outputManagers[SelectedFormatType].InitReaders(selectFileViewModel.Select(x => new SelectFileModel()
                    {
                        FullPath = x.FullPath,
                        SourceKey = x.SourceKey,
                        SourceKeys = x.SourceKeys,
                        IsDefault = x.IsDefault
                    }).ToList());;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public List<SupportedLocalization> GetSourceKeys()
        {
            return _sourceFileList?.Select(x => x.SourceKey).ToList();
        }

        public void SetDefaultKey(SupportedLocalization sourceKey)
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

        
        public SupportedLocalization GetKey(string path, DirectionType directionType)
        {
            switch (directionType)
            {
                case DirectionType.Source:
                    return _outputManagers[SelectedInputType].GetKey(path);
                case DirectionType.Target:
                    return _outputManagers[SelectedFormatType].GetKey(path);
                default:
                    throw new ArgumentOutOfRangeException(nameof(directionType), directionType, null);
            }
        }

        #endregion Methods
    }
}
