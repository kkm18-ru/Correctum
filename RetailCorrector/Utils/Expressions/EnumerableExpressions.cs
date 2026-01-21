using System.Linq.Expressions;

namespace RetailCorrector.Utils.Expressions;

public static class EnumerableExpressions
{
    public static BinaryExpression Any<T>(Expression collection, Expression predicate)
    {
        var param = Expression.Parameter(typeof(T), "item");
        var lambda = Expression.Lambda(predicate, param);

        var allMethod = typeof(Enumerable).GetMethods()
            .Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T));
        return Expression.Equal(Expression.Call(allMethod, collection, lambda), Expression.Constant(true));
    }
    
    public static BinaryExpression All<T>(Expression collection, LambdaExpression predicate)
    {
        var param = Expression.Parameter(typeof(T), "item");
        var lambda = Expression.Lambda(predicate, param);

        var allMethod = typeof(Enumerable).GetMethods()
            .Single(m => m.Name == nameof(Enumerable.All) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T));
        return Expression.Equal(Expression.Call(allMethod, collection, lambda), Expression.Constant(true));
    }
}