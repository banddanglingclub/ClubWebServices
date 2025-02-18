﻿using AnglingClubShared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnglingClubShared.Enums
{
    public static class EnumUtils
    {
        /// <summary>
        /// Enumerates the values of the enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Returns the season that contains the passed date or null if no matches.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Season? SeasonForDate(DateTime date)
        {
            Season? matchingSeason = null;

            foreach (var season in GetValues<Season>())
            {
                if (date >= season.SeasonStarts() && date <= season.SeasonEnds())
                {
                    matchingSeason = season;
                    break;
                }
            }

            return matchingSeason;
        }

        /// <summary>
        /// Returns the current season.
        /// </summary>
        /// <returns></returns>
        public static Season CurrentSeason()
        {
            return SeasonForDate(DateTime.Now)!.Value;
        }

        /// <summary>
        /// Returns the season as 2022/2023 etc
        /// </summary>
        /// <returns></returns>
        public static string SeasonDisplay(Season season)
        {
            return $"20{Convert.ToInt32(season)}/20{Convert.ToInt32(season) + 1}";
        }
    }
}
