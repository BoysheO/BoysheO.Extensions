using System;

namespace PooledCancellationToken;

public readonly struct CancelUse:IDisposable
{
    internal readonly int Version;
    internal readonly int UseId;

    internal CancelUse(int version, int useId)
    {
        Version = version;
        UseId = useId;
    }

    public void Dispose()
    {
        if (Version == 0) return;
        ZCancellationTokenManager.Instance.Unuse(Version, UseId);
    }
}