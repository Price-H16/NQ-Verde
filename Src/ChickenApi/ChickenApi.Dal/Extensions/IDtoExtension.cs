using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace ChickenAPI.DAL.Extensions
{
    public static class DtoExtensions
    {
        #region Methods

        public static object GetKey<T>(this T obj) where T : IDto => DtoKeyHelper<T>.KeyProperty.GetValue(obj);

        #endregion

        #region Classes

        private class DtoKeyHelper<T>
        {
            #region Instantiation

            static DtoKeyHelper()
            {
                PropertyInfo keyProperty = null;
                foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (property.GetCustomAttribute<KeyAttribute>() == null)
                    {
                        continue;
                    }

                    if (keyProperty != null)
                    {
                        throw new ArgumentException("You can't have multiple KeyAttribute in your object");
                    }

                    keyProperty = property;
                }

                KeyProperty = keyProperty ?? throw new ArgumentException("Dto should at least contain one");

                ParameterExpression par = Expression.Parameter(typeof(T));

                MethodInfo get = typeof(PropertyInfo).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(object) }, null);
                ConstantExpression prop = Expression.Constant(keyProperty);
                MethodCallExpression getValueExpr = Expression.Call(prop, get, par);

                Expression<Func<T, object>> lambda = Expression.Lambda<Func<T, object>>(getValueExpr, par);

                //GetKey = lambda.Compile();
            }

            #endregion

            #region Properties

            public static PropertyInfo KeyProperty { get; }

            #endregion
        }

        #endregion
    }
}