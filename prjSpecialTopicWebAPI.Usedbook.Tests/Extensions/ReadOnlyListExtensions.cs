namespace prjSpecialTopicWebAPI.Usedbook.Tests.Extensions
{
    public static class ReadOnlyListExtensions
    {
        public static T? Pick<T>(this IReadOnlyList<T> list)
        {
            if (list is null) throw new ArgumentNullException(nameof(list));
            int n = list.Count;
            if (n == 0) return default;

            int idx = Random.Shared.Next(n); // [0, n)
            return list[idx];
        }
    }
}