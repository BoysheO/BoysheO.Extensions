using System;
using System.Linq;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public partial class MathLibraryTest
{
    [TestCase(0, ExpectedResult = "1")]
    [TestCase(1, ExpectedResult = "1")]
    [TestCase(2, ExpectedResult = "2")]
    [TestCase(3, ExpectedResult = "6")]
    [TestCase(4, ExpectedResult = "24")]
    [TestCase(12, ExpectedResult = "479001600")]
    public string Factorial1(int src)
    {
        return MathLibrary.Factorial1(src).ToString();
    }
    
    [TestCase(0, ExpectedResult = "1")]
    [TestCase(1, ExpectedResult = "1")]
    [TestCase(2, ExpectedResult = "2")]
    [TestCase(20, ExpectedResult = "2432902008176640000")]
    public string Factorial2(int src)
    {
        return MathLibrary.Factorial2(src).ToString();
    }

    [TestCase(1,1, ExpectedResult = "1")]
    [TestCase(2,1, ExpectedResult = "2")]
    [TestCase(3,2, ExpectedResult = "6")]
    [TestCase(12,6, ExpectedResult = "665280")]
    [TestCase(12,12, ExpectedResult = "479001600")]
    public string Permutation(int n, int k)
    {
        return MathLibrary.Permutation(n, k).ToString();
    }
    
    [TestCase(1,1, ExpectedResult = "1")]
    [TestCase(2,1, ExpectedResult = "2")]
    [TestCase(3,2, ExpectedResult = "3")]
    [TestCase(12,6, ExpectedResult = "924")]
    [TestCase(12,12, ExpectedResult = "1")]
    public string Combination(int n, int k)
    {
        return MathLibrary.Combination(n, k).ToString();
    }
}