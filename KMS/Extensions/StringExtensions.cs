using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace KMS.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] DefaultCRLFChars = { '\r', '\n' };
        private static readonly char[] DigitsOnly = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static void OutputColoredLine(this string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{value}");
            Console.ResetColor();
        }

        public static string EnsureEndsWith(this string value, string requiredEnd, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(value))
            {
                return requiredEnd;
            }

            if (!value.EndsWith(requiredEnd, stringComparison))
            {
                return $"{value}{requiredEnd}";
            }

            return value;
        }

        public static string EnsureMinLengthOrLonger(this string value, int length, char fillChar = ' ')
        {
            return value.EnsureLength(length, fillChar, fillIfShorter: true, cropIfLonger: false);
        }

        public static string EnsureExactLength(this string value, int length, char fillChar = ' ')
        {
            return value.EnsureLength(length, fillChar, fillIfShorter: true, cropIfLonger: true);
        }

        public static string EnsureMaxLengthOrShorter(this string value, int length, char fillChar = ' ')
        {
            return value.EnsureLength(length, fillChar, fillIfShorter: false, cropIfLonger: true);
        }

        public static string EnsureLength(this string value, int length, char fillChar = ' ', bool fillIfShorter = true, bool cropIfLonger = false)
        {
            value ??= string.Empty;
            var maxLength = Math.Max(0, length);

            if (fillChar == 0x00)
            {
                fillChar = ' ';
            }

            if (value.Length > maxLength)
            {
                if (cropIfLonger)
                {
                    return value.Substring(0, maxLength);
                }
                else
                {
                    return value;
                }
            }
            else if (value.Length < maxLength)
            {
                if (fillIfShorter)
                {
                    // fill up with fillChar to reach the desired length
                    var sb = new StringBuilder(value);
                    for (var index = value.Length; index < maxLength; index++)
                    {
                        sb.Append(fillChar);
                    }

                    value = sb.ToString();
                }
                else
                {
                    return value;
                }
            }

            return value;
        }

        public static string RemoveIfEndsWith(this string value, string unwantedEnd, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(unwantedEnd))
            {
                return value;
            }

            if (value.EndsWith(unwantedEnd, stringComparison))
            {
                var foundIndex = value.Length - unwantedEnd.Length;
                return value.Substring(0, foundIndex);
            }

            return value;
        }

        public static string AppendCRLF(this string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (!value.EndsWith(Environment.NewLine))
            {
                return $"{value}{Environment.NewLine}";
            }

            return value;
        }

        public static string TrimCRLF(this string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim(DefaultCRLFChars);
        }

        public static string TrimStartCRLF(this string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.TrimStart(DefaultCRLFChars);
        }

        public static string TrimEndCRLF(this string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.TrimEnd(DefaultCRLFChars);
        }

        public static string TrimUnwantedCharacters(this string value, params char[] unwantedChars)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim(unwantedChars);
        }

        public static string TrimStartUnwantedCharacters(this string value, params char[] unwantedChars)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.TrimStart(unwantedChars);
        }

        public static string TrimEndUnwantedCharacters(this string value, params char[] unwantedChars)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.TrimEnd(unwantedChars);
        }

        public static string KeepCharacters(this string value, params char[] wantedChars)
        {
            if (value == null)
            {
                return string.Empty;
            }

            IEnumerable<char> wc = wantedChars;
            var sb = new StringBuilder();

            value.ForEachCharacter(c =>
            {
                if (wc.ContainsItem(wantedChar => wantedChar == c))
                {
                    sb.Append(c);
                }
            });

            return sb.ToString();
        }

        public static void ForEachCharacter(this string value, Action<char> action)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            foreach (var c in value)
            {
                action?.Invoke(c);
            }
        }

        public static long ToInt64(this string value, long defaultValue = 0, bool keepDigitsOnly = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (keepDigitsOnly)
            {
                value = value.KeepCharacters(DigitsOnly);
            }

            if (long.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static int ToInt32(this string value, int defaultValue = 0, bool keepDigitsOnly = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (keepDigitsOnly)
            {
                value = value.KeepCharacters(DigitsOnly);
            }

            if (int.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static short ToInt16(this string value, short defaultValue = 0, bool keepDigitsOnly = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (keepDigitsOnly)
            {
                value = value.KeepCharacters(DigitsOnly);
            }

            if (short.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static ulong ToUInt64(this string value, ulong defaultValue = 0, bool keepDigitsOnly = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (keepDigitsOnly)
            {
                value = value.KeepCharacters(DigitsOnly);
            }

            if (ulong.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static uint ToUInt32(this string value, uint defaultValue = 0, bool keepDigitsOnly = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (keepDigitsOnly)
            {
                value = value.KeepCharacters(DigitsOnly);
            }

            if (uint.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static ushort ToUInt16(this string value, ushort defaultValue = 0, bool keepDigitsOnly = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (keepDigitsOnly)
            {
                value = value.KeepCharacters(DigitsOnly);
            }

            if (ushort.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static byte ToByte(this string value, byte defaultValue = 0, bool keepDigitsOnly = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (keepDigitsOnly)
            {
                value = value.KeepCharacters(DigitsOnly);
            }

            if (byte.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Get only digits from string if anything.
        /// </summary>
        public static string Digits(this string value)
        {
            return new string(value?.Where(c => char.IsDigit(c)).ToArray());
        }

        /// <summary>
        /// The method to get only alphaNumerics from given string.
        /// </summary>
        public static string GetAlphaNumerics(this string value)
        {
            if (value == null)
            {
                return value;
            }

            return Regex.Replace(value, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Get has algorithm for the given input string.
        /// </summary>
        public static string GetHashAlgorithm(this string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
            byte[] byteValue = Encoding.UTF8.GetBytes(input);
            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        /// <summary>
        /// The method to get only Numerics from given string.
        /// </summary>
        public static string GetNumericals(this string value)
        {
            if (value == null)
            {
                return value;
            }

            return Regex.Replace(value, "[^0-9]", string.Empty);
        }

        /// <summary>
        /// The method to removes other than numerics and period from given string.
        /// </summary>
        public static string GetNumericals(this string value, bool allowDot)
        {
            if (!allowDot)
            {
                return GetNumericals(value);
            }
            else
            {
                return Regex.Replace(value, "[^0-9|^.]", string.Empty);
            }
        }

        /// <summary>
        /// Removes anything except numerics and period from given string.
        /// </summary>
        public static string GetNumericals(this string value, bool allowDot, bool allowComma)
        {
            if (!allowDot && !allowComma)
            {
                return GetNumericals(value);
            }
            else if (allowDot && !allowComma)
            {
                return GetNumericals(value, true);
            }
            else if (!allowDot && allowComma)
            {
                return Regex.Replace(value, "[^0-9|^,]", string.Empty);
            }
            else
            {
                return Regex.Replace(value, "[^0-9|^.|^,]", string.Empty);
            }
        }

        /// <summary>
        /// Checks if the String contains only Unicode letters, digits. null will return false.
        /// An empty String ("") will return false.
        /// </summary>
        public static bool IsAlphaNumeric(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }

            return val.Trim().Replace(" ", string.Empty).All(char.IsLetterOrDigit);
        }

        /// <summary>
        /// The method checks if the give string is Date type.
        /// </summary>
        public static bool IsDate(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return DateTime.TryParse(value, out _);
            }

            return false;
        }

        /// <summary>
        /// Validate email address.
        /// </summary>
        public static bool IsEmailAddress(this string email)
        {
            string pattern =
                "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            return Regex.Match(email, pattern).Success;
        }

        /// <summary>
        /// The method checks if the give string is Guid type.
        /// </summary>
        public static bool IsGuid(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string pattern = "^[A-Fa-f0-9]{32}$|" +
                "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$";

                return Regex.Match(value, pattern).Success;
            }

            return false;
        }

        /// <summary>
        /// Compare check sum of the downloaded update file with the algorithm and check sum given.
        /// </summary>
        public static bool IsValidChecksum(this string checksum, string hashingAlgorithm, string fileName)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(hashingAlgorithm))
            {
                using var stream = File.OpenRead(fileName);
                if (hashAlgorithm != null)
                {
                    var hash = hashAlgorithm.ComputeHash(stream);
                    var fileChecksum = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();

                    if (fileChecksum == checksum.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validates if a string is valid IPv4
        ///  Regular expression taken from http://regexlib.com/REDetails.aspx?regexp_id=2035 Regex reference.
        /// </summary>
        public static bool IsValidIPv4(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }

            var pattern = @"(?:^|\s)([a-z]{3,6}(?=://))?(://)?((?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?))(?::(\d{2,5}))?(?:\s|$)";
            return Regex.Match(val, pattern).Success;
        }


        /// <summary>
        /// Extracts the left part of the input string limited with the length parameter.
        /// </summary>
        public static string Left(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException(nameof(val));
            }

            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    "length cannot be higher than total string length or less than 0");
            }

            return val.Substring(0, length);
        }

        /// <summary>
        /// Extracts the right part of the input string limited with the length parameter.
        /// </summary>
        public static string Right(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException(nameof(val));
            }

            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    "length cannot be higher than total string length or less than 0");
            }

            return val.Substring(val.Length - length);
        }

        /// <summary>
        /// Converts string to its boolean equivalent.
        /// </summary>
        public static bool ToBoolean(this string value, bool defaultValue)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            string val = value.ToLower().Trim();
            switch (val)
            {
                case "false":
                case "f":
                case "no":
                case "n":
                case "0":
                    return false;

                case "true":
                case "t":
                case "yes":
                case "y":
                case "1":
                    return true;

                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// Parse string value to enum value.
        /// </summary>
        public static TEnum ToEnum<TEnum>(this string value, bool ignoreCase = false)
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException($"{typeof(TEnum).Name} must be an enum type.");
            }

            if (Enum.TryParse<TEnum>(value, ignoreCase, out var result))
            {
                return result;
            }

            return default;
        }
    }
    public static class IEnumerableExtensions
    {
        public static bool ContainsItem<T>(this IEnumerable<T> array, Predicate<T> match)
        {
            if (array == null)
            {
                return false;
            }

            foreach (var item in array)
            {
                var result = match?.Invoke(item);

                if (result.HasValue && result.Value)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Enumerates for each in this collection.
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            T[] array = source.ToArray();

            foreach (T item in array)
            {
                action(item);
            }

            return array;
        }

        /// <summary>
        /// Enumerates for each in this collection.
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            T[] array = source.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }

            return array;
        }

        /// <summary>
        /// Check if any one of the item present in the collection.
        /// </summary>
        public static bool ContainsAny<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, params T[] values)
        {
            return values.Select(x => x)
                .Intersect(source, comparer)
                .Any();
        }

        /// <summary>
        /// Check if all the given records present in the collection.
        /// </summary>
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, params T[] values)
        {
            return values.Select(x => x)
                .Intersect(source, comparer)
                .Count() == values.Count();
        }

        /// <summary>
        /// Check if the collection is empty or not.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source?.Any() == false;
        }

        /// <summary>
        /// Check if the collection is empty or not.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            predicate ??= (x => true);
            return source?.Any(predicate) == false;
        }

        /// <summary>
        /// check if collection is empty or null.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || source?.Any() == false;
        }

        /// <summary>
        /// Converts List of string and create a delimited string.
        /// </summary>
        public static string Join<T>(this IEnumerable<T> source, Func<T, string> itemOutput = null, string delimiter = ",")
        {
            itemOutput ??= (x => x.ToString());
            return string.Join(delimiter, source.Select(itemOutput).ToArray());
        }

        /// <summary>
        /// Converts Enumerable collection to hashset type.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}
