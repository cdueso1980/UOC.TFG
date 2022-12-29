using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace UOC
{
    public static class PropertiesValuesExtensions
    {
        public static T SetPropertyValue<T, TReturnType>(this T source, Expression<Func<T, TReturnType>> propertyExpression, TReturnType value)
        {
            return source.SetPropertyValue(source.GetPropertyName(propertyExpression), value);
        }

        public static T SetPropertyValue<T, TReturnType>(this T source, string propertyName, TReturnType value)
        {
            var sourceType = source.GetType();
            var propertyInfo = sourceType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object boxed = source;
            propertyInfo.SetValue(boxed, value, null);
            return (T)boxed;
        }

        public static TSource SetPropertyOnDefaultValue<TSource, TValue>(this TSource config, Expression<Func<TSource, TValue>> property, TValue defaultValue)
        {
            return config.SetPropertyOnDefaultValue(property, defaultValue, (a) => { });
        }

        public static TSource SetPropertyOnDefaultValue<TSource, TValue>(this TSource config, Expression<Func<TSource, TValue>> property, TValue defaultValue, Action<TSource> onDefaultValue)
        {
            var currentValue = config.GetPropertyValue(property);
            TSource result = config;

            if(currentValue.IsDefault())
            {
                result = (TSource)config.SetPropertyValue(property, defaultValue);
                onDefaultValue(result);
            }
            
            return result;
        }

        public static TValue GetPropertyValue<T, TValue>(this T source, Expression<Func<T, TValue>> propertyExpression)
        {
            var sourceType = source.GetType();
            var propertyInfo = sourceType.GetProperty(source.GetPropertyName(propertyExpression), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (TValue)propertyInfo.GetValue(source);
        }

        public static string GetPropertyName<TSource, TReturnType>(this TSource obj, Expression<Func<TSource, TReturnType>> propertyExpression)
        {
            var visitor = new ExprVisitor();
            visitor.Visit(propertyExpression);
            if (visitor.IsFound)
                return visitor.MemberName;
            else
                throw new ArgumentException(string.Format("Expression is not a MemberExpression to: {0}", obj), "propertyExpression");
        }
		
		public static string GetPropertyName<TSource, TReturnType>(this Expression<Func<TSource, TReturnType>> propertyExpression)
        {
            var visitor = new ExprVisitor();
            visitor.Visit(propertyExpression);
            if (visitor.IsFound)
                return visitor.MemberName;
            else
                throw new ArgumentException("Expression is not a MemberExpression", "propertyExpression");
        }

        internal class ExprVisitor : ExpressionVisitor
        {
            public bool IsFound { get; private set; }
            public string MemberName { get; private set; }
            protected override Expression VisitMember(MemberExpression node)
            {
                if (!IsFound && node.Member.MemberType == MemberTypes.Property)
                {
                    IsFound = true;
                    MemberName = node.Member.Name;
                }
                return base.VisitMember(node);
            }
        }

        private static bool IsDefault<TSource>(this TSource source) 
        {
			return EqualityComparer<TSource>.Default.Equals(source, default(TSource));
		}
    }


}