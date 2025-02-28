using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BoysheO.ProcessSystem.LogSystem;

namespace BoysheO.ProcessSystem
{
    public static class CommandLineHelper
    {
        /// <summary>
        /// 特别提示：返回isSuccess为true仅仅是代表进程正常结束，而不是运行正确。要判断命令行是否成功执行应当判断exitCode是否等于0
        /// </summary>
        public static async Task<(bool isSuccesss, List<Log>? consoleLog, int exitCode)> ExecuteCommandAsync(
            string command,
            string arguments, bool requireElevation = false,
            bool withLog = false,
            IObserver<Log>? logger = null, CancellationToken cancellationToken = default)
        {
            var platform = PlatformDetection.GetOSKind();
            if (platform == PlatformDetection.OSKind.MacOSX || platform == PlatformDetection.OSKind.Linux)
            {
                if (!CheckIfFileIsExecutable(command))
                {
                    logger.LogError(
                        $"The command '{command}' is not executable or does not exist.Please check the file exist and run sudo chmod +x");
                    return (false, null, -1); // Return a non-zero exit code to indicate failure
                }
            }

            var processStartInfo = CreateProcessStartInfo(command, arguments, requireElevation);

            using var process = new Process { StartInfo = processStartInfo };
            logger.LogInformation($"Starting process: {command} {arguments}");

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
                Arguments = $"-c %A {filePath}",
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
    }
}