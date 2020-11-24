using System;

namespace EFCoreLocalizationPoC.Common.Data
{
    public static class EntityFrameworkJsonExtensions
    {
        public static string Value(string expression, string path)
            => throw new InvalidOperationException($"{nameof(Value)} cannot be called");
    }
}
