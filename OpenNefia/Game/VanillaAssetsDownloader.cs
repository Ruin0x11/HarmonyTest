using OpenNefia.Core.Data;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.UI.Layer;
using OpenNefia.Mod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Game
{
    internal class VanillaAssetsDownloader : IProgressableJob
    {
        private const string URL_YLVANIA_ELONA122 = "http://ylvania.style.coocan.jp/file/elona122.zip";

        private static string CachePath = new ModLocalPath(typeof(CoreMod), "Cache/Deps/elona122.zip").Resolve();

        public VanillaAssetsDownloader()
        {
        }
        
        public static bool NeedsDownload()
        {
            return !Directory.Exists("Assets/Elona/Graphic") 
                   || !Directory.Exists("Assets/Elona/Sound");
        }

        public uint NumberOfSteps => 2;

        private async Task DownloadElona122Zip()
        {
            if (File.Exists(CachePath))
                return;

            if (!Directory.Exists(CachePath))
                Directory.CreateDirectory(Path.GetDirectoryName(CachePath)!);

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(URL_YLVANIA_ELONA122);
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync(); 
                using (var fileStream = new FileStream(CachePath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }

        private void ExtractSubdirectory(ZipArchive archive, string fromDirectory, string toDirectory)
        {
            var toPath = new ModLocalPath(typeof(CoreMod), toDirectory).Resolve();
            Directory.CreateDirectory(toPath);

            var matching = archive.Entries
                .Where(entry => Path.GetDirectoryName(entry.FullName)!.Replace("\\", "/").StartsWith(fromDirectory) && !string.IsNullOrEmpty(entry.Name));

            foreach (var entry in matching)
            {
                var fullPath = Path.Combine(toPath, entry.FullName.Replace("\\", "/").RemovePrefix(fromDirectory + "/"));
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                entry.ExtractToFile(fullPath, true);
            }
        }

        private Task UnpackVanillaAssets()
        {
            using (var fileStream = new FileStream(CachePath, FileMode.Open, FileAccess.Read))
            {
                using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    ExtractSubdirectory(zip, "elona/graphic", "Assets/Elona/Graphic");
                    ExtractSubdirectory(zip, "elona/sound", "Assets/Elona/Sound");
                }
            }
            return Task.CompletedTask;
        }

        public IEnumerator<ProgressStep> GetEnumerator()
        {
            yield return new ProgressStep("Downloading 1.22...", DownloadElona122Zip());
            yield return new ProgressStep("Unpacking assets...", UnpackVanillaAssets());
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
