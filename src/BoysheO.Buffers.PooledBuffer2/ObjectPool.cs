using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

//using BoysheO.Extensions;

namespace BoysheO.Buffers
{
    internal class ObjectPool<TCollection> where TCollection : new()
    {
        public static readonly ObjectPool<TCollection> Share = new ObjectPool<TCollection>();
        private SortedList<TCollection, long> lst2version = new();
        private Stack<TCollection> idleLst = new();
        private long _curversion;
        private readonly object gate = new object();

        public TCollection Rent(out long version)
        {
            lock (gate)
            {
                TCollection ins = idleLst.Count > 0 ? idleLst.Pop() : new TCollection();
                version = ++_curversion;
                lst2version[ins] = version;
                return ins;
            }
        }

        public long GetVersion(TCollection list)
        {
            return lst2version.TryGetValue(list, out var v) ? v : 0;
        }

        public void Return(TCollection list)
        {
            var r = lst2version.Remove(list);
            if (r) idleLst.Push(list);
        }
    }
}