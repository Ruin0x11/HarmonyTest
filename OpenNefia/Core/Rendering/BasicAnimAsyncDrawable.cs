using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    internal class FrameCounter
    {
        public float FrameDelayMillis;
        public uint MaxFrames;

        private float Dt = 0f;
        public uint Frame { get; private set; } = 0;
        public bool IsFinished { get => Frame >= MaxFrames; }

        public FrameCounter(float delayMillis, uint maxFrames)
        {
            FrameDelayMillis = delayMillis;
            MaxFrames = maxFrames;
        }
        
        public void Update(float dt)
        {
            Dt += dt * 1000f;
            Frame = Math.Clamp((uint)(Dt / FrameDelayMillis), 0, MaxFrames);
        }
    }

    public class BasicAnimAsyncDrawable : BaseAsyncDrawable
    {
        public BasicAnimDef BasicAnim { get; }

        private FrameCounter Counter;
        private AssetDrawable AssetDrawable;

        public BasicAnimAsyncDrawable(BasicAnimDef basicAnim)
        {
            this.BasicAnim = basicAnim;

            var animeWait = 20f;
            var maxFrames = this.BasicAnim.Asset.CountX;
            if (this.BasicAnim.FrameCount != null)
                maxFrames = this.BasicAnim.FrameCount.Value;

            this.Counter = new FrameCounter(animeWait + this.BasicAnim.FrameDelayMillis / 2, maxFrames);
            this.AssetDrawable = new AssetDrawable(this.BasicAnim.Asset);
        }

        public override void OnEnqueue()
        {
            if (this.BasicAnim.Sound != null)
            {
                // TODO positional audio
                Gui.PlaySound(this.BasicAnim.Sound);
            }
        }

        public override void Update(float dt)
        {
            Counter.Update(dt);
            if (Counter.IsFinished)
            {
                this.Finish();
            }
        }

        public override void Draw()
        {
            Love.Graphics.SetColor(Love.Color.White);
            this.AssetDrawable.DrawRegion(Counter.Frame.ToString(), this.X + 24, this.Y + 8);
        }
    }
}
