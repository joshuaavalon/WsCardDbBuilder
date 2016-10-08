using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Type = WsDeckDatabase.Model.Type;

namespace WsDeckDatabase.Download
{
    internal static class Utils
    {
        /// <summary>
        ///     Tries parse a string into enum (Type T).
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Parsed enum.</returns>
        public static T EnumParse<T>(this string input) where T : struct, IComparable, IConvertible, IFormattable
        {
            if (!typeof(T).IsEnum) return default(T);
            foreach (
                var item in
                    GetValues<T>()
                        .Where(
                            item =>
                                item.ToString(CultureInfo.InvariantCulture)
                                    .Equals(input, StringComparison.InvariantCultureIgnoreCase)))
                return item;
            return default(T);
        }

        /// <summary>
        ///     Gets all the values of enum (Type T).
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }


        /// <summary>
        ///     Prase the text and return card type. Default is character.
        /// </summary>
        /// <param name="text">Text to be prased</param>
        /// <returns>Card type</returns>
        public static Type ParseType(this string text)
        {
            if (text.Equals("イベント"))
                return Type.Event;
            return text.Equals("クライマックス") ? Type.Climax : Type.Character;
        }


        /// <summary>
        ///     Tries parse a string into integer.
        ///     Returns default if it fails.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Parsed integer.</returns>
        public static int IntParseOrDefault(this string input)
        {
            int temp;
            int.TryParse(input, out temp);
            return temp;
        }


        /// <summary>
        ///     Remove html br tag.
        /// </summary>
        /// <param name="html">sting to be handle</param>
        /// <returns>Resulted string</returns>
        public static string ReplaceBr(this string html)
        {
            return html.Replace("<br/>", "\n").Replace("<br>", "\n");
        }
    }
}