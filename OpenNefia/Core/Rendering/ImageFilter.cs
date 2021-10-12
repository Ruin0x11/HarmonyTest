using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public class ImageFilter : IDefSerializable
    {
        public Love.FilterMode Min { get; set; } = Love.FilterMode.None;
        public Love.FilterMode Mag { get; set; } = Love.FilterMode.None;
        public float Anisotropy { get; set; } = 1f;
    }
}
