using System;

namespace Hrx
{
    public readonly struct Element<T>
    {
        public enum StateEnum
        {
            OnCompete = 0,
            OnNext,
            OnError,
        }

        public readonly StateEnum State;
        public readonly T Value;
        public readonly Exception Exception;

        public Element(T value)
        {
            State = StateEnum.OnNext;
            Value = value;
            Exception = null;
        }

        public Element(Unit _)
        {
            State = StateEnum.OnCompete;
            Value = default;
            Exception = null;
        }

        public Element(Exception exception)
        {
            State = StateEnum.OnError;
            Value = default;
            Exception = exception;
        }
    }
}