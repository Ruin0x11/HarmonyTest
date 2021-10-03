using OpenNefia.Core.UI.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public class UiFocusManager : IUiFocusManager
    {
        public IUiInputElement? FocusedElement { get; set; }
    }
}
