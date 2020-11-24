using EFCoreLocalizationPoC.Common.Domain.ValueObjects.Localization;
using EFCoreLocalizationPoC.Data;
using EFCoreLocalizationPoC.Domain;
using EFCoreLocalizationPoC.Dto;
using EFCoreLocalizationPoC.LocalizedQueryable;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreLocalizationPoC
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new MyDbContext())
            {
                if (!db.Categories.Any())
                    CreateSampleData(db);

                //// update
                //var product13 = db.Products.FirstOrDefault(p => p.Id == 13);
                //product13.Name["default"] = $"UPDATED PRODUCT 13 NAME";
                ////product13.Name["default"] = null;
                //product13.Name["fr"] = null;
                //db.Products.Update(product13);
                //db.SaveChanges();

                // read
                var products = db.Products
                    .AsNoTracking()
                    .Localize("fr")
                    .Select(ProductDto.Projection)
                    .Where(x => x.Name.Contains("999"))
                    .OrderBy(p => p.Name)
                    .Skip(0).Take(100)
                    .ToList();
            }
        }

        static void CreateSampleData(MyDbContext dbContext)
        {
            // categories
            var nCategories = 500;
            var categories = new List<Category>();
            for (int i = 0; i < nCategories; i++)
            {
                categories.Add(new Category
                {
                    Name = $"CATEGORY {i}"
                });
            }
            dbContext.Categories.AddRange(categories);
            dbContext.SaveChanges();

            // products
            int nProducts = 15000;
            var products = new List<Product>();
            for (int i = 0; i < nProducts; i++)
            {
                var randomCategoryIndex = Convert.ToInt32(new Random().NextDouble() * (nCategories - 1) + 0);
                products.Add(new Product
                {
                    Name = LocalizedValueObjectExtensions.Create($"PRODUCT NAME {i}", new Dictionary<string, string>
                    {
                        { "pt", $"NOME PRODUTO {i}" },
                        { "fr", $"NOM PRODUIT {i}" },
                        { "es", $"NOMBRE PRODUCTO {i}" }
                    }),
                    Description = $"DESCRIPTION {i}",
                    Category = categories.ElementAt(randomCategoryIndex)
                });
            }
            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();
        }
    }
}
