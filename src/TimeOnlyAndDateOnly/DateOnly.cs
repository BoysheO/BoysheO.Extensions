// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Globalization;
using DateAndTime;

namespace System
{
    /// <summary>
    /// Represents dates with values ranging from January 1, 0001 Anno Domini (Common Era) through December 31, 9999 A.D. (C.E.) in the Gregorian calendar.
    /// </summary>
    public readonly struct DateOnly
        : IComparable,
            IComparable<DateOnly>,
            IEquatable<DateOnly>
    {
        private readonly int _dayNumber;

        // Maps to Jan 1st year 1
        private const int MinDayNumber = 0;

        // Maps to December 31 year 9999. The value calculated from "new DateTime(9999, 12, 31).Ticks / TimeSpan.TicksPerDay"
        private const int MaxDayNumber = 3_652_058;

        private static int DayNumberFromDateTime(DateTime dt) => (int)((ulong)dt.Ticks / TimeSpan.TicksPerDay);

        private DateTime GetEquivalentDateTime() => new DateTime(_dayNumber * TimeSpan.TicksPerDay);

        private DateOnly(int dayNumber)
        {
            Debug.Assert((uint)dayNumber <= MaxDayNumber);
            _dayNumber = dayNumber;
        }

        /// <summary>
        /// Gets the earliest possible date that can be created.
        /// </summary>
        public static DateOnly MinValue => new DateOnly(MinDayNumber);

        /// <summary>
        /// Gets the latest possible date that can be created.
        /// </summary>
        public static DateOnly MaxValue => new DateOnly(MaxDayNumber);

        /// <summary>
        /// Creates a new instance of the DateOnly structure to the specified year, month, and day.
        /// </summary>
        /// <param name="year">The year (1 through 9999).</param>
        /// <param name="month">The month (1 through 12).</param>
        /// <param name="day">The day (1 through the number of days in <paramref name="month" />).</param>
        public DateOnly(int year, int month, int day) =>
            _dayNumber = DayNumberFromDateTime(new DateTime(year, month, day));

        /// <summary>
        /// Creates a new instance of the DateOnly structure to the specified year, month, and day for the specified calendar.
        /// </summary>
        /// <param name="year">The year (1 through the number of years in calendar).</param>
        /// <param name="month">The month (1 through the number of months in calendar).</param>
        /// <param name="day">The day (1 through the number of days in <paramref name="month"/>).</param>
        /// <param name="calendar">The calendar that is used to interpret year, month, and day.<paramref name="month"/>.</param>
        public DateOnly(int year, int month, int day, Calendar calendar) =>
            _dayNumber = DayNumberFromDateTime(new DateTime(year, month, day, calendar));

        /// <summary>
        /// Creates a new instance of the DateOnly structure to the specified number of days.
        /// </summary>
        /// <param name="dayNumber">The number of days since January 1, 0001 in the Proleptic Gregorian calendar.</param>
        public static DateOnly FromDayNumber(int dayNumber)
        {
            if ((uint)dayNumber > MaxDayNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(dayNumber));
            }

            return new DateOnly(dayNumber);
        }

        /// <summary>
        /// Gets the year component of the date represented by this instance.
        /// </summary>
        public int Year => GetEquivalentDateTime().Year;

        /// <summary>
        /// Gets the month component of the date represented by this instance.
        /// </summary>
        public int Month => GetEquivalentDateTime().Month;

        /// <summary>
        /// Gets the day component of the date represented by this instance.
        /// </summary>
        public int Day => GetEquivalentDateTime().Day;

        /// <summary>
        /// Gets the day of the week represented by this instance.
        /// </summary>
        public DayOfWeek DayOfWeek => (DayOfWeek)(((uint)_dayNumber + 1) % 7);

        /// <summary>
        /// Gets the day of the year represented by this instance.
        /// </summary>
        public int DayOfYear => GetEquivalentDateTime().DayOfYear;

        /// <summary>
        /// Gets the number of days since January 1, 0001 in the Proleptic Gregorian calendar represented by this instance.
        /// </summary>
        public int DayNumber => _dayNumber;

        /// <summary>
        /// Adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">The number of days to add. To subtract days, specify a negative number.</param>
        /// <returns>An instance whose value is the sum of the date represented by this instance and the number of days represented by value.</returns>
        public DateOnly AddDays(int value)
        {
            int newDayNumber = _dayNumber + value;
            if ((uint)newDayNumber > MaxDayNumber)
            {
                ThrowOutOfRange();
            }

            return new DateOnly(newDayNumber);

            static void ThrowOutOfRange() =>
                throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_AddValue);
        }

        /// <summary>
        /// Adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="value">A number of months. The months parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date represented by this instance and months.</returns>
        public DateOnly AddMonths(int value) =>
            new DateOnly(DayNumberFromDateTime(GetEquivalentDateTime().AddMonths(value)));

        /// <summary>
        /// Adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date represented by this instance and the number of years represented by value.</returns>
        public DateOnly AddYears(int value) =>
            new DateOnly(DayNumberFromDateTime(GetEquivalentDateTime().AddYears(value)));

        /// <summary>
        /// Determines whether two specified instances of DateOnly are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left and right represent the same date; otherwise, false.</returns>
        public static bool operator ==(DateOnly left, DateOnly right) => left._dayNumber == right._dayNumber;

        /// <summary>
        /// Determines whether two specified instances of DateOnly are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left and right do not represent the same date; otherwise, false.</returns>
        public static bool operator !=(DateOnly left, DateOnly right) => left._dayNumber != right._dayNumber;

        /// <summary>
        /// Determines whether one specified DateOnly is later than another specified DateTime.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is later than right; otherwise, false.</returns>
        public static bool operator >(DateOnly left, DateOnly right) => left._dayNumber > right._dayNumber;

        /// <summary>
        /// Determines whether one specified DateOnly represents a date that is the same as or later than another specified DateOnly.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is the same as or later than right; otherwise, false.</returns>
        public static bool operator >=(DateOnly left, DateOnly right) => left._dayNumber >= right._dayNumber;

        /// <summary>
        /// Determines whether one specified DateOnly is earlier than another specified DateOnly.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is earlier than right; otherwise, false.</returns>
        public static bool operator <(DateOnly left, DateOnly right) => left._dayNumber < right._dayNumber;

        /// <summary>
        /// Determines whether one specified DateOnly represents a date that is the same as or earlier than another specified DateOnly.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if left is the same as or earlier than right; otherwise, false.</returns>
        public static bool operator <=(DateOnly left, DateOnly right) => left._dayNumber <= right._dayNumber;

        /// <summary>
        /// Returns a DateTime that is set to the date of this DateOnly instance and the time of specified input time.
        /// </summary>
        /// <param name="time">The time of the day.</param>
        /// <returns>The DateTime instance composed of the date of the current DateOnly instance and the time specified by the input time.</returns>
        public DateTime ToDateTime(TimeOnly time) => new DateTime(_dayNumber * TimeSpan.TicksPerDay + time.Ticks);

        /// <summary>
        /// Returns a DateTime instance with the specified input kind that is set to the date of this DateOnly instance and the time of specified input time.
        /// </summary>
        /// <param name="time">The time of the day.</param>
        /// <param name="kind">One of the enumeration values that indicates whether ticks specifies a local time, Coordinated Universal Time (UTC), or neither.</param>
        /// <returns>The DateTime instance composed of the date of the current DateOnly instance and the time specified by the input time.</returns>
        public DateTime ToDateTime(TimeOnly time, DateTimeKind kind) =>
            new DateTime(_dayNumber * TimeSpan.TicksPerDay + time.Ticks, kind);

        /// <summary>
        /// Returns a DateOnly instance that is set to the date part of the specified dateTime.
        /// </summary>
        /// <param name="dateTime">The DateTime instance.</param>
        /// <returns>The DateOnly instance composed of the date part of the specified input time dateTime instance.</returns>
        public static DateOnly FromDateTime(DateTime dateTime) => new DateOnly(DayNumberFromDateTime(dateTime));

        /// <summary>
        /// Compares the value of this instance to a specified DateOnly value and returns an integer that indicates whether this instance is earlier than, the same as, or later than the specified DateTime value.
        /// </summary>
        /// <param name="value">The object to compare to the current instance.</param>
        /// <returns>Less than zero if this instance is earlier than value. Greater than zero if this instance is later than value. Zero if this instance is the same as value.</returns>
        public int CompareTo(DateOnly value) => _dayNumber.CompareTo(value._dayNumber);

        /// <summary>
        /// Compares the value of this instance to a specified object that contains a specified DateOnly value, and returns an integer that indicates whether this instance is earlier than, the same as, or later than the specified DateOnly value.
        /// </summary>
        /// <param name="value">A boxed object to compare, or null.</param>
        /// <returns>Less than zero if this instance is earlier than value. Greater than zero if this instance is later than value. Zero if this instance is the same as value.</returns>
        public int CompareTo(object? value)
        {
            if (value == null) return 1;
            if (!(value is DateOnly dateOnly))
            {
                throw new ArgumentException(SR.Arg_MustBeDateOnly);
            }

            return CompareTo(dateOnly);
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified DateOnly instance.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if the value parameter equals the value of this instance; otherwise, false.</returns>
        public bool Equals(DateOnly value) => _dayNumber == value._dayNumber;

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if value is an instance of DateOnly and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals( object? value) =>
            value is DateOnly dateOnly && _dayNumber == dateOnly._dayNumber;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => _dayNumber;
    }
}