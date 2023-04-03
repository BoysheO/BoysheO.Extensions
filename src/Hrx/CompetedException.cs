using System;

namespace Hrx
{
    /// <summary>
    /// 表示事件流完结
    /// </summary>
    public sealed class CompetedException : Exception
    {
        public static readonly CompetedException Instance = new CompetedException();

        private CompetedException()
        {
            //do nothing
        }
    }
}