using System.Collections.Generic;

namespace Hrx
{
    public interface IHRxList<T> 
    {
        IList<T> Content { get; }
        IHRxProperty<int> Version { get; }
    }
}