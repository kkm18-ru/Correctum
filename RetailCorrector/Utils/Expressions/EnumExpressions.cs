using System.Linq.Expressions;

namespace RetailCorrector.Utils.Expressions;

public static class EnumExpressions
{
    public static BinaryExpression HasFlag(Expression property, Expression value)
    { 
        var method = typeof(Enum).GetMethod(nameof(Enum.HasFlag), [typeof(Enum)])!;
        return Expression.Equal(Expression.Call(property, method, value), Expression.Constant(true));
    }
}