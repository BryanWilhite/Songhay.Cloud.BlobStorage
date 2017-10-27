using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Songhay.Cloud.BlobStorage.Models
{
    /// <summary>
    /// A hash table for mapping entity types to their lookup key properties.
    /// Used by data repositories.
    /// </summary>
    /// <remarks>
    /// Based on a class definition from NBlog by Chris Fulstow
    /// [https://github.com/ChrisFulstow/NBlog/blob/master/NBlog.Web/Application/Storage/RepositoryKeys.cs]
    /// </remarks>
    public class AzureBlobKeys
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobKeys"/> class.
        /// </summary>
        public AzureBlobKeys()
        {
            this._keyDictionary = new Dictionary<Type, Expression<Func<object, object>>>();
        }

        /// <summary>
        /// Adds the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        public void Add<T>(Expression<Func<T, object>> expression)
        {
            // use a conversion expression to convert Expression<Func<T, object>> to an Expression<Func<object, object>>:
            Expression<Func<object, T>> converter = obj => (T)obj;
            var param = Expression.Parameter(typeof(object));
            var body = Expression.Invoke(expression, Expression.Invoke(converter, param));
            var lambda = Expression.Lambda<Func<object, object>>(body, param);

            this._keyDictionary.Add(typeof(T), lambda);
        }

        /// <summary>
        /// Gets the name of the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public string GetKeyName<T>(T item)
        {
            return this.GetKeyName<T>();
        }


        /// <summary>
        /// Gets the name of the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public string GetKeyName<T>()
        {
            var expression = this._keyDictionary[typeof(T)];

            var conversionBody = (InvocationExpression)expression.Body;
            var conversionExpression = (Expression<Func<T, object>>)conversionBody.Expression;
            var body = (MemberExpression)conversionExpression.Body;
            var memberName = body.Member.Name;
            return memberName;
        }

        /// <summary>
        /// Gets the key value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public object GetKeyValue<T>(T item)
        {
            var getValue = this._keyDictionary[typeof(T)].Compile();
            return getValue(item);
        }

        readonly Dictionary<Type, Expression<Func<object, object>>> _keyDictionary;
    }
}
