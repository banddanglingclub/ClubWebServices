﻿using AnglingClubShared.Enums;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AnglingClubShared.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns the "Description" attribute from an enum. Returns the name of the enum value if no description is available.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flagSeparator">Optional, separator for flag enums to override the default</param>
        /// <returns></returns>
        public static string EnumDescription(this Enum value, string flagSeparator = " - ")
        {
            //pull out each value in case of flag enumeration
            var values = value.ToString().Split(',').Select(s => s.Trim());
            var type = value.GetType();

            return string.Join(flagSeparator, values.Select(enumValue => type.GetMember(enumValue)
               .FirstOrDefault()
               ?.GetCustomAttribute<DescriptionAttribute>()
               ?.Description
               ?? enumValue.ToString()));
        }


        /// <summary>
        /// Returns the season name from [Description("2021/22,2021-04-01,2022-03-31")]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SeasonName(this Season value)
        {
            var name = seasonParts(value)[0];

            return name;
        }

        /// <summary>
        /// Returns the season start date from [Description("2021/22,2021-04-01,2022-03-31")]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime SeasonStarts(this Season value)
        {
            var starts = DateTime.Parse(seasonParts(value)[1]);

            return starts;
        }

        /// <summary>
        /// Returns the season end date from [Description("2021/22,2021-04-01,2022-03-31")]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime SeasonEnds(this Season value)
        {
            var ends = DateTime.Parse(seasonParts(value)[2]).AddHours(23).AddMinutes(59);

            return ends;
        }


        public static string Ordinal(this int num)
        {
            if (num <= 0) return "";

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        /// <summary>
        /// Returns a date of the form "Sat 15th Dec 2024"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string PrettyDate(this DateTime value)
        {
            return value.ToString("ddd ") + value.Day.Ordinal() + value.ToString(" MMM yyyy");
        }

        /// <summary>
        /// Splits an enum desc of [Description("2021/22,2021-04-01,2022-03-31")] into
        /// comma separted parts
        /// </summary>
        /// <param name="season"></param>
        /// <returns></returns>
        private static string[] seasonParts(Season season)
        {
            var desc = season.EnumDescription();
            var parts = desc.Split(",");

            return parts;
        }

        /// <summary>
        /// Adds IsNullOrEmpty to string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s.Length == 0;
        }
    }
}
