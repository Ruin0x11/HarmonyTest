using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public static class Gui
    {
        public static void PlaySound(SoundDef soundDef)
        {
            var sound = Love.Audio.NewSource(soundDef.Filepath.Resolve(), Love.SourceType.Static);
            sound.SetVolume(1.0f);
            Love.Audio.Play(sound);
        }
    }
}
