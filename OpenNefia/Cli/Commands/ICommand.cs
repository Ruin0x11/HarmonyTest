using FluentResults;
using System.Threading.Tasks;

namespace OpenNefia.Cli.Commands
{
    public interface ICommand
    {
        Task<Result> Execute();
    }
}