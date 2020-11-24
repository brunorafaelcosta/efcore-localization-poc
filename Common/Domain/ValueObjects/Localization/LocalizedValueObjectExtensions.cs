using System.Collections.Generic;
using System.Linq;

namespace EFCoreLocalizationPoC.Common.Domain.ValueObjects.Localization
{
    public static class LocalizedValueObjectExtensions
    {
        public static LocalizedValueObject Create(string defaultValue, IDictionary<string, string> localizedValues = null)
        {
            var obj = new LocalizedValueObject();

            obj[LocalizedValueObject.DefaultKey] = defaultValue;

            if (localizedValues != null && localizedValues.Any())
            {
                foreach (var localizedValue in localizedValues)
                    obj[localizedValue.Key] = localizedValue.Value;
            }

            return obj;
        }
    }
}
