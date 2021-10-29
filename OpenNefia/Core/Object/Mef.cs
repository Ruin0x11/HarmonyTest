using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public class Mef : MapObject
    {
        public MefDef Def => (MefDef)BaseDef;

        public int Turns;

        public Mef(MefDef def) : base(def)
        {
            Turns = 10;
        }

        public override bool IsInLiveState => this.Turns > 0;

        public override void OnTurnStart()
        {
            this.Turns -= 1;
            if (this.Turns <= 0)
            {
                this.Destroy();
            }
        }

        public virtual void OnSteppedOn(Chara chara)
        {
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
