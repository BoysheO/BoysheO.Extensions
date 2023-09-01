using System;
using System.Threading;

namespace PooledCancellationToken;

public readonly struct ZCancellationToken
{
    /// <summary>
    /// 0:default,not cancelable
    /// 
    /// </summary>
    internal readonly int _version;

    public ZCancellationToken(bool canceled)
    {
        if (canceled) _version = -1;
        _version = default;
    }

    internal ZCancellationToken(int version)
    {
        _version = version;
    }

    /// <summary>
    /// tell the token is in 'use',and invoke the return IDisposable.Dispose to tell the token is not in 'use'
    /// </summary>
    public CancelUse Use()
    {
        var useId = ZCancellationTokenManager.Instance.Use(_version);
        return new CancelUse(_version, useId);
    }

    public bool IsCancellationRequested
    {
        get
        {
            if (_version == -1) return true;
            if (_version == 0) return false;
            return ZCancellationTokenManager.Instance.IsCancellationRequested(_version);
        }
    }

    public bool CanBeCanceled => _version != 0 && ZCancellationTokenManager.Instance.CanBeCanceled(_version);

    public static ZCancellationToken None => default;

    public object WaitHandle
    {
        get { throw new NotImplementedException(); }
    }

    public CancelRegister Register(Action callback)
    {
        if (IsCancellationRequested)
        {
            callback();
            return default;
        }
        
        return ZCancellationTokenManager.Instance.Register(_version, callback);
    }

    public bool Equals(ZCancellationToken other)
    {
        return _version == other._version;
    }

    public override bool Equals(object? other)
    {
        return other is ZCancellationToken token && Equals(token);
    }

    public override int GetHashCode()
    {
        return _version;
    }

    public static bool operator ==(ZCancellationToken left, ZCancellationToken right) => left.Equals(right);

    public static bool operator !=(ZCancellationToken left, ZCancellationToken right) => !left.Equals(right);

    public void ThrowIfCancellationRequested()
    {
        if (IsCancellationRequested)
            throw new OperationCanceledException("OperationCanceled");
    }
}