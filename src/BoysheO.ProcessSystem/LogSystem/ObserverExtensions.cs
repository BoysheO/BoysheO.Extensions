﻿using System;

namespace BoysheO.ProcessSystem.LogSystem
{
    public static class ObserverExtensions
    {
        public static void LogInformation(this IObserver<Log>? observer, string log)
        {
            observer?.OnNext(new Log(LogLevel.N, log));
        }

        public static void LogError(this IObserver<Log>? observer, string log)
        {
            observer?.OnNext(new Log(LogLevel.E, log));
        }
    }
}