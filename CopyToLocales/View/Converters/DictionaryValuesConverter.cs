using CopyToLocale.Core;

namespace CopyToLocales.View
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    public class DictionaryValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return string.Empty;

            if (!(values[0] is SupportedLocalization key))
                return values[0].ToString();
            
            if (!(values[1] is  Dictionary<SupportedLocalization, string> dict))
                return values[1].ToString();

            if (dict.ContainsKey(key))
                return dict[key];
            
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}