using System;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

[TestFixture]
public class StringUtilTest
{
    [TestCase(0UL, "0.00B")]
    [TestCase(1023UL, "1023.00B")]
    [TestCase(1024UL, "1.00KB")]
    [TestCase(1048576UL, "1.00MB")]
    [TestCase(1073741824UL, "1.00GB")]
    [TestCase(1099511627776UL, "1.00TB")]
    [TestCase(1125899906842624UL, "1.00PB")]
    [TestCase(1152921504606846976UL, "1.00EB")]
    [TestCase(1536UL, "1.50KB")] // 1536 bytes = 1.5 KB
    [TestCase(1572864UL, "1.50MB")] // 1572864 bytes = 1.5 MB
    public void BytesToReadableSize_ValidInputs_ReturnsExpectedStrings(ulong bytesCount, string expected)
    {
        // Act
        (double size, StringUtil.CapacityUnit unit) = StringUtil.BytesToReadableSize(bytesCount);

        var result = $"{size:F}{unit}";
        
        // Assert
        Assert.AreEqual(expected, result);
    }
}