using Love;
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
        public int Anisotropy { get; set; } = 1;

        public ImageFilter(FilterMode min, FilterMode mag, int anisotropy)
        {
            Min = min;
            Mag = mag;
            Anisotropy = anisotropy;
        }
    }
}
