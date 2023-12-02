using System;
using System.Collections.Generic;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class TypeExtensionsTests
{
    
    [TestCase(typeof(List<List<Int32>>), ExpectedResult = "List<List<Int32>>")]
    [TestCase(typeof(List<List<int?>>), ExpectedResult = "List<List<Int32?>>")]
    [TestCase(typeof(Dictionary<Int32,Int32>), ExpectedResult = "Dictionary<Int32,Int32>")]
    public string NiceTypeName(Type type)
    {
        return type.GetTypeCode();
    }

    [TestCase(typeof(List<List<Int32>>), ExpectedResult = "List<List<Int32>>")]
    [TestCase(typeof(Dictionary<Int32,Int32>), ExpectedResult = "Dictionary<Int32,Int32>")]
    public string NiceFullTypeName(Type type)
    {
        return type.GetTypeCode();
    }

    private class IsImplementCase
    {
        public Type Type { get; init; }
        public Type BaseType { get;init; }
        public bool Expect { get; init; }

        public static List<IsImplementCase> GetCases()
        {
            return new List<IsImplementCase>()
            {
                new IsImplementCase()
                {
                    Type = typeof(List<int>),
                    BaseType = typeof(IList<int>),
                    Expect = true,
                },
                new IsImplementCase()
                {
                    Type = typeof(List<int>),
                    BaseType = typeof(IList<>),
                    Expect = true,
                },
                new IsImplementCase()
                {
                    Type = typeof(List<>),
                    BaseType = typeof(IList<>),
                    Expect = true,
                },
                new IsImplementCase()
                {
                    Type = typeof(List<>),
                    BaseType = typeof(IList<int>),
                    Expect = false,
                },
                new IsImplementCase()
                {
                    Type = typeof(List<int>),
                    BaseType = typeof(IList<long>),
                    Expect = false,
                }
            };
        }

        public override string ToString()
        {
            return $"Type={Type.Name} Base={BaseType.Name} Expect={Expect}";
        }
    }
    
    [TestCase]
    public void IsImplement()
    {
        var cases = IsImplementCase.GetCases();
        foreach (var isImplementCase in cases)
        {
            try
            {
                var r = isImplementCase.Type.IsImplement(isImplementCase.BaseType);
                Assert.AreEqual(isImplementCase.Expect,r);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"case ({isImplementCase}) fail.ex={ex}");
            }
        }
    }
}