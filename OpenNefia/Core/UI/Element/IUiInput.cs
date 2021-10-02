using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element
{
    public interface IUiInput : IKeyInput
    {
        List<UiKeyHint> MakeKeyHints();
    }
}
