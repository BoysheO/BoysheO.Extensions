using System;

namespace BoysheO.Extensions
{
    public static class GuidExtensions
    {
        public static string ToShortString(Guid guid, int keep = 8)
        {
            return guid.ToString().Substring(0, keep);
        }
    }
}