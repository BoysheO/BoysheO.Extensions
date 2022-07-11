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
}