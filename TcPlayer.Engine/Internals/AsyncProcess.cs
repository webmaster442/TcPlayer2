// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcPlayer.Engine.Models;

namespace TcPlayer.Engine.Internals
{
    internal static class AsyncProcess
    {
        private static Task<bool> WaitForExitAsync(Process process, int timeout)
        {
            return Task.Run(() => process.WaitForExit(timeout));
        }

        public static async Task<AsyncProcesssResult> ExecuteShellCommand(string command, string arguments, int timeout = 5000)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var outputBuilder = new StringBuilder();
                var outputCloseEvent = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data == null)
                    {
                        outputCloseEvent.SetResult(true);
                    }
                    else
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                var errorBuilder = new StringBuilder();
                var errorCloseEvent = new TaskCompletionSource<bool>();

                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data == null)
                    {
                        errorCloseEvent.SetResult(true);
                    }
                    else
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                bool isStarted;

                try
                {
                    isStarted = process.Start();
                }
                catch (Exception error)
                {
                    return new AsyncProcesssResult
                    {
                        Completed = true,
                        ExitCode = -1,
                        Output = error.Message,
                    };
                }

                if (isStarted)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    var waitForExit = WaitForExitAsync(process, timeout);

                    var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

                    if (await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result)
                    {
                        return new AsyncProcesssResult
                        {
                            Completed = true,
                            ExitCode = process.ExitCode,
                            Output = process.ExitCode == 0 ? outputBuilder.ToString() : errorBuilder.ToString()
                        };
                    }
                    else
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        { 
                            //we don't do anything with exceptions here
                        }
                    }
                }
            }

            return new AsyncProcesssResult()
            {
                Completed = false,
                ExitCode = -1,
                Output = string.Empty,
            };

        }
    }
}
