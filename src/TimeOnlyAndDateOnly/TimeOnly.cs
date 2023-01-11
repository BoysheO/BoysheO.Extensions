﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DateAndTime;

namespace System
{
    /// <summary>
    /// Represents a time of day, as would be read from a clock, within the range 00:00:00 to 23:59:59.9999999.
    /// </summary>
    public readonly struct TimeOnly
        : IComparable,
            IComparable<TimeOnly>,
            IEquatable<TimeOnly>
    {
        // represent the number of ticks map to the time of the day. 1 ticks = 100-nanosecond in time measurements.
        private readonly long _ticks;

        // MinTimeTicks is the ticks for the midnight time 00:00:00.000 AM
        private const long MinTimeTicks = 0;

        // MaxTimeTicks is the max tick value for the time in the day. It is calculated using DateTime.Today.AddTicks(-1).TimeOfDay.Ticks.
        private const long MaxTimeTicks = 863_999_999_999;

        /// <summary>
        /// Represents the smallest possible value of TimeOnly.
        /// </summary>
        public static TimeOnly MinValue => new TimeOnly((ulong)MinTimeTicks);

        /// <summary>
        /// Represents the largest possible value of TimeOnly.
        /// </summary>
        public static TimeOnly MaxValue => new TimeOnly((ulong)MaxTimeTicks);

        /// <summary>
        /// Initializes a new instance of the timeOnly structure to the specified hour and the minute.
        /// </summary>
        /// <param name="hour">The hours (0 through 23).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        public TimeOnly(int hour, int minute) : this(TimeHelper.TimeToTicks(hour, minute, 0, 0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the timeOnly structure to the specified hour, minute, and second.
        /// </summary>
        /// <param name="hour">The hours (0 through 23).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="second">The seconds (0 through 59).</param>
        public TimeOnly(int hour, int minute, int second) : this(TimeHelper.TimeToTicks(hour, minute, second, 0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the timeOnly structure to the specified hour, minute, second, and millisecond.
        /// </summary>
        /// <param name="hour">The hours (0 through 23).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="second">The seconds (0 through 59).</param>
        /// <param name="millisecond">The millisecond (0 through 999).</param>
        public TimeOnly(int hour, int minute, int second, int millisecond) : this(
            TimeHelper.TimeToTicks(hour, minute, second, millisecond))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeOnly"/> structure to the specified hour, minute, second, and millisecond.
        /// </summary>
        /// <param name="hour">The hours (0 through 23).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="second">The seconds (0 through 59).</param>
        /// <param name="millisecond">The millisecond (0 through 999).</param>
        /// <param name="microsecond">The microsecond (0 through 999).</param>
        public TimeOnly(int hour, int minute, int second, int millisecond, int microsecond) : this(
            TimeHelper.TimeToTicks(hour, minute, second, millisecond, microsecond))
        {
        }

        /// <summary>
        /// Initializes a new instance of the TimeOnly structure using a specified number of ticks.
        /// </summary>
        /// <param name="ticks">A time of day expressed in the number of 100-nanosecond units since 00:00:00.0000000.</param>
        public TimeOnly(long ticks)
        {
            if ((ulong)ticks > MaxTimeTicks)
            {
                throw new ArgumentOutOfRangeException(nameof(ticks), SR.ArgumentOutOfRange_TimeOnlyBadTicks);
            }

            _ticks = ticks;
        }

        // exist to bypass the check in the public constructor.
        internal TimeOnly(ulong ticks) => _ticks = (long)ticks;

        /// <summary>
        /// Gets the hour component of the time represented by this instance.
        /// </summary>
        public int Hour => new TimeSpan(_ticks).Hours;

        /// <summary>
        /// Gets the minute component of the time represented by this instance.
        /// </summary>
        public int Minute => new TimeSpan(_ticks).Minutes;

        /// <summary>
        /// Gets the second component of the time represented by this instance.
        /// </summary>
        public int Second => new TimeSpan(_ticks).Seconds;

        /// <summary>
        /// Gets the millisecond component of the time represented by this instance.
        /// </summary>
        public int Millisecond => new TimeSpan(_ticks).Milliseconds;

        /// <summary>
        /// Gets the microsecond component of the time represented by this instance.
        /// </summary>
        public int Microsecond => TimeHelper.GetMicroseconds(_ticks);

        /// <summary>
        /// Gets the nanosecond component of the time represented by this instance.
        /// </summary>
        public int Nanosecond => TimeHelper.GetNanoseconds(_ticks);

        /// <summary>
        /// Gets the number of ticks that represent the time of this instance.
        /// </summary>
        public long Ticks => _ticks;

        private TimeOnly AddTicks(long ticks) =>
            new TimeOnly((_ticks + TimeSpan.TicksPerDay + (ticks % TimeSpan.TicksPerDay)) % TimeSpan.TicksPerDay);

        private TimeOnly AddTicks(long ticks, out int wrappedDays)
        {
            wrappedDays = (int)(ticks / TimeSpan.TicksPerDay);
            long newTicks = _ticks + ticks % TimeSpan.TicksPerDay;
            if (newTicks < 0)
            {
                wrappedDays--;
                newTicks += TimeSpan.TicksPerDay;
            }
            else
            {
                if (newTicks >= TimeSpan.TicksPerDay)
                {
                    wrappedDays++;
                    newTicks -= TimeSpan.TicksPerDay;
                }
            }

            return new TimeOnly(newTicks);
        }

        /// <summary>
        /// Returns a new TimeOnly that adds the value of the specified TimeSpan to the value of this instance.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <returns>An object whose value is the sum of the time represented by this instance and the time interval represented by value.</returns>
        public TimeOnly Add(TimeSpan value) => AddTicks(value.Ticks);

        /// <summary>
        /// Returns a new TimeOnly that adds the value of the specified TimeSpan to the value of this instance.
        /// If the result wraps past the end of the day, this method will return the number of excess days as an out parameter.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <param name="wrappedDays">When this method returns, contains the number of excess days if any that resulted from wrapping during this addition operation.</param>
        /// <returns>An object whose value is the sum of the time represented by this instance and the time interval represented by value.</returns>
        public TimeOnly Add(TimeSpan value, out int wrappedDays) => AddTicks(value.Ticks, out wrappedDays);

        /// <summary>
        /// Returns a new TimeOnly that adds the specified number of hours to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the time represented by this instance and the number of hours represented by value.</returns>
        public TimeOnly AddHours(double value) => AddTicks((long)(value * TimeSpan.TicksPerHour));

        /// <summary>
        /// Returns a new TimeOnly that adds the specified number of hours to the value of this instance.
        /// If the result wraps past the end of the day, this method will return the number of excess days as an out parameter.
        /// </summary>
        /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
        /// <param name="wrappedDays">When this method returns, contains the number of excess days if any that resulted from wrapping during this addition operation.</param>
        /// <returns>An object whose value is the sum of the time represented by this instance and the number of hours represented by value.</returns>
        public TimeOnly AddHours(double value, out int wrappedDays) =>
            AddTicks((long)(value * TimeSpan.TicksPerHour), out wrappedDays);

        /// <summary>
        /// Returns a new TimeOnly that adds the specified number of minutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the time represented by this instance and the number of minutes represented by value.</returns>
        public TimeOnly AddMinutes(double value) => AddTicks((long)(value * TimeSpan.TicksPerMinute));

        /// <summary>
        /// Returns a new TimeOnly that adds the specified number of minutes to the value of this instance.
        /// If the result wraps past the end of the day, this method will return the number of excess days as an out parameter.
        /// </summary>
        /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
        /// <param name="wrappedDays">When this method returns, contains the number of excess days if any that resulted from wrapping during this addition operation.</param>
        /// <returns>An object whose value is the sum of the time represented by this instance and the number of minutes represented by value.</returns>
        public TimeOnly AddMinutes(double value, out int wrappedDays) =>
            AddTicks((long)(value * TimeSpan.TicksPerMinute), out wrappedDays);

        /// <summary>
        /// Determines if a time falls within the range provided.
        /// Supports both "normal" ranges such as 10:00-12:00, and ranges that span midnight such as 23:00-01:00.
        /// </summary>
        /// <param name="start">The starting time of day, inclusive.</param>
        /// <param name="end">The ending time of day, exclusive.</param>
        /// <returns>True, if the time falls within the range, false otherwise.</returns>
        /// <remarks>
        /// If <paramref name="start"/> and <paramref name="end"/> are equal, this method returns false, meaning there is zero elapsed time between the two values.
        /// If you wish to treat such cases as representing one or more whole days, then first check for equality before calling this method.
        /// </remarks>
        public bool IsBetween(TimeOnly start, TimeOnly end)
        {
            long startTicks = start._ticks;
            long endTicks = end._ticks;

            return startTicks <= endTicks
                ? (startTicks <= _ticks && endTicks > _ticks)
                : (startTicks <= _ticks || endTicks > _ticks);
        }

        /// <summary>
        /// Determines whether two specified instances of TimeOnly are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left and right represent the same time; otherwise, false.</returns>
        /// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)" />
        public static bool operator ==(TimeOnly left, TimeOnly right) => left._ticks == right._ticks;

        /// <summary>
        /// Determines whether two specified instances of TimeOnly are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left and right do not represent the same time; otherwise, false.</returns>
        /// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)" />
        public static bool operator !=(TimeOnly left, TimeOnly right) => left._ticks != right._ticks;

        /// <summary>
        /// Determines whether one specified TimeOnly is later than another specified TimeOnly.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is later than right; otherwise, false.</returns>
        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
        public static bool operator >(TimeOnly left, TimeOnly right) => left._ticks > right._ticks;

        /// <summary>
        /// Determines whether one specified TimeOnly represents a time that is the same as or later than another specified TimeOnly.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is the same as or later than right; otherwise, false.</returns>
        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
        public static bool operator >=(TimeOnly left, TimeOnly right) => left._ticks >= right._ticks;

        /// <summary>
        /// Determines whether one specified TimeOnly is earlier than another specified TimeOnly.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is earlier than right; otherwise, false.</returns>
        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
        public static bool operator <(TimeOnly left, TimeOnly right) => left._ticks < right._ticks;

        /// <summary>
        /// Determines whether one specified TimeOnly represents a time that is the same as or earlier than another specified TimeOnly.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is the same as or earlier than right; otherwise, false.</returns>
        /// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
        public static bool operator <=(TimeOnly left, TimeOnly right) => left._ticks <= right._ticks;

        /// <summary>
        ///  Gives the elapsed time between two points on a circular clock, which will always be a positive value.
        /// </summary>
        /// <param name="t1">The first TimeOnly instance.</param>
        /// <param name="t2">The second TimeOnly instance..</param>
        /// <returns>The elapsed time between t1 and t2.</returns>
        public static TimeSpan operator -(TimeOnly t1, TimeOnly t2) =>
            new TimeSpan((t1._ticks - t2._ticks + TimeSpan.TicksPerDay) % TimeSpan.TicksPerDay);

        /// <summary>
        /// Constructs a TimeOnly object from a TimeSpan representing the time elapsed since midnight.
        /// </summary>
        /// <param name="timeSpan">The time interval measured since midnight. This value has to be positive and not exceeding the time of the day.</param>
        /// <returns>A TimeOnly object representing the time elapsed since midnight using the timeSpan value.</returns>
        public static TimeOnly FromTimeSpan(TimeSpan timeSpan) => new TimeOnly(timeSpan.Ticks);

        /// <summary>
        /// Constructs a TimeOnly object from a DateTime representing the time of the day in this DateTime object.
        /// </summary>
        /// <param name="dateTime">The time DateTime object to extract the time of the day from.</param>
        /// <returns>A TimeOnly object representing time of the day specified in the DateTime object.</returns>
        public static TimeOnly FromDateTime(DateTime dateTime) => new TimeOnly(dateTime.TimeOfDay.Ticks);

        /// <summary>
        /// Convert the current TimeOnly instance to a TimeSpan object.
        /// </summary>
        /// <returns>A TimeSpan object spanning to the time specified in the current TimeOnly object.</returns>
        public TimeSpan ToTimeSpan() => new TimeSpan(_ticks);

        internal DateTime ToDateTime() => new DateTime(_ticks);

        /// <summary>
        /// Compares the value of this instance to a specified TimeOnly value and indicates whether this instance is earlier than, the same as, or later than the specified TimeOnly value.
        /// </summary>
        /// <param name="value">The object to compare to the current instance.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and the value parameter.
        /// Less than zero if this instance is earlier than value.
        /// Zero if this instance is the same as value.
        /// Greater than zero if this instance is later than value.
        /// </returns>
        public int CompareTo(TimeOnly value) => _ticks.CompareTo(value._ticks);

        /// <summary>
        /// Compares the value of this instance to a specified object that contains a specified TimeOnly value, and returns an integer that indicates whether this instance is earlier than, the same as, or later than the specified TimeOnly value.
        /// </summary>
        /// <param name="value">A boxed object to compare, or null.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and the value parameter.
        /// Less than zero if this instance is earlier than value.
        /// Zero if this instance is the same as value.
        /// Greater than zero if this instance is later than value.
        /// </returns>
        public int CompareTo(object? value)
        {
            if (value == null) return 1;
            if (!(value is TimeOnly timeOnly))
            {
                throw new ArgumentException(SR.Arg_MustBeTimeOnly);
            }

            return CompareTo(timeOnly);
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified TimeOnly instance.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if the value parameter equals the value of this instance; otherwise, false.</returns>
        public bool Equals(TimeOnly value) => _ticks == value._ticks;

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if value is an instance of TimeOnly and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object? value) => value is TimeOnly timeOnly && _ticks == timeOnly._ticks;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            long ticks = _ticks;
            return unchecked((int)ticks) ^ (int)(ticks >> 32);
        }
    }
}