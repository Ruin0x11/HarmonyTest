using CommandLine;
using CommandLine.Text;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Cli
{
    public class CliEntryPoint
    {
        private static IEnumerable<Type> EnumerateVerbs()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t => t.GetCustomAttribute<VerbAttribute>() != null))
                {
                    yield return type;
                }
            }
        }

        private static string GetHeading()
        {
            Assembly assembly = Assembly.GetEntryAssembly()!;
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return $"{assembly.GetName()} {fvi.FileVersion}";
        }

        private static void DisplayUsage<T>(ParserResult<T> result, IEnumerable<Error> errors)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = GetHeading();
                h.Copyright = "© 2021 Ruin0x11. Licensed under the MIT License.";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            });

            Console.WriteLine(helpText);
        }

        private static async Task RunCommand(object obj)
        {
            Engine.ModLoader.Execute();
            DefLoader.LoadAll();

            switch (obj)
            {
                case Commands.GenerateThemeDefOptions o:
                    await new Commands.GenerateThemeDefCommand(o).Execute();
                    break;
                default:
                    throw new NotImplementedException($"Unimplemented command for {obj}");
            }
        }

        public static async Task Run(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var verbs = EnumerateVerbs().ToArray();

            var result = parser.ParseArguments(args, verbs);

            result = await result.WithParsedAsync(RunCommand);
            result = result.WithNotParsed(errors => DisplayUsage(result, errors));
        }
    }
}
