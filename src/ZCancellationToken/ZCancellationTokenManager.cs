using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ZCancellationToken.Test")]
namespace PooledCancellationToken
{
    internal class ZCancellationTokenManager
    {
        private struct TokenInfo
        {
            public readonly struct ActionOrToken
            {
                private static ulong _id = 0;
                public readonly Action? Action;
                public readonly int Token;
                public readonly ulong Id;

                public ActionOrToken(Action action)
                {
                    Action = action ?? throw new ArgumentNullException(nameof(action));
                    Token = 0;
                    Id = _id++;
                }

                public ActionOrToken(int token)
                {
                    this.Action = null;
                    Token = token;
                    Id = _id++;
                }
            }

            public readonly int Order;
            public bool IsCancel;
            public List<int>? UseRequestIds;
            public int LastUseRequestId;

            public bool IsDisposed;

            //it is stack
            public List<ActionOrToken>? OnCancel;

            public TokenInfo(int order)
            {
                this.Order = order;
                IsCancel = default;
                UseRequestIds = default;
                OnCancel = default;
                IsDisposed = default;
                LastUseRequestId = default;
            }

            public ActionOrToken Pop()
            {
                var lastIdx = OnCancel.Count - 1;
                var last = OnCancel[lastIdx];
                OnCancel.RemoveAt(lastIdx);
                return last;
            }

            public void Push(ActionOrToken aot)
            {
                OnCancel.Add(aot);
            }
        }

        private readonly Stack<List<TokenInfo.ActionOrToken>> _pool = new();
        private readonly Stack<List<int>> _useIdsPool = new();
        internal static readonly ZCancellationTokenManager Instance = new();
        private int tokenCount;
        private readonly SortedDictionary<int, TokenInfo> _ver2tkInfo = new();

        internal int CreatAndUse()
        {
            var v = 0;
            while (v == -1 || v == 0 || _ver2tkInfo.ContainsKey(v))
            {
                v = tokenCount++;
            }

            var inf = new TokenInfo(v);
            inf.UseRequestIds = _useIdsPool.Count > 0 ? _useIdsPool.Pop() : new List<int>();
            inf.UseRequestIds.Add(0);
            _ver2tkInfo[v] = inf;
            return v;
        }

        //ret=>useRequestId
        internal int Use(int version)
        {
            var inf = GetTokenInfOrThrowException(version);
            var id = inf.LastUseRequestId++;
            inf.UseRequestIds.Add(id);
            _ver2tkInfo[version] = inf;
            return id;
        }

        //ret:=>is disposed totally
        internal bool Unuse(int version, int useId)
        {
            if (!_ver2tkInfo.TryGetValue(version, out var inf))
            {
                // disposed totally
                return true;
            }

            inf.UseRequestIds.Remove(useId);
            if (inf.UseRequestIds.Count == 0)
            {
                if (inf.OnCancel != null)
                {
                    inf.OnCancel.Clear();
                    _pool.Push(inf.OnCancel);
                    inf.OnCancel = null;
                }

                _useIdsPool.Push(inf.UseRequestIds);
                inf.UseRequestIds = null;
                _ver2tkInfo.Remove(version);
                return true;
            }

            _ver2tkInfo[version] = inf;
            return false;
        }

        public bool IsCancellationRequested(int version)
        {
            var inf = GetTokenInfOrThrowException(version);
            return inf.IsCancel;
        }

        public bool CanBeCanceled(int token)
        {
            if (token == 0) return false;
            var inf = GetTokenInfOrThrowException(token);
            ThrowIfTokenIsDisposed(inf);
            return true;
        }

        private TokenInfo GetTokenInfOrThrowException(int token)
        {
            if (!_ver2tkInfo.TryGetValue(token, out var inf))
            {
                throw new Exception($"this token={token} is disposed totally");
            }

            return inf;
        }

        private void ThrowIfTokenIsDisposed(TokenInfo inf)
        {
            if (inf.IsDisposed) throw new Exception($"this token={inf.Order} is disposed");
        }

        public void Cancel(int version, bool throwOnFirstException, List<Exception>? unHandleException)
        {
            var inf = GetTokenInfOrThrowException(version);
            ThrowIfTokenIsDisposed(inf);
            unHandleException?.Clear();
            if (inf.IsCancel) return;
            inf.IsCancel = true;
            if (inf.OnCancel != null)
            {
                try
                {
                    if (throwOnFirstException)
                    {
                        while (inf.OnCancel.Count > 0)
                        {
                            var ac = inf.Pop();
                            if (ac.Action != null)
                            {
                                ac.Action.Invoke();
                            }
                            else
                            {
                                Cancel(ac.Token, throwOnFirstException, unHandleException);
                            }
                        }
                    }
                    else
                    {
                        while (inf.OnCancel.Count > 0)
                        {
                            var ac = inf.Pop();
                            try
                            {
                                if (ac.Action != null)
                                {
                                    ac.Action.Invoke();
                                }
                                else
                                {
                                    Cancel(ac.Token, throwOnFirstException, unHandleException);
                                }
                            }
                            catch (Exception exception)
                            {
                                unHandleException?.Add(exception);
                            }
                        }
                    }
                }
                finally
                {
                    inf.OnCancel.Clear();
                    _pool.Push(inf.OnCancel);
                    inf.OnCancel = null;
                    _ver2tkInfo[version] = inf;
                }
            }
            else _ver2tkInfo[version] = inf;
        }

        public void MarkSourceDispose(int version)
        {
            var inf = GetTokenInfOrThrowException(version);
            ThrowIfTokenIsDisposed(inf);
            var isTotallyDisposed = Unuse(version,0);
            if (isTotallyDisposed) return;
            inf.IsDisposed = true;
            _ver2tkInfo[version] = inf;
        }

        public int CreateLinkedTokenSource(int src)
        {
            var tk = GetTokenInfOrThrowException(src);
            ThrowIfTokenIsDisposed(tk);
            var newTk = CreatAndUse();
            if (tk.IsCancel)
            {
                Cancel(newTk, true, null);
            }
            else
            {
                AppendSthOnCancel(ref tk, new TokenInfo.ActionOrToken(newTk));
            }

            return newTk;
        }

        public int CreateLinkedTokenSource(int src1, int src2)
        {
            var tk1 = GetTokenInfOrThrowException(src1);
            ThrowIfTokenIsDisposed(tk1);
            var tk2 = GetTokenInfOrThrowException(src2);
            ThrowIfTokenIsDisposed(tk2);

            var newTk = CreatAndUse();
            if (tk1.IsCancel || tk2.IsCancel)
            {
                Cancel(newTk, true, null);
                return newTk;
            }

            AppendSthOnCancel(ref tk1, new TokenInfo.ActionOrToken(newTk));
            AppendSthOnCancel(ref tk2, new TokenInfo.ActionOrToken(newTk));

            return newTk;
        }

        public int CreateLinkedTokenSource(ZCancellationToken[] tokens, int count)
        {
            var newTk = CreatAndUse();
            for (int i = 0; i < count; i++)
            {
                var tk = tokens[i]._version;
                var inf = GetTokenInfOrThrowException(tk);
                ThrowIfTokenIsDisposed(inf);
                if (inf.IsCancel)
                {
                    Cancel(newTk, true, null);
                    return newTk;
                }

                AppendSthOnCancel(ref inf, new TokenInfo.ActionOrToken(newTk));
            }

            return newTk;
        }


        private void AppendSthOnCancel(ref TokenInfo inf, TokenInfo.ActionOrToken actionOrToken)
        {
            if (inf.OnCancel == null)
            {
                if (_pool.Count > 0)
                {
                    inf.OnCancel = _pool.Pop();
                }
                else
                {
                    inf.OnCancel = new();
                }
            }

            inf.Push(actionOrToken);
        }

        public CancelRegister Register(int version, Action callback)
        {
            var inf = GetTokenInfOrThrowException(version);
            ThrowIfTokenIsDisposed(inf);
            var arg = new TokenInfo.ActionOrToken(callback);
            AppendSthOnCancel(ref inf, arg);
            return new CancelRegister(version, arg.Id);
        }

        public void UnRegister(int version, ulong actionOrTokenId)
        {
            var inf = GetTokenInfOrThrowException(version);
            if (inf.OnCancel == null) return;
            for (var index = 0; index < inf.OnCancel.Count; index++)
            {
                var actionOrToken = inf.OnCancel[index];
                if (actionOrToken.Id == actionOrTokenId)
                {
                    inf.OnCancel.RemoveAt(index);
                    return;
                }
            }
        }

        public bool IsDisposed(int version)
        {
            var inf = GetTokenInfOrThrowException(version);
            return inf.IsDisposed;
        }
    }
}