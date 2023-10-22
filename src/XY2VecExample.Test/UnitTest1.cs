namespace XY2VecExample.Test;

public class Tests
{
    [Test]
    public void Unsafe1()
    {
        var x = Random.Shared.Next(int.MinValue, int.MaxValue);
        var y = Random.Shared.Next(int.MinValue, int.MaxValue);
        var xy = Helper.UnsafeCombine(x, y);
        var r = Helper.UnsafeDepart(xy);
        Assert.That(r, Is.EqualTo((x, y)));
    }

    [Test]
    public void Unsafe2()
    {
        var x = Random.Shared.Next(int.MinValue, int.MaxValue);
        var y = Random.Shared.Next(int.MinValue, int.MaxValue);
        var xy = Helper.UnsafeCombine(x, y);
        var r = Helper.UnsafeDepart2(xy);
        Assert.That(r, Is.EqualTo((x, y)));
    }

    [Test]
    public void Unsafe3()
    {
        var x = Random.Shared.Next(int.MinValue, int.MaxValue);
        var y = Random.Shared.Next(int.MinValue, int.MaxValue);
        var xy = Helper.UnsafeCombine2(x, y);
        var r = Helper.UnsafeDepart2(xy);
        Assert.That(r, Is.EqualTo((x, y)));
    }

    [Test]
    public void Bit()
    {
        var x = Random.Shared.Next(int.MinValue, int.MaxValue);
        var y = Random.Shared.Next(int.MinValue, int.MaxValue);
        var xy = Helper.BitCombine(x, y);
        var r = Helper.BitDepart(xy);
        Assert.That(r, Is.EqualTo((x, y)));
    }

    [Test]
    public void Cast()
    {
        var x = Random.Shared.Next(int.MinValue, int.MaxValue);
        var y = Random.Shared.Next(int.MinValue, int.MaxValue);
        var xy = Helper.CastCombine(x, y);
        var r = Helper.CastDepart(xy);
        Assert.That(r, Is.EqualTo((x, y)));
    }

    [Test]
    public void BitCombineAndCastDepart()
    {
        var x = Random.Shared.Next(int.MinValue, int.MaxValue);
        var y = Random.Shared.Next(int.MinValue, int.MaxValue);
        var xy = Helper.BitCombine(x, y);
        var r = Helper.UnsafeDepart2(xy);
        Assert.That(r, Is.EqualTo((x, y)));
    } 
}