using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public enum SoundPosType
    {
        Tile,
        Screen
    }

    public static class Sounds
    {
        private static Dictionary<object, Love.Source> OneShotSources = new Dictionary<object, Love.Source>();

        public static void PlayOneShot(SoundDef soundDef, int? screenX = null, int? screenY = null, float? volume = null, object? channel = null)
        {
            if (!Config.EnableSound)
                return;

            var source = Love.Audio.NewSource(soundDef.Filepath.Resolve(), Love.SourceType.Static);
            
            if (source.GetChannelCount() == 1)
            {
                if (screenX != null && screenY != null)
                {
                    source.SetRelative(false);
                    source.SetPosition(screenX.Value, screenY.Value, 0f);
                    source.SetAttenuationDistances(100, 500);
                }
                else
                {
                    source.SetRelative(true);
                    source.SetAttenuationDistances(0, 0);
                }
            }

            source.SetVolume(Math.Clamp(volume ?? soundDef.Volume, 0f, 1f));

            Love.Audio.Play(source);

            if (channel != null)
            {
                if (OneShotSources.TryGetValue(channel, out var existingSource))
                {
                    Love.Audio.Stop(existingSource);
                }
                OneShotSources[channel] = source;
            }
        }

        public static void PlayOneShot(SoundDef soundDef, TilePos pos, float? volume = null, object? channel = null)
        {
            var coords = GraphicsEx.Coords;
            coords.TileToScreen(pos.X, pos.Y, out var screenX, out var screenY);
            screenX += coords.TileWidth / 2;
            screenY += coords.TileHeight / 2;
            PlayOneShot(soundDef, screenX, screenY, volume, channel);
        }

        public static bool IsChannelPlayingSound(object channel) => OneShotSources.ContainsKey(channel);
    }
}
