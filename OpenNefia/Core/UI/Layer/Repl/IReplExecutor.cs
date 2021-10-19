using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer.Repl
{
    public interface IReplExecutor
    {
        void Init();
        ReplExecutionResult Execute(string code);
    }
}
