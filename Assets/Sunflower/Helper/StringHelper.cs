using System.Text.RegularExpressions;

namespace Sunflower.Helper
{
    public class StringHelper
    {
        public static string SlicingFrom(string str, string from)
        {
            int index = str.IndexOf(from);
            if (index != -1)
            {
                return str.Substring(index + from.Length);
            }
            return null;
        }

        public static string SlicingFromWithParam(string str, string from)
        {
            int index = str.IndexOf(from);
            if (index != -1)
            {
                return string.Concat(from, str.Substring(index + from.Length));
            }
            return null;
        }
    }
}
