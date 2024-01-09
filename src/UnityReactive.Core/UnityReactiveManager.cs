using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("UnityReactive.Core.Test")]

namespace UnityReactive.Core
{
    internal sealed class UnityReactiveManager
    {
        public static UnityReactiveManager Instance { get; } = new();
        private long _observableVersion;
        private long _unsubscribeVersion;

        private sealed class Observable
        {
            public long Ptr;
            public Type? ElementType; //for debug

            public string? TrackMsg; //for debug

            //timeLife: allocate at first observer added and release every count=0
            public IUObserver?[]? ObserverList;
            public long[]? Unsubscribe;
            public int ObserverCount = 0;
        }

        private readonly ReaderWriterLockSlim _observablesLocker = new();
        private readonly Dictionary<long, Observable> _observables = new();
        private readonly ConcurrentStack<Observable> _pool = new();


        private readonly Dictionary<long, Observable> _unsubscribe2observable = new();

        private Observable Rent(Type type, string? trackingMsg)
        {
            if (!_pool.TryPop(out var ins)) ins = new Observable();
            ins.Ptr = Interlocked.Increment(ref _observableVersion);
            ins.ElementType = type;
            ins.TrackMsg = trackingMsg;
            return ins;
        }

        private void Return(Observable ins)
        {
            ins.Ptr = 0;
            ins.ElementType = null;
            if (ins.ObserverCount > 0)
            {
                Array.Clear(ins.Unsubscribe!, 0, ins.ObserverCount);
                Array.Clear(ins.ObserverList!, 0, ins.ObserverCount);
                ArrayPool<IUObserver?>.Shared.Return(ins.ObserverList);
                ArrayPool<long>.Shared.Return(ins.Unsubscribe);
                ins.ObserverList = null;
                ins.Unsubscribe = null;
                ins.TrackMsg = null;
            }

            ins.ObserverCount = 0;
            _pool.Push(ins);
        }

        internal long CreatObservable(Type elementType, string? trackingMsg)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            var ob = Rent(elementType, trackingMsg);
            _observablesLocker.EnterWriteLock();
            _observables.Add(ob.Ptr, ob);
            _observablesLocker.ExitWriteLock();
            return ob.Ptr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr">observable ptr</param>
        /// <param name="element"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>errorCode.0:success 1.dead observable</returns>
        internal int OnNext<T>(long ptr, T element)
        {
            _observablesLocker.EnterReadLock();
            _observables.TryGetValue(ptr, out var observable);
            _observablesLocker.ExitReadLock();
            if (observable == null) return 1; //dead observable
            if (typeof(T) != observable.ElementType)
                throw new Exception($"type mismatch,obType={observable.ElementType},but arg={typeof(T)}");

            lock (observable)
            {
                if (observable.ObserverCount > 0)
                {
                    for (var i = observable.ObserverCount - 1; i >= 0; i--)
                    {
                        if (observable.ObserverList == null) break; //all observer has been removed
                        var oob = observable.ObserverList[i];
                        if (oob == null) continue; //this observer has been removed
                        var ob = oob as IUObserver<T>;
                        ob?.OnNext(element);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr">observable ptr</param>
        /// <returns>error code.0:success 1:dead observable</returns>
        internal int OnCompeted(long ptr)
        {
            _observablesLocker.EnterWriteLock();
            var observable = _observables.TryGetValue(ptr, out var o) ? o : null;
            if (observable != null) _observables.Remove(ptr);
            _observablesLocker.ExitWriteLock();
            if (observable == null) return 1; //dead

            lock (observable)
            {
                if (observable.ObserverCount > 0)
                {
                    for (var i = observable.ObserverCount - 1; i >= 0; i--)
                    {
                        var ary = observable.ObserverList;
                        if (ary == null) break; //all observer has been removed
                        var ob = ary[i];
                        if (ob == null) continue; //this observer has been removed
                        ob.OnDead();
                    }
                }

                if (observable.ObserverCount > 0)
                {
                    lock (_unsubscribe2observable)
                    {
                        var unsubscribeAry = observable.Unsubscribe!;
                        foreach (var l in unsubscribeAry)
                        {
                            _unsubscribe2observable.Remove(l); //remove all unsubscribe from dic
                        }
                    }
                }
            }

            Return(observable);
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr">observable ptr</param>
        /// <param name="observer"></param>
        /// <typeparam name="T">element type</typeparam>
        /// <returns>unsubscribe ptr.0 if dead</returns>
        internal long Subscribe<T>(long ptr, IUObserver<T> observer)
        {
            return Subscribe(ptr, (IUObserver)observer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr">observable ptr</param>
        /// <param name="observer"></param>
        /// <returns>unsubscribe ptr.0 if dead</returns>
        internal long Subscribe(long ptr, IUObserver observer)
        {
            _observablesLocker.EnterReadLock();
            var observable = _observables.TryGetValue(ptr, out var o) ? o : null;
            _observablesLocker.ExitReadLock();
            if (observable == null)
            {
                observer.OnDead();
                return 0; //dead observable
            }

            long unsubscribe;
            lock (observable)
            {
                var requestSize = observable.ObserverCount + 1;
                if (observable.ObserverList == null || observable.ObserverList.Length < requestSize)
                {
                    var newObserverBuffer = ArrayPool<IUObserver>.Shared.Rent(requestSize);
                    var newUnsubscribeBuffer = ArrayPool<long>.Shared.Rent(requestSize);
                    if (observable.ObserverCount > 0)
                    {
                        Array.Copy(observable.ObserverList!, newObserverBuffer, observable.ObserverCount);
                        Array.Copy(observable.Unsubscribe!, newUnsubscribeBuffer, observable.ObserverCount);
                        ArrayPool<IUObserver?>.Shared.Return(observable.ObserverList, true);
                        ArrayPool<long>.Shared.Return(observable.Unsubscribe, false);
                    }

                    observable.ObserverList = newObserverBuffer;
                    observable.Unsubscribe = newUnsubscribeBuffer;
                }

                observable.ObserverList[observable.ObserverCount] = observer;
                unsubscribe = Interlocked.Increment(ref _unsubscribeVersion);
                observable.Unsubscribe![observable.ObserverCount] = unsubscribe;
                observable.ObserverCount++;
                lock (_unsubscribe2observable)
                {
                    _unsubscribe2observable.Add(unsubscribe, observable);
                }
            }

            return unsubscribe;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsubscribe">unsubscribe ptr</param>
        /// <returns>error code.0:success 1:dead observable,no such unsubscribe or unsubscribed</returns>
        internal int Unsubscribe(long unsubscribe)
        {
            Observable observable;
            lock (_unsubscribe2observable)
            {
                if (!_unsubscribe2observable.TryGetValue(unsubscribe, out observable))
                {
                    return 1;
                }

                _unsubscribe2observable.Remove(unsubscribe);
            }

            lock (observable)
            {
                if (observable.ObserverCount == 0) return 1; //invalid unsubscribe
                var observers = observable.ObserverList!;
                var unsubscribes = observable.Unsubscribe!;
                var idx = Array.IndexOf(unsubscribes, unsubscribe);
                if (idx < 0) return 1; //invalid unsubscribe

                int end = observable.ObserverCount - 2;
                for (int i = idx; i < end; i++)
                {
                    observers[idx] = observers[i + 1];
                }

                observers[observable.ObserverCount - 1] = null;

                for (int i = idx; i < end; i++)
                {
                    unsubscribes[idx] = unsubscribes[i + 1];
                }

                observable.ObserverCount--;
                if (observable.ObserverCount == 0)
                {
                    ArrayPool<IUObserver>.Shared.Return(observable.ObserverList!);
                    ArrayPool<long>.Shared.Return(observable.Unsubscribe);
                    observable.ObserverList = null;
                    observable.Unsubscribe = null;
                }
            }

            return 0;
        }

        internal int ObserverCount(long observablePtr)
        {
            _observablesLocker.EnterReadLock();
            var observable = _observables.TryGetValue(observablePtr, out var o) ? o : null;
            _observablesLocker.ExitReadLock();
            return observable?.ObserverCount ?? 0;
        }

        internal bool IsDead(long ptr)
        {
            _observablesLocker.EnterReadLock();
            var isAlive = _observables.ContainsKey(ptr);
            _observablesLocker.ExitReadLock();
            return !isAlive;
        }
    }
}