using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Util
{
    public static class UnsafeUtil
    {
        /// <summary>
        ///     Look through the value as bytes in memory.<br />
        ///     *<b>UNSAFE</b>:Changing the return value causes the parameter value changed.
        ///     If you change the string,the program maybe crash.The suggestion
        ///     is use this for debug only.And,I don't promise the return value
        ///     is same in different hardware environment.Only the base value
        ///     can be the parameters.<br />
        ///     将该值类型转换成内存中byte表示，无复制,仅限基础类型，否则会异常.
        ///     *不安全：修改返回的span数组会修改初始值.在不清楚自己在做什么的时候不要修改返回值
        /// </summary>
        public static Span<byte> AsMemoryByteSpan<T>(ref T value) where T : unmanaged
        {
            unsafe
            {
                void* ptr = Unsafe.AsPointer(ref value);
                return new Span<byte>(ptr, sizeof(T));
                // fixed (T* p = &value)
                // {
                //     var span = new Span<byte>(p, sizeof(T));
                //     // Console.WriteLine(span.ToHexString());
                //     return span;
                // }
            }
        }
        
        public static Span<TTo> As<TFrom, TTo>(Span<TFrom> src)
        {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            unsafe
            {
                fixed (void* p = src)
                {
                    var c = (TTo*)p;
                    return new Span<TTo>(c, src.Length);
                }
            }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        }
        
        public static ReadOnlySpan<TTo> As<TFrom, TTo>(ReadOnlySpan<TFrom> src)
        {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            unsafe
            {
                fixed (void* p = src)
                {
                    var c = (TTo*)p;
                    return new ReadOnlySpan<TTo>(c, src.Length);
                }
            }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        }
    }
}