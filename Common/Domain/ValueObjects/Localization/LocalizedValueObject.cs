using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EFCoreLocalizationPoC.Common.Domain.ValueObjects.Localization
{
    public class LocalizedValueObject : ValueObject
    {
        public string Serialized { get; set; }

        #region Localized values
        public const string DefaultKey = "default";

        [NotMapped]
        public IReadOnlyDictionary<string, string> Values
        {
            get { return Serialized == null ? new Dictionary<string, string>() : JsonConvert.DeserializeObject<Dictionary<string, string>>(Serialized); }
            protected set { Serialized = JsonConvert.SerializeObject(value); }
        }

        public string this[string culture]
        {
            get
            {
                return Values.FirstOrDefault(x => x.Key == culture).Value;
            }
            set
            {
                var valuesCopy = Values.ToDictionary(p => p.Key, p => p.Value);

                var existingValue = valuesCopy.FirstOrDefault(x => x.Key == culture);
                if (existingValue.Key != null)
                    valuesCopy.Remove(existingValue.Key);

                if (!string.IsNullOrEmpty(value))
                    valuesCopy.Add(culture, value);

                Values = valuesCopy;
            }
        }

        public bool HasCulture(string culture)
        {
            return Values.Any(x => x.Key == culture);
        }
        #endregion

        [LocalizedQueryPlaceHolder]
        public string Localized => Values.ContainsKey(DefaultKey) ? Values[DefaultKey] : default;

        public override string ToString()
        {
            return Values.ContainsKey(DefaultKey) ? Values[DefaultKey] : default;
        }
    }
}
