using System.Collections.Generic;

namespace BoysheO.Extensions
{
    public static class KeyValuePairExtensions
    {
        //这个函数实现解构函数的方式是错误的
        //this method is incorrect
        // public static (TK key, TV value) Deconstruct<TK, TV>(this KeyValuePair<TK, TV> kvp)
        // {
        //     return (kvp.Key, kvp.Value);
        // }

        public static void Deconstruct<TK, TV>(this KeyValuePair<TK, TV> kvp, out TK key, out TV value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}