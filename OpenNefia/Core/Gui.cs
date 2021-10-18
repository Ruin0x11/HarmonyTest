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
    public static class Gui
    {
        private static Playback? MidiPlayback = null;
        private static OutputDevice? MidiDevice = null;

        public static void PlaySound(SoundDef soundDef, int? screenX = null, int? screenY = null)
        {
            var sound = Love.Audio.NewSource(soundDef.Filepath.Resolve(), Love.SourceType.Static);
            sound.SetVolume(1.0f);
            Love.Audio.Play(sound);
        }

        public static void PlaySoundAtTilePos(SoundDef soundDef, int tileX, int tileY)
        {
            GraphicsEx.Coords.TileToScreen(tileX, tileY, out var screenX, out var screenY);
            PlaySound(soundDef, screenX, screenY);
        }

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
