namespace CopyToLocales.Model
{
    using System;
    using System.Collections.Generic;
    using CopyToLocales.DAL;
    using CopyToLocales.Model.Interfaces;

    public class SettingsManager : ISettingsManager
    {
        #region Constants

        private const string FILE_NAME = "Settings.json";

        #endregion

        #region Fields

        private readonly IFileManager _fileManager;
        private readonly SettingsDto _settingsDto;

        #endregion Fields

        #region Constuctors

        public SettingsManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
            _settingsDto = _fileManager.GetSettings(FILE_NAME);
        }

        #endregion Constuctors

        #region Methods

        public void Save()
        {
            _fileManager.SaveSettings(FILE_NAME, _settingsDto);
        }

        public List<string> GetBlackList()
        {
            return _settingsDto.BlackList;
        }

        public void SetFormatType(string selectedFormatType, DirectionType source)
        {
            switch (source)
            {
                case DirectionType.Source:
                    _settingsDto.SelectedInputType = selectedFormatType;
                    break;
                case DirectionType.Target:
                    _settingsDto.SelectedOutputType = selectedFormatType;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public string GetFormatType(DirectionType source)
        {
            switch (source)
            {
                case DirectionType.Source:
                    return _settingsDto.SelectedInputType;
                case DirectionType.Target:
                    return _settingsDto.SelectedOutputType;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public List<KeyValuePair<string, string>> GetPathList(DirectionType source)
        {
            switch (source)
            {
                case DirectionType.Source:
                    return _settingsDto.SourcesPath;
                case DirectionType.Target:
                    return _settingsDto.TargetPath;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public void UpdatePathList(DirectionType source, List<KeyValuePair<string, string>> ls)
        {
            switch (source)
            {
                case DirectionType.Source:
                    _settingsDto.SourcesPath.Clear();
                    _settingsDto.SourcesPath.AddRange(ls);
                    return;
                case DirectionType.Target:
                    _settingsDto.TargetPath.Clear();
                    _settingsDto.TargetPath.AddRange(ls);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        #endregion Methods

    }
}
