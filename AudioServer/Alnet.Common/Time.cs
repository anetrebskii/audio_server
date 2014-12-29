using System;

namespace Alnet.Common
{
   /// <summary>
   /// Some helper utilities for working with time.
   /// </summary>
   public static class Time
   {
      /// <summary>
      /// Time used to convert from <see cref="DateTime"/> to <code>time_t</code> c++ type.
      /// </summary>
      private static readonly DateTime ZERO_T_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

      /// <summary>
      /// Converts .NET <see cref="DateTime"/> to C++ time_t time format.
      /// </summary>
      /// <param name="dateTime">Time to convert. Utc time.</param>
      /// <returns>time in C++ <code>time_t</code> fromat.</returns>
      public static UInt64 ToTimeT(this DateTime dateTime)
      {
         Guard.VerifyArgumentIsUtc(dateTime, "dateTime");
         var startSpan = dateTime - ZERO_T_TIME;
         return Convert.ToUInt64(startSpan.TotalSeconds);
      }

      /// <summary>
      /// Converts <c>SYSTEM_TIME</c> parameter to <see cref="DateTime"/>.
      /// </summary>
      /// <param name="time_t"><c>SYSTEM_TIME</c></param>
      /// <returns>UTC <see cref="DateTime"/>.</returns>
      public static DateTime ToDateTime(UInt64 time_t)
      {
         return ZERO_T_TIME.AddSeconds(time_t);
      }

      /// <summary>
      /// Converts <c>FILE_TIME</c> to nullable UTC <see cref="DateTime"/>.
      /// </summary>
      /// <param name="fileTime"><c>FILE_TIME</c></param>
      /// <returns>Nullable UTC <see cref="DateTime"/>.</returns>
      public static DateTime? ToNullableDateTimeFromFileTime(Int64 fileTime)
      {
         if (fileTime == default(Int64))
         {
            return null;
         }

         return DateTime.FromFileTimeUtc(fileTime);
      }

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
   }
}
