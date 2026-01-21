using System.Linq.Expressions;

namespace RetailCorrector.Utils.Expressions;

public static class EnumerableExpression
{
    public static Expression Any<T>(Expression collection, Expression predicate)
    {
        var param = Expression.Parameter(typeof(T), "item");
        var lambda = Expression.Lambda(predicate, param);

        var allMethod = typeof(Enumerable).GetMethods()
            .Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T));

        return Expression.Call(allMethod, collection, lambda);
    }
    
    public static Expression All<T>(Expression collection, LambdaExpression predicate)
    {
        var allMethod = typeof(Enumerable).GetMethods()
            .Single(m => m.Name == nameof(Enumerable.All) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T));

        return Expression.Call(allMethod, collection, predicate);
    }
}