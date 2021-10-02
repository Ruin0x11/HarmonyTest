using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types.Assets
{
    public class TopicWindowAsset : Asset
    {
        public TopicWindowAsset(string id) : base(id) {}

        public override Dictionary<string, Asset.Region> GetRegions(int width, int height)
        {
            var regions = new Dictionary<string, Asset.Region>();

            regions["top_mid"] = new Asset.Region(16, 0, 16, 16);
            regions["bottom_mid"] = new Asset.Region(16, 32, 16, 16);
            regions["top_mid2"] = new Asset.Region(16, 0, width % 16, 16);
            regions["bottom_mid2"] = new Asset.Region(16, 32, width % 16, 16);
            regions["left_mid"] = new Asset.Region(0, 16, 16, 16);
            regions["right_mid"] = new Asset.Region(32, 16, 16, 16);
            regions["left_mid2"] = new Asset.Region(0, 16, 16, height % 16);
            regions["right_mid2"] = new Asset.Region(32, 16, 16, height % 16);
            regions["top_left"] = new Asset.Region(0, 0, 16, 16);
            regions["bottom_left"] = new Asset.Region(0, 32, 16, 16);
            regions["top_right"] = new Asset.Region(32, 0, 16, 16);
            regions["bottom_right"] = new Asset.Region(32, 32, 16, 16);

            return regions;
        }
    }
}
