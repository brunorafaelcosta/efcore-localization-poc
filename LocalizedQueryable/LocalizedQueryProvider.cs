using System.Linq;
using System.Linq.Expressions;

namespace EFCoreLocalizationPoC.LocalizedQueryable
{
    public class LocalizedQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider baseProvider;
        private readonly ExpressionVisitor visitor;

        private readonly string _culture;

        public LocalizedQueryProvider(IQueryProvider baseProvider, ExpressionVisitor visitor, string culture)
        {
            this.baseProvider = baseProvider;
            this.visitor = visitor;

            _culture = culture;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new LocalizedQueryable<TElement>(baseProvider.CreateQuery<TElement>(expression), _culture);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new LocalizedQueryable(baseProvider.CreateQuery(expression), _culture);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return baseProvider.Execute<TResult>(visitor.Visit(expression));
        }

        public object Execute(Expression expression)
        {
            return baseProvider.Execute(visitor.Visit(expression));
        }
    }
}
