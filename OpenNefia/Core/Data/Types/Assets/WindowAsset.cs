using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types.Assets
{
    public class WindowAsset : AssetDef
    {
        public WindowAsset(string id) : base(id) { }

        public override Dictionary<string, AssetDef.Region> GetRegions(int width, int height)
        {
            var regions = new Dictionary<string, AssetDef.Region>();

            regions["fill"] = new AssetDef.Region(24, 24, 228, 144);
            regions["top_left"] = new AssetDef.Region(0, 0, 64, 48);
            regions["top_right"] = new AssetDef.Region(208, 0, 56, 48);
            regions["bottom_left"] = new AssetDef.Region(0, 144, 64, 48);
            regions["bottom_right"] = new AssetDef.Region(208, 144, 56, 48);
            for (int i = 0; i < 19; i++) 
            {
                regions[$"top_mid_{i}"] = new AssetDef.Region(i * 8 + 36, 0, 8, 48);
                regions[$"bottom_mid_{i}"] = new AssetDef.Region(i * 8 + 54, 144, 8, 48);
            }

            for (int j = 0; j < 13; j++) 
            {
                regions[$"mid_left_{j}"] = new AssetDef.Region(0, j * 8 + 48, 64, 8);

                for (int i = 0; i < 19; i++)
                {
                    regions[$"mid_mid_{j}_{i}"] = new AssetDef.Region(i * 8 + 64, j * 8 + 48, 8, 8);
                }

                regions[$"mid_right_{j}"] = new AssetDef.Region(208, j * 8 + 48, 56, 8);
            }

            return regions;
        }
    }
}
