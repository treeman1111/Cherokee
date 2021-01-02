using System.Collections.Generic;
using System.Linq;

namespace Cherokee.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string str)
        {
            if (str == null) return false;
            if (str.Length == 0) return false;
            return str.All(c => char.IsDigit(c));
        }

        public static IEnumerable<int> AllIndicesOf(this string str, char value)
        {
            if (str == null) yield break;
            if (!str.Contains(value)) yield break;

            var idx = str.IndexOf(value);

            while (idx > 0)
            {
                yield return idx;
                idx = str.IndexOf(value, idx + 1);
            }
        }

        public static string ReplaceFirst(this string str, string toReplace, string replaceBy)
        {
            if (str == null) return null;

            var indexOf = str.IndexOf(toReplace);

            if (indexOf == -1) return str;

            return str.Substring(0, indexOf) + replaceBy + str.Substring(indexOf + toReplace.Length);
        }
    }
}
