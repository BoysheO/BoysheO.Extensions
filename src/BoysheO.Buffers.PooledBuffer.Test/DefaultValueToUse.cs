namespace BoysheO.Buffers.PooledBuffer.Test;

public class Tests
{
    private PooledListBuffer<int> defaultList = default;
    private PooledDictionaryBuffer<int, int> defaultDic = default;

    /// <summary>
    /// should dispose no exception
    /// </summary>
    [Test]
    public void Disposable()
    {
        defaultList.Dispose();
        defaultDic.Dispose();
    }
}