using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using BoysheO.Extensions.CustomerAttributeMeta;
using UnityEngine.Scripting;

namespace BoysheO.Extensions.CustomerAttributeData.Test;

[MyInfo("hello2")]
public class Tests2
{
    [UnityEngine.Scripting.Preserve]
    private class Tests2_AtbMeta : CustomerAttributeMeta.CustomerAttributeMeta
    {
        public Tests2_AtbMeta() : base(typeof(Tests2), new MyInfoAttribute("hello2")) { }
    }
    
    [Test]
    public void Test1()
    {
        var type = typeof(Tests2);
        var atb = type.GetCustomAttribute<MyInfoAttribute>() ?? throw new Exception();
        var atb2 = type.GetCustomerAttributeMeta<MyInfoAttribute>(true) ?? throw new Exception();
        Assert.That(atb2.Str, Is.EqualTo(atb.Str));
    }
}