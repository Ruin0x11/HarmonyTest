using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    /// <summary>
    /// A simple animation with a series of frames displayed at a constant rate.
    /// </summary>
    public class BasicAnimDef : Def
    {
        public BasicAnimDef(string id) : base(id)
        {
        }

        /// <summary>
        /// How much time to wait between frames of this animation.
        /// </summary>
        public float FrameDelayMillis = 50f;

        /// <summary>
        /// How many frames this animation holds. 
        /// 
        /// Omit to default to the asset's <see cref="AssetDef.CountX"/> property.
        /// </summary>
        public uint? FrameCount;

        /// <summary>
        /// How much the asset should be rotated, in radians.
        /// </summary>
        public float Rotation = 0f;

        /// <summary>
        /// Asset to display.
        /// 
        /// This asset should have a <see cref="AssetDef.CountX"/> property with
        /// the number of frames in the animation.
        /// </summary>
        [DefRequired]
        public AssetDef Asset = null!;

        /// <summary>
        /// A sound to play when this animation is displayed.
        /// </summary>
        public SoundDef? Sound;
    }
}
