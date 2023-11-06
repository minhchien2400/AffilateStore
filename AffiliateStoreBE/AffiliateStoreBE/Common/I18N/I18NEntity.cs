using Microsoft.Extensions.Localization;
using System.Globalization;

namespace AffiliateStoreBE.Common.I18N
{
    public static class I18NEntity
    {
        public static LocalizedString GetString(string key)
        {
            return I18NResource.GetString(key);
        }
        public static LocalizedString GetString(string key, CultureInfo cultureInfo)
        {
            return I18NResource.GetString(key, cultureInfo);
        }
        public static LocalizedString GetString(string key, params object[] args)
        {
            return I18NResource.GetString(key, args);
        }
    }
}
