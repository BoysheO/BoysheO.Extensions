#nullable enable
using System;
using System.Collections.Immutable;

namespace GetCustomerAttributeBaseCodeGen
{
    public interface ICustomerAttributeInfoRepo
    {
        ImmutableDictionary<Type, ImmutableArray<object>> Type2Attributes { get; }
    }
}