using EFCoreLocalizationPoC.Common.Data;
using EFCoreLocalizationPoC.Common.Domain.ValueObjects.Localization;
using EFCoreLocalizationPoC.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCoreLocalizationPoC.Data
{
    public class MyDbContext : DbContext
    {
        static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => 
            builder.AddDebug().AddFilter((category, level) => level == LogLevel.Information && !category.EndsWith("Connection")));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);

            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=EFCoreLocalizationPoC;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddJsonDbFunctionTranslation();

            modelBuilder.Owned<LocalizedValueObject>();

            modelBuilder.Entity<Product>(conf =>
            {
                conf.OwnsOne(p => p.Name);
            });
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
