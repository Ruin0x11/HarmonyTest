using System;

namespace OpenNefia.Core.UI.Element
{
    public interface IUiText : IDrawable, IDisposable
    {
        public string Text { get; set; }
    }
}