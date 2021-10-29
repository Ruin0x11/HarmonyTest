using System;
using System.Text.Unicode;

namespace OpenNefia.Core.Extensions
{
    public static class StringExtensions
    {
        public static LocaleKey ToLocaleKey(this string str) => new LocaleKey(str);

        public static int GetWideLength(this string str) => UnicodeWidth.GetWidthCJK(str);
        public static int GetWideWidth(this char c) => UnicodeWidth.GetWidth(c);

        public static string WideSubstring(this string str, int? startIndex = null, int? length = null)
        {
            int? boundLeft = null;
            bool foundRight = false;

            if (startIndex == null)
            {
                boundLeft = 0;
                startIndex = 0;
            }
            if (length == null)
            {
                length = str.GetWideLength();
            }

            int i = startIndex.Value;
            int innerLength = 0;
            int normalLength = 0;
            var len = 0;

            for (int pos = 0; pos <= str.Length; pos++)
            {
                if (boundLeft == null && i < 1)
                    boundLeft = pos;

                if (boundLeft != null && foundRight)
                    break;

                if (pos >= str.Length)
                    break;

                len = str[pos].GetWideWidth();
                i -= len;

                if (innerLength + len > length)
                {
                    foundRight = true;
                    break;
                }

                if (boundLeft != null)
                {
                    normalLength += 1;
                    innerLength += len;
                }
            }

            if (boundLeft == null)
                boundLeft = str.Length;
            if (!foundRight)
                return str.Substring(boundLeft.Value);

            return str.Substring(boundLeft.Value, normalLength);
        }
    }
}
