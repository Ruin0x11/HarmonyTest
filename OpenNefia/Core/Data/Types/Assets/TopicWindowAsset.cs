using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types.Assets
{
    public class TopicWindowAsset : AssetDef
    {
        public TopicWindowAsset(string id) : base(id) {}

        public override Dictionary<string, AssetDef.Region> GetRegions(int width, int height)
        {
            var regions = new Dictionary<string, AssetDef.Region>();

            regions["top_mid"] = new AssetDef.Region(16, 0, 16, 16);
            regions["bottom_mid"] = new AssetDef.Region(16, 32, 16, 16);
            regions["top_mid2"] = new AssetDef.Region(16, 0, width % 16, 16);
            regions["bottom_mid2"] = new AssetDef.Region(16, 32, width % 16, 16);
            regions["left_mid"] = new AssetDef.Region(0, 16, 16, 16);
            regions["right_mid"] = new AssetDef.Region(32, 16, 16, 16);
            regions["left_mid2"] = new AssetDef.Region(0, 16, 16, height % 16);
            regions["right_mid2"] = new AssetDef.Region(32, 16, 16, height % 16);
            regions["top_left"] = new AssetDef.Region(0, 0, 16, 16);
            regions["bottom_left"] = new AssetDef.Region(0, 32, 16, 16);
            regions["top_right"] = new AssetDef.Region(32, 0, 16, 16);
            regions["bottom_right"] = new AssetDef.Region(32, 32, 16, 16);

            return regions;
        }
    }
}
