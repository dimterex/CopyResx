using System;
using System.Collections.Generic;
using CopyToLocale.Core.Models;

namespace CopyToLocale.Core.Modules
{
    public interface IFormatManager
    {
        event EventHandler<string> StateChangeEvent;
        
        string FormatType { get; }
        Dictionary<string, ResourceModel> InitReaders(IList<SelectFileModel> selectFileViewModel);
        void Update(List<ChangedResourceModel> forUpdate, IList<SelectFileModel> targetFileList);
        List<string> GetAnotherLanguages(string path);
    }
}
