using System;
using System.Linq.Expressions;

namespace Provausio.Data.Collections
{
    internal static class ExpressionReplacer
    {
        /// <summary>
        /// Composes the specified second.
        /// </summary>
        /// <typeparam name="TFirstParam">The type of the first parameter.</typeparam>
        /// <typeparam name="TIntermediate">The type of the intermediate.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static Expression<Func<TFirstParam, TResult>> Compose<TFirstParam, TIntermediate, TResult>(
            this Expression<Func<TFirstParam, TIntermediate>> first, // the member expression
            Expression<Func<TIntermediate, TResult>> second) // the comparison expression
        {
            var param = Expression.Parameter(typeof(TFirstParam), "param");

            var newFirst = first.Body.Replace(first.Parameters[0], param);
            var newSecond = second.Body.Replace(second.Parameters[0], newFirst);

            return Expression.Lambda<Func<TFirstParam, TResult>>(newSecond, param);
        }

        public static Expression Replace(this Expression expression, Expression searchExpr, Expression replaceExpr)
        {
            // replace all Search expressions with the replace expression
            return new ReplaceVisitor(searchExpr, replaceExpr).Visit(expression);
        }

        internal class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression _oldExpr;
            private readonly Expression _newExpr;

            public ReplaceVisitor(Expression oldExpr, Expression newExpr)
            {
                _oldExpr = oldExpr;
                _newExpr = newExpr;
            }

            public override Expression Visit(Expression node)
            {
                return node == _oldExpr
                    ? _newExpr
                    : base.Visit(node);
            }
        }
    }
}
