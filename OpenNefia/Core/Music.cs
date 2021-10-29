using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using OpenNefia.Core.Data.Types;

namespace OpenNefia.Core
{
    public static class Music
    {
        private static Playback? MidiPlayback = null;
        private static OutputDevice? MidiDevice = null;

        private static OutputDevice GetMidiOutputDevice() => OutputDevice.GetByIndex(0);

        public static void PlayMusic(MusicDef musicDef)
        {
            if (MidiPlayback != null)
                StopMusic();

            if (!Config.EnableMusic)
                return;

            var path = musicDef.Filepath.Resolve();

            if (path.EndsWith(".mid"))
            {
                var midiFile = MidiFile.Read(path);

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
