//SynchronizeRandom弃用了
// using BoysheO.Toolkit;
//
// namespace BoysheO.Util
// {
//     public static class SynchronizeRandomUtil
//     {
//         public static readonly SynchronizeRandom Random = new SynchronizeRandom();
//
//         #region value
//
//         public static float Float => (float) Random.NextDouble();
//
//         public static long Long
//         {
//             get
//             {
//                 long low = Int;
//                 long high = Int;
//                 return (high << 32) | low;
//             }
//         }
//
//         public static short Short => (short) Int;
//
//         public static int Int => Random.Next();
//
//         public static bool Boolean => (Int & 0x0000_0001) == 0x0000_0001;
//
//         #endregion
//
//         #region Range
//
//         public static float MoreEqualMinLessEqualMax(float min, float max)
//         {
//             return MathLibrary.Remap(Float, 0f, 1f, min, max);
//         }
//
//         public static int MoreEqualMinLessMax(int min, int max)
//         {
//             return Random.Next(min, max);
//         }
//
//         public static long MoreEqualMinLessMax(long min, long max)
//         {
//             var num = max - min;
//             return Long * num + min;
//         }
//
//         public static uint MoreEqualMinLessMax(uint min, uint max)
//         {
//             var num = max - min;
//             return (uint) Int * num + min;
//         }
//
//         #endregion
//     }
// }