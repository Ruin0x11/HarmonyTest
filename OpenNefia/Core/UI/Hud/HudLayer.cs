using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Hud
{
    public class HudLayer : BaseUiLayer<UiNoResult>
    {
        internal IHudMessageWindow MessageWindow { get; }

        public HudLayer()
        {
            this.MessageWindow = new SimpleMessageWindow();
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
        }
    }
}
