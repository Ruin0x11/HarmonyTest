using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object.Aspect.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public sealed class Mef : MapObject
    {
        public new MefDef Def => (MefDef)base.Def;

        public int Turns;

        internal Mef(MefDef def) : base(def)
        {
            Turns = 10;
        }

        internal Mef() : this(null!) {}

        public override bool IsInLiveState => this.Turns > 0;

        public override void OnTurnStart()
        {
            this.Turns -= 1;
            if (this.Turns <= 0)
            {
                this.Destroy();
            }
        }

        public override void Expose(DataExposer data)
        {
            base.Expose(data);

            data.ExposeValue(ref this.Turns, nameof(Turns));
        }

        public void Event_OnSteppedOn(Chara chara)
        {
            foreach (var aspect in this.GetAspects<ICanStepOnAspect>())
            {
                aspect.Event_OnSteppedOn(chara);
                if (!this.IsAlive)
                {
                    return;
                }
            }
        }

        public override void ProduceMemory(MapObjectMemory memory)
        {
            memory.ChipIndex = Def.Chip.Image.TileIndex;
            memory.Color = this.Color;
            memory.IsVisible = true;
            memory.ScreenXOffset = 0;
            memory.ScreenYOffset = 0;
        }

        public override void Refresh()
        {
        }
    }
}
