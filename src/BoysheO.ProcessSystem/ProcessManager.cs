using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boysheo.ProcessSystem;

public class ProcessManager
{
    /// <summary>
    ///    最基础的无处理Invoke
    /// </summary>
    public static async Task<int> BaseInvokeAsync(string command,
        string arg, string? workDir, ILogger logger, ILogger consoleLog,
        CancellationToken killProcess = default)
    {
        if (killProcess.IsCancellationRequested) return -1;
        var id = new Random().Next();
        var threadId = Thread.CurrentThread.ManagedThreadId;
        var head = $"[id:{id}][thread:{threadId}]";
        logger.LogInformation($"{head}exec:{command} {arg}");
        using var proc = System.Diagnostics.Process.Start(new ProcessStartInfo()
        {
            Arguments = arg,
            FileName = command,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            StandardErrorEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute = false,
            WorkingDirectory = workDir,
        }) ?? throw new Exception("creat process fail");
        using var stopReadLog = new CancellationTokenSource();
        // ReSharper disable once AccessToDisposedClosure
        using var _ = killProcess.Register(() =>
        {
            stopReadLog.Cancel();
            proc.Kill();
        });

        List<(string msg, DateTimeOffset time, bool isEr)> msgs =
            new List<(string msg, DateTimeOffset time, bool)>();
        var t1 = Task.Run(async () =>
        {
            //开始读取
            using var sr = proc.StandardOutput;
            while (!sr.EndOfStream)
            {
                stopReadLog.Token.ThrowIfCancellationRequested();
                var line = await sr.ReadLineAsync();
                lock (msgs)
                {
                    msgs.Add((line, DateTimeOffset.Now, false));
                }

                consoleLog.LogInformation(line);
            }
        }, stopReadLog.Token);

        var t2 = Task.Run(async () =>
        {
            using var sr = proc.StandardError;
            while (!sr.EndOfStream)
            {
                stopReadLog.Token.ThrowIfCancellationRequested();
                var line = await sr.ReadLineAsync();
                lock (msgs)
                {
                    msgs.Add((line, DateTimeOffset.Now, true));
                }

                consoleLog.LogError(line);
            }
        }, stopReadLog.Token);

        //wait progress exit
        while (!proc.HasExited)
        {
            await Task.Yield();
        }

        stopReadLog.Cancel();

        //wait read task exit 
        try
        {
            await Task.WhenAll(t1, t2);
        }
        catch (OperationCanceledException)
        {
            //ignore
        }

        return proc.ExitCode;
    }

    public static async Task AskForSudo()
    {
        await BaseInvokeAsync("sudo", "-v", null, NullLogger.Instance, NullLogger.Instance);
    }
}