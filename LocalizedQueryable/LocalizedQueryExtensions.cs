using System.Linq;

namespace EFCoreLocalizationPoC.LocalizedQueryable
{
    public static class LocalizedQueryExtensions
    {
        public static ILocalizedQueryable<T> Localize<T>(this IQueryable<T> query, string culture)
        {
            return new LocalizedQueryable<T>(query, culture);
        }

        public static ILocalizedQueryable Localize(this IQueryable query, string culture)
        {
            return new LocalizedQueryable(query, culture);
        }
    }
}
