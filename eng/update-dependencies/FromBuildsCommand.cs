// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace DotNet.Docker;

public class FromBuildsCommand : Command
{
    public FromBuildsCommand() : base("from-builds", "Update dependencies from BAR builds")
    {
        Arguments.Add(
            new Argument<int[]>("builds")
            {
                Arity = ArgumentArity.OneOrMore,
                Description = "One or more BAR build IDs to use as a source for the update."
            });
    }

    public class Handler(GreetingService greetingService) : BaseCommandAction
    {
        private readonly GreetingService _greetingService = greetingService;

        public int[] Builds { get; init; } = [];

        protected override Task<int> RunAsync()
        {
            Console.WriteLine(_greetingService.GetGreeting());

            Console.WriteLine($"Options: [{string.Join(", ", Builds)}]");

            return Task.FromResult(0);
        }
    }
}

public interface IGreetingService
{
    public string GetGreeting();
}

public class GreetingService : IGreetingService
{
    public string GetGreeting()
    {
        return "Hello, World????";
    }
}
