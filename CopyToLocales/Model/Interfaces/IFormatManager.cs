namespace CopyToLocales.Model.Interfaces
{
    using System.Collections.Generic;
    using CopyToLocales.ViewModel;
    using CopyToLocales.ViewModel.Common;
    using CopyToLocales.ViewModel.Enums;

    public interface IFormatManager
    {
        FormatTypes FormatType { get; }
        Dictionary<string, ResourceModel> InitReaders(IList<SelectFileViewModel> selectFileViewModel);
        void Update(List<ChangedResourceViewModel> forUpdate, IList<SelectFileViewModel> targetFileList);
        List<string> GetAnotherLanguages(string path);
    }
}
