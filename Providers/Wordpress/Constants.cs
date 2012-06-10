using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Contrib.ImportExport.Providers.Wordpress {
    class Constants {
        /// <summary>
        /// Parse an RSS format date/time to a C# DateTime.
        /// </summary>
        /// <param name="date">
        /// The date/time from the RSS feed.
        /// </param>
        /// <returns>
        /// The RSS time as a <see cref="System.DateTime"/>
        /// </returns>
        public static string ParseRssDate(string date) {
            return DateTime.ParseExact(date, "ddd, dd MMM yyyy HH:mm:ss zz00",
                                       (new CultureInfo("en-US")).DateTimeFormat).ToString("s");
        }

        /// <summary>
        /// Create a "slug" - all lower case, no spaces, no special characters.
        /// </summary>
        /// <param name="text">
        /// The text to turn into a slug
        /// </param>
        /// <returns>
        /// The slug
        /// </returns>
        public static string Slug(string text) {
            return Regex.Replace(text, @"[^\w\.-]", "-").ToLower();
        }
    }
}