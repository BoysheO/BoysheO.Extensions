using BoysheO.Buffers;

namespace BoysheO.Buffer.PooledBuffer.Dictionary;

public interface IReadOnlyPooledDictionaryBuff<TKey, TValue>
{
    TValue this[TKey key] { get; }

    bool ContainsKey(TKey key);

    bool TryGetValue(TKey key, out TValue value);

    TValue GetValueOrDefault(TKey key);

    DictionaryBufferEnumerator<TKey, TValue> GetEnumerator();

    PooledListBuffer<TKey> GetKeys();
    PooledListBuffer<TValue> GetValues();
}