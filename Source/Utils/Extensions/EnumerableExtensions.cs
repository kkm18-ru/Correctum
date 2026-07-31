namespace Correctum.Utils.Extensions;

public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> enumerable)
    {
        public void ForEach(Action<T> action)
        {
            foreach (var element in enumerable)
                action.Invoke(element);
        }
    }
}
