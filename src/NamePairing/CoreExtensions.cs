using System.Text;

namespace NamePairing
{
    public static class CoreExtensions
    {
        public static string ToHuman(this int idx)
        {
            return (idx + 1).ToString();
        }

        private static Random rng = new Random();

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }

        public static T GetRandom<T>(this IEnumerable<T> input) {
            var list = input.ToList();
            return list.ElementAt(ThreadSafeRandom.ThisThreadsRandom.Next(list.Count()));
        }

        public static string EncodeToBase64(this string input) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string DecodeFromBase64(this string encoded) {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random? _local;

        public static Random ThisThreadsRandom
        {
            get { return _local ??= new Random(unchecked(Environment.TickCount * 31 + Environment.CurrentManagedThreadId)); }
        }
    }
}