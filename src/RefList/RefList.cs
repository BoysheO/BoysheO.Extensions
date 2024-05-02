using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using BoysheO.Extensions.Util;

namespace RefList;

/// <summary>
/// Helpful to build Span
/// 帮助创建合适的Span对象。如果需要更复杂的操作，例如查找、排序，在Span上进行操作
/// </summary>
public ref struct RefList<T>
{
    private T[] _items;
    private int _count;
    private readonly ArrayPool<T> _pool;
    public int Count => _count;

    public RefList() : this(0)
    {
    }

    public RefList(int capacity, ArrayPool<T>? pool = null)
    {
        _count = 0;
        _pool = pool ?? ArrayPool<T>.Shared;
        _items = _pool.Rent(capacity);
    }

    public Span<T> AsSpan
    {
        get
        {
            ThrowIfDisposed();
            return _items.AsSpan(0, _count);
        }
    }

    public void Add(T item)
    {
        ThrowIfDisposed();
        RefArrayPoolUtil.Add(ref _items, ref _count, item, _pool);
    }

    public void Remove(T item)
    {
        ThrowIfDisposed();
        RefArrayPoolUtil.Remove(_items, ref _count, item);
    }

    public void Push(T item)
    {
        Add(item);
    }

    public T Pop()
    {
        return TryPop(out var item) ? item : throw new InvalidOperationException("EmptyStack");
    }

    public bool TryPop([MaybeNullWhen(false)] out T item)
    {
        if (TryPeek(out item))
        {
            _items[^1] = default;
            return true;
        }

        return false;
    }

    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
        ThrowIfDisposed();
        var lastIdx = _count - 1;
        if (lastIdx < 0)
        {
            item = default;
            return false;
        }

        item = _items[lastIdx];
        return true;
    }

    public void Clear()
    {
        AsSpan.Clear();
        _count = 0;
    }

    private void ThrowIfDisposed()
    {
        if (_items == null) throw new ObjectDisposedException(nameof(RefList<T>));
    }

    public void Disposable()
    {
        ThrowIfDisposed();
        _pool.Return(_items);
        _items = null!;
        _count = 0;
    }
}