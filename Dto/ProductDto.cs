using EFCoreLocalizationPoC.Domain;
using System;
using System.Linq.Expressions;

namespace EFCoreLocalizationPoC.Dto
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        #region Projection

        public static Expression<Func<Product, ProductDto>> Projection
        {
            get
            {
                return e => new ProductDto
                {
                    Name = e.Name.Localized,
                    Description = e.Description
                };
            }
        }

        #endregion Projection
    }
}
