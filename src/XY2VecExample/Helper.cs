using System.Runtime.CompilerServices;

namespace XY2VecExample
{
    public static class Helper
    {
        public static ulong UnsafeCombine(int x, int y)
        {
            ulong xy = 0;
            unsafe
            {
                var p = (byte*) &xy;
                var px = (byte*) &x;
                var py = (byte*) &y;
                p[0] = px[0];
                p[1] = px[1];
                p[2] = px[2];
                p[3] = px[3];
                p[4] = py[0];
                p[5] = py[1];
                p[6] = py[2];
                p[7] = py[3];
            }

            return xy;
        }

        public static ulong UnsafeCombine2(int x, int y)
        {
            var d = (x, y);
            unsafe
            {
                return *(ulong*) &d;
            }
        }

        public static (int x, int y) UnsafeDepart(ulong xy)
        {
            int x = 0, y = 0;
            unsafe
            {
                var p = (byte*) &xy;
                var px = (byte*) &x;
                var py = (byte*) &y;
                px[0] = p[0];
                px[1] = p[1];
                px[2] = p[2];
                px[3] = p[3];
                py[0] = p[4];
                py[1] = p[5];
                py[2] = p[6];
                py[3] = p[7];
            }

            return (x, y);
        }

        public static (int x, int y) UnsafeDepart2(ulong xy)
        {
            unsafe
            {
                (int x, int y)* d1 = ((int x, int y)*) (&xy);
                return *d1;
            }
        }

        public static ulong BitCombine(int x, int y)
        {
            var xx = unchecked((ulong) x) & 0x00000000FFFFFFFF;
            var yy = unchecked((ulong) y) << 8 * sizeof(int);
            ulong xy = xx | yy; //*重要不能是加法，加法会导致进一位
            return xy;
        }

        public static (int x, int y) BitDepart(ulong xy)
        {
            var xpp = unchecked((int) xy);
            var ypp = unchecked((int) (xy >> 8 * sizeof(int)));
            return (xpp, ypp);
        }

        public static ulong CastCombine(int x, int y)
        {
            var d = (x, y);
            return Unsafe.As<(int, int), ulong>(ref d);
        }

        public static (int x, int y) CastDepart(ulong xy)
        {
            return Unsafe.As<ulong, (int, int)>(ref xy);
        }
    }
}