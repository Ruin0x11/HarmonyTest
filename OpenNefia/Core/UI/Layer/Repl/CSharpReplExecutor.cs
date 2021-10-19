using CSharpRepl.Services;
using CSharpRepl.Services.Logging;
using CSharpRepl.Services.Roslyn;
using CSharpRepl.Services.Roslyn.Scripting;
using FluentResults;
using PrettyPrompt.Consoles;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace OpenNefia.Core.UI.Layer.Repl
{
    public class CSharpReplExecutor : IReplExecutor
    {
        private IConsole Console;
        private Configuration Config;
        private RoslynServices Roslyn;

        internal sealed class NullLogger : ITraceLogger
        {
            public void Log(string message) {}
            public void Log(Func<string> message) {}
            public void LogPaths(string message, Func<IEnumerable<string?>> paths) {}
        }

        public CSharpReplExecutor()
        {
            Console = new SystemConsole();
            Config = new Configuration()
            {
                References = new HashSet<string>() { Assembly.GetExecutingAssembly().Location },
            };
            Roslyn = new RoslynServices(Console, Config, new NullLogger());
        }

        public ReplExecutionResult Execute(string code)
        {
            var result = Roslyn.EvaluateAsync(code, Config.LoadScriptArgs, new CancellationToken()).GetAwaiter().GetResult();

            switch (result)
            {
                case EvaluationResult.Success success:
                    return new ReplExecutionResult.Success($"{success.ReturnValue}");
                case EvaluationResult.Error err:
                    return new ReplExecutionResult.Error(err.Exception);
                default:
                    return new ReplExecutionResult.Error(new InvalidOperationException("Could not process REPL result"));
            }
        }
    }
}
