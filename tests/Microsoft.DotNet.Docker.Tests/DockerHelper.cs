// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xunit.Abstractions;

namespace Microsoft.DotNet.Docker.Tests
{
    public class DockerHelper(ITestOutputHelper outputHelper) : IDockerCli
    {
        private readonly ITestOutputHelper _outputHelper = outputHelper;

        private static readonly Lazy<string> s_dockerOS = new(ExecuteStatic("version -f \"{{ .Server.Os }}\""));
        public static string DockerOS => s_dockerOS.Value;
        public static bool IsLinuxContainerModeEnabled => string.Equals(DockerOS, "linux", StringComparison.OrdinalIgnoreCase);

        public void Build(
            string tag = "",
            string dockerfile = "",
            string target = "",
            string contextDir = ".",
            bool pull = false,
            string platform = "",
            string output = "",
            params string[] buildArgs
        )
        {
            var args = new List<string>();

            // Optional basic flags
            if (!string.IsNullOrWhiteSpace(tag))
            {
                args.Add("-t");
                args.Add(tag);
            }

            if (!string.IsNullOrWhiteSpace(dockerfile))
            {
                args.Add("-f");
                args.Add(dockerfile);
            }

            if (!string.IsNullOrWhiteSpace(target))
            {
                args.Add("--target");
                args.Add(target);
            }

            // Build args
            if (buildArgs is not null)
            {
                foreach (string buildArg in buildArgs)
                {
                    if (!string.IsNullOrWhiteSpace(buildArg))
                    {
                        args.Add("--build-arg");
                        args.Add(buildArg);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(platform))
            {
                args.Add("--platform");
                args.Add(platform);
            }

            if (!string.IsNullOrWhiteSpace(output))
            {
                args.Add("--output");
                args.Add(output);
            }

            if (pull)
            {
                args.Add("--pull");
            }

            args.Add(contextDir);

            Execute($"build {string.Join(' ', args)}");
        }

        public string Execute(string args, DockerCliRunOptions? options = null)
        {
            options ??= new DockerCliRunOptions();
            if (options.LogOutput)
            {
                return ExecuteWithLogging(args, ignoreErrors: options.IgnoreErrors, autoRetry: options.AutoRetry);
            }
            else
            {
                return ExecuteStatic(args, ignoreErrors: options.IgnoreErrors, autoRetry: options.AutoRetry);
            }
        }

        private string ExecuteWithLogging(string args, bool ignoreErrors = false, bool autoRetry = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _outputHelper.WriteLine($"Executing: docker {args}");
            string result = ExecuteStatic(args, outputHelper: _outputHelper, ignoreErrors: ignoreErrors, autoRetry: autoRetry);

            stopwatch.Stop();
            _outputHelper.WriteLine($"Execution Elapsed Time: {stopwatch.Elapsed}");

            return result;
        }

        public static bool ImageExists(string tag) => ResourceExists("image", tag);

        public static bool ContainerExists(string name) => ResourceExists("container", $"-f \"name={name}\"");

        public static bool ContainerIsRunning(string name) => ExecuteStatic($"inspect --format=\"{{{{.State.Running}}}}\" {name}") == "true";

        private static bool ResourceExists(string type, string filterArg)
        {
            string output = ExecuteStatic($"{type} ls -a -q {filterArg}", ignoreErrors: true);
            return output != "";
        }

        private static string ExecuteStatic(
            string args,
            bool ignoreErrors = false,
            bool autoRetry = false,
            ITestOutputHelper? outputHelper = null)
        {
            (Process Process, string StdOut, string StdErr) result;
            if (autoRetry)
            {
                result = ExecuteWithRetry(args, outputHelper, ExecuteProcess);
            }
            else
            {
                result = ExecuteProcess(args, outputHelper);
            }

            if (!ignoreErrors && result.Process.ExitCode != 0)
            {
                ProcessStartInfo startInfo = result.Process.StartInfo;
                string msg = $"Failed to execute {startInfo.FileName} {startInfo.Arguments}" +
                    $"{Environment.NewLine}Exit code: {result.Process.ExitCode}" +
                    $"{Environment.NewLine}Standard Error: {result.StdErr}";
                throw new InvalidOperationException(msg);
            }

            return result.StdOut;
        }

        private static (Process Process, string StdOut, string StdErr) ExecuteWithRetry(
            string args,
            ITestOutputHelper? outputHelper,
            Func<string, ITestOutputHelper?, (Process Process, string StdOut, string StdErr)> executor)
        {
            const int maxRetries = 5;
            const int waitFactor = 5;

            int retryCount = 0;

            (Process Process, string StdOut, string StdErr) result = executor(args, outputHelper);
            while (result.Process.ExitCode != 0)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    break;
                }

                int waitTime = Convert.ToInt32(Math.Pow(waitFactor, retryCount - 1));
                if (outputHelper != null)
                {
                    outputHelper.WriteLine($"Retry {retryCount}/{maxRetries}, retrying in {waitTime} seconds...");
                }

                Thread.Sleep(waitTime * 1000);
                result = executor(args, outputHelper);
            }

            return result;
        }

        private static (Process Process, string StdOut, string StdErr) ExecuteProcess(string args, ITestOutputHelper? outputHelper) =>
            ExecuteHelper.ExecuteProcess("docker", args, outputHelper);
    }
}
