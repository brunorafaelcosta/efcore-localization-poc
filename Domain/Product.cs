using EFCoreLocalizationPoC.Common.Domain.ValueObjects.Localization;

namespace EFCoreLocalizationPoC.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public LocalizedValueObject Name { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
