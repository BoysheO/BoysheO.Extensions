using System;
using System.Reflection;

namespace BoysheO.Extensions.CustomerAttributeMeta
{
    /// <summary>
    /// Rules:<br/>
    /// 1.Implement it by a POCO class name XXX_AtbMeta,XXX is your type name<br />
    /// 2.Write the point 1 code in the same file same location the ICustomerAttributeMeta.Owner written in.<br />
    /// 3.Don't modify it in runtime,keep all members immutable.<br />
    /// *if count of attribute is 1,set in CustomerAttribute and make CustomerAttributes null for reduce memory.
    /// </summary>
    public interface ICustomerAttributeMeta
    {
        Type Owner { get; }
        Attribute? CustomerAttribute { get; }
        Attribute[]? CustomerAttributes { get; }
    }
}