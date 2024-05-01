using BoysheO.Buffer.PooledBuffer;

namespace BoysheO.Buffers.PooledBuffer.Test.BinSearchSortedSet;

public class SortedSetTest
{
    [TestCase(1000)]
    [TestCase(10000000)]
    public void Add(int times)
    {
        var set = new SortedSet<int>();
        var mySet = PooledBinSearchSortedSetPoolBuffer<int>.Rent(Comparer<int>.Default);

        for (int i = 0; i < times; i++)
        {
            var r = Random.Shared.Next(0, 100);
            set.Add(r);
            mySet.AsSet.Add(r);
        }

        var ary = set.ToArray();
        Array.Sort(ary);
        var res = ary.AsSpan().SequenceEqual(mySet.AsReadOnlyList.Span);
        Assert.IsTrue(res);
    }

    [Test]
    public void SortedSet_Generic_IntersectWith_SupersetEnumerableWithDups()
    {
        var a = PooledBinSearchSortedSetPoolBuffer<int>.Rent();
        var set = a.AsSet;
        set.UnionWith(new[] {1, 3, 5, 7, 9});
        set.IntersectWith(new[] {5, 7, 3, 7, 11, 7, 5, 2});
        Assert.That(set, Is.EqualTo(new[] {3, 5, 7}));
    }

    [Test]
    public void DirtyTest()
    {
        GC.TryStartNoGCRegion(1000000);
        var a = PooledBinSearchSortedSetPoolBuffer<int>.Rent();
        a.AsSortedSet.AddRange(new[] {1, 3, 5, 7, 9}.AsSpan());
        a.Dispose();

        var b = PooledBinSearchSortedSetPoolBuffer<int>.Rent();
        Assert.That(b.AsSet.Count == 0, Is.True);
        b.Dispose();
        GC.EndNoGCRegion();
    }
}