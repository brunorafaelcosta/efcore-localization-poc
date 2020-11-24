using System.Collections.Generic;

namespace EFCoreLocalizationPoC.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
