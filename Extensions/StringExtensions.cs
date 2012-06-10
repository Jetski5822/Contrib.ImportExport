using System;

namespace Contrib.ExternalImportExport.Extensions {
    public static class StringExtensions {
        public static bool IsValidUrl(this string text) {
            Uri temp;
            return Uri.TryCreate(text, UriKind.Absolute, out temp);
        }

        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        public static string Truncate(this string source, int length) {
            if (source.Length > length) {
                source = source.Substring(0, length);
            }
            return source;
        }

        /// <summary>
        /// Get a substring of the first N characters. [Slow]
        /// </summary>
        public static string Truncate2(this string source, int length) {
            return source.Substring(0, Math.Min(length, source.Length));
        }

    }
}