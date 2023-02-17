using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace DateAndTime
{
    public static class TimeHelper
    {
        // Number of 100ns ticks per time unit
        internal const int MicrosecondsPerMillisecond = 1000;
        private const long TicksPerMicrosecond = 10;
        private const long TicksPerMillisecond = TicksPerMicrosecond * MicrosecondsPerMillisecond;

        private const int HoursPerDay = 24;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;
        private const long TicksPerMinute = TicksPerSecond * 60;
        private const long TicksPerHour = TicksPerMinute * 60;
        private const long TicksPerDay = TicksPerHour * HoursPerDay;

        // Number of milliseconds per time unit
        private const int MillisPerSecond = 1000;

        // Number of days in a non-leap year
        private const int DaysPerYear = 365;

        // Number of days in 4 years
        private const int DaysPer4Years = DaysPerYear * 4 + 1; // 1461

        // Number of days in 100 years
        private const int DaysPer100Years = DaysPer4Years * 25 - 1; // 36524

        // Number of days in 400 years
        private const int DaysPer400Years = DaysPer100Years * 4 + 1; // 146097

        // Number of days from 1/1/0001 to 12/31/9999
        private const int DaysTo10000 = DaysPer400Years * 25 - 366; // 3652059

        internal const long MaxTicks = DaysTo10000 * TicksPerDay - 1;

        // Return the tick count corresponding to the given hour, minute, second.
        // Will check the if the parameters are valid.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong TimeToTicks(int hour, int minute, int second)
        {
            if ((uint)hour >= 24 || (uint)minute >= 60 || (uint)second >= 60)
            {
                throw new ArgumentOutOfRangeException(null,
                    "args can not be (uint)hour >= 24 || (uint)minute >= 60 || (uint)second >= 60");
            }

            int totalSeconds = hour * 3600 + minute * 60 + second;
            return (uint)totalSeconds * (ulong)TicksPerSecond;
        }

        public static ulong TimeToTicks(int hour, int minute, int second, int millisecond)
        {
            ulong ticks = TimeToTicks(hour, minute, second);

            if ((uint)millisecond >= MillisPerSecond) throw new ArgumentOutOfRangeException(nameof(millisecond));

            ticks += (uint)millisecond * (uint)TicksPerMillisecond;

            Debug.Assert(ticks <= MaxTicks, "Input parameters validated already");

            return ticks;
        }

        public static ulong TimeToTicks(int hour, int minute, int second, int millisecond, int microsecond)
        {
            ulong ticks = TimeToTicks(hour, minute, second, millisecond);

            if ((uint)microsecond >= MicrosecondsPerMillisecond)
                throw new ArgumentOutOfRangeException(nameof(microsecond));

            ticks += (uint)microsecond * (uint)TicksPerMicrosecond;

            Debug.Assert(ticks <= MaxTicks, "Input parameters validated already");

            return ticks;
        }

        /// <summary>
        /// Gets the microseconds component of the time interval represented by the current <see cref="TimeSpan"/> structure.
        /// </summary>
        /// <remarks>
        /// The <see cref="Microseconds"/> property represents whole microseconds, whereas the
        /// <see cref="TotalMicroseconds"/> property represents whole and fractional microseconds.
        /// </remarks>
        public static int GetMicroseconds(long _ticks) => (int)((_ticks / TicksPerMicrosecond) % 1000);

        /// <summary>
        /// Gets the nanoseconds component of the time interval represented by the current <see cref="TimeSpan"/> structure.
        /// </summary>
        /// <remarks>
        /// The <see cref="Nanoseconds"/> property represents whole nanoseconds, whereas the
        /// <see cref="TotalNanoseconds"/> property represents whole and fractional nanoseconds.
        /// </remarks>
        public static int GetNanoseconds(long _ticks) => (int)((_ticks % TicksPerMicrosecond) * 100);
    }
}