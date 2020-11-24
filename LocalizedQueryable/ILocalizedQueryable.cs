using System.Linq;

namespace EFCoreLocalizationPoC.LocalizedQueryable
{
    public interface ILocalizedQueryable<out T> : IOrderedQueryable<T>
    {
    }

    public interface ILocalizedQueryable : IOrderedQueryable
    {
    }
}
