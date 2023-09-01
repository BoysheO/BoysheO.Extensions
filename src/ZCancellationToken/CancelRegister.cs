using System;

namespace PooledCancellationToken;

public readonly struct CancelRegister : IDisposable
{
    internal readonly int version;
    internal readonly ulong actionOrTokenId;

    internal CancelRegister(int version, ulong actionOrTokenId)
    {
        this.version = version;
        this.actionOrTokenId = actionOrTokenId;
    }

    public void Dispose()
    {
        if (version == 0) return;
        ZCancellationTokenManager.Instance.UnRegister(version, actionOrTokenId);
    }
}