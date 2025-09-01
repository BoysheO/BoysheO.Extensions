using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoysheO
{
    public static class CommandLineTool
    {
        /// <summary>
        /// 异步执行命令并返回完整结果
        /// </summary>
        /// <param name="cmd">要执行的命令字符串</param>
        /// <param name="cwd">工作目录，null 则使用当前目录</param>
        /// <param name="env">环境变量，null 则使用默认环境变量</param>
        /// <returns>退出码和完整日志输出</returns>
        public static async Task<(int code, IReadOnlyList<Output> log)> RunCommandAsync(string cmd,
            DirectoryInfo? cwd = null,
            IReadOnlyDictionary<string, string>? env = null)
        {
            List<Output> logs = new();
            await RunCommandAsync(cmd, v => { logs.Add(v); }, cwd, env);
            var last = logs[logs.Count - 1];
            logs.RemoveAt(logs.Count - 1); //最后一条是错误码，分离出来
            return (last.ExitCode, logs);
        }

        public struct Output
        {
            public bool IsErrorLog;
            public string Log;
            public int ExitCode; //日志全部输出完成，最后 输出ExitCode
        }

        /// <summary>
        /// 实时执行命令并通过回调函数返回流式输出
        /// </summary>
        /// <param name="cmd">要执行的命令字符串</param>
        /// <param name="onOutput">输出回调函数</param>
        /// <param name="cwd">工作目录，null 则使用当前目录</param>
        /// <param name="env">环境变量，null 则使用默认环境变量</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>执行任务</returns>
        public static async Task RunCommandAsync(string cmd, Action<Output> onOutput,
            DirectoryInfo? cwd = null, IReadOnlyDictionary<string, string>? env = null,
            CancellationToken cancellationToken = default)
        {
            var (fileName, arguments) = GetShellCommand(cmd);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
                WorkingDirectory = cwd?.FullName ?? Directory.GetCurrentDirectory()
            };

            // 设置环境变量
            if (env != null)
            {
                foreach (var kvp in env)
                {
                    processStartInfo.Environment[kvp.Key] = kvp.Value;
                }
            }

            using var process = new Process { StartInfo = processStartInfo };
            var tcsOut = new TaskCompletionSource<bool>();
            var tcsErr = new TaskCompletionSource<bool>();
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    tcsOut.TrySetResult(true);
                }
                else
                {
                    onOutput(new Output
                    {
                        IsErrorLog = false,
                        Log = e.Data,
                        ExitCode = -1 // 还未结束
                    });
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    tcsErr.TrySetResult(true);
                }
                else
                {
                    onOutput(new Output { IsErrorLog = true, Log = e.Data, ExitCode = -1 });
                }
            };
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // 等待进程退出，同时检查取消令牌
#if NET5_0_OR_GREATER
                await Task.WhenAll(process.WaitForExitAsync(cancellationToken), tcsOut.Task, tcsErr.Task);
#else
                // 兼容旧框架：简易等待 + 轮询取消
                while (!process.WaitForExit(50))
                    cancellationToken.ThrowIfCancellationRequested();
                await Task.WhenAll(tcsOut.Task, tcsErr.Task);
#endif
            }
            catch (OperationCanceledException)
            {
                TryKill(process);
                throw;
            }

            // 发送最终退出码
            onOutput(new Output
            {
                IsErrorLog = false,
                Log = string.Empty,
                ExitCode = process.ExitCode
            });
        }

        private static void TryKill(Process p)
        {
            try
            {
                if (!p.HasExited)
                {
#if NET5_0_OR_GREATER
                    p.Kill(true);
#else
                    p.Kill();
#endif
                    p.WaitForExit();
                }
            }
            catch { /* 忽略杀进程失败 */ }
        }
        
        /// <summary>
        /// 根据当前操作系统获取合适的 shell 命令
        /// </summary>
        /// <param name="cmd">要执行的命令</param>
        /// <returns>shell 程序和参数</returns>
        private static (string fileName, string arguments) GetShellCommand(string cmd)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows 使用 cmd.exe
                return ("cmd.exe", $"/c \"{cmd}\"");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Linux 和 macOS 使用 bash
                return ("/bin/bash", $"-c \"{cmd.Replace("\"", "\\\"")}\"");
            }
            else
            {
                // 默认使用 bash（对于其他 Unix-like 系统）
                return ("/bin/bash", $"-c \"{cmd.Replace("\"", "\\\"")}\"");
            }
        }
    }
}