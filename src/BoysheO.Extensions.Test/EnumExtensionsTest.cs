using System;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class EnumExtensionsTest
{
    public enum NormalEnum
    {
        Min = int.MinValue,
        Max = int.MaxValue,
    }

    public enum ByteEnum : byte
    {
        Min = Byte.MinValue,
        Max = Byte.MaxValue,
    }

    public enum ULongEnum : ulong
    {
        Min = ulong.MinValue,
        Max = ulong.MaxValue,
    }

    [TestCase(NormalEnum.Min, ExpectedResult = (int) NormalEnum.Min)]
    [TestCase(NormalEnum.Max, ExpectedResult = (int) NormalEnum.Max)]
    public int NormalAsInt(NormalEnum normalEnum)
    {
        return normalEnum.AsInt();
    }

    [TestCase(ByteEnum.Min, ExpectedResult = (int) ByteEnum.Min)]
    [TestCase(ByteEnum.Max, ExpectedResult = (int) ByteEnum.Max)]
    public int ByteAsInt(ByteEnum e1)
    {
        Console.WriteLine(e1.AsInt());
        return e1.AsInt();
    }

    // [TestCase(E3.Max, ExpectedResult = (ulong) E3.Max)]
    // [TestCase(E3.Min, ExpectedResult = (ulong) E3.Min)]
    // public ulong UlongEnumAsInt(E3 e1)
    // {
    //     var i = e1.AsInt();
    //     Console.WriteLine($"{e1}={i},act={(ulong) e1}");
    //     return (ulong) i;
    // }
}