using System.Linq.Expressions;

namespace RetailCorrector.Utils.Expressions;

public static class StringExpressions
{
    public static BinaryExpression StartsWith(Expression text, Expression value, bool ignoreCase)
    {
        var sc = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture; 
        var scType = typeof(StringComparison);
        var comparison = Expression.Constant(sc, scType);
        var method = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string), scType])!;
        return Expression.Equal(Expression.Call(text, method, value, comparison), Expression.Constant(true));
    }
        
    public static BinaryExpression EndsWith(Expression text, Expression value, bool ignoreCase)
    {
        var sc = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture; 
        var scType = typeof(StringComparison);
        var comparison = Expression.Constant(sc, scType);
        var method = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string), scType])!;
        return Expression.Equal(Expression.Call(text, method, value, comparison), Expression.Constant(true));
    }
        
    public static BinaryExpression Contains(Expression text, Expression value, bool ignoreCase)
    {
        var sc = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture; 
        var scType = typeof(StringComparison);
        var comparison = Expression.Constant(sc, scType);
        var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string), scType])!;
        return Expression.Equal(Expression.Call(text, method, value, comparison), Expression.Constant(true));
    }
}