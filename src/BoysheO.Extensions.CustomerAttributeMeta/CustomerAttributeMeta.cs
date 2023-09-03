using System;

namespace BoysheO.Extensions.CustomerAttributeMeta
{
    public abstract class CustomerAttributeMeta : ICustomerAttributeMeta
    {
        protected CustomerAttributeMeta(Type type, Attribute attribute)
        {
            Owner = type ?? throw new ArgumentNullException(nameof(type));
            CustomerAttribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            CustomerAttributes = null;
        }

        protected CustomerAttributeMeta(Type type, params Attribute[] attributes)
        {
            Owner = type ?? throw new ArgumentNullException(nameof(type));
            CustomerAttributes = attributes;
        }

        public Type Owner { get; }
        public Attribute? CustomerAttribute { get; } = null;
        public Attribute[]? CustomerAttributes { get; } = null;
    }
}