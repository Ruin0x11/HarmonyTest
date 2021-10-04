﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public class UiListChoiceKey
    {
        public Keys Key { get; set; } = Keys.None;
        public bool UseKeybind = true;

        public UiListChoiceKey(Keys key = Keys.None, bool useKeybind = true)
        {
            this.Key = key;
            this.UseKeybind = useKeybind;
        }
    }
}