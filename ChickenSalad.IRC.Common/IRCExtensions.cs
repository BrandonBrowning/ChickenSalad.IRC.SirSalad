using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenSalad.IRC.Common
{
    public static class IRCExtensions
    {
        public static bool In<T>(this T item, params T[] items)
        {
            return items.Contains(item);
        }

        public static IEnumerable<T> Unfold<T>(this T value, Func<T, Unfolder<T>> f)
        {
            var output = value;

            while (true)
            {
                var unfolder = f(output);

                if (unfolder.Success)
                {
                    yield return output;
                    output = unfolder.NextValue;
                }
                else
                {
                    break;
                }
            }
        }

        private static int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        /// <summary>
        /// Credit: http://stackoverflow.com/a/9658378 Adam S
        /// </summary>
        public static IEnumerable<IList<T>> Powerset<T>(this IList<T> startingSet, int minSubsetSize = 0)
        {
            int startingCount = startingSet.Count();

            var startingSetIndexes = Enumerable.Range(0, startingCount).ToList();

            var candidates = Enumerable.Range((1 << minSubsetSize) - 1, 1 << startingCount)
                                       .Where(p => NumberOfSetBits(p) >= minSubsetSize)
                                       .ToList();

            foreach (int p in candidates)
            {
                yield return startingSetIndexes
                    .Where(setInd => (p & (1 << setInd)) != 0)
                    .Select(setInd => startingSet[setInd])
                    .ToList();
            }
        }
    }
}
