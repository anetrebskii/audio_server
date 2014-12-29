using System;

namespace Alnet.Common
{
   /// <summary>
   /// Some helper utilities for working with time.
   /// </summary>
   public static class DateTimeExtensions
   {
      /// <summary>
      /// Threat specified <see cref="DateTime"/> as UTC. Without any timezone conversion.
      /// </summary>
      /// <param name="dateTime">Date to convert.</param>
      /// <returns>UTC Date time.</returns>
      public static DateTime AsUtcTime(this DateTime dateTime)
      {
         if (dateTime.Kind == DateTimeKind.Utc)
         {
            return dateTime;
         }

         return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
      }

      /// <summary>
      /// Truncates the milliseconds.
      /// </summary>
      /// <param name="dateTime">The date time.</param>
      public static DateTime TruncateToSeconds(this DateTime dateTime)
      {
         return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
      }

      /// <summary>
      /// Returns last moment of current day.
      /// </summary>
      /// <remarks>
      /// I.e. if it 12.12.2014 10:15 it returns 12.12.2014 23:59:59.999.99999
      /// </remarks>
      public static DateTime EndOfDate(this DateTime dateTime)
      {
         return dateTime.Date.AddDays(1).AddTicks(-1);
      }

      /// <summary>
      /// Convert DateTime to UNIX time.
      /// </summary>
      /// <param name="dateTime"><see cref="DateTime"/></param>
      /// <returns>Amount seconds after 1970.1.1 00:00:00.000.</returns>
      public static int ToUnixTime(this DateTime dateTime)
      {
         return (int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
      }
   }
}
