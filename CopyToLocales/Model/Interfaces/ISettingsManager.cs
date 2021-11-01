using CopyToLocale.Core;
using CopyToLocales.DAL;

namespace CopyToLocales.Model.Interfaces
{
    using System.Collections;
    using System.Collections.Generic;

    public interface ISettingsManager
    {
        void Save();
        List<string> GetBlackList();
        void SetFormatType(string selectedFormatType, DirectionType source);
        string GetFormatType(DirectionType source);
        List<FileDto> GetPathList(DirectionType source);
        void UpdatePathList(DirectionType source, List<FileDto> ls);
    }
}
