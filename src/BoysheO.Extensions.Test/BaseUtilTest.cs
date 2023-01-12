using System;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using Base62;
using BoysheO.Util;
using Libraries;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public static class BaseUtilTest
{
    //等效性测试
    [TestCase]
    public static void TestBase62()
    {
        var randBytes = new byte[Random.Shared.Next(5, 10)];
        Random.Shared.NextBytes(randBytes);
        var libCode = randBytes.ToBase62();
        var charsCount = BaseNUtil.ToBase62AsPooledChar(randBytes, out var chars);
        var str = chars.AsSpan(0, charsCount).AsReadOnly().CreatString();
        Assert.AreSame(libCode,str);
    }
    
    //等效性测试
    [TestCase]
    public static void TestBase621()
    {
        var src = new byte[] { 20 };
        var lib = src.ToBase62();
        Console.WriteLine(lib);
        var charsCount = BaseNUtil.ToBase62AsPooledChar(src, out var chars);
        var str = chars.AsSpan(0, charsCount).AsReadOnly().CreatString();
        Console.WriteLine(str);
    }
    
    [TestCase]
    public static void TestBase622()
    {
        var src = Enumerable.Range(0, 64).Select(v => (byte)v).ToArray();
        var e = Convert.ToBase64String(src);
        Console.WriteLine(e);
        var cCount = BaseNUtil.ToBase64AsPooledChar(src, out var c);
        var me = c.AsSpan(0, cCount).AsReadOnly().CreatString();
        Console.WriteLine(me);
        var crypt = new Base64Crypt();
        var hisB = crypt.Encode(src);
        var his = Encoding.UTF8.GetString(hisB);
        Console.WriteLine(his);
    }
}