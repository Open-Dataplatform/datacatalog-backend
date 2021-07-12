using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Extensions
{
    public static class ReflectionExtensions
    {
        public static void SetInternalProperty<TType, TPropertyType>(
            this TType instance,
            Expression<Func<TType, TPropertyType>> propertyExpression, 
            TPropertyType newValue)
        {
            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
            
            typeof(TType).GetProperty(propertyName)
                ?.SetValue(instance, newValue, null);
        }

        public static T CreateInstance<T>(params object[] args)
        {
            var type = typeof(T);
            var instance = type.Assembly.CreateInstance(
                type.FullName, false,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, args, null, null);
            return (T)instance;
        }
    }
}