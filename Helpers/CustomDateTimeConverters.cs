using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace BackEndGasApp.Helpers
{
    /// <summary>
    /// Custom converter to handle \N as NULL values
    /// </summary>
    public class StringNullConverter : StringConverter
    {
        public override object ConvertFromString(
            string text,
            IReaderRow row,
            MemberMapData memberMapData
        )
        {
            // Handle \N as NULL
            if (string.IsNullOrEmpty(text) || text == "\\N")
            {
                return null;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }
    }

    /// <summary>
    /// Custom converter to handle \N as NULL for int? type
    /// </summary>
    public class NullableIntConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(
            string text,
            IReaderRow row,
            MemberMapData memberMapData
        )
        {
            // Handle \N as NULL
            if (string.IsNullOrEmpty(text) || text == "\\N")
            {
                return null;
            }

            if (int.TryParse(text, out int result))
            {
                return result;
            }

            return null;
        }
    }

    /// <summary>
    /// Custom converter to handle \N as NULL for double? type
    /// </summary>
    public class NullableDoubleConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(
            string text,
            IReaderRow row,
            MemberMapData memberMapData
        )
        {
            // Handle \N as NULL
            if (string.IsNullOrEmpty(text) || text == "\\N")
            {
                return null;
            }

            if (
                double.TryParse(
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double result
                )
            )
            {
                return result;
            }

            return null;
        }
    }

    public class CustomDateTimeConverter : DefaultTypeConverter
    {
        private static readonly string[] DateFormats = new[]
        {
            "yyyy-MM-dd", // ISO format (priority)
            "yyyy/MM/dd", // Alternative ISO format
            "dd/MM/yyyy", // European format
            "MM/dd/yyyy", // US format
            "dd-MM-yyyy", // Alternative European format
            "dd.MM.yyyy", // Another common format
        };

        public override object ConvertFromString(
            string text,
            IReaderRow row,
            MemberMapData memberMapData
        )
        {
            // Handle \N as NULL
            if (string.IsNullOrEmpty(text) || text == "\\N")
                return null;

            // First try the ISO format directly since we know that's the expected format
            if (
                DateTime.TryParseExact(
                    text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime dateValue
                )
            )
            {
                return dateValue;
            }

            // Fall back to other formats if needed
            if (
                DateTime.TryParseExact(
                    text,
                    DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateValue
                )
            )
            {
                return dateValue;
            }

            if (
                DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateValue
                )
            )
            {
                return dateValue;
            }

            throw new Exception(
                $"Cannot convert '{text}' to DateTime using any of the supported formats."
            );
        }
    }

    public class CustomNullableDateTimeConverter : DefaultTypeConverter
    {
        private readonly string[] DateFormats = new[]
        {
            "dd/MM/yyyy", // European format (25/08/1993) - priority based on sample data
            "yyyy-MM-dd", // ISO format
            "yyyy/MM/dd", // Alternative ISO format
            "MM/dd/yyyy", // US format (6/15/1940)
            "M/d/yyyy", // US format without leading zeros
            "d/M/yyyy", // European format without leading zeros
            "dd-MM-yyyy", // Alternative European format
            "dd.MM.yyyy", // Another common format
        };

        public override object ConvertFromString(
            string text,
            IReaderRow row,
            MemberMapData memberMapData
        )
        {
            // Handle \N or empty values as NULL
            if (string.IsNullOrEmpty(text) || text == "\\N")
                return null;

            try
            {
                // First try European format (DD/MM/YYYY) parsing which matches the sample data
                if (
                    DateTime.TryParseExact(
                        text,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal | DateTimeStyles.NoCurrentDateDefault,
                        out DateTime dateValue
                    )
                )
                {
                    // Create a datetime at midnight with UTC kind for PostgreSQL compatibility
                    return new DateTime(
                        dateValue.Year,
                        dateValue.Month,
                        dateValue.Day,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc
                    );
                }

                // Try other formats if needed
                if (
                    DateTime.TryParseExact(
                        text,
                        DateFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal | DateTimeStyles.NoCurrentDateDefault,
                        out dateValue
                    )
                )
                {
                    return new DateTime(
                        dateValue.Year,
                        dateValue.Month,
                        dateValue.Day,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc
                    );
                }

                // Last fallback to general parsing
                if (
                    DateTime.TryParse(
                        text,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal | DateTimeStyles.NoCurrentDateDefault,
                        out dateValue
                    )
                )
                {
                    return new DateTime(
                        dateValue.Year,
                        dateValue.Month,
                        dateValue.Day,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc
                    );
                }
            }
            catch
            {
                // If any error occurs during parsing, return null
                return null;
            }

            // If we couldn't parse the date, return null
            return null;
        }
    }
}
