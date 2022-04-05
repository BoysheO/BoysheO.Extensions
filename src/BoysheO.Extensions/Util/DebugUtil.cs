using System.Runtime.CompilerServices;

namespace BoysheO.Util
{
    public static class DebugUtil
    {
        /// <summary>
        ///     获取调用方信息
        /// </summary>
        public static string GetCallerContext(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            return $"{sourceFilePath}@{memberName}:{sourceLineNumber}";
        }
    }
}