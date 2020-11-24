using EFCoreLocalizationPoC.Common.Data;
using EFCoreLocalizationPoC.Common.Domain.ValueObjects.Localization;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EFCoreLocalizationPoC.LocalizedQueryable
{
    internal class LocalizationExpressionVisitor : ExpressionVisitor
    {
        private static readonly ConcurrentDictionary<PropertyInfo, PropertyLocalizationMapping> PropertyLocalizationMappings = new ConcurrentDictionary<PropertyInfo, PropertyLocalizationMapping>();

        private bool directlyInSelectMethod;

        private readonly string _culture;

        public LocalizationExpressionVisitor(string culture)
        {
            _culture = culture ?? throw new ArgumentNullException(nameof(culture));
        }

        protected override Expression VisitNew(NewExpression node)
        {
            directlyInSelectMethod = false;

            return Expression.New(node.Constructor, node.Arguments.Select(arg =>
            {
                var rightSide = arg as MemberExpression;
                var property = rightSide?.Member as PropertyInfo;

                if (property != null)
                {
                    PropertyLocalizationMapping mapping = GetLocalizationMapping(property);
                    if (mapping != null)
                    {
                        var localizationExpresison = GetLocalizationExpression(rightSide, mapping, _culture);
                        if (localizationExpresison != null)
                            return localizationExpresison;
                    }
                }

                return Visit(arg);
            }));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var property = node.Member as PropertyInfo;

            if (directlyInSelectMethod)
            {
                PropertyLocalizationMapping mapping = GetLocalizationMapping(property);
                if (mapping != null)
                {
                    var localizationExpresison = GetLocalizationExpression(node, mapping, _culture, property.PropertyType);
                    if (localizationExpresison != null)
                        return localizationExpresison;
                }

                return base.VisitMember(node);
            }

            return base.VisitMember(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            directlyInSelectMethod = false;

            var rightSide = node.Expression as MemberExpression;
            if (rightSide is null && node.Expression != null && node.Expression.NodeType == ExpressionType.Convert)
                rightSide = ((UnaryExpression)node.Expression).Operand as MemberExpression;

            var property = rightSide?.Member as PropertyInfo;

            if (property != null)
            {
                PropertyLocalizationMapping mapping = GetLocalizationMapping(property);
                if (mapping != null)
                {
                    var localizationExpresison = GetLocalizationExpression(rightSide, mapping, _culture);
                    if (localizationExpresison != null)
                        node = Expression.Bind(node.Member, localizationExpresison);
                }

                return base.VisitMemberAssignment(node);
            }

            return base.VisitMemberAssignment(node);
        }

        private static Expression GetLocalizationExpression(MemberExpression memberExpression, PropertyLocalizationMapping mapping,
            string culture, Type customReturnType = null)
        {
            // this method transforms member expression { e.Property.Localized }
            // to { EntityFrameworkJsonExtensions.Value(e.Property.Serialized, "$.culture") ?? EntityFrameworkJsonExtensions.Value(e.Property.Serialized, "$.default") }

            var baseLocalizationProperty = memberExpression.Expression; // {e.Property}

            var serializedLocalizationProperty = Expression.Property(baseLocalizationProperty, mapping.SerializedPropertyInfo); // {e.Property.Serialized}

            var localizedExpression = Expression.Call(typeof(EntityFrameworkJsonExtensions), nameof(EntityFrameworkJsonExtensions.Value)
                , null, serializedLocalizationProperty, Expression.Constant($"$.{culture}"));
            var defaultExpression = Expression.Call(typeof(EntityFrameworkJsonExtensions), nameof(EntityFrameworkJsonExtensions.Value)
                , null, serializedLocalizationProperty, Expression.Constant($"$.{LocalizedValueObject.DefaultKey}"));

            var localizedOrDefaultExpression = Expression.Coalesce(
                localizedExpression,
                defaultExpression);

            var result = (customReturnType != null)
                ? Expression.Convert(localizedOrDefaultExpression, customReturnType) as Expression
                : localizedOrDefaultExpression;

            return result;
        }

        private PropertyLocalizationMapping GetLocalizationMapping(PropertyInfo sourceProperty)
        {
            if (sourceProperty is null)
                return null;
            else if (sourceProperty.PropertyType != typeof(string))
                return null;

            var replaceNodeAttribute = sourceProperty
                .GetCustomAttributes(typeof(LocalizedQueryPlaceHolderAttribute), false)
                .Cast<LocalizedQueryPlaceHolderAttribute>()
                .FirstOrDefault();
            if (replaceNodeAttribute is null)
                return null;

            return PropertyLocalizationMappings.GetOrAdd(sourceProperty, _ =>
            {
                if (sourceProperty.ReflectedType is null || sourceProperty.ReflectedType != typeof(LocalizedValueObject))
                    return null;

                var mapping = new PropertyLocalizationMapping
                {
                    ValueObjectType = sourceProperty.ReflectedType,
                };

                mapping.SerializedPropertyInfo = mapping.ValueObjectType.GetProperties().FirstOrDefault(p =>
                    p.PropertyType == typeof(string) && p.Name == nameof(LocalizedValueObject.Serialized));

                if (mapping.SerializedPropertyInfo is null)
                    return null;

                return mapping;
            });
        }

        private class PropertyLocalizationMapping
        {
            public Type ValueObjectType { get; set; }
            public PropertyInfo SerializedPropertyInfo { get; set; }
        }
    }
}
