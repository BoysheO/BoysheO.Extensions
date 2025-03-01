using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BoysheO.Extensions;
using BoysheO.ProcessSystem.LogSystem;

namespace BoysheO.ProcessSystem
{
    public static class CommandLineHelper
    {
        /// <summary>
        /// 特别提示：返回isSuccess为true仅仅是代表进程正常结束，而不是运行正确。要判断命令行是否成功执行应当判断exitCode是否等于0
        /// 在windows上，由于没有CheckIfFileIsExecutable这一步，因此cmd执行会顺利完成，但是返回的exitCode不为0
        /// 在mac上，由于有CheckIfFileIsExecutable这一步，因此bash无法执行
        /// </summary>
        public static async Task<(bool isSuccesss, List<Log>? consoleLog, int exitCode)> ExecuteCommandAsync(
            string command,
            string arguments, bool requireElevation = false,
            IObserver<Log>? logger = null, string? workingDirectory = null,
            CancellationToken cancellationToken = default)
        {
            var platform = PlatformDetection.GetOSKind();
            if (platform == PlatformDetection.OSKind.MacOSX)
            {
                if (!IsCommandAvailableOnMac(command))
                {
                    logger.LogError(
                        $"The command '{command}' is not executable or does not exist.Please check the file exist and run sudo chmod +x");
                    return (false, null, -1); // Return a non-zero exit code to indicate failure
                }
            }
            else if (platform == PlatformDetection.OSKind.Windows)
            {
                //ignore
            }
            else throw new PlatformNotSupportedException();

            var processStartInfo = CreateProcessStartInfo(command, arguments, workingDirectory, requireElevation);

            using var process = new Process { StartInfo = processStartInfo };
            if (workingDirectory.IsNotNullOrWhiteSpace())
            {
                logger.LogInformation($"Starting process: {command} {arguments} at {workingDirectory}");
            }
            else
            {
                logger.LogInformation($"Starting process: {command} {arguments}");
            }

            // Start the process
            process.Start();

            // Register a callback to kill the process if the cancellation token is triggered
#if NET6_0_OR_GREATER
            await using (cancellationToken.Register(() =>
#else
            using (cancellationToken.Register(() =>
#endif
                         {
                             // ReSharper disable AccessToDisposedClosure
                             try
                             {
                                 if (!process.HasExited)
                                 {
                                     logger.LogInformation("Cancellation requested. Killing process...");
#if NET6_0_OR_GREATER
                                     process.Kill(true); // Kill the process and its children
#else
                               process.Kill();
#endif
                                 }
                             }
                             catch (Exception ex)
                             {
                                 logger.LogError($"Error killing process: {ex.Message}");
                             }
                             // ReSharper restore AccessToDisposedClosure
                         }))
            {
                // Asynchronously read the output and error streams
                var task = SteamReaderHelper.ReadAllAsync(process.StandardOutput, process.StandardError,
                    cancellationToken);

                // Wait for the process to exit or for the cancellation token to be triggered
                try
                {
#if NET6_0_OR_GREATER
                    await process.WaitForExitAsync(cancellationToken);
#else
                    process.WaitForExit();
#endif
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Process execution was cancelled.");
                    return (false, null, -1); // Return a non-zero exit code to indicate cancellation
                }

                var logList = await task;

                logger.LogInformation($"Process exited with code: {process.ExitCode}");

                return (true, logList, process.ExitCode);
            }
        }

        public static ProcessStartInfo CreateProcessStartInfo(string command, string arguments,
            string? workingDirectory,
            bool requireElevation)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (workingDirectory != null)
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }

            if (requireElevation)
            {
                var plat = PlatformDetection.GetOSKind();
                if (plat == PlatformDetection.OSKind.Windows)
                {
                    processStartInfo.Verb = "runas"; // Run as administrator on Windows
                }
                else if (plat == PlatformDetection.OSKind.MacOSX || plat == PlatformDetection.OSKind.Linux)
                {
                    processStartInfo.FileName = "sudo";
                    processStartInfo.Arguments = $"{command} {arguments}";
                }
            }

            return processStartInfo;
        }

        [Obsolete("use IsCommandAvailableOnMac instead")]
        public static bool CheckIfFileIsExecutable(string filePath, IObserver<Log>? logger = null)
        {
            var os = PlatformDetection.GetOSKind();
            if (os is not PlatformDetection.OSKind.MacOSX && os is not PlatformDetection.OSKind.Linux)
            {
                throw new PlatformNotSupportedException("Executable check is only valid on MacOSX or Linux");
            }

            if (!File.Exists(filePath))
            {
                logger?.OnNext(new Log(LogLevel.E, $"The file '{filePath}' does not exist."));
                logger?.OnCompleted();
                return false;
            }

            // Use the 'stat' command to check if the file is executable
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "stat",
                Arguments = $"-l ‘{filePath}’",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                process.WaitForExit();

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error))
                {
                    logger?.OnNext(new Log(LogLevel.E, $"Error checking file permissions: {error}"));
                    logger?.OnCompleted();
                    return false;
                }

                // Check if the file has executable permissions for the current user
                // The output of 'stat -c %A' looks like: -rwxr-xr-x
                // 'x' indicates executable permission
                if (output.Contains("x"))
                {
                    logger?.OnCompleted();
                    return true;
                }
                else
                {
                    logger?.OnNext(new Log(LogLevel.E,
                        $"The file '{filePath}' does not have executable permissions."));
                    logger?.OnCompleted();
                    return false;
                }
            }
        }

        /// <summary>
        /// 在mac中判断一个命令是否有效可执行
        /// 有以下几种情况：
        /// 命令不存在,command -v 无日志 ，返回1
        /// 没有执行权限（没有x权限），command -v 无日志 ，返回1
        /// 被Mac安全机制阻止执行，command有日志，返回0
        /// 命令需要是可执行的，protoc命令无法使用command判定，但是./protoc可以用command判定。
        /// 所以对于非内置命令最好用绝对路径
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool IsCommandAvailableOnMac(string command)
        {
            var os = PlatformDetection.GetOSKind();
            if (os != PlatformDetection.OSKind.MacOSX)
                throw new PlatformNotSupportedException("This method only work in Mac");
            try
            {
                // 使用 command -c 执行命令并检查输出
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    //这里不能直接执行command命令，而是要通过bash来执行，否则使用不了bash里的环境变量
                    FileName = "/bin/bash",
                    //command这里不能使用单引号。zsh里面手动调能用单引号，但是代码调bash时如果用单引号则报错
                    Arguments = $"-c \"command -v '{command}'\"",
                    // RedirectStandardOutput = true,
                    // RedirectStandardError = true,   
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using Process process = Process.Start(startInfo)!;
                process.WaitForExit();
                // //下面信息可用于debug
                // string output = process.StandardOutput.ReadToEnd();
                // string error = process.StandardError.ReadToEnd();

                // 如果命令存在，command -v 会返回命令的路径
                //如果命令不存在，则无日志并且返回代码1
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}