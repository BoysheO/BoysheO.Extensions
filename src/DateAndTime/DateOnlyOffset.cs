using System;

namespace DateAndTime
{
    public readonly struct DateOnlyOffset
    {
        public readonly DateOnly DateOnly;
        public readonly TimeSpan Offset;

        public DateOnlyOffset(DateOnly dateOnly, TimeSpan offset)
        {
            DateOnly = dateOnly;
            Offset = offset;
        }

        public DateOnlyOffset ToOffset(TimeSpan timeSpan)
        {
            var date = DateOnly.ToDateTime(TimeOnly.FromTimeSpan(Offset));
            date = date - Offset + timeSpan;
            return new DateOnlyOffset(new DateOnly(date.Year, date.Month, date.Day), timeSpan);
        }
    }
}