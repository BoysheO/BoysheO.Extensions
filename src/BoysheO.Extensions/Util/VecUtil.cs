namespace BoysheO.Util
{
    //provide api about xy2ulong.benchmark and test see XY2VecExample projects
    public class VecUtil
    {
        public static ulong CombineXY(int x, int y)
        {
            var xx = unchecked((ulong) x) & 0x00000000FFFFFFFF;
            var yy = unchecked((ulong) y) << 8 * sizeof(int);
            ulong xy = xx | yy; 
            return xy;
        }
        
        public static (int x, int y) DepartXY(ulong xy)
        {
            unsafe
            {
                (int x, int y)* d1 = ((int x, int y)*) (&xy);
                return *d1;
            }
        }
    }
}