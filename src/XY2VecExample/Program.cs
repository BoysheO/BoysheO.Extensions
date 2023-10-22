// // See https://aka.ms/new-console-template for more information
//
// using System.Diagnostics;
// using System.Runtime.InteropServices;
// using BoysheO.Extensions;
// using XY2VecExample;
// using MemoryExtensions = System.MemoryExtensions;
//
// Console.WriteLine("Hello, World!");
//
// var x = Random.Shared.Next() * (Random.Shared.NextSingle() > 0.5f ? -1 : 1);
// var y = Random.Shared.Next() * (Random.Shared.NextSingle() > 0.5f ? -1 : 1);
// Console.WriteLine(
//     $"x={x}[{x.AsMemoryByteSpan().ToArray().ToHexText()}] y={y}[{y.AsMemoryByteSpan().ToArray().ToHexText()}]");
// {
//     var d = (x, y);
//     var size = 2 * sizeof(int);
//     Span<byte> span = stackalloc byte[size];
//     MemoryMarshal.Write(span, ref d);
//     var res = MemoryMarshal.Read<(int x, int y)>(span);
//
//     Console.WriteLine($"元组MemoryMarshal组合法={span.ToArray().ToHexText()}");
//     Console.WriteLine($"元组MemoryMarshal解法xy=({res.x},{res.y})");
// }
// Console.WriteLine();
// {
//     var xx = unchecked((ulong) x) << 8 * sizeof(int);
//     Console.WriteLine($"ulong_x[{xx.AsMemoryByteSpan().ToArray().ToHexText()}");
//     var yy = unchecked((ulong) y) & 0x00000000FFFFFFFF;
//     Console.WriteLine($"ulong_y[{yy.AsMemoryByteSpan().ToArray().ToHexText()}]");
//     ulong xy = xx | yy; //*重要不能是加法，加法会导致进一位
//
//     var xp = unchecked((int) (xy >> 8 * sizeof(int)));
//     var yp = unchecked((int) (xy));
//
//     Console.WriteLine($"位运算组合法xy={xy}[{xy.AsMemoryByteSpan().ToArray().ToHexText()}]");
//     Console.WriteLine($"位运算解法xy = {(xp, yp)}");
// }
// Console.WriteLine();
// {
//     ulong xy = Helper.UnsafeCombine(x, y);
//     var (x1, y1) = Helper.UnsafeDepart(xy);
//     Console.WriteLine($"unsafe组合法：xy={xy}[{xy.AsMemoryByteSpan().ToArray().ToHexText()}]");
//     Console.WriteLine($"unsafe解法：x={x1} y={y1}");
// }