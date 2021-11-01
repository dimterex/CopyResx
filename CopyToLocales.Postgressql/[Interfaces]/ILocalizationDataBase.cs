using System.Collections.Generic;
using CopyToLocales.Postgressql.Dto;

namespace CopyToLocales.Postgressql._Interfaces_
{
    public interface ILocalizationDataBase
    {
        IList<LocalizationStringModel> GetLocalization();
    }
}