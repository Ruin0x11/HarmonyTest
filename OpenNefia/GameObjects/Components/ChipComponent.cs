﻿using System;
using OpenNefia.Core.Maths;
using OpenNefia.Core.Prototypes;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Serialization.Manager.Attributes;

namespace OpenNefia.Core.GameObjects
{
    [RegisterComponent]
    public class ChipComponent : Component
    {
        public override string Name => "Chip";

        [DataField("id", required: true)]
        public PrototypeId<ChipPrototype> ChipID { get; set; }

        [DataField]
        public Color Color { get; set; } = Color.White;
    }
}
