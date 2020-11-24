using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFCoreLocalizationPoC.LocalizedQueryable
{
    internal class LocalizedQueryable<T> : ILocalizedQueryable<T>
    {
        private readonly ExpressionVisitor _queryRewriteVisitor;
        private readonly IQueryable<T> _wrappedQuery;

        public LocalizedQueryable(IQueryable<T> query, string culture)
        {
            _wrappedQuery = query;
            _queryRewriteVisitor = new LocalizationExpressionVisitor(culture);

            ElementType = typeof(T);
            Expression = query.Expression;
            Provider = new LocalizedQueryProvider(query.Provider, _queryRewriteVisitor, culture);
        }

        public override string ToString()
        {
            return GetRewritenQuery().ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetRewritenQuery().GetEnumerator();
        }

        private IQueryable<T> GetRewritenQuery()
        {
            return _wrappedQuery
                .Provider
                .CreateQuery<T>(_queryRewriteVisitor.Visit(_wrappedQuery.Expression));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }
    }

    internal class LocalizedQueryable : ILocalizedQueryable
    {
        private readonly IQueryable _wrappedQuery;
        private readonly ExpressionVisitor _translationVisitor;

        public LocalizedQueryable(IQueryable query, string culture)
        {
            _wrappedQuery = query;
            _translationVisitor = new LocalizationExpressionVisitor(culture);
            
            ElementType = query.ElementType;
            Expression = query.Expression;
            Provider = new LocalizedQueryProvider(query.Provider, _translationVisitor, culture);
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public IEnumerator GetEnumerator()
        {
            return
                _wrappedQuery
                    .Provider
                    .CreateQuery(_translationVisitor.Visit(_wrappedQuery.Expression))
                    .GetEnumerator();
        }
    }
}
