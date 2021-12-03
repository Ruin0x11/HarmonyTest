﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNefia.Core.Serialization.Manager.Attributes;

namespace OpenNefia.Core.GameObjects
{
    [RegisterComponent]
    public class StackableComponent : Component
    {
        public override string Name => "Stackable";

        [DataField]
        public int Amount { get; set; } = 1;
    }
}