using System.Collections.Generic;
using System.Linq;
using CopyToLocales.Postgressql._Interfaces_;
using CopyToLocales.Postgressql.Dto;

namespace CopyToLocales.Postgressql
{
    public class LocalizationDataBase : ILocalizationDataBase
    {
        public IList<LocalizationStringModel> GetLocalization()
        {
            using (var dataBase = new ApplicationContext())
            {
                var localizationStringModels = dataBase.LocalizationStringModels.ToList();
                return localizationStringModels;
            }
        }
    }
}