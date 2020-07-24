using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    static class Extensions
    {
        static readonly Random r = new Random();

        // Blantently lifted from https://stackoverflow.com/a/59642186/404006 Allows for fancy splitting when outputting to CSV.
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int numberOfItemsPerGroup)
        {
            return source.Select((x, index) => new { Value = x, Index = index })
                  .GroupBy(x => (int)(x.Index / numberOfItemsPerGroup))
                  .Select(x => x.Select(c => c.Value));

        }

        public static T AnyOne<T>(this T[] ra) where T : class
        {
            int k = ra.Length;
            int ri = r.Next(0, k);
            return ra[ri];
        }

        //Taken from https://stackoverflow.com/a/110570/404006 to randomly shuffle an array
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
