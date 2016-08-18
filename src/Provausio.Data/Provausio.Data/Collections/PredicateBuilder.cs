using System;
using System.Linq.Expressions;

namespace Provausio.Data.Collections
{
    internal static class PredicateBuilder
    {
        /// <summary>
        /// Func(T) = true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// Func(T) = false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0],
                expr1.Parameters[0]);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expr1.Body, secondBody),
                expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0],
                expr1.Parameters[0]);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expr1.Body, secondBody),
                expr1.Parameters);
        }
    }
}
