namespace prjSpecialTopicWebAPI.Usedbook.Tests.Extensions
{
    public static class ListExtensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);                    // 產生 [0, n] 的隨機索引
                (list[k], list[n]) = (list[n], list[k]);    // 交換元素
            }
        }
    }
}
