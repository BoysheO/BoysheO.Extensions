using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Boysheo.ProcessSystem.LogSystem;

namespace Boysheo.ProcessSystem
{
    internal static class SteamReaderHelper
    {
        public static async Task<List<Log>> ReadAllAsync(StreamReader normal, StreamReader error, CancellationToken cancellationToken)
        {
            var lst = new List<Log>();
            var t1 = Task.Run(async () =>
            {
                //开始读取
                using var sr = normal;
                while (!sr.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var line = await sr.ReadLineAsync() ?? "";
                    lock (lst)
                    {
                        lst.Add(new Log(LogLevel.N, line));
                    }
                }
            },cancellationToken);
            
            var t2 = Task.Run(async () =>
            {
                //开始读取
                using var sr = error;
                while (!sr.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var line = await sr.ReadLineAsync() ?? "";
                    lock (lst)
                    {
                        lst.Add(new Log(LogLevel.E, line));
                    }
                }
            },cancellationToken);

            await Task.WhenAll(t1, t2);
            return lst;
        }
    }
}