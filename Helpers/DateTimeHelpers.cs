using System;
using System.Globalization;

namespace Contrib.ImportExport.Helpers {
    public static class DateTimeHelpers {
        public static bool IsEarlierThan(this DateTime dateTime, DateTime? dateTime2) {
            if (!dateTime2.HasValue)
                return false;

            return dateTime.CompareTo(dateTime2.Value) < 0;
        }

        public static bool IsLaterThan(this DateTime dateTime, DateTime? dateTime2) {
            if (!dateTime2.HasValue)
                return false;

            return dateTime.CompareTo(dateTime2.Value) > 0;
        }

        public static bool IsNotEmpty(this DateTime dateTime) {
            return !dateTime.ToString(CultureInfo.InvariantCulture).Equals(new DateTime().ToString(CultureInfo.InvariantCulture));
        }
    }
}
