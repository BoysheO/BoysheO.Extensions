using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using BoysheO.Extensions.CustomerAttributeMeta;
using UnityEngine.Scripting;

namespace BoysheO.Extensions.CustomerAttributeData.Test;



[MyInfo("hello")]
public class Tests
{
    [UnityEngine.Scripting.Preserve]
    private class Tests_AtbMeta : ICustomerAttributeMeta
    {
        public Type Owner => typeof(Tests);
        public Attribute? CustomerAttribute { get; } = new MyInfoAttribute("hello");

        public Attribute[] CustomerAttributes { get; } = null;
    }
    
    [Test]
    public void Test1()
    {
        var type = typeof(Tests);
        var atb = type.GetCustomAttribute<MyInfoAttribute>() ?? throw new Exception();
        var atb2 = type.GetCustomerAttributeMeta<MyInfoAttribute>(true) ?? throw new Exception();
        Assert.That(atb2.Str, Is.EqualTo(atb.Str));
    }
}