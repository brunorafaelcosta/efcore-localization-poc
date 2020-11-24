using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EFCoreLocalizationPoC.Common.Data
{
    public static class EntityFrameworkModelBuilderExtensions
    {
        public static DbFunctionBuilder AddJsonDbFunctionTranslation(this ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
                throw new ArgumentNullException(nameof(modelBuilder));

            var efJsonValueMethodInfo = typeof(EntityFrameworkJsonExtensions)
                .GetRuntimeMethod(
                    nameof(EntityFrameworkJsonExtensions.Value),
                    new[] { typeof(string), typeof(string) }
            );

            return
                modelBuilder
                    .HasDbFunction(efJsonValueMethodInfo)
                    .HasTranslation(args =>
                        new SqlFunctionExpression(
                            "JSON_VALUE",
                            args,
                            true,
                            new List<bool>(),
                            typeof(string),
                            null
                        )
                    );
        }
    }
}
