using System;
using UniRx;

public static class RxExtensions
{
        /// <summary>
        /// 同步两个rxCollection
        /// 会首先将t1s的数据同步到t2s
        /// </summary>
        public static IDisposable KeepSame<T>(this ReactiveCollection<T> t1s,
            ReactiveCollection<T> t2s,
            Func<T, T, bool> eq)
        {
            if (t2s.Count != 0) throw new Exception("need empty");
            for (var index = 0; index < t1s.Count; index++)
            {
                var t1 = t1s[index];
                t2s.Add(t1);
            }

            var un1 = t1s.ObserveAdd().Subscribe(v =>
            {
                if (t2s.Count < t1s.Count) t2s.Insert(v.Index, v.Value);
            });
            var un2 = t2s.ObserveAdd().Subscribe(v =>
            {
                if (t1s.Count < t2s.Count) t1s.Insert(v.Index, v.Value);
            });
            var un3 = t1s.ObserveMove().Subscribe(v =>
            {
                if (eq(t2s[v.OldIndex], t1s[v.OldIndex]) && eq(t2s[v.NewIndex], t2s[v.NewIndex])) return;
                t2s.Move(v.OldIndex, v.NewIndex);
            });
            var un4 = t2s.ObserveMove().Subscribe(v =>
            {
                if (eq(t2s[v.OldIndex], t1s[v.OldIndex]) && eq(t2s[v.NewIndex], t2s[v.NewIndex])) return;
                t1s.Move(v.OldIndex, v.NewIndex);
            });
            var un5 = t1s.ObserveRemove().Subscribe(v =>
            {
                if (t2s.Count > t1s.Count) t2s.RemoveAt(v.Index);
            });
            var un6 = t2s.ObserveRemove().Subscribe(v =>
            {
                if (t1s.Count > t2s.Count) t1s.RemoveAt(v.Index);
            });
            var un7 = t1s.ObserveReplace().Subscribe(v =>
            {
                if (eq(t2s[v.Index], v.NewValue)) return;
                t2s[v.Index] = v.NewValue;
            });
            var un8 = t2s.ObserveReplace().Subscribe(v =>
            {
                if (eq(t1s[v.Index], v.NewValue)) return;
                t1s[v.Index] = v.NewValue;
            });
            var un9 = t1s.ObserveReset().Subscribe(v =>
            {
                if(t2s.Count!=0) t2s.Clear();
            });
            var un10 = t2s.ObserveReset().Subscribe(_ =>
            {
                if(t1s.Count!=0) t1s.Clear();
            });
            return Disposable.Create(() =>
            {
                un1.Dispose();
                un2.Dispose();
                un3.Dispose();
                un4.Dispose();
                un5.Dispose();
                un6.Dispose();
                un7.Dispose();
                un8.Dispose();
                un9.Dispose();
                un10.Dispose();
            });
        }
        
        
}