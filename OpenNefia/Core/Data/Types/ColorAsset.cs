using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class ColorAsset : IDataType
    {
        public string Id { get; private set; }
        public ColorAsset(string id) { this.Id = id; }

        public Love.Color Value { get; private set; }

        public static implicit operator Love.Color(ColorAsset c) => c.Value;

        public static class Entries
        {
            public static ColorAsset TextForeground = new ColorAsset($"Core.{nameof(TextForeground)}") { Value = Love.Color.White };
            public static ColorAsset TextBackground = new ColorAsset($"Core.{nameof(TextBackground)}") { Value = Love.Color.Black };
        }
    }
}
