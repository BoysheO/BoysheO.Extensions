using System;
using System.Collections.Generic;
using System.Threading;

namespace PooledCancellationToken;

public readonly struct ZCancellationTokenSource : IDisposable
{
    /// <summary>
    /// 0:default.0 can not be canceled.
    /// -1:canceled.it's not possible to this value.
    /// </summary>
    private readonly int _version;

    internal ZCancellationTokenSource(int version)
    {
        _version = version;
    }

    public static ZCancellationTokenSource Creat()
    {
        var version = ZCancellationTokenManager.Instance.CreatAndUse();
        return new ZCancellationTokenSource(version);
    }

    public void Dispose()
    {
        ZCancellationTokenManager.Instance.MarkSourceDispose(_version);
    }

    public bool IsCancellationRequested => ZCancellationTokenManager.Instance.IsCancellationRequested(_version);

    public ZCancellationToken Token
    {
        get
        {
            ThrowIfDisposed();
            return new ZCancellationToken(_version);
        }
    }

    public void Cancel() => Cancel(false);

    public void Cancel(bool throwOnFirstException)
    {
        ZCancellationTokenManager.Instance.Cancel(_version, throwOnFirstException, null);
    }

    public void Cancel(bool throwOnFirstException, List<Exception> exceptionsOut)
    {
        ZCancellationTokenManager.Instance.Cancel(_version, throwOnFirstException, exceptionsOut);
    }

    private void ThrowIfDisposed()
    {
        if (ZCancellationTokenManager.Instance.IsDisposed(_version))
        {
            throw new ObjectDisposedException(null, "ZCancellationTokenSource_Disposed");
        }
    }

    public static ZCancellationTokenSource CreateLinkedTokenSource(ZCancellationToken token1, ZCancellationToken token2)
    {
        int version = ZCancellationTokenManager.Instance.CreateLinkedTokenSource(token1._version, token2._version);
        return new ZCancellationTokenSource(version);
    }

    public static ZCancellationTokenSource CreateLinkedTokenSource(ZCancellationToken token)
    {
        int version = ZCancellationTokenManager.Instance.CreateLinkedTokenSource(token._version);
        return new ZCancellationTokenSource(version);
    }

    public static ZCancellationTokenSource CreateLinkedTokenSource(ZCancellationToken[] tokens)
    {
        return CreateLinkedTokenSource(tokens, tokens.Length);
    }

    public static ZCancellationTokenSource CreateLinkedTokenSource(ZCancellationToken[] tokens, int count)
    {
        int version = ZCancellationTokenManager.Instance.CreateLinkedTokenSource(tokens, count);
        return new ZCancellationTokenSource(version);
    }
}