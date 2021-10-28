using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
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

    public static class Sound
    {
        private static Playback? MidiPlayback = null;
        private static OutputDevice? MidiDevice = null;
        private static Dictionary<object, Love.Source> OneShotSources = new Dictionary<object, Love.Source>();

        public static void PlayOneShot(SoundDef soundDef, int? screenX = null, int? screenY = null, float? volume = null, object? channel = null)
        {
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

        private static OutputDevice GetMidiOutputDevice() => OutputDevice.GetByIndex(0);

        public static void PlayMusic(MusicDef musicDef)
        {
            var path = musicDef.Filepath.Resolve();

            if (path.EndsWith(".mid"))
            {
                var midiFile = MidiFile.Read(path);

                if (MidiPlayback != null)
                    StopMusic();

                MidiDevice = GetMidiOutputDevice();
                MidiPlayback = midiFile.GetPlayback(MidiDevice);
                MidiPlayback.Loop = true;
                MidiPlayback.Start();
            }
        }

        public static void StopMusic()
        {
            if (MidiPlayback != null)
            {
                MidiPlayback.Dispose();
                MidiPlayback = null;
            }
            if (MidiDevice != null)
            {
                MidiDevice.Dispose();
                MidiDevice = null;
            }
        }
    }
}
