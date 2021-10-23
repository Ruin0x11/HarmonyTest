using FluentResults;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Map.Generator
{
    public class Elona122MapLoader : BaseMapGenerator
    {
        [DefRequired]
        public string IdxFilePath;

        public Elona122MapLoader(string idxFilePath)
        {
            IdxFilePath = idxFilePath;
        }

        internal Elona122MapLoader() : this(string.Empty) { }

        private static Dictionary<int, TileDef> BuildTileMapping(int atlasNo)
        {
            var mapping = new Dictionary<int, TileDef>();

            foreach (var tileDef in DefStore<TileDef>.Enumerate().Where(d => d.ElonaId != null && d.ElonaAtlas == atlasNo))
            {
                mapping[tileDef.ElonaId!.Value] = tileDef;
            }

            return mapping;
        }

        private BinaryReader OpenFileStreamGzip(string path)
        {
            return new BinaryReader(new GZipStream(File.Open(path, FileMode.Open), CompressionMode.Decompress));
        }

        public override Result<InstancedMap> Generate(MapDef mapDef, InstancedArea area, int floor)
        {
            var fileBaseName = PathUtils.GetFullPathWithoutExtension(IdxFilePath);
            var mapFilePath = $"{fileBaseName}.map";
            var objFilePath = $"{fileBaseName}.obj";

            if (!File.Exists(IdxFilePath))
            {
                return Result.Fail($"1.22 .idx file {IdxFilePath} does not exist.");
            }
            if (!File.Exists(mapFilePath))
            {
                return Result.Fail($"1.22 .map file {mapFilePath} does not exist.");
            }
            if (!File.Exists(objFilePath))
            {
                return Result.Fail($"1.22 .obj file {objFilePath} does not exist.");
            }

            InstancedMap map;
            int width, height, atlasNum, regen, stairUpPos;

            using (var reader = OpenFileStreamGzip(IdxFilePath))
            {
                width = reader.ReadInt32();
                height = reader.ReadInt32();
                atlasNum = reader.ReadInt32();
                regen = reader.ReadInt32();
                stairUpPos = reader.ReadInt32();

                map = new InstancedMap(width, height, mapDef);
            }

            var tileMapping = BuildTileMapping(atlasNum);

            using (var reader = OpenFileStreamGzip(mapFilePath))
            {
                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        var elonaTileId = reader.ReadInt32();
                        var tileDef = tileMapping[elonaTileId];
                        map.SetTile(x, y, tileDef);
                    }
                }
            }

            // TODO objects

            return Result.Ok(map);
        }
    }
}
