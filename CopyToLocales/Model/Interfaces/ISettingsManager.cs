namespace CopyToLocales.Model.Interfaces
{
    using System.Collections;
    using System.Collections.Generic;
    using CopyToLocales.ViewModel.Enums;

    public interface ISettingsManager
    {
        void Save();
        List<string> GetBlackList();
        void SetFormatType(FormatTypes selectedFormatType, DirectionType source);
        FormatTypes GetFormatType(DirectionType source);
        List<KeyValuePair<string, string>> GetPathList(DirectionType source);
        void UpdatePathList(DirectionType source, List<KeyValuePair<string,string>> ls);
    }
}
