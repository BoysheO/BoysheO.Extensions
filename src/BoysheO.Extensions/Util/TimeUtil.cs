﻿using System;
using System.Globalization;
using BoysheO.Extensions;

namespace DateAndTime
{
    public static class TimeUtil
    {
        public const int MillSecsADay = 86400000;

        /// <summary>
        /// 计算一个每日时刻在一段时间中的出现次数<br />
        /// calculate the count of the time of day between start to end 
        /// </summary>
        /// <param name="startMs">起始时间戳，需要保证起点为0点</param>
        /// <param name="endMs">终止时间戳，需要保证0时刻与startMs语义一致</param>
        /// <param name="timeMsOfDay">每日时刻</param>
        /// <param name="timeOffset">时区，指明timeMsOfDay所表达的时区</param>
        /// <returns></returns>
        public static long GetCountOfTheTimeBetween(long startMs, long endMs, int timeMsOfDay, int timeOffset)
        {
            if (startMs > endMs)
                throw new ArgumentOutOfRangeException(nameof(startMs), "startMs should be less than endMs");
            if (!(timeMsOfDay >= 0 && timeMsOfDay < MillSecsADay))
                throw new ArgumentOutOfRangeException(nameof(timeMsOfDay), "timeMsOfDay should be in [0,86400000)");
            if (!(timeOffset >= -MillSecsADay / 2 && timeOffset <= MillSecsADay / 2))
                throw new ArgumentOutOfRangeException(nameof(timeOffset));
            timeMsOfDay -= timeOffset;
            while (timeMsOfDay < 0)
            {
                timeMsOfDay += MillSecsADay;
            }

            var startDays = startMs / MillSecsADay;
            var endDays = endMs / MillSecsADay;
            var isSameDay = endDays == startDays;
            var timeMsOfStartDay = startMs % MillSecsADay;
            var timeMsOfEndDay = endMs % MillSecsADay;
            if (isSameDay)
            {
                return timeMsOfStartDay >= timeMsOfDay && timeMsOfEndDay < timeMsOfDay ? 1 : 0;
            }

            var times = 0L;
            if (timeMsOfStartDay <= timeMsOfDay) times++;
            if (timeMsOfEndDay > timeMsOfDay) times++;
            var startMidNight = startDays + 1;
            var deltaDay = endDays - startMidNight;
            times += deltaDay;
            return times;
        }
    }
}